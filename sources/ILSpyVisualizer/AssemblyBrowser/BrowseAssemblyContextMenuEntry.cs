using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			return (selectedNodes.Count() == 1)
					&& (selectedNodes.Single() is AssemblyTreeNode);
		}

		public bool IsEnabled(SharpTreeNode[] selectedNodes)
		{
			return true;
		}

		public void Execute(SharpTreeNode[] selectedNodes)
		{
			var assemblyTreeNode = selectedNodes.Single() as AssemblyTreeNode;
			var assemblyDefinition = assemblyTreeNode.LoadedAssembly.AssemblyDefinition;
			
			var window = new AssemblyBrowserWindow(assemblyDefinition)
			             	{
			             		Owner = MainWindow.Instance
			             	};
			window.Show();
		}
	}
}
