using System.Collections.Generic;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.ILSpy;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	class TypeViewModel : ViewModelBase
	{
		private readonly TypeDefinition _typeDefinition;
		private readonly IList<TypeViewModel> _derivedTypes = new List<TypeViewModel>();
		private bool _showMembers;

		private int _descendantsCount;
		private int _directDescendantsCount;

		private readonly AssemblyBrowserWindowViewModel _windowViewModel;

		public TypeViewModel(TypeDefinition typeDefinition, AssemblyBrowserWindowViewModel windowViewModel)
		{
			_typeDefinition = typeDefinition;
			_windowViewModel = windowViewModel;

			var properties = typeDefinition.Properties
				.Where(p => p.GetMethod != null && p.GetMethod.IsPublic
							|| p.SetMethod != null && p.SetMethod.IsPublic)
				.Select(p => new PropertyViewModel(p))
				.OfType<MemberViewModel>();

			var events = typeDefinition.Events
				.Where(e => e.AddMethod.IsPublic)
				.Select(e => new EventViewModel(e))
				.OfType<MemberViewModel>();

			var methods = typeDefinition.Methods
				.Where(m => m.IsPublic && !m.IsGetter && !m.IsSetter && !m.IsAddOn && !m.IsRemoveOn)
				.Select(m => new MethodViewModel(m))
				.OfType<MemberViewModel>();

			Members = properties.Concat(events).Concat(methods);

			VisualizeCommand = new DelegateCommand(VisualizeCommandHandler);
			NavigateCommand = new DelegateCommand(NavigateCommandHandler);
		}

		public ICommand VisualizeCommand { get; private set; }
		public ICommand NavigateCommand { get; private set; }

		public TypeDefinition TypeDefinition
		{
			get { return _typeDefinition; }
		}

		public IEnumerable<TypeViewModel> FlattenedHierarchy
		{
			get 
			{ 
				var list = new List<TypeViewModel> {this};
				foreach (var typeViewModel in DerivedTypes)
				{
					list.AddRange(typeViewModel.FlattenedHierarchy);
				}
				return list;
			}
		}

		public string Name
		{
			get { return _typeDefinition.Name; }
		}

		public string FullName
		{
			get { return _typeDefinition.FullName; }
		}

		public int DescendantsCount
		{
			get { return _descendantsCount; }
		}

		public int DirectDescendantsCount
		{
			get { return _directDescendantsCount; }
		}

		public IEnumerable<MemberViewModel> Members { get; set; }

		public IEnumerable<TypeViewModel> DerivedTypes
		{
			get { return _derivedTypes; }
		}

		public TypeViewModel BaseType { get; private set; }

		public string BaseTypeName
		{
			get { return _typeDefinition.BaseType.Name; }
		}

		public bool ShowMembers
		{
			get { return _showMembers; }
			set 
			{ 
				_showMembers = value;
				OnPropertyChanged("ShowMembers");
			}
		} 

		public bool ShowVisualizeCommand
		{
			get { return DerivedTypes.Count() > 0; }
		}

		public void AddDerivedType(TypeViewModel typeViewModel)
		{
			_derivedTypes.Add(typeViewModel);
			typeViewModel.BaseType = this;
		}

		public void CountDescendants()
		{
			_descendantsCount = FlattenedHierarchy.Count() - 1;
			_directDescendantsCount = DerivedTypes.Count();

			OnPropertyChanged("DescendantsCount");
			OnPropertyChanged("DirectDescendantsCount");
		}

		private void VisualizeCommandHandler()
		{
			_windowViewModel.ShowGraph(this);
		}

		private void NavigateCommandHandler()
		{
			MainWindow.Instance.JumpToReference(_typeDefinition);
		}
	}
}
