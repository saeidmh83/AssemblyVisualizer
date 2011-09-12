// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if ILSpy

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;
using ILSpyVisualizer.Properties;
using ILSpyVisualizer.Model;
using ILSpyVisualizer.AncestryBrowser;

namespace ILSpyVisualizer.HAL.ILSpy
{
	[ExportContextMenuEntry(Header = "Browse Ancestry")]
	sealed class BrowseAncestryContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(SharpTreeNode[] selectedNodes)
		{
			return (selectedNodes.Count() == 1) 
				   && (selectedNodes.Single() is TypeTreeNode);
		}

		public bool IsEnabled(SharpTreeNode[] selectedNodes)
		{
			return true;
		}

		public void Execute(SharpTreeNode[] selectedNodes)
		{
			var typeDefinition = selectedNodes
				.OfType<TypeTreeNode>()
				.Single().TypeDefinition;	
			
			var window = new AncestryBrowserWindow(HAL.Converter.Type(typeDefinition))
			             	{
			             		Owner = MainWindow.Instance
			             	};
			window.Show();
		}
	}
}
#endif