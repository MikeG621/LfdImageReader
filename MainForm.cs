/*
 * LFD Image Reader.exe, Exports images and animation frames from LFD resources
 * Copyright (C) 2008-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Version: 1.2
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
		private string _plttName;
		private string _plttFile;
		private ImageForm _frmImage;
		private string _installPath = "";
		LfdFile _resource = null;
		string _unk { get { return "Unknown"; } }

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
				else MessageBox.Show("Selection cancelled, cannot continue without standard install data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			if (_installPath != "") opnFile.InitialDirectory = _installPath;
			else cmdFile.Enabled = false;
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
			if (lstLFD.SelectedIndex != -1 && _plttFile != _unk) cmdShow.Enabled = true;
			else cmdShow.Enabled = false;
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
			_plttName = _unk;
			_plttFile = _unk;
			// Init standard Palette
			setPltt((Pltt)(new LfdFile(_installPath + "\\RESOURCE\\EMPIRE.LFD").Resources["PLTTstandard"]));
			// Okay, now we need to take into account for special cases where the PLTT isn't stored in the selected LFD
			if (opnFile.FileName.ToUpper().IndexOf("BATTLE") != -1)
			{
				_plttFile = _installPath + "\\RESOURCE\\TOURDESK.LFD";
				_plttName = "PLTTtoddesk";
				setPltt((Pltt)(new LfdFile(_plttFile).Resources[_plttName]));
			}
			else if (opnFile.FileName.ToUpper().IndexOf("SHIP") != -1)
			{
				_plttFile = _installPath + "\\RESOURCE\\LAUNCH.LFD";
				_plttName = "PLTTlaunch";
				setPltt((Pltt)(new LfdFile(_plttFile).Resources[_plttName]));
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
					_plttFile = opnFile.FileName;
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
			for (int i = palette.StartIndex; i < palette.EndIndex; i++) _palette.Entries[i] = palette.Entries[i];
			System.Diagnostics.Debug.WriteLine(palette.ToString() + " loaded");
		}

		private void opnTie_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{

		}
	}
}
