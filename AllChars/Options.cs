using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace AllCharsNS
{
	/// <summary>
	/// Summary description for Options.
	/// </summary>
	public class Options : Form
	{
        private Label fileLabel;
		private GroupBox unknownGroup;
		private CheckBox tryReverseCBx;
		private CheckBox tryCaseInsensitiveCBx;
		private CheckBox beepCBx;
		private GroupBox integrationGroup;
		private CheckBox useSpeakerCBx;
		private GroupBox groupBox1;
		private CheckBox unixSemanticsCBx;
		private RadioButton ctrlBtn;
		private RadioButton shiftBtn;
		private RadioButton lCtrlBtn;
		private RadioButton rCtrlBtn;
		private RadioButton lShiftBtn;
		private RadioButton rShiftBtn;
		private RadioButton lAltBtn;
		private RadioButton rAltBtn;
		private RadioButton lWinBtn;
		private RadioButton rWinBtn;
		private RadioButton menuBtn;
		private RadioButton escapeBtn;
		private GroupBox extraGroup;
		private CheckBox translateDecimalCBx;
		private Button okButton;
		private Button cancelButton;
		private CheckBox sendUndefinedCBx;
		private CheckBox hideIconCBx;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;
        private RadioButton capsBtn;
        private TextBox fileName;

		private Configuration config;

		public Options()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		public Options(Configuration config)
		{
			InitializeComponent();
			
			this.config = config;
			fileName.Text = config.FileName;
			getConfig();
		}

		private void getConfig()
		{
			switch (config.HotKey) {
				case Keys.ControlKey:
					ctrlBtn.Checked = true;
					break;
				case Keys.LControlKey:
					lCtrlBtn.Checked = true;
					break;
				case Keys.RControlKey:
					rCtrlBtn.Checked = true;
					break;
				case Keys.ShiftKey:
					shiftBtn.Checked = true;
					break;
				case Keys.LShiftKey:
					lShiftBtn.Checked = true;
					break;
				case Keys.RShiftKey:
					rShiftBtn.Checked = true;
					break;
				case Keys.LMenu:
					lAltBtn.Checked = true;
					break;
				case Keys.RMenu:
					rAltBtn.Checked = true;
					break;
				case Keys.LWin:
					lWinBtn.Checked = true;
					break;
				case Keys.RWin:
					rWinBtn.Checked = true;
					break;
				case Keys.Escape:
					escapeBtn.Checked = true;
					break;
				case Keys.Apps:
					menuBtn.Checked = true;
					break;
                case Keys.CapsLock:
			        capsBtn.Checked = true;
			        break;
			}

			tryReverseCBx.Checked = config.TryReverse;
			tryCaseInsensitiveCBx.Checked = config.TryCaseInsensitive;
			sendUndefinedCBx.Checked = config.SendUndefined;
			beepCBx.Checked = config.UseBeeps;
			useSpeakerCBx.Checked = config.UseSpeaker;
			hideIconCBx.Checked = config.HideTaskbarIcon;
			unixSemanticsCBx.Checked = config.UnixSemantics;
			translateDecimalCBx.Checked = config.TranslateDecimal;
		}

		private void setConfig() {
            if (ctrlBtn.Checked) config.HotKey = Keys.ControlKey;
            else if (lCtrlBtn.Checked) config.HotKey = Keys.LControlKey;
            else if (rCtrlBtn.Checked) config.HotKey = Keys.RControlKey;
            else if (shiftBtn.Checked) config.HotKey = Keys.ShiftKey;
            else if (lShiftBtn.Checked) config.HotKey = Keys.LShiftKey;
            else if (rShiftBtn.Checked) config.HotKey = Keys.RShiftKey;
            else if (lAltBtn.Checked) config.HotKey = Keys.LMenu;
            else if (rAltBtn.Checked) config.HotKey = Keys.RMenu;
            else if (lWinBtn.Checked) config.HotKey = Keys.LWin;
            else if (rWinBtn.Checked) config.HotKey = Keys.RWin;
            else if (escapeBtn.Checked) config.HotKey = Keys.Escape;
            else if (menuBtn.Checked) config.HotKey = Keys.Apps;
            else if (capsBtn.Checked) config.HotKey = Keys.CapsLock;
			
			config.TryReverse = tryReverseCBx.Checked;
			config.TryReverse = tryReverseCBx.Checked;
			config.TryCaseInsensitive = tryCaseInsensitiveCBx.Checked;
			config.SendUndefined = sendUndefinedCBx.Checked;
			config.UseBeeps = beepCBx.Checked;
			config.UseSpeaker = useSpeakerCBx.Checked;
			config.HideTaskbarIcon = hideIconCBx.Checked;
			config.UnixSemantics = unixSemanticsCBx.Checked;
			config.TranslateDecimal = translateDecimalCBx.Checked;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.fileLabel = new System.Windows.Forms.Label();
            this.unknownGroup = new System.Windows.Forms.GroupBox();
            this.beepCBx = new System.Windows.Forms.CheckBox();
            this.sendUndefinedCBx = new System.Windows.Forms.CheckBox();
            this.tryCaseInsensitiveCBx = new System.Windows.Forms.CheckBox();
            this.tryReverseCBx = new System.Windows.Forms.CheckBox();
            this.integrationGroup = new System.Windows.Forms.GroupBox();
            this.hideIconCBx = new System.Windows.Forms.CheckBox();
            this.useSpeakerCBx = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.capsBtn = new System.Windows.Forms.RadioButton();
            this.escapeBtn = new System.Windows.Forms.RadioButton();
            this.menuBtn = new System.Windows.Forms.RadioButton();
            this.rWinBtn = new System.Windows.Forms.RadioButton();
            this.lWinBtn = new System.Windows.Forms.RadioButton();
            this.rAltBtn = new System.Windows.Forms.RadioButton();
            this.lAltBtn = new System.Windows.Forms.RadioButton();
            this.rShiftBtn = new System.Windows.Forms.RadioButton();
            this.lShiftBtn = new System.Windows.Forms.RadioButton();
            this.rCtrlBtn = new System.Windows.Forms.RadioButton();
            this.lCtrlBtn = new System.Windows.Forms.RadioButton();
            this.shiftBtn = new System.Windows.Forms.RadioButton();
            this.ctrlBtn = new System.Windows.Forms.RadioButton();
            this.unixSemanticsCBx = new System.Windows.Forms.CheckBox();
            this.extraGroup = new System.Windows.Forms.GroupBox();
            this.translateDecimalCBx = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.fileName = new System.Windows.Forms.TextBox();
            this.unknownGroup.SuspendLayout();
            this.integrationGroup.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.extraGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileLabel
            // 
            this.fileLabel.Location = new System.Drawing.Point(5, 9);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(354, 16);
            this.fileLabel.TabIndex = 0;
            this.fileLabel.Text = "Configuration File (dobule-click to show in Explorer):";
            this.fileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // unknownGroup
            // 
            this.unknownGroup.Controls.Add(this.beepCBx);
            this.unknownGroup.Controls.Add(this.sendUndefinedCBx);
            this.unknownGroup.Controls.Add(this.tryCaseInsensitiveCBx);
            this.unknownGroup.Controls.Add(this.tryReverseCBx);
            this.unknownGroup.Location = new System.Drawing.Point(8, 66);
            this.unknownGroup.Name = "unknownGroup";
            this.unknownGroup.Size = new System.Drawing.Size(354, 72);
            this.unknownGroup.TabIndex = 2;
            this.unknownGroup.TabStop = false;
            this.unknownGroup.Text = "If compose sequence is unknown";
            // 
            // beepCBx
            // 
            this.beepCBx.Location = new System.Drawing.Point(160, 40);
            this.beepCBx.Name = "beepCBx";
            this.beepCBx.Size = new System.Drawing.Size(152, 24);
            this.beepCBx.TabIndex = 3;
            this.beepCBx.Text = "&Beep";
            // 
            // sendUndefinedCBx
            // 
            this.sendUndefinedCBx.Location = new System.Drawing.Point(160, 16);
            this.sendUndefinedCBx.Name = "sendUndefinedCBx";
            this.sendUndefinedCBx.Size = new System.Drawing.Size(152, 24);
            this.sendUndefinedCBx.TabIndex = 2;
            this.sendUndefinedCBx.Text = "&Output characters as-is";
            // 
            // tryCaseInsensitiveCBx
            // 
            this.tryCaseInsensitiveCBx.Location = new System.Drawing.Point(8, 40);
            this.tryCaseInsensitiveCBx.Name = "tryCaseInsensitiveCBx";
            this.tryCaseInsensitiveCBx.Size = new System.Drawing.Size(136, 24);
            this.tryCaseInsensitiveCBx.TabIndex = 1;
            this.tryCaseInsensitiveCBx.Text = "try &Case Insensitive";
            // 
            // tryReverseCBx
            // 
            this.tryReverseCBx.Enabled = false;
            this.tryReverseCBx.Location = new System.Drawing.Point(8, 16);
            this.tryReverseCBx.Name = "tryReverseCBx";
            this.tryReverseCBx.Size = new System.Drawing.Size(136, 24);
            this.tryReverseCBx.TabIndex = 0;
            this.tryReverseCBx.Text = "try &Reverse (broken)";
            // 
            // integrationGroup
            // 
            this.integrationGroup.Controls.Add(this.hideIconCBx);
            this.integrationGroup.Controls.Add(this.useSpeakerCBx);
            this.integrationGroup.Location = new System.Drawing.Point(8, 146);
            this.integrationGroup.Name = "integrationGroup";
            this.integrationGroup.Size = new System.Drawing.Size(354, 72);
            this.integrationGroup.TabIndex = 3;
            this.integrationGroup.TabStop = false;
            this.integrationGroup.Text = "System Integration";
            // 
            // hideIconCBx
            // 
            this.hideIconCBx.Enabled = false;
            this.hideIconCBx.Location = new System.Drawing.Point(8, 40);
            this.hideIconCBx.Name = "hideIconCBx";
            this.hideIconCBx.Size = new System.Drawing.Size(296, 24);
            this.hideIconCBx.TabIndex = 1;
            this.hideIconCBx.Text = "&Hide taskbar icon (type <Compose> A M for Menu)";
            // 
            // useSpeakerCBx
            // 
            this.useSpeakerCBx.Location = new System.Drawing.Point(8, 16);
            this.useSpeakerCBx.Name = "useSpeakerCBx";
            this.useSpeakerCBx.Size = new System.Drawing.Size(296, 24);
            this.useSpeakerCBx.TabIndex = 0;
            this.useSpeakerCBx.Text = "use &Speaker for Beeps";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.capsBtn);
            this.groupBox1.Controls.Add(this.escapeBtn);
            this.groupBox1.Controls.Add(this.menuBtn);
            this.groupBox1.Controls.Add(this.rWinBtn);
            this.groupBox1.Controls.Add(this.lWinBtn);
            this.groupBox1.Controls.Add(this.rAltBtn);
            this.groupBox1.Controls.Add(this.lAltBtn);
            this.groupBox1.Controls.Add(this.rShiftBtn);
            this.groupBox1.Controls.Add(this.lShiftBtn);
            this.groupBox1.Controls.Add(this.rCtrlBtn);
            this.groupBox1.Controls.Add(this.lCtrlBtn);
            this.groupBox1.Controls.Add(this.shiftBtn);
            this.groupBox1.Controls.Add(this.ctrlBtn);
            this.groupBox1.Controls.Add(this.unixSemanticsCBx);
            this.groupBox1.Location = new System.Drawing.Point(8, 226);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(354, 152);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Key to use as <Compose>";
            // 
            // capsBtn
            // 
            this.capsBtn.Location = new System.Drawing.Point(6, 90);
            this.capsBtn.Name = "capsBtn";
            this.capsBtn.Size = new System.Drawing.Size(82, 24);
            this.capsBtn.TabIndex = 13;
            this.capsBtn.Text = "CapsLock";
            // 
            // escapeBtn
            // 
            this.escapeBtn.Location = new System.Drawing.Point(250, 18);
            this.escapeBtn.Name = "escapeBtn";
            this.escapeBtn.Size = new System.Drawing.Size(72, 24);
            this.escapeBtn.TabIndex = 12;
            this.escapeBtn.Text = "Esc";
            // 
            // menuBtn
            // 
            this.menuBtn.Location = new System.Drawing.Point(250, 90);
            this.menuBtn.Name = "menuBtn";
            this.menuBtn.Size = new System.Drawing.Size(72, 24);
            this.menuBtn.TabIndex = 11;
            this.menuBtn.Text = "Menu";
            // 
            // rWinBtn
            // 
            this.rWinBtn.Location = new System.Drawing.Point(172, 90);
            this.rWinBtn.Name = "rWinBtn";
            this.rWinBtn.Size = new System.Drawing.Size(72, 24);
            this.rWinBtn.TabIndex = 10;
            this.rWinBtn.Text = "Win Right";
            // 
            // lWinBtn
            // 
            this.lWinBtn.Location = new System.Drawing.Point(94, 90);
            this.lWinBtn.Name = "lWinBtn";
            this.lWinBtn.Size = new System.Drawing.Size(72, 24);
            this.lWinBtn.TabIndex = 9;
            this.lWinBtn.Text = "Win Left";
            // 
            // rAltBtn
            // 
            this.rAltBtn.Location = new System.Drawing.Point(172, 66);
            this.rAltBtn.Name = "rAltBtn";
            this.rAltBtn.Size = new System.Drawing.Size(72, 24);
            this.rAltBtn.TabIndex = 8;
            this.rAltBtn.Text = "AltGr";
            // 
            // lAltBtn
            // 
            this.lAltBtn.Location = new System.Drawing.Point(94, 66);
            this.lAltBtn.Name = "lAltBtn";
            this.lAltBtn.Size = new System.Drawing.Size(72, 24);
            this.lAltBtn.TabIndex = 7;
            this.lAltBtn.Text = "Alt Left";
            // 
            // rShiftBtn
            // 
            this.rShiftBtn.Location = new System.Drawing.Point(172, 42);
            this.rShiftBtn.Name = "rShiftBtn";
            this.rShiftBtn.Size = new System.Drawing.Size(80, 24);
            this.rShiftBtn.TabIndex = 6;
            this.rShiftBtn.Text = "Shift Right";
            // 
            // lShiftBtn
            // 
            this.lShiftBtn.Location = new System.Drawing.Point(94, 42);
            this.lShiftBtn.Name = "lShiftBtn";
            this.lShiftBtn.Size = new System.Drawing.Size(72, 24);
            this.lShiftBtn.TabIndex = 5;
            this.lShiftBtn.Text = "Shift Left";
            // 
            // rCtrlBtn
            // 
            this.rCtrlBtn.Location = new System.Drawing.Point(172, 18);
            this.rCtrlBtn.Name = "rCtrlBtn";
            this.rCtrlBtn.Size = new System.Drawing.Size(80, 24);
            this.rCtrlBtn.TabIndex = 4;
            this.rCtrlBtn.Text = "Ctrl Right";
            // 
            // lCtrlBtn
            // 
            this.lCtrlBtn.Location = new System.Drawing.Point(94, 18);
            this.lCtrlBtn.Name = "lCtrlBtn";
            this.lCtrlBtn.Size = new System.Drawing.Size(72, 24);
            this.lCtrlBtn.TabIndex = 3;
            this.lCtrlBtn.Text = "Ctrl Left";
            // 
            // shiftBtn
            // 
            this.shiftBtn.Location = new System.Drawing.Point(6, 43);
            this.shiftBtn.Name = "shiftBtn";
            this.shiftBtn.Size = new System.Drawing.Size(48, 24);
            this.shiftBtn.TabIndex = 2;
            this.shiftBtn.Text = "Shift";
            // 
            // ctrlBtn
            // 
            this.ctrlBtn.Location = new System.Drawing.Point(6, 19);
            this.ctrlBtn.Name = "ctrlBtn";
            this.ctrlBtn.Size = new System.Drawing.Size(48, 24);
            this.ctrlBtn.TabIndex = 1;
            this.ctrlBtn.Text = "Ctrl";
            // 
            // unixSemanticsCBx
            // 
            this.unixSemanticsCBx.Location = new System.Drawing.Point(8, 120);
            this.unixSemanticsCBx.Name = "unixSemanticsCBx";
            this.unixSemanticsCBx.Size = new System.Drawing.Size(296, 24);
            this.unixSemanticsCBx.TabIndex = 0;
            this.unixSemanticsCBx.Text = "*nix-Feelalike";
            // 
            // extraGroup
            // 
            this.extraGroup.Controls.Add(this.translateDecimalCBx);
            this.extraGroup.Location = new System.Drawing.Point(8, 386);
            this.extraGroup.Name = "extraGroup";
            this.extraGroup.Size = new System.Drawing.Size(354, 48);
            this.extraGroup.TabIndex = 5;
            this.extraGroup.TabStop = false;
            this.extraGroup.Text = "Extra";
            // 
            // translateDecimalCBx
            // 
            this.translateDecimalCBx.Location = new System.Drawing.Point(8, 16);
            this.translateDecimalCBx.Name = "translateDecimalCBx";
            this.translateDecimalCBx.Size = new System.Drawing.Size(296, 24);
            this.translateDecimalCBx.TabIndex = 1;
            this.translateDecimalCBx.Text = "translate numeric keypad &Period into decimal symbol";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(206, 440);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(287, 440);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "&Cancel";
            // 
            // fileName
            // 
            this.fileName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fileName.Location = new System.Drawing.Point(8, 28);
            this.fileName.Multiline = true;
            this.fileName.Name = "fileName";
            this.fileName.ReadOnly = true;
            this.fileName.Size = new System.Drawing.Size(354, 34);
            this.fileName.TabIndex = 9;
            this.fileName.DoubleClick += new System.EventHandler(this.fileName_DoubleClick);
            // 
            // Options
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(374, 473);
            this.Controls.Add(this.fileName);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.extraGroup);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.integrationGroup);
            this.Controls.Add(this.unknownGroup);
            this.Controls.Add(this.fileLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.Text = "AllChars Configuration";
            this.unknownGroup.ResumeLayout(false);
            this.integrationGroup.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.extraGroup.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void okButton_Click(object sender, EventArgs e) {
			setConfig();
			config.Save();
			Close();
		}

        private void fileName_DoubleClick(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "/e,/select,\"" + config.FileName +"\"");
        }

	}
}
