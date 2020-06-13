/*
 * LFD Image Reader.exe, Exports images and animation frames from LFD resources
 * Copyright (C) 2008-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Version: 1.2
 */

using Idmr.LfdReader;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Idmr.LfdImageReader
{
	public partial class ImageForm : System.Windows.Forms.Form
	{
		private ColorPalette _palette;
		private int _frame = -1;
		private Bitmap _image;
		private Anim _animation = null;
		string _resource;

		public ImageForm(Anim anim, ColorPalette palette)
		{
			InitializeComponent();
			_palette = palette;
			_resource = anim.ToString();
			_animation = anim;
			_animation.SetPalette(palette);
			_frame = 1;
			lblFrame.Text = _frame + "/" + _animation.NumberOfFrames;
			Width = (_animation.Width + 6 < 120 ? 120 : _animation.Width + 6);
			Height = _animation.Height + 90;
			lblFrame.Visible = true;
			cmdNext.Visible = true;
			cmdPrev.Visible = true;
			lblFrame.Top = Height - 84;
			cmdNext.Top = lblFrame.Top;
			cmdPrev.Top = cmdNext.Top;
			cmdSave.Top = cmdNext.Top + 20;
			cmdClose.Top = cmdSave.Top;
			lblFrame.Left = Width / 2 - 22;
			cmdNext.Left = lblFrame.Left + 40;
			cmdPrev.Left = lblFrame.Left - 32;
			cmdSave.Left = cmdPrev.Left;
			cmdClose.Left = cmdSave.Left + 56;
			pctImage.Width = _animation.Width;
			pctImage.Height = _animation.Height;
			getAnim();
		}

		public ImageForm(Delt delt, ColorPalette palette)
		{
			InitializeComponent();
			_palette = palette;
			_resource = delt.ToString();
			Width = (delt.Width + 6 < 120 ? 120 : delt.Width + 6);
			Height = delt.Height + 66;
			_image = delt.Image;
			_image.Palette = palette;
			cmdSave.Top = Height - 64;
			cmdClose.Top = cmdSave.Top;
			cmdSave.Left = Width / 2 - 52;
			cmdClose.Left = cmdSave.Left + 56;
			pctImage.Size = delt.Image.Size;
			Bitmap t = _image;
			t.MakeTransparent(Color.Black);
			pctImage.Image = t;
		}

		private void getAnim()
		{
			_image = _animation.Frames[_frame-1].Image;
			_animation.RelativePosition = true;
			pctImage.Image = _animation.Frames[_frame - 1].Image;
			_animation.RelativePosition = false;
		}

		private void cmdPrev_Click(object sender, EventArgs e)
		{
			if (_frame == 1) return;
			_frame--;
			lblFrame.Text = _frame + "/" + _animation.NumberOfFrames;
			getAnim();
		}
		private void cmdNext_Click(object sender, EventArgs e)
		{
			if (_frame == _animation.NumberOfFrames) return;
			_frame++;
			lblFrame.Text = _frame + "/" + _animation.NumberOfFrames;
			getAnim();
		}
		private void cmdClose_Click(object sender, EventArgs e)
		{
			Close();
		}
		private void cmdSave_Click(object sender, EventArgs e)
		{
			savFile.FileName = _resource;
			if (_frame != -1) savFile.FileName += "_" + _frame;
			savFile.ShowDialog();
		}
		private void savFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_image.Save(savFile.FileName,ImageFormat.Bmp);		// bmImage is already in 8bbp Indexed, just have to save it.
			// NOTE: may not match 100%, save function tends to switch pixel data to other pixel values of same color
		}
	}
}
