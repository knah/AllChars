using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Timer=System.Threading.Timer;

namespace AllCharsNS {
    internal class AllCharsHook {
        private readonly AllChars ac;
        private readonly Configuration config;
        private readonly IntPtr keyboardHook = IntPtr.Zero;
        private readonly LowLevelKeyboardProc keyboardProc;

        private readonly byte[] keyState = new byte[256];
        private readonly object syncRoot = new object();

        private readonly Translator translator;

        #region Construction/Destruction

        public AllCharsHook(AllChars ac, Configuration config, Translator translator) {
            idleTimer = new Timer(setInactive);
            this.ac = ac;
            this.config = config;
            this.translator = translator;

            keyboardProc = keyboardHookProc;
            keyboardHook =
                SetWindowsHookEx(HookType.WH_KEYBOARD_LL, keyboardProc,
                                 GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);

            if (keyboardHook == IntPtr.Zero) throw new Win32Exception();

            GetKeyboardState(keyState);
        }

        ~AllCharsHook() {
            Debug.WriteLine("Removing AllCharsHook");
            if (keyboardHook != IntPtr.Zero)
                UnhookWindowsHookEx(keyboardHook);
        }

        #endregion

        #region AllChars keyboard hook

        private string composeChars = "";
        private readonly List<Keys> grabbedKeys = new List<Keys>(4);
        private readonly Timer idleTimer;
        private readonly TimeSpan nonPeriodic = new TimeSpan(0, 0, 0, 0, -1);
        private IntPtr foregroundWindow = IntPtr.Zero;

        private HookState hookState;

        private static bool isKeyDown(LLKHF flags) {
            return (flags&LLKHF.UP) == 0;
        }

        private static bool isKeyUp(LLKHF flags) {
            return (flags&LLKHF.UP) == LLKHF.UP;
        }

        private char getAscii(Keys vkCode, int scanCode, LLKHF flags) {
            StringBuilder keyChar = new StringBuilder(2);
            int nChars = ToAscii(vkCode, scanCode, keyState, keyChar, flags);

            if (nChars == 2)
                return keyChar[1];
            if (nChars == 1)
                return keyChar[0];
            return '\0';
        }


        private void TrackShift(Keys vkCode, LLKHF flags) {
            switch (vkCode) {
                case Keys.RShiftKey:
                case Keys.LShiftKey:
                    keyState[(int)vkCode] = (byte)(isKeyDown(flags)?128:0);
                    keyState[(int)Keys.ShiftKey] = (byte)(keyState[(int)Keys.LShiftKey]|keyState[(int)Keys.RShiftKey]);
                    break;
            }
        }

        private void setInactive(object state) {
            lock (syncRoot) {
                if (state != null)
                    Debug.WriteLine("Abort composing due to timeout");

                if (hookState != HookState.WaitCompose) {
                    ac.SetNotify(TaskbarState.Inactive);
                    hookState = HookState.WaitCompose;
                }
            }
        }

