// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using ICSharpCode.ILSpy;
using ICSharpCode.TreeView;

namespace ILSpyVisualizer.Analyzer
{
	//[ExportContextMenuEntry(Header = "Analyze in window")]
	sealed class AnalyzeInWindowContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(SharpTreeNode[] selectedNodes)
		{
			return true;
		}

		public bool IsEnabled(SharpTreeNode[] selectedNodes)
		{
			return true;
		}

		public void Execute(SharpTreeNode[] selectedNodes)
		{
			
		}
	}

}
