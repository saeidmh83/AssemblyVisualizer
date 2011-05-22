using System.Windows.Media;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using Mono.Cecil;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	class EventViewModel : MemberViewModel
	{
		private readonly EventDefinition _eventDefinition;

		public EventViewModel(EventDefinition eventDefinition)
		{
			_eventDefinition = eventDefinition;
		}

		public override ImageSource Icon
		{
			get { return EventTreeNode.GetIcon(_eventDefinition); }
		}

		public override string Text
		{
			get
			{
				return EventTreeNode
					.GetText(_eventDefinition, MainWindow.Instance.CurrentLanguage) as string;
			}
		}
	}
}
