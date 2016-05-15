using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AllCharsNS
{

    #region Keyboard handling helper data types
    public enum KeyKind : byte {
        VirtKey,
        Unicode
    }

    [StructLayout(LayoutKind.Explicit, Size = 16, CharSet = CharSet.Ansi)]
    public struct KeyInput {
        [FieldOffset(0)] public KeyKind Kind;
        [FieldOffset(1)] public Keys VirtKey;
        [FieldOffset(1)] public char Char;

        public KeyInput(Keys virtKey) {
            Kind = KeyKind.VirtKey;
            Char = char.MinValue;
            VirtKey = virtKey;
        }

        public KeyInput(char c) {
            Kind = KeyKind.Unicode;
            VirtKey = Keys.None;
            Char = c;
        }
    }
    #endregion

    public class Native
    {

        #region Keyboard hooking and handling

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal int dwFlags;
            internal int time;
            internal IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            internal short wVk;
            internal short wScan;
            internal int dwFlags;
            internal int time;
            internal IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT64
        {
            [FieldOffset(0)]
            internal int type;
            [FieldOffset(8)]
            internal MOUSEINPUT mi;
            [FieldOffset(8)]
            internal KEYBDINPUT ki;
            [FieldOffset(8)]
            internal HARDWAREINPUT hi;            
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT32
        {
            [FieldOffset(0)]
            internal int type;
            [FieldOffset(4)]
            internal MOUSEINPUT mi;
            [FieldOffset(4)]
            internal KEYBDINPUT ki;
            [FieldOffset(4)]
            internal HARDWAREINPUT hi;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, ref INPUT32 pInputs, int cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, ref INPUT64 pInputs, int cbSize);

        private static uint SendKeyboardInput(KEYBDINPUT kbdInput)
        {
            switch (IntPtr.Size)
            {
                case 4: // 32bit
                    INPUT32[] inputs32 = new INPUT32[1];
                    inputs32[0].type = 1; // INPUT_KEYBOARD
                    inputs32[0].ki = kbdInput;
                    return SendInput((uint)inputs32.Length, ref inputs32[0], Marshal.SizeOf(typeof(INPUT32)));
                case 8: // 64bit
                    INPUT64[] inputs64 = new INPUT64[1];
                    inputs64[0].type = 1; // INPUT_KEYBOARD
                    inputs64[0].ki = kbdInput;
                    return SendInput((uint)inputs64.Length, ref inputs64[0], Marshal.SizeOf(typeof(INPUT64)));
                default:
                    Debug.WriteLine("SendKeyboardInput: Unknown pointer size "+IntPtr.Size);
                    return 0;
            }
        }

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetMessageExtraInfo();

        public static void SendChars(char[] chars)
        {
            KEYBDINPUT ki;

            for (int i = 0; i < chars.Length; i++ )
            {
                ki.wVk = 0;
                ki.wScan = (short)chars[i];
                ki.dwFlags = 4; // KEYEVENTF_UNICODE
                ki.time = 0;
                ki.dwExtraInfo = GetMessageExtraInfo();
                uint result = SendKeyboardInput(ki);
                Debug.WriteLine("Sending unicode inputs: " + result);
                if (result == 0)
                    Debug.WriteLine("  Win32 LastError: " + Marshal.GetLastWin32Error());
            }
        }

        public static void BSandSendKeys(int nBS, KeyInput[] keyInputs)
        {
            KEYBDINPUT ki;

            int i;
            for (i = 0; i < nBS; i ++)
            {
                ki.wVk = 8; // VK_BACK (backspace)
                ki.wScan = 0;
                ki.dwFlags = 0;
                ki.time = 0;
                ki.dwExtraInfo = GetMessageExtraInfo();
                SendKeyboardInput(ki);

                ki.wVk = 8; // VK_BACK (backspace)
                ki.wScan = 0;
                ki.dwFlags = 2; // KEYEVENTF_KEYUP
                ki.time = 0;
                ki.dwExtraInfo = GetMessageExtraInfo();
                SendKeyboardInput(ki);
            }

            for (int k = 0; k < keyInputs.Length; k++)
            {
                KeyInput currentKey = keyInputs[k];
                switch (currentKey.Kind)
                {
                    case KeyKind.VirtKey:
                        ki.wVk = (short)currentKey.VirtKey;
                        ki.wScan = 0;
                        ki.dwFlags = 0;
                        ki.time = 0;
                        ki.dwExtraInfo = GetMessageExtraInfo();
                        SendKeyboardInput(ki);

                        ki.wVk = (short)currentKey.VirtKey;
                        ki.wScan = 0;
                        ki.dwFlags = 2; // KEYEVENTF_KEYUP
                        ki.time = 0;
                        ki.dwExtraInfo = GetMessageExtraInfo();
                        SendKeyboardInput(ki);
                        break;
                    case KeyKind.Unicode:
                        ki.wVk = 0;
                        ki.wScan = (short)currentKey.Char;
                        ki.dwFlags = 4; // KEYEVENTF_UNICODE
                        ki.time = 0;
                        ki.dwExtraInfo = GetMessageExtraInfo();
                        SendKeyboardInput(ki);
                        break;
                }
            }
        }

        internal static void SendChar(char x)
        {
            SendChars(new char[] { x });
        }

    #endregion

        #region MessageBeep

        internal enum MessageBeepType
        {
            Default = -1,
            Ok = 0x00000000,
            Error = 0x00000010,
            Question = 0x00000020,
            Warning = 0x00000030,
            Information = 0x00000040
        }

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "MessageBeep")]
        internal static extern bool Beep(MessageBeepType type);

        #endregion

        #region Finding focused control helper

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        #endregion

        #region Resident memory consumption reduction helper

        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        internal static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }

        #endregion

        #region WM_PAINT suppression helper

        public static void SuspendPainting()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}