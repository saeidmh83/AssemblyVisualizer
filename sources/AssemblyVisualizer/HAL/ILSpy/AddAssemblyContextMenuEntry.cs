// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if ILSpy

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;
using AssemblyVisualizer.Properties;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.HAL.ILSpy
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
				.Select(n => HAL.Converter.Assembly(n.LoadedAssembly.AssemblyDefinition))
				.ToList();

			var window = WindowManager.AssemblyBrowsers.Single();
			window.ViewModel.AddAssemblies(assemblyDefinitions);
		}
	}
}
#endif