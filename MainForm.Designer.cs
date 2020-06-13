/*
 * LFD Image Reader.exe, Exports images and animation frames from LFD resources
 * Copyright (C) 2008-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Version: 1.2
 */
 
namespace Idmr.LfdImageReader
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.txtFile = new System.Windows.Forms.TextBox();
			this.cmdFile = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.lstLFD = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cmdShow = new System.Windows.Forms.Button();
			this.cmdExit = new System.Windows.Forms.Button();
			this.opnFile = new System.Windows.Forms.OpenFileDialog();
			this.opnTie = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// txtFile
			// 
			this.txtFile.Location = new System.Drawing.Point(8, 32);
			this.txtFile.Name = "txtFile";
			this.txtFile.Size = new System.Drawing.Size(226, 20);
			this.txtFile.TabIndex = 0;
			// 
			// cmdFile
			// 
			this.cmdFile.Location = new System.Drawing.Point(240, 29);
			this.cmdFile.Name = "cmdFile";
			this.cmdFile.Size = new System.Drawing.Size(48, 24);
			this.cmdFile.TabIndex = 1;
			this.cmdFile.Text = "&Open";
			this.cmdFile.Click += new System.EventHandler(this.cmdFile_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "LFD File";
			// 
			// lstLFD
			// 
			this.lstLFD.Enabled = false;
			this.lstLFD.Location = new System.Drawing.Point(8, 88);
			this.lstLFD.Name = "lstLFD";
			this.lstLFD.Size = new System.Drawing.Size(168, 186);
			this.lstLFD.TabIndex = 3;
			this.lstLFD.SelectedIndexChanged += new System.EventHandler(this.lstLFD_SelectedIndexChanged);
			this.lstLFD.DoubleClick += new System.EventHandler(this.lstLFD_DoubleClick);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(152, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "DELT and ANIM resources";
			// 
			// cmdShow
			// 
			this.cmdShow.Enabled = false;
			this.cmdShow.Location = new System.Drawing.Point(200, 104);
			this.cmdShow.Name = "cmdShow";
			this.cmdShow.Size = new System.Drawing.Size(88, 32);
			this.cmdShow.TabIndex = 5;
			this.cmdShow.Text = "&Show";
			this.cmdShow.Click += new System.EventHandler(this.cmdShow_Click);
			// 
			// cmdExit
			// 
			this.cmdExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdExit.Location = new System.Drawing.Point(200, 160);
			this.cmdExit.Name = "cmdExit";
			this.cmdExit.Size = new System.Drawing.Size(88, 32);
			this.cmdExit.TabIndex = 5;
			this.cmdExit.Text = "E&xit";
			this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
			// 
			// opnFile
			// 
			this.opnFile.DefaultExt = "LFD";
			this.opnFile.Filter = "LFD Files|*.lfd";
			this.opnFile.FileOk += new System.ComponentModel.CancelEventHandler(this.opnFile_FileOk);
			// 
			// opnTie
			// 
			this.opnTie.DefaultExt = "exe";
			this.opnTie.Filter = "TIE95|TIE95.exe|TIE original|TIE.exe";
			this.opnTie.FileOk += new System.ComponentModel.CancelEventHandler(this.opnTie_FileOk);
			// 
			// MainForm
			// 
			this.AcceptButton = this.cmdShow;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdExit;
			this.ClientSize = new System.Drawing.Size(304, 313);
			this.Controls.Add(this.cmdShow);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lstLFD);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cmdFile);
			this.Controls.Add(this.txtFile);
			this.Controls.Add(this.cmdExit);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "LFD Image Reader";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.TextBox txtFile;
		private System.Windows.Forms.Button cmdFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox lstLFD;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button cmdShow;
		private System.Windows.Forms.Button cmdExit;
		private System.Windows.Forms.OpenFileDialog opnFile;
		private System.Windows.Forms.OpenFileDialog opnTie;
	}
}