        private int keyboardHookProc(int nCode, WindowsMessages wParam, [In] KBDLLHOOKSTRUCT lParam) {
            lock (syncRoot) {
                bool grabKey = false;
                bool isHotKey = lParam.vkCode == config.HotKey;

                Debug.WriteLine("LL_Keyboard: "+lParam.vkCode+" / "+lParam.scanCode+" / "+lParam.flags);

                TrackShift(lParam.vkCode, lParam.flags);

                // swallow keyup events for keys which were swallowed on keydown
                if (isKeyUp(lParam.flags) && grabbedKeys.Contains(lParam.vkCode)) {
                    grabbedKeys.Remove(lParam.vkCode);
                    grabKey = true;
                }

                if (hookState != HookState.WaitCompose && foregroundWindow != Native.GetForegroundWindow()) {
                    // input focus changed from when <Compose> was pressed, abort composing and macro sequence gathering
                    Debug.WriteLine("Abort composing due to focus change");

                    foregroundWindow = IntPtr.Zero;

                    setInactive(null);
                }

                if ((lParam.flags&LLKHF.INJECTED) == 0)
                    switch (hookState) {
                        case HookState.WaitCompose:
                            if (isKeyDown(lParam.flags))
                                if (isHotKey) {
                                    Debug.WriteLine("LL_Keyboard: Got hotkey");
                                    composeChars = "";
                                    foregroundWindow = Native.GetForegroundWindow();
                                    if (foregroundWindow == IntPtr.Zero)
                                        Debug.WriteLine(" ** could not determine focused control on <Compose>");
                                    else
                                    {
                                        StringBuilder className = new StringBuilder(100);
                                        int res = GetClassName(foregroundWindow, className, className.Capacity);
                                        if (res != 0)
                                        {
                                            Debug.WriteLine("LL_Keyboard: fg class name \"" + className + "\"");
                                            if (config.IgnoredWindowClasses.Contains(className.ToString()))
                                                break;
                                        }
                                    }
                                    grabKey = true;
                                    grabbedKeys.Add(lParam.vkCode);
                                    ac.SetNotify(TaskbarState.WaitFirst);
                                    hookState = HookState.WaitChar;
                                    idleTimer.Change(config.TypingTimeout, nonPeriodic);
                                }
                                else if (lParam.vkCode > Keys.Help) {
                                    // nothing
                                }
                            break;
                        case HookState.WaitChar:
                            if (isKeyDown(lParam.flags))
                                if (lParam.vkCode == config.HotKey || lParam.vkCode == Keys.Escape) {
                                    Debug.WriteLine("LL_Keyboard: Got hotkey");
                                    setInactive(null);
                                }
                                else if (lParam.vkCode > Keys.Help && lParam.vkCode != Keys.RShiftKey && lParam.vkCode != Keys.LShiftKey) {
                                    grabKey = true;
                                    grabbedKeys.Add(lParam.vkCode);
                                    char c = getAscii(lParam.vkCode, lParam.scanCode, lParam.flags);
                                    if (c != '\0') {
                                        composeChars += c;
                                        Debug.WriteLine(string.Format("Got next char: '{0}'", c));
                                        if (translator.Compose(composeChars))
                                        {
                                            ac.SetNotify(TaskbarState.Inactive);
                                            hookState = HookState.WaitCompose;
                                        }
                                    }
                                    idleTimer.Change(config.TypingTimeout, nonPeriodic);
                                }
                            break;
                    }

                if (!grabKey)
                    return CallNextHookEx(keyboardHook, nCode, wParam, lParam);
                return 1; // keyboard event handled, stop processing
            }
        }

        private enum HookState {
            WaitCompose,
            WaitChar
        }

        #endregion

