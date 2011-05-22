using System.Linq;
using ILSpyVisualizer.Infrastructure;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	class HierarchyViewModel : ViewModelBase
	{
		private readonly TypeViewModel _root;

		public HierarchyViewModel(TypeViewModel root)
		{
			_root = root;
		}

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
	}
}
