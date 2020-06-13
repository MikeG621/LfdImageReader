/*
 * LFD Image Reader.exe, Exports images and animation frames from LFD resources
 * Copyright (C) 2008-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Version: 1.2
 */
 
using System;
using System.Windows.Forms;

namespace Idmr.LfdImageReader
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.Run(new MainForm());
		}
	}
}
