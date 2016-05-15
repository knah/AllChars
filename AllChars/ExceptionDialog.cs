using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace AllCharsNS
{
	/// <summary>
	/// Summary description for ExceptionDialog.
	/// </summary>
	public class ExceptionDialog : Form
	{
		private Label label1;
        private Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
        private Container components = null;
        private Button closeButton;
        private TextBox eDetailInfo;
        private Button copyButton;
        private LinkLabel bugLink;

		private Exception exception;

		private ExceptionDialog(Exception exception)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            bugLink.Links.Add(0, bugLink.Text.Length, bugLink.Text);
			
			this.exception = exception;

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            eDetailInfo.Text = "AllChars Version " + version.ToString() +"\r\n"+ exceptionInfo(exception);
		}

        internal static string unravelException(Exception exception)
		{
			string unraveled = exception.StackTrace;
			if (exception.InnerException != null) 
			{
				Exception inner = exception.InnerException;
				return unraveled+"\r\n\r\nInner Exception ("+inner.GetType().FullName+") message:\r\n"+
					inner.Message+"\r\n\r\nStack Trace:\r\n"+unravelException(inner);
			}
			return unraveled;
		}
		
		internal static string exceptionInfo(Exception exception)
		{
            return "-- EXCEPTION -- type "+exception.GetType().FullName+"\r\n"+
                "Message:\r\n------------------------------------------------------------\r\n"+
                exception.Message + "\r\n------------------------------------------------------------\r\n" +
                "Stack trace:\r\n"+unravelException(exception)+"\r\n";
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

		public static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
		{
			ExceptionDialog me = new ExceptionDialog(e.Exception);
		    me.ShowDialog();
			me.Dispose();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.eDetailInfo = new System.Windows.Forms.TextBox();
            this.copyButton = new System.Windows.Forms.Button();
            this.bugLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(496, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "An unhandled error occured within AllChars. We are sorry for any inconvenience..." +
    "";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(496, 32);
            this.label2.TabIndex = 1;
            this.label2.Text = "To help us improve AllChars, please submit a bug and provide a detailed descripti" +
    "on of what you were doing when the error occured; along with the information sho" +
    "wn below:";
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(433, 506);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 6;
            this.closeButton.Text = "Close";
            // 
            // eDetailInfo
            // 
            this.eDetailInfo.Location = new System.Drawing.Point(12, 86);
            this.eDetailInfo.Multiline = true;
            this.eDetailInfo.Name = "eDetailInfo";
            this.eDetailInfo.ReadOnly = true;
            this.eDetailInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.eDetailInfo.Size = new System.Drawing.Size(496, 414);
            this.eDetailInfo.TabIndex = 14;
            this.eDetailInfo.WordWrap = false;
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(12, 506);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(112, 23);
            this.copyButton.TabIndex = 15;
            this.copyButton.Text = "Copy to Clipboard";
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // bugLink
            // 
            this.bugLink.Location = new System.Drawing.Point(12, 64);
            this.bugLink.Name = "bugLink";
            this.bugLink.Size = new System.Drawing.Size(496, 19);
            this.bugLink.TabIndex = 16;
            this.bugLink.TabStop = true;
            this.bugLink.Text = "https://github.com/knah/AllChars/issues";
            this.bugLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bugLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.bugLink_LinkClicked);
            // 
            // ExceptionDialog
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(520, 541);
            this.Controls.Add(this.bugLink);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.eDetailInfo);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExceptionDialog";
            this.Text = "Sorry! This should not have happened...";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private void bugLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData.ToString());
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            int start, length;
            start = eDetailInfo.SelectionStart;
            length = eDetailInfo.SelectionLength;

            eDetailInfo.SelectAll();
            eDetailInfo.Copy();

            eDetailInfo.Select(start, length);
        }
	}
}
