using ICSharpCode.ILSpy;
using ICSharpCode.TreeView;

namespace ILSpyVisualizer.Analyzer
{
	[ExportContextMenuEntry(Header = "Analyze in window")]
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
			/*foreach (IMemberTreeNode node in selectedNodes)
			{
				var analyzerTreeNode = GetAnalyzerTreeNode(node.Member);
				if (analyzerTreeNode != null)
				{
					var window = new AnalyzerWindow(analyzerTreeNode)
					             	{
					             		Owner = MainWindow.Instance
					             	};
					window.Show();
				}
			}*/
		}

		/*private static AnalyzerTreeNode GetAnalyzerTreeNode(MemberReference member)
		{
			var field = member as FieldDefinition;
			if (field != null)
			{
				return new AnalyzedFieldTreeNode(field);
			}
			
			var method = member as MethodDefinition;
			if (method != null)
			{
				return new AnalyzedMethodTreeNode(method);
			}
			
			var propertyAnalyzer = AnalyzedPropertyTreeNode.TryCreateAnalyzer(member);
			if (propertyAnalyzer != null)
			{
				return propertyAnalyzer;
			}
			
			var eventAnalyzer = AnalyzedEventTreeNode.TryCreateAnalyzer(member);
			if (eventAnalyzer != null)
			{
				return eventAnalyzer;
			}

			return null;
		}*/
	}

}
