/*
 * LFD Image Reader.exe, Exports images and animation frames from LFD resources
 * Copyright (C) 2008-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Version: 1.2+
 */

/* CHANGELOG
 * v1.2.1
 * [UPD] Removed lockout if platform not defined, hard-coded default palettes if error
 * [UPD] Transparency switched to fuschia
 */

using Idmr.LfdReader;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Idmr.LfdImageReader
{
	/// <summary>
	/// Lists image types contained within an LFD file
	/// </summary>
	public partial class MainForm : Form
	{
		private ColorPalette _palette;
		private ImageForm _frmImage;
		private string _installPath = "";
		LfdFile _resource = null;

		public MainForm()
		{
			InitializeComponent();

			RegistryKey reg;

			// original registry
			reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\LucasArts Entertainment Company LLC\\TIE95\\1.0");
			if (reg == null)
			{
				reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\LucasArts Entertainment Company LLC\\TIE95\\1.0");
			}
			if (reg != null)
			{
				_installPath = (string)reg.GetValue("Install Path");
				reg.Close();
			}

			// Markus Egger MSI install
			if (_installPath == "")
			{
				reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Installer\\UserData\\S-1-5-18\\Products");
				if (reg != null)
				{
					string[] subs = reg.GetSubKeyNames();
					foreach (string k in subs)
					{
						string s = reg.Name + "\\" + k + "\\InstallProperties";
						RegistryKey sub = Registry.LocalMachine.OpenSubKey(s.Substring(19));

						if (sub == null)
						{
							continue;
						}

						if ((string)sub.GetValue("DisplayName") == "Star Wars: X. C. S. - TIE Fighter 95")
						{
							string path = (string)sub.GetValue("Readme");
							_installPath = path.Remove(path.Length - 11);
							reg.Close();
							break;
						}
					}
				}
			}

			// Steam detection
			if (_installPath == "")
			{
				reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 355250");
				if (reg != null)
				{
					_installPath = (string)reg.GetValue("InstallLocation") + "\\remastered";
					reg.Close();
				}
			}

			if (_installPath == "")
			{
				MessageBox.Show("TIE installation not detected, please select a TIE.exe or TIE95.exe to define the install location", "Install Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				DialogResult dr = opnTie.ShowDialog();
				if (dr == DialogResult.OK) _installPath = Path.GetDirectoryName(opnTie.FileName);
			}

			if (_installPath != "") opnFile.InitialDirectory = _installPath;
		}

		private void cmdChangeInstall_Click(object sender, EventArgs e)
		{
			DialogResult dr = opnTie.ShowDialog();
			if (dr == DialogResult.OK) _installPath = Path.GetDirectoryName(opnTie.FileName);
		}
		private void cmdExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}
		private void cmdFile_Click(object sender, EventArgs e)
		{
			opnFile.ShowDialog();
		}
		private void cmdShow_Click(object sender, EventArgs e)
		{
			if (lstLFD.SelectedIndex == -1) return;
			int top = Top, left = Left + Width + 10;
			if (_frmImage != null)
			{
				_frmImage.Close();
				top = _frmImage.Top;
				left = _frmImage.Left;
			}
			int index;
			for (index = 0; index < _resource.Resources.Count; index++)
				if (_resource.Resources[index].ToString() == lstLFD.Items[lstLFD.SelectedIndex].ToString())
					break;

			_palette.Entries[0] = Color.Fuchsia;	// redefine the transpareny color, since Black is regularly used elsewhere

			if (_resource.Resources[index].Type == Resource.ResourceType.Anim)
				_frmImage = new ImageForm((Anim)_resource.Resources[index], _palette);
			else _frmImage = new ImageForm((Delt)_resource.Resources[index], _palette);
			_frmImage.Top = top;
			_frmImage.Left = left;
			_frmImage.Show();
		}

		private void lstLFD_DoubleClick(object sender, EventArgs e)
		{
			cmdShow_Click("lstLFD_DoubleClick", new EventArgs());
		}
		private void lstLFD_SelectedIndexChanged(object sender, EventArgs e)
		{
			cmdShow.Enabled = (lstLFD.SelectedIndex != -1);
		}

		private void opnFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			cmdShow.Enabled = false;
			_palette = new Bitmap(1, 1, PixelFormat.Format8bppIndexed).Palette;
			for (int i = 0; i < 256; i++) _palette.Entries[i] = Color.Transparent;
			FileStream fsFile;
			try
			{
				fsFile = File.OpenRead(opnFile.FileName);
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			txtFile.Text = opnFile.FileName;
			lstLFD.Items.Clear();
			lstLFD.Enabled = true;
			// I don't really like hard-coding the colors, but this will allow loading if there's a load failure or TIE isn't detected.
			// Init standard Palette
			try { setPltt((Pltt)(new LfdFile(_installPath + "\\RESOURCE\\EMPIRE.LFD").Resources["PLTTstandard"])); }
			catch
			{
				//Not really concerned with why it failed, just load the default.
				#region colors
				_palette.Entries[0] = Color.FromArgb(0, 0, 0);
				_palette.Entries[1] = Color.FromArgb(0, 0, 171);
				_palette.Entries[2] = Color.FromArgb(0, 171, 0);
				_palette.Entries[3] = Color.FromArgb(0, 171, 171);
				_palette.Entries[4] = Color.FromArgb(171, 0, 0);
				_palette.Entries[5] = Color.FromArgb(171, 0, 171);
				_palette.Entries[6] = Color.FromArgb(171, 87, 0);
				_palette.Entries[7] = Color.FromArgb(171, 171, 171);
				_palette.Entries[8] = Color.FromArgb(87, 87, 87);
				_palette.Entries[9] = Color.FromArgb(87, 87, 255);
				_palette.Entries[10] = Color.FromArgb(87, 255, 87);
				_palette.Entries[11] = Color.FromArgb(87, 255, 255);
				_palette.Entries[12] = Color.FromArgb(255, 87, 87);
				_palette.Entries[13] = Color.FromArgb(255, 87, 255);
				_palette.Entries[14] = Color.FromArgb(255, 255, 87);
				_palette.Entries[15] = Color.FromArgb(255, 255, 255);
				_palette.Entries[16] = Color.FromArgb(0, 0, 0);
				_palette.Entries[17] = Color.FromArgb(19, 19, 19);
				_palette.Entries[18] = Color.FromArgb(35, 35, 35);
				_palette.Entries[19] = Color.FromArgb(51, 51, 51);
				_palette.Entries[20] = Color.FromArgb(71, 71, 71);
				_palette.Entries[21] = Color.FromArgb(87, 87, 87);
				_palette.Entries[22] = Color.FromArgb(103, 103, 103);
				_palette.Entries[23] = Color.FromArgb(119, 119, 119);
				_palette.Entries[24] = Color.FromArgb(139, 139, 139);
				_palette.Entries[25] = Color.FromArgb(155, 155, 155);
				_palette.Entries[26] = Color.FromArgb(171, 171, 171);
				_palette.Entries[27] = Color.FromArgb(187, 187, 187);
				_palette.Entries[28] = Color.FromArgb(207, 207, 207);
				_palette.Entries[29] = Color.FromArgb(223, 223, 223);
				_palette.Entries[30] = Color.FromArgb(239, 239, 239);
				_palette.Entries[31] = Color.FromArgb(255, 255, 255);
				#endregion
			}
			// Okay, now we need to take into account for special cases where the PLTT isn't stored in the selected LFD
			if (opnFile.FileName.ToUpper().IndexOf("BATTLE") != -1)
				try { setPltt((Pltt)(new LfdFile(_installPath + "\\RESOURCE\\TOURDESK.LFD").Resources["PLTTtoddesk"])); }
				catch
				{
					#region colors
					_palette.Entries[32] = Color.FromArgb(52, 92, 152);
					_palette.Entries[33] = Color.FromArgb(52, 96, 156);
					_palette.Entries[34] = Color.FromArgb(56, 104, 152);
					_palette.Entries[35] = Color.FromArgb(56, 112, 164);
					_palette.Entries[36] = Color.FromArgb(56, 116, 168);
					_palette.Entries[37] = Color.FromArgb(60, 128, 176);
					_palette.Entries[38] = Color.FromArgb(60, 144, 192);
					_palette.Entries[39] = Color.FromArgb(64, 156, 204);
					_palette.Entries[40] = Color.FromArgb(68, 172, 208);
					_palette.Entries[41] = Color.FromArgb(60, 144, 164);
					_palette.Entries[42] = Color.FromArgb(44, 128, 160);
					_palette.Entries[43] = Color.FromArgb(36, 112, 160);
					_palette.Entries[44] = Color.FromArgb(52, 88, 144);
					_palette.Entries[45] = Color.FromArgb(44, 140, 172);
					_palette.Entries[46] = Color.FromArgb(32, 104, 156);
					_palette.Entries[47] = Color.FromArgb(28, 96, 156);
					_palette.Entries[48] = Color.FromArgb(32, 96, 144);
					_palette.Entries[49] = Color.FromArgb(56, 72, 104);
					_palette.Entries[50] = Color.FromArgb(52, 68, 104);
					_palette.Entries[51] = Color.FromArgb(52, 72, 100);
					_palette.Entries[52] = Color.FromArgb(40, 76, 112);
					_palette.Entries[53] = Color.FromArgb(48, 64, 96);
					_palette.Entries[54] = Color.FromArgb(56, 68, 96);
					_palette.Entries[55] = Color.FromArgb(56, 72, 100);
					_palette.Entries[56] = Color.FromArgb(48, 60, 88);
					_palette.Entries[57] = Color.FromArgb(44, 64, 92);
					_palette.Entries[58] = Color.FromArgb(64, 80, 112);
					_palette.Entries[59] = Color.FromArgb(64, 88, 120);
					_palette.Entries[60] = Color.FromArgb(72, 88, 112);
					_palette.Entries[61] = Color.FromArgb(72, 84, 104);
					_palette.Entries[62] = Color.FromArgb(48, 56, 48);
					_palette.Entries[63] = Color.FromArgb(56, 72, 56);
					_palette.Entries[64] = Color.FromArgb(36, 104, 144);
					_palette.Entries[65] = Color.FromArgb(40, 112, 144);
					_palette.Entries[66] = Color.FromArgb(60, 108, 148);
					_palette.Entries[67] = Color.FromArgb(56, 100, 144);
					_palette.Entries[68] = Color.FromArgb(60, 96, 136);
					_palette.Entries[69] = Color.FromArgb(64, 104, 136);
					_palette.Entries[70] = Color.FromArgb(64, 96, 128);
					_palette.Entries[71] = Color.FromArgb(56, 92, 128);
					_palette.Entries[72] = Color.FromArgb(56, 88, 124);
					_palette.Entries[73] = Color.FromArgb(56, 80, 120);
					_palette.Entries[74] = Color.FromArgb(52, 80, 132);
					_palette.Entries[75] = Color.FromArgb(52, 72, 132);
					_palette.Entries[76] = Color.FromArgb(32, 80, 148);
					_palette.Entries[77] = Color.FromArgb(28, 80, 140);
					_palette.Entries[78] = Color.FromArgb(28, 88, 136);
					_palette.Entries[79] = Color.FromArgb(28, 84, 132);
					_palette.Entries[80] = Color.FromArgb(28, 72, 132);
					_palette.Entries[81] = Color.FromArgb(28, 64, 132);
					_palette.Entries[82] = Color.FromArgb(28, 52, 132);
					_palette.Entries[83] = Color.FromArgb(16, 56, 128);
					_palette.Entries[84] = Color.FromArgb(12, 44, 132);
					_palette.Entries[85] = Color.FromArgb(16, 48, 120);
					_palette.Entries[86] = Color.FromArgb(16, 44, 112);
					_palette.Entries[87] = Color.FromArgb(20, 40, 104);
					_palette.Entries[88] = Color.FromArgb(20, 36, 88);
					_palette.Entries[89] = Color.FromArgb(56, 92, 136);
					_palette.Entries[90] = Color.FromArgb(120, 156, 216);
					_palette.Entries[91] = Color.FromArgb(40, 124, 172);
					_palette.Entries[92] = Color.FromArgb(20, 32, 80);
					_palette.Entries[93] = Color.FromArgb(20, 32, 72);
					_palette.Entries[94] = Color.FromArgb(20, 36, 64);
					_palette.Entries[95] = Color.FromArgb(24, 40, 64);
					_palette.Entries[96] = Color.FromArgb(32, 40, 64);
					_palette.Entries[97] = Color.FromArgb(36, 48, 68);
					_palette.Entries[98] = Color.FromArgb(40, 48, 64);
					_palette.Entries[99] = Color.FromArgb(40, 52, 72);
					_palette.Entries[100] = Color.FromArgb(44, 56, 76);
					_palette.Entries[101] = Color.FromArgb(48, 56, 72);
					_palette.Entries[102] = Color.FromArgb(48, 48, 64);
					_palette.Entries[103] = Color.FromArgb(56, 48, 64);
					_palette.Entries[104] = Color.FromArgb(48, 44, 56);
					_palette.Entries[105] = Color.FromArgb(40, 44, 56);
					_palette.Entries[106] = Color.FromArgb(36, 44, 56);
					_palette.Entries[107] = Color.FromArgb(32, 36, 56);
					_palette.Entries[108] = Color.FromArgb(32, 36, 52);
					_palette.Entries[109] = Color.FromArgb(24, 32, 52);
					_palette.Entries[110] = Color.FromArgb(24, 28, 48);
					_palette.Entries[111] = Color.FromArgb(24, 28, 40);
					_palette.Entries[112] = Color.FromArgb(36, 32, 40);
					_palette.Entries[113] = Color.FromArgb(24, 24, 32);
					_palette.Entries[114] = Color.FromArgb(24, 20, 24);
					_palette.Entries[115] = Color.FromArgb(20, 24, 28);
					_palette.Entries[116] = Color.FromArgb(16, 20, 32);
					_palette.Entries[117] = Color.FromArgb(12, 20, 36);
					_palette.Entries[118] = Color.FromArgb(12, 20, 40);
					_palette.Entries[119] = Color.FromArgb(80, 84, 104);
					_palette.Entries[120] = Color.FromArgb(80, 84, 96);
					_palette.Entries[121] = Color.FromArgb(76, 80, 92);
					_palette.Entries[122] = Color.FromArgb(72, 72, 88);
					_palette.Entries[123] = Color.FromArgb(72, 72, 84);
					_palette.Entries[124] = Color.FromArgb(64, 72, 88);
					_palette.Entries[125] = Color.FromArgb(60, 72, 92);
					_palette.Entries[126] = Color.FromArgb(56, 64, 88);
					_palette.Entries[127] = Color.FromArgb(48, 64, 92);
					_palette.Entries[128] = Color.FromArgb(16, 24, 40);
					_palette.Entries[129] = Color.FromArgb(16, 32, 48);
					_palette.Entries[130] = Color.FromArgb(12, 32, 48);
					_palette.Entries[131] = Color.FromArgb(12, 36, 56);
					_palette.Entries[132] = Color.FromArgb(20, 40, 60);
					_palette.Entries[133] = Color.FromArgb(20, 32, 56);
					_palette.Entries[134] = Color.FromArgb(24, 28, 56);
					_palette.Entries[135] = Color.FromArgb(16, 24, 56);
					_palette.Entries[136] = Color.FromArgb(12, 20, 48);
					_palette.Entries[137] = Color.FromArgb(4, 20, 32);
					_palette.Entries[138] = Color.FromArgb(12, 16, 24);
					_palette.Entries[139] = Color.FromArgb(16, 16, 20);
					_palette.Entries[140] = Color.FromArgb(12, 12, 16);
					_palette.Entries[141] = Color.FromArgb(8, 8, 8);
					_palette.Entries[142] = Color.FromArgb(4, 4, 8);
					_palette.Entries[143] = Color.FromArgb(4, 4, 16);
					_palette.Entries[144] = Color.FromArgb(4, 8, 20);
					_palette.Entries[145] = Color.FromArgb(4, 8, 32);
					_palette.Entries[146] = Color.FromArgb(36, 60, 176);
					_palette.Entries[147] = Color.FromArgb(24, 60, 196);
					_palette.Entries[148] = Color.FromArgb(12, 64, 212);
					_palette.Entries[149] = Color.FromArgb(32, 56, 156);
					_palette.Entries[150] = Color.FromArgb(12, 52, 176);
					_palette.Entries[151] = Color.FromArgb(24, 36, 56);
					_palette.Entries[152] = Color.FromArgb(16, 44, 64);
					_palette.Entries[153] = Color.FromArgb(24, 48, 68);
					_palette.Entries[154] = Color.FromArgb(24, 48, 72);
					_palette.Entries[155] = Color.FromArgb(28, 44, 72);
					_palette.Entries[156] = Color.FromArgb(0, 0, 4);
					_palette.Entries[157] = Color.FromArgb(52, 72, 116);
					_palette.Entries[158] = Color.FromArgb(56, 76, 112);
					_palette.Entries[159] = Color.FromArgb(60, 80, 112);
					_palette.Entries[160] = Color.FromArgb(32, 44, 72);
					_palette.Entries[161] = Color.FromArgb(32, 44, 80);
					_palette.Entries[162] = Color.FromArgb(28, 40, 80);
					_palette.Entries[163] = Color.FromArgb(36, 52, 80);
					_palette.Entries[164] = Color.FromArgb(36, 52, 88);
					_palette.Entries[165] = Color.FromArgb(40, 52, 80);
					_palette.Entries[166] = Color.FromArgb(48, 56, 84);
					_palette.Entries[167] = Color.FromArgb(48, 64, 84);
					_palette.Entries[168] = Color.FromArgb(44, 64, 84);
					_palette.Entries[169] = Color.FromArgb(40, 56, 88);
					_palette.Entries[170] = Color.FromArgb(36, 56, 76);
					_palette.Entries[171] = Color.FromArgb(24, 56, 80);
					_palette.Entries[172] = Color.FromArgb(16, 56, 80);
					_palette.Entries[173] = Color.FromArgb(16, 64, 88);
					_palette.Entries[174] = Color.FromArgb(20, 68, 96);
					_palette.Entries[175] = Color.FromArgb(20, 72, 104);
					_palette.Entries[176] = Color.FromArgb(28, 72, 104);
					_palette.Entries[177] = Color.FromArgb(28, 72, 120);
					_palette.Entries[178] = Color.FromArgb(32, 92, 120);
					_palette.Entries[179] = Color.FromArgb(24, 64, 120);
					_palette.Entries[180] = Color.FromArgb(24, 64, 116);
					_palette.Entries[181] = Color.FromArgb(24, 56, 104);
					_palette.Entries[182] = Color.FromArgb(24, 52, 104);
					_palette.Entries[183] = Color.FromArgb(36, 48, 104);
					_palette.Entries[184] = Color.FromArgb(28, 48, 116);
					_palette.Entries[185] = Color.FromArgb(36, 56, 116);
					_palette.Entries[186] = Color.FromArgb(36, 60, 120);
					_palette.Entries[187] = Color.FromArgb(60, 80, 108);
					_palette.Entries[188] = Color.FromArgb(64, 76, 104);
					_palette.Entries[189] = Color.FromArgb(64, 80, 104);
					_palette.Entries[190] = Color.FromArgb(64, 76, 96);
					_palette.Entries[191] = Color.FromArgb(72, 76, 96);
					_palette.Entries[192] = Color.FromArgb(72, 80, 96);
					_palette.Entries[193] = Color.FromArgb(84, 76, 96);
					_palette.Entries[194] = Color.FromArgb(80, 72, 92);
					_palette.Entries[195] = Color.FromArgb(84, 72, 80);
					_palette.Entries[196] = Color.FromArgb(76, 64, 80);
					_palette.Entries[197] = Color.FromArgb(76, 64, 72);
					_palette.Entries[198] = Color.FromArgb(76, 64, 68);
					_palette.Entries[199] = Color.FromArgb(64, 56, 68);
					_palette.Entries[200] = Color.FromArgb(60, 56, 68);
					_palette.Entries[201] = Color.FromArgb(60, 52, 72);
					_palette.Entries[202] = Color.FromArgb(60, 124, 160);
					_palette.Entries[203] = Color.FromArgb(16, 48, 72);
					_palette.Entries[204] = Color.FromArgb(60, 64, 76);
					_palette.Entries[205] = Color.FromArgb(64, 68, 80);
					_palette.Entries[206] = Color.FromArgb(60, 64, 80);
					_palette.Entries[207] = Color.FromArgb(56, 60, 80);
					_palette.Entries[208] = Color.FromArgb(68, 60, 80);
					_palette.Entries[209] = Color.FromArgb(92, 80, 88);
					_palette.Entries[210] = Color.FromArgb(88, 80, 100);
					_palette.Entries[211] = Color.FromArgb(88, 88, 100);
					_palette.Entries[212] = Color.FromArgb(88, 92, 104);
					_palette.Entries[213] = Color.FromArgb(96, 88, 104);
					_palette.Entries[214] = Color.FromArgb(96, 92, 112);
					_palette.Entries[215] = Color.FromArgb(96, 96, 108);
					_palette.Entries[216] = Color.FromArgb(100, 100, 112);
					_palette.Entries[217] = Color.FromArgb(108, 96, 112);
					_palette.Entries[218] = Color.FromArgb(112, 100, 116);
					_palette.Entries[219] = Color.FromArgb(104, 108, 116);
					_palette.Entries[220] = Color.FromArgb(112, 112, 120);
					_palette.Entries[221] = Color.FromArgb(120, 120, 128);
					_palette.Entries[222] = Color.FromArgb(128, 128, 136);
					_palette.Entries[223] = Color.FromArgb(140, 140, 148);
					_palette.Entries[224] = Color.FromArgb(148, 144, 152);
					_palette.Entries[225] = Color.FromArgb(108, 128, 168);
					_palette.Entries[226] = Color.FromArgb(100, 120, 156);
					_palette.Entries[227] = Color.FromArgb(92, 116, 152);
					_palette.Entries[228] = Color.FromArgb(92, 112, 144);
					_palette.Entries[229] = Color.FromArgb(84, 108, 132);
					_palette.Entries[230] = Color.FromArgb(76, 96, 128);
					_palette.Entries[231] = Color.FromArgb(76, 96, 124);
					_palette.Entries[232] = Color.FromArgb(84, 92, 112);
					_palette.Entries[233] = Color.FromArgb(88, 76, 124);
					_palette.Entries[234] = Color.FromArgb(100, 88, 128);
					_palette.Entries[235] = Color.FromArgb(108, 92, 104);
					_palette.Entries[236] = Color.FromArgb(120, 104, 168);
					_palette.Entries[237] = Color.FromArgb(136, 100, 200);
					_palette.Entries[238] = Color.FromArgb(112, 72, 48);
					_palette.Entries[239] = Color.FromArgb(112, 64, 72);
					_palette.Entries[240] = Color.FromArgb(96, 56, 40);
					_palette.Entries[241] = Color.FromArgb(72, 40, 32);
					_palette.Entries[242] = Color.FromArgb(52, 40, 24);
					_palette.Entries[243] = Color.FromArgb(96, 64, 44);
					_palette.Entries[244] = Color.FromArgb(156, 76, 44);
					_palette.Entries[245] = Color.FromArgb(220, 84, 20);
					_palette.Entries[246] = Color.FromArgb(248, 156, 4);
					_palette.Entries[247] = Color.FromArgb(24, 20, 20);
					_palette.Entries[248] = Color.FromArgb(44, 40, 48);
					_palette.Entries[249] = Color.FromArgb(180, 164, 116);
					_palette.Entries[250] = Color.FromArgb(172, 140, 108);
					_palette.Entries[251] = Color.FromArgb(40, 24, 16);
					_palette.Entries[252] = Color.FromArgb(120, 96, 80);
					_palette.Entries[253] = Color.FromArgb(140, 88, 72);
					_palette.Entries[254] = Color.FromArgb(156, 104, 72);
					_palette.Entries[255] = Color.FromArgb(156, 120, 80);
					#endregion
				}
			else if (opnFile.FileName.ToUpper().IndexOf("SHIP") != -1)
				try
				{
					setPltt((Pltt)(new LfdFile(_installPath + "\\RESOURCE\\LAUNCH.LFD").Resources["PLTTlaunch"]));
					setPltt((Pltt)(new LfdFile(_installPath + "\\RESOURCE\\LAUNCH.LFD").Resources["PLTTl-bg-bay"]));
				}
				catch
				{
					#region colors
					// launch. close, but not the same as EMPIRE:standard
					_palette.Entries[1] = Color.FromArgb(0, 0, 168);
					_palette.Entries[2] = Color.FromArgb(0, 168, 0);
					_palette.Entries[3] = Color.FromArgb(0, 168, 168);
					_palette.Entries[4] = Color.FromArgb(168, 0, 0);
					_palette.Entries[5] = Color.FromArgb(168, 0, 168);
					_palette.Entries[6] = Color.FromArgb(168, 84, 0);
					_palette.Entries[7] = Color.FromArgb(168, 168, 168);
					_palette.Entries[8] = Color.FromArgb(84, 84, 84);
					_palette.Entries[9] = Color.FromArgb(84, 84, 252);
					_palette.Entries[10] = Color.FromArgb(84, 252, 84);
					_palette.Entries[11] = Color.FromArgb(84, 252, 252);
					_palette.Entries[12] = Color.FromArgb(252, 84, 84);
					_palette.Entries[13] = Color.FromArgb(252, 84, 252);
					_palette.Entries[14] = Color.FromArgb(252, 252, 84);
					_palette.Entries[15] = Color.FromArgb(252, 252, 252);
					_palette.Entries[16] = Color.FromArgb(0, 0, 0);
					_palette.Entries[17] = Color.FromArgb(4, 4, 8);
					_palette.Entries[18] = Color.FromArgb(16, 16, 28);
					_palette.Entries[19] = Color.FromArgb(28, 32, 52);
					_palette.Entries[20] = Color.FromArgb(36, 40, 64);
					_palette.Entries[21] = Color.FromArgb(44, 48, 80);
					_palette.Entries[22] = Color.FromArgb(60, 64, 88);
					_palette.Entries[23] = Color.FromArgb(92, 92, 104);
					_palette.Entries[24] = Color.FromArgb(104, 104, 108);
					_palette.Entries[25] = Color.FromArgb(116, 116, 116);
					_palette.Entries[26] = Color.FromArgb(132, 128, 128);
					_palette.Entries[27] = Color.FromArgb(148, 140, 140);
					_palette.Entries[28] = Color.FromArgb(164, 156, 156);
					_palette.Entries[29] = Color.FromArgb(180, 172, 172);
					_palette.Entries[30] = Color.FromArgb(196, 188, 188);
					_palette.Entries[31] = Color.FromArgb(212, 204, 204);
					// l-bg-bay
					_palette.Entries[32] = Color.FromArgb(0, 0, 0);
					_palette.Entries[33] = Color.FromArgb(4, 4, 8);
					_palette.Entries[34] = Color.FromArgb(8, 8, 12);
					_palette.Entries[35] = Color.FromArgb(12, 12, 16);
					_palette.Entries[36] = Color.FromArgb(16, 16, 20);
					_palette.Entries[37] = Color.FromArgb(12, 16, 24);
					_palette.Entries[38] = Color.FromArgb(20, 20, 24);
					_palette.Entries[39] = Color.FromArgb(24, 24, 28);
					_palette.Entries[40] = Color.FromArgb(20, 24, 32);
					_palette.Entries[41] = Color.FromArgb(24, 28, 32);
					_palette.Entries[42] = Color.FromArgb(28, 32, 40);
					_palette.Entries[43] = Color.FromArgb(32, 32, 36);
					_palette.Entries[44] = Color.FromArgb(36, 36, 40);
					_palette.Entries[45] = Color.FromArgb(40, 40, 44);
					_palette.Entries[46] = Color.FromArgb(36, 40, 48);
					_palette.Entries[47] = Color.FromArgb(40, 44, 48);
					_palette.Entries[48] = Color.FromArgb(44, 48, 52);
					_palette.Entries[49] = Color.FromArgb(48, 52, 56);
					_palette.Entries[50] = Color.FromArgb(52, 52, 64);
					_palette.Entries[51] = Color.FromArgb(56, 56, 64);
					_palette.Entries[52] = Color.FromArgb(60, 60, 72);
					_palette.Entries[53] = Color.FromArgb(64, 64, 68);
					_palette.Entries[54] = Color.FromArgb(64, 64, 76);
					_palette.Entries[55] = Color.FromArgb(68, 68, 80);
					_palette.Entries[56] = Color.FromArgb(72, 72, 76);
					_palette.Entries[57] = Color.FromArgb(72, 72, 84);
					_palette.Entries[58] = Color.FromArgb(76, 76, 88);
					_palette.Entries[59] = Color.FromArgb(80, 80, 84);
					_palette.Entries[60] = Color.FromArgb(80, 80, 92);
					_palette.Entries[61] = Color.FromArgb(84, 84, 96);
					_palette.Entries[62] = Color.FromArgb(88, 88, 92);
					_palette.Entries[63] = Color.FromArgb(88, 88, 100);
					_palette.Entries[64] = Color.FromArgb(92, 92, 104);
					_palette.Entries[65] = Color.FromArgb(96, 96, 100);
					_palette.Entries[66] = Color.FromArgb(96, 96, 108);
					_palette.Entries[67] = Color.FromArgb(100, 100, 112);
					_palette.Entries[68] = Color.FromArgb(104, 104, 108);
					_palette.Entries[69] = Color.FromArgb(104, 104, 116);
					_palette.Entries[70] = Color.FromArgb(104, 108, 120);
					_palette.Entries[71] = Color.FromArgb(108, 112, 120);
					_palette.Entries[72] = Color.FromArgb(112, 116, 120);
					_palette.Entries[73] = Color.FromArgb(116, 120, 124);
					_palette.Entries[74] = Color.FromArgb(120, 124, 128);
					_palette.Entries[75] = Color.FromArgb(124, 128, 132);
					_palette.Entries[76] = Color.FromArgb(128, 132, 136);
					_palette.Entries[77] = Color.FromArgb(132, 136, 140);
					_palette.Entries[78] = Color.FromArgb(132, 132, 144);
					_palette.Entries[79] = Color.FromArgb(136, 140, 144);
					_palette.Entries[80] = Color.FromArgb(140, 144, 148);
					_palette.Entries[81] = Color.FromArgb(144, 148, 152);
					_palette.Entries[82] = Color.FromArgb(148, 152, 156);
					_palette.Entries[83] = Color.FromArgb(152, 156, 160);
					_palette.Entries[84] = Color.FromArgb(156, 160, 168);
					_palette.Entries[85] = Color.FromArgb(160, 164, 165);
					_palette.Entries[86] = Color.FromArgb(164, 168, 172);
					_palette.Entries[87] = Color.FromArgb(168, 172, 176);
					_palette.Entries[88] = Color.FromArgb(172, 176, 180);
					_palette.Entries[89] = Color.FromArgb(176, 180, 184);
					_palette.Entries[90] = Color.FromArgb(180, 184, 188);
					_palette.Entries[91] = Color.FromArgb(184, 188, 192);
					_palette.Entries[92] = Color.FromArgb(188, 192, 196);
					_palette.Entries[93] = Color.FromArgb(200, 200, 208);
					_palette.Entries[94] = Color.FromArgb(120, 120, 136);
					_palette.Entries[95] = Color.FromArgb(116, 116, 132);
					_palette.Entries[96] = Color.FromArgb(0, 0, 0);
					_palette.Entries[97] = Color.FromArgb(8, 4, 4);
					_palette.Entries[98] = Color.FromArgb(8, 8, 12);
					_palette.Entries[99] = Color.FromArgb(16, 12, 12);
					_palette.Entries[100] = Color.FromArgb(12, 16, 20);
					_palette.Entries[101] = Color.FromArgb(16, 20, 24);
					_palette.Entries[102] = Color.FromArgb(24, 20, 20);
					_palette.Entries[103] = Color.FromArgb(24, 24, 28);
					_palette.Entries[104] = Color.FromArgb(28, 28, 32);
					_palette.Entries[105] = Color.FromArgb(32, 32, 36);
					_palette.Entries[106] = Color.FromArgb(24, 28, 40);
					_palette.Entries[107] = Color.FromArgb(20, 24, 36);
					_palette.Entries[108] = Color.FromArgb(24, 32, 48);
					_palette.Entries[109] = Color.FromArgb(28, 36, 56);
					_palette.Entries[110] = Color.FromArgb(36, 40, 48);
					_palette.Entries[111] = Color.FromArgb(44, 48, 52);
					_palette.Entries[112] = Color.FromArgb(48, 52, 56);
					_palette.Entries[113] = Color.FromArgb(48, 56, 64);
					_palette.Entries[114] = Color.FromArgb(48, 56, 72);
					_palette.Entries[115] = Color.FromArgb(40, 52, 72);
					_palette.Entries[116] = Color.FromArgb(40, 48, 64);
					_palette.Entries[117] = Color.FromArgb(32, 44, 64);
					_palette.Entries[118] = Color.FromArgb(52, 44, 64);
					_palette.Entries[119] = Color.FromArgb(56, 60, 64);
					_palette.Entries[120] = Color.FromArgb(64, 56, 56);
					_palette.Entries[121] = Color.FromArgb(72, 60, 60);
					_palette.Entries[122] = Color.FromArgb(80, 64, 56);
					_palette.Entries[123] = Color.FromArgb(84, 64, 64);
					_palette.Entries[124] = Color.FromArgb(80, 72, 72);
					_palette.Entries[125] = Color.FromArgb(80, 64, 76);
					_palette.Entries[126] = Color.FromArgb(68, 72, 80);
					_palette.Entries[127] = Color.FromArgb(64, 68, 72);
					// launch
					_palette.Entries[128] = Color.FromArgb(72, 72, 64);
					_palette.Entries[129] = Color.FromArgb(72, 56, 48);
					_palette.Entries[130] = Color.FromArgb(64, 52, 44);
					_palette.Entries[131] = Color.FromArgb(56, 48, 48);
					_palette.Entries[132] = Color.FromArgb(56, 44, 40);
					_palette.Entries[133] = Color.FromArgb(60, 32, 40);
					_palette.Entries[134] = Color.FromArgb(64, 24, 32);
					_palette.Entries[135] = Color.FromArgb(56, 20, 24);
					_palette.Entries[136] = Color.FromArgb(40, 20, 24);
					_palette.Entries[137] = Color.FromArgb(40, 24, 32);
					_palette.Entries[138] = Color.FromArgb(44, 32, 32);
					_palette.Entries[139] = Color.FromArgb(44, 36, 40);
					_palette.Entries[140] = Color.FromArgb(48, 36, 48);
					_palette.Entries[141] = Color.FromArgb(76, 32, 36);
					_palette.Entries[142] = Color.FromArgb(80, 40, 48);
					_palette.Entries[143] = Color.FromArgb(88, 48, 52);
					_palette.Entries[144] = Color.FromArgb(96, 52, 56);
					_palette.Entries[145] = Color.FromArgb(112, 52, 52);
					_palette.Entries[146] = Color.FromArgb(124, 48, 48);
					_palette.Entries[147] = Color.FromArgb(116, 40, 44);
					_palette.Entries[148] = Color.FromArgb(104, 40, 44);
					_palette.Entries[149] = Color.FromArgb(104, 32, 32);
					_palette.Entries[150] = Color.FromArgb(100, 24, 24);
					_palette.Entries[151] = Color.FromArgb(116, 28, 28);
					_palette.Entries[152] = Color.FromArgb(132, 24, 28);
					_palette.Entries[153] = Color.FromArgb(140, 28, 28);
					_palette.Entries[154] = Color.FromArgb(144, 40, 40);
					_palette.Entries[155] = Color.FromArgb(160, 36, 28);
					_palette.Entries[156] = Color.FromArgb(176, 32, 20);
					_palette.Entries[157] = Color.FromArgb(184, 40, 28);
					_palette.Entries[158] = Color.FromArgb(192, 24, 8);
					_palette.Entries[159] = Color.FromArgb(184, 16, 0);
					_palette.Entries[160] = Color.FromArgb(164, 20, 4);
					_palette.Entries[161] = Color.FromArgb(140, 16, 4);
					_palette.Entries[162] = Color.FromArgb(120, 20, 12);
					_palette.Entries[163] = Color.FromArgb(100, 8, 4);
					_palette.Entries[164] = Color.FromArgb(84, 16, 16);
					_palette.Entries[165] = Color.FromArgb(80, 20, 24);
					_palette.Entries[166] = Color.FromArgb(76, 12, 8);
					_palette.Entries[167] = Color.FromArgb(76, 0, 0);
					_palette.Entries[168] = Color.FromArgb(52, 4, 4);
					_palette.Entries[169] = Color.FromArgb(32, 16, 20);
					_palette.Entries[170] = Color.FromArgb(60, 56, 24);
					_palette.Entries[171] = Color.FromArgb(88, 80, 40);
					_palette.Entries[172] = Color.FromArgb(96, 84, 48);
					_palette.Entries[173] = Color.FromArgb(108, 100, 40);
					_palette.Entries[174] = Color.FromArgb(112, 104, 56);
					_palette.Entries[175] = Color.FromArgb(116, 80, 72);
					_palette.Entries[176] = Color.FromArgb(124, 76, 72);
					_palette.Entries[177] = Color.FromArgb(144, 72, 72);
					_palette.Entries[178] = Color.FromArgb(148, 76, 80);
					_palette.Entries[179] = Color.FromArgb(140, 88, 92);
					_palette.Entries[180] = Color.FromArgb(120, 88, 88);
					_palette.Entries[181] = Color.FromArgb(112, 88, 84);
					_palette.Entries[182] = Color.FromArgb(104, 84, 80);
					_palette.Entries[183] = Color.FromArgb(104, 92, 88);
					_palette.Entries[184] = Color.FromArgb(96, 88, 88);
					_palette.Entries[185] = Color.FromArgb(92, 80, 88);
					_palette.Entries[186] = Color.FromArgb(88, 80, 80);
					_palette.Entries[187] = Color.FromArgb(96, 80, 72);
					_palette.Entries[188] = Color.FromArgb(96, 72, 68);
					_palette.Entries[189] = Color.FromArgb(108, 64, 68);
					_palette.Entries[190] = Color.FromArgb(132, 56, 60);
					_palette.Entries[191] = Color.FromArgb(140, 64, 68);
					_palette.Entries[192] = Color.FromArgb(156, 60, 60);
					_palette.Entries[193] = Color.FromArgb(172, 64, 64);
					_palette.Entries[194] = Color.FromArgb(164, 52, 44);
					_palette.Entries[195] = Color.FromArgb(208, 20, 0);
					_palette.Entries[196] = Color.FromArgb(252, 36, 8);
					_palette.Entries[197] = Color.FromArgb(252, 48, 20);
					_palette.Entries[198] = Color.FromArgb(168, 152, 56);
					_palette.Entries[199] = Color.FromArgb(180, 164, 68);
					_palette.Entries[200] = Color.FromArgb(200, 184, 68);
					_palette.Entries[201] = Color.FromArgb(208, 188, 64);
					_palette.Entries[202] = Color.FromArgb(232, 212, 64);
					_palette.Entries[203] = Color.FromArgb(244, 224, 68);
					_palette.Entries[204] = Color.FromArgb(148, 136, 52);
					_palette.Entries[205] = Color.FromArgb(136, 120, 48);
					_palette.Entries[206] = Color.FromArgb(140, 132, 84);
					_palette.Entries[207] = Color.FromArgb(116, 112, 88);
					_palette.Entries[208] = Color.FromArgb(108, 96, 96);
					_palette.Entries[209] = Color.FromArgb(108, 104, 104);
					_palette.Entries[210] = Color.FromArgb(100, 104, 112);
					_palette.Entries[211] = Color.FromArgb(92, 104, 116);
					_palette.Entries[212] = Color.FromArgb(88, 96, 112);
					_palette.Entries[213] = Color.FromArgb(84, 96, 120);
					_palette.Entries[214] = Color.FromArgb(92, 104, 128);
					_palette.Entries[215] = Color.FromArgb(100, 112, 124);
					_palette.Entries[216] = Color.FromArgb(108, 112, 120);
					_palette.Entries[217] = Color.FromArgb(112, 116, 128);
					_palette.Entries[218] = Color.FromArgb(112, 120, 136);
					_palette.Entries[219] = Color.FromArgb(112, 124, 144);
					_palette.Entries[220] = Color.FromArgb(120, 128, 148);
					_palette.Entries[221] = Color.FromArgb(128, 132, 148);
					_palette.Entries[222] = Color.FromArgb(124, 136, 152);
					_palette.Entries[223] = Color.FromArgb(140, 140, 148);
					_palette.Entries[224] = Color.FromArgb(128, 144, 168);
					_palette.Entries[225] = Color.FromArgb(120, 124, 136);
					_palette.Entries[226] = Color.FromArgb(124, 116, 116);
					_palette.Entries[227] = Color.FromArgb(132, 104, 112);
					_palette.Entries[228] = Color.FromArgb(96, 100, 104);
					_palette.Entries[229] = Color.FromArgb(92, 96, 100);
					_palette.Entries[230] = Color.FromArgb(88, 92, 96);
					_palette.Entries[231] = Color.FromArgb(84, 88, 92);
					_palette.Entries[232] = Color.FromArgb(76, 80, 88);
					_palette.Entries[233] = Color.FromArgb(72, 80, 96);
					_palette.Entries[234] = Color.FromArgb(64, 76, 96);
					_palette.Entries[235] = Color.FromArgb(68, 80, 104);
					_palette.Entries[236] = Color.FromArgb(76, 88, 108);
					_palette.Entries[237] = Color.FromArgb(84, 92, 104);
					_palette.Entries[238] = Color.FromArgb(56, 68, 88);
					_palette.Entries[239] = Color.FromArgb(56, 64, 80);
					_palette.Entries[240] = Color.FromArgb(48, 60, 80);
					_palette.Entries[241] = Color.FromArgb(40, 56, 80);
					_palette.Entries[242] = Color.FromArgb(40, 60, 88);
					_palette.Entries[243] = Color.FromArgb(48, 64, 88);
					_palette.Entries[244] = Color.FromArgb(8, 80, 88);
					_palette.Entries[245] = Color.FromArgb(20, 96, 104);
					_palette.Entries[246] = Color.FromArgb(4, 108, 120);
					_palette.Entries[247] = Color.FromArgb(16, 120, 132);
					_palette.Entries[248] = Color.FromArgb(4, 136, 148);
					_palette.Entries[249] = Color.FromArgb(0, 156, 172);
					_palette.Entries[250] = Color.FromArgb(4, 172, 184);
					_palette.Entries[251] = Color.FromArgb(0, 208, 228);
					_palette.Entries[252] = Color.FromArgb(0, 232, 252);
					_palette.Entries[253] = Color.FromArgb(100, 112, 136);
					_palette.Entries[254] = Color.FromArgb(176, 140, 144);
					_palette.Entries[255] = Color.FromArgb(4, 56, 64);
					#endregion
				}

			_resource = new LfdFile(fsFile);
			fsFile.Close();

			for (int i = 0; i < _resource.Resources.Count; i++)
			{
				if (_resource.Resources[i].Type == Resource.ResourceType.Delt || _resource.Resources[i].Type == Resource.ResourceType.Anim)
				{
					lstLFD.Items.Add(_resource.Resources[i].ToString());
				}
				else if (_resource.Resources[i].Type == Resource.ResourceType.Pltt)
				{
					setPltt((Pltt)_resource.Resources[i]);
				}
			}
		}

		void setPltt(string path, long filePosition)
		{
			FileStream fsFile = null;
			try
			{
				fsFile = File.OpenRead(path);
				Pltt pltFile = new Pltt(fsFile, filePosition);
				setPltt(pltFile);
			}
			catch
			{
				System.Diagnostics.Debug.WriteLine("failed to set external PLTT");
			}
			if (fsFile != null) fsFile.Close();
		}
		void setPltt(Pltt palette)
		{
			// NOTE: there are PLTTs that use FF00FF and 0000FF as either placeholders or "skip" markers, not sure.
			// if that gets figured out, will need to add that filtering here.
			for (int i = palette.StartIndex; i < palette.EndIndex; i++) _palette.Entries[i] = palette.Entries[i];
			System.Diagnostics.Debug.WriteLine(palette.ToString() + " loaded");
		}
	}
}
