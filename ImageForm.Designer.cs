namespace Idmr.LfdImageReader
{
	partial class ImageForm
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
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageForm));
			this.pctImage = new System.Windows.Forms.PictureBox();
			this.lblFrame = new System.Windows.Forms.Label();
			this.cmdNext = new System.Windows.Forms.Button();
			this.cmdPrev = new System.Windows.Forms.Button();
			this.cmdSave = new System.Windows.Forms.Button();
			this.cmdClose = new System.Windows.Forms.Button();
			this.savFile = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.pctImage)).BeginInit();
			this.SuspendLayout();
			// 
			// pctImage
			// 
			this.pctImage.Location = new System.Drawing.Point(0, 0);
			this.pctImage.Name = "pctImage";
			this.pctImage.Size = new System.Drawing.Size(168, 168);
			this.pctImage.TabIndex = 0;
			this.pctImage.TabStop = false;
			// 
			// lblFrame
			// 
			this.lblFrame.Location = new System.Drawing.Point(112, 216);
			this.lblFrame.Name = "lblFrame";
			this.lblFrame.Size = new System.Drawing.Size(40, 16);
			this.lblFrame.TabIndex = 1;
			this.lblFrame.Text = "0/0";
			this.lblFrame.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblFrame.Visible = false;
			// 
			// cmdNext
			// 
			this.cmdNext.Location = new System.Drawing.Point(176, 216);
			this.cmdNext.Name = "cmdNext";
			this.cmdNext.Size = new System.Drawing.Size(32, 16);
			this.cmdNext.TabIndex = 1;
			this.cmdNext.Text = ">";
			this.cmdNext.Visible = false;
			this.cmdNext.Click += new System.EventHandler(this.cmdNext_Click);
			// 
			// cmdPrev
			// 
			this.cmdPrev.Location = new System.Drawing.Point(72, 216);
			this.cmdPrev.Name = "cmdPrev";
			this.cmdPrev.Size = new System.Drawing.Size(32, 16);
			this.cmdPrev.TabIndex = 2;
			this.cmdPrev.Text = "<";
			this.cmdPrev.Visible = false;
			this.cmdPrev.Click += new System.EventHandler(this.cmdPrev_Click);
			// 
			// cmdSave
			// 
			this.cmdSave.Location = new System.Drawing.Point(72, 240);
			this.cmdSave.Name = "cmdSave";
			this.cmdSave.Size = new System.Drawing.Size(48, 24);
			this.cmdSave.TabIndex = 3;
			this.cmdSave.Text = "&Save";
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			// 
			// cmdClose
			// 
			this.cmdClose.Location = new System.Drawing.Point(136, 240);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(48, 24);
			this.cmdClose.TabIndex = 3;
			this.cmdClose.Text = "&Close";
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// savFile
			// 
			this.savFile.Filter = "Bitmaps|*.bmp";
			this.savFile.FileOk += new System.ComponentModel.CancelEventHandler(this.savFile_FileOk);
			// 
			// ImageForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 261);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdSave);
			this.Controls.Add(this.cmdPrev);
			this.Controls.Add(this.cmdNext);
			this.Controls.Add(this.lblFrame);
			this.Controls.Add(this.pctImage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "ImageForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Image";
			((System.ComponentModel.ISupportInitialize)(this.pctImage)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.PictureBox pctImage;
		private System.Windows.Forms.Label lblFrame;
		private System.Windows.Forms.Button cmdNext;
		private System.Windows.Forms.Button cmdPrev;
		private System.Windows.Forms.Button cmdSave;
		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.SaveFileDialog savFile;
	}
}
