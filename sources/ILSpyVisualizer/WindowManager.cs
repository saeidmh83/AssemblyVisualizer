// Copyright 2011 Denis Markelov
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.AssemblyBrowser;

namespace ILSpyVisualizer
{
	static class WindowManager
	{
		private static readonly IList<AssemblyBrowserWindow> _assemblyBrowsers = 
			new List<AssemblyBrowserWindow>();

		public static IList<AssemblyBrowserWindow> AssemblyBrowsers
		{
			get { return _assemblyBrowsers; }
		}

		public static void AddAssemblyBrowser(AssemblyBrowserWindow window)
		{
			_assemblyBrowsers.Add(window);
		}

		public static void RemoveAssemblyBrowser(AssemblyBrowserWindow window)
		{
			_assemblyBrowsers.Remove(window);
		}
	}
}
