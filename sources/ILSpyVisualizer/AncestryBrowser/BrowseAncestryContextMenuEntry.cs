// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;

namespace ILSpyVisualizer.AncestryBrowser
{
	//[ExportContextMenuEntry(Header = "Browse Ancestry")]
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
			
			var window = new AncestryBrowserWindow(typeDefinition)
			             	{
			             		Owner = MainWindow.Instance
			             	};
			window.Show();
		}
	}
}