        #region P/Invoke hook interface
        // ReSharper disable UnusedMemberInPrivateClass

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, WindowsMessages wParam,
                                                 [In] KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType hook, LowLevelKeyboardProc callback,
                                                      IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        private static extern int ToAscii(Keys virtKey, int scanCode, byte[] keyState,
                                          [Out] StringBuilder outChar, LLKHF flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        #region Nested type: HookType

        private enum HookType {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        #endregion

        #region Nested type: KBDLLHOOKSTRUCT

        [StructLayout(LayoutKind.Sequential)]
        private class KBDLLHOOKSTRUCT {
            public readonly Keys vkCode;
            public readonly int scanCode;
            public readonly LLKHF flags;
#pragma warning disable 169
            public int time;
            public IntPtr dwExtraInfo;
#pragma warning restore 169

            public KBDLLHOOKSTRUCT(Keys vkCode, int scanCode, LLKHF flags) {
                this.vkCode = vkCode;
                this.flags = flags;
                this.scanCode = scanCode;
            }
        }

        #endregion

        #region Nested type: LLKHF

        [Flags]
        private enum LLKHF : byte {
            EXTENDED = 1,
            INJECTED = 16,
            ALTDOWN = 32,
            UP = 128
        }

        #endregion

        #region Nested type: LowLevelKeyboardProc

        private delegate int LowLevelKeyboardProc(int nCode, WindowsMessages wParam, [In] KBDLLHOOKSTRUCT lParam);

        #endregion

        #region Nested type: WindowsMessages

        private enum WindowsMessages {
            WM_ACTIVATE = 0x6,
            WM_ACTIVATEAPP = 0x1C,
            WM_AFXFIRST = 0x360,
            WM_AFXLAST = 0x37F,
            WM_APP = 0x8000,
            WM_ASKCBFORMATNAME = 0x30C,
            WM_CANCELJOURNAL = 0x4B,
            WM_CANCELMODE = 0x1F,
            WM_CAPTURECHANGED = 0x215,
            WM_CHANGECBCHAIN = 0x30D,
            WM_CHAR = 0x102,
            WM_CHARTOITEM = 0x2F,
            WM_CHILDACTIVATE = 0x22,
            WM_CLEAR = 0x303,
            WM_CLOSE = 0x10,
            WM_COMMAND = 0x111,
            WM_COMPACTING = 0x41,
            WM_COMPAREITEM = 0x39,
            WM_CONTEXTMENU = 0x7B,
            WM_COPY = 0x301,
            WM_COPYDATA = 0x4A,
            WM_CREATE = 0x1,
            WM_CTLCOLORBTN = 0x135,
            WM_CTLCOLORDLG = 0x136,
            WM_CTLCOLOREDIT = 0x133,
            WM_CTLCOLORLISTBOX = 0x134,
            WM_CTLCOLORMSGBOX = 0x132,
            WM_CTLCOLORSCROLLBAR = 0x137,
            WM_CTLCOLORSTATIC = 0x138,
            WM_CUT = 0x300,
            WM_DEADCHAR = 0x103,
            WM_DELETEITEM = 0x2D,
            WM_DESTROY = 0x2,
            WM_DESTROYCLIPBOARD = 0x307,
            WM_DEVICECHANGE = 0x219,
            WM_DEVMODECHANGE = 0x1B,
            WM_DISPLAYCHANGE = 0x7E,
            WM_DRAWCLIPBOARD = 0x308,
            WM_DRAWITEM = 0x2B,
            WM_DROPFILES = 0x233,
            WM_ENABLE = 0xA,
            WM_ENDSESSION = 0x16,
            WM_ENTERIDLE = 0x121,
            WM_ENTERMENULOOP = 0x211,
            WM_ENTERSIZEMOVE = 0x231,
            WM_ERASEBKGND = 0x14,
            WM_EXITMENULOOP = 0x212,
            WM_EXITSIZEMOVE = 0x232,
            WM_FONTCHANGE = 0x1D,
            WM_GETDLGCODE = 0x87,
            WM_GETFONT = 0x31,
            WM_GETHOTKEY = 0x33,
            WM_GETICON = 0x7F,
            WM_GETMINMAXINFO = 0x24,
            WM_GETOBJECT = 0x3D,
            WM_GETSYSMENU = 0x313,
            WM_GETTEXT = 0xD,
            WM_GETTEXTLENGTH = 0xE,
            WM_HANDHELDFIRST = 0x358,
            WM_HANDHELDLAST = 0x35F,
            WM_HELP = 0x53,
            WM_HOTKEY = 0x312,
            WM_HSCROLL = 0x114,
            WM_HSCROLLCLIPBOARD = 0x30E,
            WM_ICONERASEBKGND = 0x27,
            WM_IME_CHAR = 0x286,
            WM_IME_COMPOSITION = 0x10F,
            WM_IME_COMPOSITIONFULL = 0x284,
            WM_IME_CONTROL = 0x283,
            WM_IME_ENDCOMPOSITION = 0x10E,
            WM_IME_KEYDOWN = 0x290,
            WM_IME_KEYLAST = 0x10F,
            WM_IME_KEYUP = 0x291,
            WM_IME_NOTIFY = 0x282,
            WM_IME_REQUEST = 0x288,
            WM_IME_SELECT = 0x285,
            WM_IME_SETCONTEXT = 0x281,
            WM_IME_STARTCOMPOSITION = 0x10D,
            WM_INITDIALOG = 0x110,
            WM_INITMENU = 0x116,
            WM_INITMENUPOPUP = 0x117,
            WM_INPUTLANGCHANGE = 0x51,
            WM_INPUTLANGCHANGEREQUEST = 0x50,
            WM_KEYDOWN = 0x100,
            WM_KEYFIRST = 0x100,
            WM_KEYLAST = 0x108,
            WM_KEYUP = 0x101,
            WM_KILLFOCUS = 0x8,
            WM_LBUTTONDBLCLK = 0x203,
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_MBUTTONDBLCLK = 0x209,
            WM_MBUTTONDOWN = 0x207,
            WM_MBUTTONUP = 0x208,
            WM_MDIACTIVATE = 0x222,
            WM_MDICASCADE = 0x227,
            WM_MDICREATE = 0x220,
            WM_MDIDESTROY = 0x221,
            WM_MDIGETACTIVE = 0x229,
            WM_MDIICONARRANGE = 0x228,
            WM_MDIMAXIMIZE = 0x225,
            WM_MDINEXT = 0x224,
            WM_MDIREFRESHMENU = 0x234,
            WM_MDIRESTORE = 0x223,
            WM_MDISETMENU = 0x230,
            WM_MDITILE = 0x226,
            WM_MEASUREITEM = 0x2C,
            WM_MENUCHAR = 0x120,
            WM_MENUCOMMAND = 0x126,
            WM_MENUDRAG = 0x123,
            WM_MENUGETOBJECT = 0x124,
            WM_MENURBUTTONUP = 0x122,
            WM_MENUSELECT = 0x11F,
            WM_MOUSEACTIVATE = 0x21,
            WM_MOUSEFIRST = 0x200,
            WM_MOUSEHOVER = 0x2A1,
            WM_MOUSELAST = 0x20A,
            WM_MOUSELEAVE = 0x2A3,
            WM_MOUSEMOVE = 0x200,
            WM_MOUSEWHEEL = 0x20A,
            WM_MOVE = 0x3,
            WM_MOVING = 0x216,
            WM_NCACTIVATE = 0x86,
            WM_NCCALCSIZE = 0x83,
            WM_NCCREATE = 0x81,
            WM_NCDESTROY = 0x82,
            WM_NCHITTEST = 0x84,
            WM_NCLBUTTONDBLCLK = 0xA3,
            WM_NCLBUTTONDOWN = 0xA1,
            WM_NCLBUTTONUP = 0xA2,
            WM_NCMBUTTONDBLCLK = 0xA9,
            WM_NCMBUTTONDOWN = 0xA7,
            WM_NCMBUTTONUP = 0xA8,
            WM_NCMOUSEHOVER = 0x2A0,
            WM_NCMOUSELEAVE = 0x2A2,
            WM_NCMOUSEMOVE = 0xA0,
            WM_NCPAINT = 0x85,
            WM_NCRBUTTONDBLCLK = 0xA6,
            WM_NCRBUTTONDOWN = 0xA4,
            WM_NCRBUTTONUP = 0xA5,
            WM_NEXTDLGCTL = 0x28,
            WM_NEXTMENU = 0x213,
            WM_NOTIFY = 0x4E,
            WM_NOTIFYFORMAT = 0x55,
            WM_NULL = 0x0,
            WM_PAINT = 0xF,
            WM_PAINTCLIPBOARD = 0x309,
            WM_PAINTICON = 0x26,
            WM_PALETTECHANGED = 0x311,
            WM_PALETTEISCHANGING = 0x310,
            WM_PARENTNOTIFY = 0x210,
            WM_PASTE = 0x302,
            WM_PENWINFIRST = 0x380,
            WM_PENWINLAST = 0x38F,
            WM_POWER = 0x48,
            WM_PRINT = 0x317,
            WM_PRINTCLIENT = 0x318,
            WM_QUERYDRAGICON = 0x37,
            WM_QUERYENDSESSION = 0x11,
            WM_QUERYNEWPALETTE = 0x30F,
            WM_QUERYOPEN = 0x13,
            WM_QUERYUISTATE = 0x129,
            WM_QUEUESYNC = 0x23,
            WM_QUIT = 0x12,
            WM_RBUTTONDBLCLK = 0x206,
            WM_RBUTTONDOWN = 0x204,
            WM_RBUTTONUP = 0x205,
            WM_RENDERALLFORMATS = 0x306,
            WM_RENDERFORMAT = 0x305,
            WM_SETCURSOR = 0x20,
            WM_SETFOCUS = 0x7,
            WM_SETFONT = 0x30,
            WM_SETHOTKEY = 0x32,
            WM_SETICON = 0x80,
            WM_SETREDRAW = 0xB,
            WM_SETTEXT = 0xC,
            WM_SETTINGCHANGE = 0x1A,
            WM_SHOWWINDOW = 0x18,
            WM_SIZE = 0x5,
            WM_SIZECLIPBOARD = 0x30B,
            WM_SIZING = 0x214,
            WM_SPOOLERSTATUS = 0x2A,
            WM_STYLECHANGED = 0x7D,
            WM_STYLECHANGING = 0x7C,
            WM_SYNCPAINT = 0x88,
            WM_SYSCHAR = 0x106,
            WM_SYSCOLORCHANGE = 0x15,
            WM_SYSCOMMAND = 0x112,
            WM_SYSDEADCHAR = 0x107,
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_SYSTIMER = 0x118, // undocumented, see http://support.microsoft.com/?id=108938
            WM_TCARD = 0x52,
            WM_TIMECHANGE = 0x1E,
            WM_TIMER = 0x113,
            WM_UNDO = 0x304,
            WM_UNINITMENUPOPUP = 0x125,
            WM_USER = 0x400,
            WM_USERCHANGED = 0x54,
            WM_VKEYTOITEM = 0x2E,
            WM_VSCROLL = 0x115,
            WM_VSCROLLCLIPBOARD = 0x30A,
            WM_WINDOWPOSCHANGED = 0x47,
            WM_WINDOWPOSCHANGING = 0x46,
            WM_WININICHANGE = 0x1A,
            WM_XBUTTONDBLCLK = 0x20D,
            WM_XBUTTONDOWN = 0x20B,
            WM_XBUTTONUP = 0x20C
        }

        #endregion

        // ReSharper restore UnusedMemberInPrivateClass
        #endregion
    }
}