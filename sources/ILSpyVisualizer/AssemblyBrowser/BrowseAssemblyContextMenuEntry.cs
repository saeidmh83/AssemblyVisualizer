using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;

namespace ILSpyVisualizer.AssemblyBrowser
{
	[ExportContextMenuEntry(Header = "Browse Assembly")]
	sealed class BrowseAssemblyContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(SharpTreeNode[] selectedNodes)
		{
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
			
			var window = new AssemblyBrowserWindow(assemblyDefinitions)
			             	{
			             		Owner = MainWindow.Instance,
			             		WindowState = WindowState.Maximized
			             	};
			window.Show();
		}
	}
}
