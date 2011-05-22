using System.Windows.Media;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	abstract class MemberViewModel
	{
		public abstract ImageSource Icon { get; }
		public abstract string Text { get; }
	}
}
