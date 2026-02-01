/*
 * LFD Image Reader.exe, Exports images and animation frames from LFD resources
 * Copyright (C) 2008-2026 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Version: 1.2+
 */

/* CHANGELOG
 * [ADD] ability to scale images
 * v1.2.1
 * [UPD] Transparency switched to fuschia
 * [ADD] transparency now works with ANIM
 */

using Idmr.LfdReader;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Idmr.LfdImageReader
{
	public partial class ImageForm : System.Windows.Forms.Form
	{
		//readonly ColorPalette _palette;
		int _frame = -1;
		Bitmap _image;
		readonly Anim _animation = null;
		readonly string _resource;

		public ImageForm(Anim anim, ColorPalette palette)
		{
			InitializeComponent();
			//_palette = palette;
			_resource = anim.ToString();
			_animation = anim;
			_animation.SetPalette(palette);
			_frame = 1;
			cboScale.SelectedIndex = 0;
			lblFrame.Text = _frame + "/" + _animation.NumberOfFrames;
			lblFrame.Visible = true;
			cmdNext.Visible = true;
			cmdPrev.Visible = true;
			scaleForm(_animation.Width, _animation.Height);
			getAnim();
		}

		public ImageForm(Delt delt, ColorPalette palette)
		{
			InitializeComponent();
			//_palette = palette;
			_resource = delt.ToString();
			_image = delt.Image;
			_image.Palette = palette;
			//Bitmap t = _image;
			_image.MakeTransparent(Color.Fuchsia);
			//pctImage.Image = t;
			cboScale.SelectedIndex = 0;
			scaleForm(delt.Width, delt.Height);
		}

		private void getAnim()
		{
			_image = _animation.Frames[_frame - 1].Image;
			_animation.RelativePosition = true;
			//Bitmap t = _animation.Frames[_frame - 1].Image;
			_image.MakeTransparent(Color.Fuchsia);
			//pctImage.Image = t;
			_animation.RelativePosition = false;
			pctImage.Invalidate();
		}

		void scaleForm(int width, int height)
		{
			width *= cboScale.SelectedIndex + 1;
			height *= cboScale.SelectedIndex + 1;
			Width = (width + 6 < 150 ? 150 : width + 6);
			Height = height + 96;
			if (_animation != null)
			{
				Height += 24;
				lblFrame.Top = Height - 114;
				cmdNext.Top = lblFrame.Top;
				cmdPrev.Top = cmdNext.Top;
				lblFrame.Left = Width / 2 - 32;
				cmdNext.Left = lblFrame.Left + 50;
				cmdPrev.Left = lblFrame.Left - 32;
			}
			cmdSave.Top = Height - 94;
			cmdClose.Top = cmdSave.Top;
			lblScale.Top = cmdSave.Top + 32;
			cboScale.Top = lblScale.Top - 2;
			cmdSave.Left = Width / 2 - 64;
			cmdClose.Left = cmdSave.Left + 56;
			lblScale.Left = cmdSave.Left;
			cboScale.Left = lblScale.Left + 40;
			pctImage.Width = width;
			pctImage.Height = height;
			pctImage.Invalidate();
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

		private void cboScale_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_animation != null) scaleForm(_animation.Width, _animation.Height);
			else scaleForm(_image.Width, _image.Height);
		}

		private void pctImage_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			var g = e.Graphics;
			g.DrawImage(_image, 0, 0, _image.Width * (cboScale.SelectedIndex + 1), _image.Height * (cboScale.SelectedIndex + 1));
		}
	}
}
