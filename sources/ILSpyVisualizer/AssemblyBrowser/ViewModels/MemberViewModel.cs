using System.Windows.Media;
using ILSpyVisualizer.Infrastructure;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	abstract class MemberViewModel : ViewModelBase
	{
		public abstract ImageSource Icon { get; }
		public abstract string Text { get; }
	}
}
