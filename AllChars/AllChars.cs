using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using AllCharsNS.Properties;

namespace AllCharsNS {
    internal enum TaskbarState {
        Inactive,
        WaitFirst,
        WaitSecond
    }

    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class AllChars : Form {
        private readonly Configuration config;
        private readonly Translator translator;
        private Mutex acMutex;
        private Button bombButton;
        private IContainer components;
        private MenuItem composeOutput;
        private MenuItem cOutHTML;
        private MenuItem cOutUnicode;
        private Label label1;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private MenuItem menuAbout;
        private MenuItem menuConfig;
        private MenuItem menuExit;
        private MenuItem menuItem1;
        private MenuItem menuItem2;
        private AllCharsHook myHook;
        private Button okButton;
        private PictureBox pictureBox1;
        private MenuItem showCompose;
        private NotifyIcon taskbarIcon;
        private ContextMenu taskbarMenu;
        private LinkLabel linkLabel3;
        private Label versionLabel;

        public AllChars(string configFileName) {
            acMutex = new Mutex(false, "AllChars40");

            if (!acMutex.WaitOne(0, false)) {
                Debug.WriteLine("Contention on 'AllChars40' mutex");
                acMutex.Close();
                acMutex = null;
                return;
            }

            config = new Configuration(configFileName);
            translator = new Translator(config);

            InitializeComponent();

            linkLabel1.Links.Add(0, linkLabel1.Text.Length, linkLabel1.Text);
            linkLabel2.Links.Add(0, linkLabel2.Text.Length, linkLabel2.Text);
            linkLabel3.Links.Add(0, linkLabel3.Text.Length, linkLabel3.Text);

            versionLabel.Text = "Version "+Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) components.Dispose();
                acMutex.ReleaseMutex();
                acMutex.Close();
                acMutex = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main() {
            Application.ThreadException += ExceptionDialog.ThreadExceptionHandler;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string configFileName =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AllChars.xml");
            try {
                AllChars me = new AllChars(configFileName);
                if (me.acMutex == null) {
                    MessageBox.Show(
                        "A version of AllChars is already running on your computer.\r\nAllChars can only run once per system.\r\nPlease exit the running instance of AllCharsNS to use this version.",
                        "AllChars already running", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, 0, false);
                    Application.Exit();
                    return;
                }
                if (Process.GetProcessesByName("AllChars").Length > 1) {
                    MessageBox.Show(
                        "A version of AllChars appears to be running on your computer.\r\nAllChars can only run once per system.\r\nPlease end the running 'AllCharsNS' process to use this version.",
                        "AllChars already running", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, 0, false);
                    Application.Exit();
                    return;
                }

                me.Visible = false;
                me.Initialize();

                // this would reduce the *apparent* memory usage (working set size as reported by TaskManager),
                // but IMO is - if anything - detrimental for actual memory consumption or performance.
                // Native.FlushMemory();

                Application.Run();
            }
            catch (ConfigurationVersionException cve) {
                switch (cve.Disposition) {
                    case VersionDisposition.Unversioned:
                        MessageBox.Show(
                            string.Format(
                                "The version within the configuration file ('{0}') could not be determined.\nIt is probably too old to be used with this version of AllCharsNS.\n\nAllChars will now terminate.",
                                configFileName),
                            "Configuration version unknown", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, 0, false);
                        break;
                    case VersionDisposition.Invalid:
                        MessageBox.Show(
                            string.Format(
                                "The version within the configuration file ('{0}') is not readable.\nIt is probably corrupt.\n\nAllChars will now terminate.",
                                configFileName),
                            "Configuration version unreadable", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, 0, false);
                        break;
                    case VersionDisposition.OlderUnconvertable:
                        MessageBox.Show(
                            string.Format(
                                "The version within the configuration file ('{0}') is older than expected.\nCurrently, you will have to manually update the configuration file to the new version.\n\nAllChars will now terminate.",
                                configFileName),
                            "Configuration version too old", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, 0, false);
                        break;
                    case VersionDisposition.Newer:
                        MessageBox.Show(
                            string.Format(
                                "The version within the configuration file ('{0}') is newer than expected.\nThis may have happened because you tried a newer version and then went back to this older version.\nYou will have to manually adapt the configuration file.\n\nAllChars will now terminate.",
                                configFileName),
                            "Configuration version newer than application", MessageBoxButtons.OK, MessageBoxIcon.Error,
                            0, 0, false);
                        break;
                }
                Application.Exit();
            }
            catch (XmlException xe) {
                MessageBox.Show(
                    "An XML error was detected when reading the configuration file ("+configFileName+
                    "). Please fix the configuration file and restart AllChars.\r\n\r\n"+
                    "XML error details follow:\r\n"+xe+" / "+xe.Message+"\r\n"+xe.StackTrace,
                    "AllChars XML Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, 0, false);
                Application.Exit();
            }
            catch (Exception e) {
                new ThreadExceptionDialog(e).ShowDialog();
            }
        }

        private void Initialize() {
            myHook = new AllCharsHook(this, config, translator);

            taskbarIcon.Icon = Resources.Inactive;
        }

        internal void SetNotify(TaskbarState state) {
            switch (state) {
                case TaskbarState.Inactive:
                    taskbarIcon.Icon = Resources.Inactive;
                    break;
                case TaskbarState.WaitFirst:
                    taskbarIcon.Icon = Resources.WaitFirst;
                    break;
                case TaskbarState.WaitSecond:
                    taskbarIcon.Icon = Resources.WaitSecond;
                    break;
            }
        }

        private void menuExit_Click(object sender, EventArgs e) {
            taskbarIcon.Visible = false;
            Application.Exit();
        }

        private void ShowWindow() {
            if (!Visible) {
                WindowState = FormWindowState.Normal;
                Visible = true;
            } else
                Activate();
        }

        private void HideWindow() {
            Visible = false;
            WindowState = FormWindowState.Minimized;
        }

        private void TaskbarIcon_DoubleClick(object sender, EventArgs e) {
            ShowWindow();
        }


        private void okButton_Click(object sender, EventArgs e) {
            HideWindow();
        }

        private void menuConfig_Click(object sender, EventArgs e) {
            (new Options(config)).ShowDialog();
        }

        private void menuAbout_Click(object sender, EventArgs e) {
            ShowWindow();
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(e.Link.LinkData.ToString());
        }

        private void bombButton_Click(object sender, EventArgs e) {
            throw new Exception("Exploding...");
        }

        private void showCompose_Click(object sender, EventArgs e) {
            (new ComposeHelp(config)).Show();
        }

        private void cOutUnicode_Click(object sender, EventArgs e) {
            cOutUnicode.Checked = true;
            cOutHTML.Checked = false;
            translator.Mode = TranslationMode.Unicode;
        }

        private void cOutHTML_Click(object sender, EventArgs e) {
            cOutHTML.Checked = true;
            cOutUnicode.Checked = false;
            translator.Mode = TranslationMode.HTMLEntity;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AllChars));
            this.taskbarIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.taskbarMenu = new System.Windows.Forms.ContextMenu();
            this.menuConfig = new System.Windows.Forms.MenuItem();
            this.showCompose = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.composeOutput = new System.Windows.Forms.MenuItem();
            this.cOutUnicode = new System.Windows.Forms.MenuItem();
            this.cOutHTML = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuAbout = new System.Windows.Forms.MenuItem();
            this.menuExit = new System.Windows.Forms.MenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.bombButton = new System.Windows.Forms.Button();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // taskbarIcon
            // 
            this.taskbarIcon.ContextMenu = this.taskbarMenu;
            this.taskbarIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("taskbarIcon.Icon")));
            this.taskbarIcon.Text = "AllChars";
            this.taskbarIcon.Visible = true;
            this.taskbarIcon.DoubleClick += new System.EventHandler(this.TaskbarIcon_DoubleClick);
            // 
            // taskbarMenu
            // 
            this.taskbarMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuConfig,
            this.showCompose,
            this.menuItem1,
            this.composeOutput,
            this.menuItem2,
            this.menuAbout,
            this.menuExit});
            this.taskbarMenu.Popup += new System.EventHandler(this.taskbarMenu_Popup);
            // 
            // menuConfig
            // 
            this.menuConfig.Index = 0;
            this.menuConfig.Text = "Configuration...";
            this.menuConfig.Click += new System.EventHandler(this.menuConfig_Click);
            // 
            // showCompose
            // 
            this.showCompose.Index = 1;
            this.showCompose.Text = "Show compose sequences...";
            this.showCompose.Click += new System.EventHandler(this.showCompose_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            this.menuItem1.Text = "-";
            // 
            // composeOutput
            // 
            this.composeOutput.Index = 3;
            this.composeOutput.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.cOutUnicode,
            this.cOutHTML});
            this.composeOutput.Text = "Compose Output";
            // 
            // cOutUnicode
            // 
            this.cOutUnicode.Checked = true;
            this.cOutUnicode.Index = 0;
            this.cOutUnicode.RadioCheck = true;
            this.cOutUnicode.Text = "Unicode Character";
            this.cOutUnicode.Click += new System.EventHandler(this.cOutUnicode_Click);
            // 
            // cOutHTML
            // 
            this.cOutHTML.Index = 1;
            this.cOutHTML.RadioCheck = true;
            this.cOutHTML.Text = "HTML Entity";
            this.cOutHTML.Click += new System.EventHandler(this.cOutHTML_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 4;
            this.menuItem2.Text = "-";
            // 
            // menuAbout
            // 
            this.menuAbout.Index = 5;
            this.menuAbout.Text = "About AllChars";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // menuExit
            // 
            this.menuExit.Index = 6;
            this.menuExit.Text = "Exit AllChars";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(8, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 40);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(56, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "AllChars for Windows";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // versionLabel
            // 
            this.versionLabel.Location = new System.Drawing.Point(104, 32);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(184, 16);
            this.versionLabel.TabIndex = 2;
            this.versionLabel.Text = "Version <...>";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(40, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(216, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Licensed under GNU GPL 3";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(40, 176);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(224, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "By Joachim Breuer and Alex Grasser";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(40, 266);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(224, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "Based on AllChars 1 - 3";
            // 
            // linkLabel1
            // 
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.Location = new System.Drawing.Point(40, 192);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(224, 16);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://allchars.zwolnet.com/";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(40, 282);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(224, 16);
            this.label6.TabIndex = 7;
            this.label6.Text = "© 1994-2007 by Jeroen Laarhoven";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(211, 301);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 24);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // linkLabel2
            // 
            this.linkLabel2.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel2.Location = new System.Drawing.Point(40, 208);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(224, 16);
            this.linkLabel2.TabIndex = 9;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "http://www.sourceforge.net/projects/allchars";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 60F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.label7.Location = new System.Drawing.Point(8, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(280, 96);
            this.label7.TabIndex = 10;
            this.label7.Text = "Beta";
            this.label7.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // bombButton
            // 
            this.bombButton.Location = new System.Drawing.Point(8, 302);
            this.bombButton.Name = "bombButton";
            this.bombButton.Size = new System.Drawing.Size(75, 23);
            this.bombButton.TabIndex = 11;
            this.bombButton.Text = "*Bomb*";
            this.bombButton.Visible = false;
            this.bombButton.Click += new System.EventHandler(this.bombButton_Click);
            // 
            // linkLabel3
            // 
            this.linkLabel3.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel3.Location = new System.Drawing.Point(40, 224);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(224, 16);
            this.linkLabel3.TabIndex = 9;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "https://github.com/knah/AllChars";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // AllChars
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(298, 337);
            this.ControlBox = false;
            this.Controls.Add(this.bombButton);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label7);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AllChars";
            this.ShowInTaskbar = false;
            this.Text = "AllChars";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private void taskbarMenu_Popup(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}