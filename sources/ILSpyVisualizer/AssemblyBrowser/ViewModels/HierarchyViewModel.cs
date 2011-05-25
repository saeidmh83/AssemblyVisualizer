using System.Linq;
using ILSpyVisualizer.Infrastructure;
using System.Windows.Input;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	class HierarchyViewModel : ViewModelBase
	{
		private readonly AssemblyBrowserWindowViewModel _windowViewModel;
		private readonly TypeViewModel _root;

		public HierarchyViewModel(TypeViewModel root, AssemblyBrowserWindowViewModel windowViewModel)
		{
			_root = root;
			_windowViewModel = windowViewModel;

			VisualizeCommand = new DelegateCommand(VisualizeCommandHandler);
		}

		public ICommand VisualizeCommand { get; private set; }

		public TypeViewModel Root
		{
			get { return _root; }
		}

		public int DirectDescendantsCount
		{
			get { return _root.DerivedTypes.Count(); }
		}

		public int DescendantsCount
		{
			get { return _root.FlattenedHierarchy.Count() - 1; }
		}

		private void VisualizeCommandHandler()
		{
			_windowViewModel.ShowGraph(_root);
		}
	}
}
