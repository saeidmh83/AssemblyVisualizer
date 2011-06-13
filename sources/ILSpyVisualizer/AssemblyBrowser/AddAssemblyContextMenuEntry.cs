// Copyright 2011 Denis Markelov
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;

namespace ILSpyVisualizer.AssemblyBrowser
{
	[ExportContextMenuEntry(Header = "Add to Browser")]
	sealed class AddAssemblyContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(SharpTreeNode[] selectedNodes)
		{
			if (WindowManager.AssemblyBrowsers.Count != 1)
			{
				return false;
			}

			var window = WindowManager.AssemblyBrowsers.Single();
			if (!window.ViewModel.Screen.AllowAssemblyDrop)
			{
				return false;
			}

			return selectedNodes.All(n => n is AssemblyTreeNode);
		}

		public bool IsEnabled(SharpTreeNode[] selectedNodes)
		{
			return true;
		}

		public void Execute(SharpTreeNode[] selectedNodes)
		{
			var assemblyDefinitions = selectedNodes
				.OfType<AssemblyTreeNode>()
				.Select(n => n.LoadedAssembly.AssemblyDefinition)
				.ToList();

			var window = WindowManager.AssemblyBrowsers.Single();
			window.ViewModel.AddAssemblies(assemblyDefinitions);
		}
	}
}
