using System.Collections.Generic;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.ILSpy;
using ILSpyVisualizer.AssemblyBrowser.Screens;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	class TypeViewModel : ViewModelBase
	{
		private readonly TypeDefinition _typeDefinition;
		private readonly AssemblyViewModel _assembly;
		private readonly IList<TypeViewModel> _derivedTypes = new List<TypeViewModel>();
		private bool _showMembers;

		private int _descendantsCount;
		private int _directDescendantsCount;
		private readonly int _membersCount;

		private bool _isCurrent;
		private readonly string _name;
		private readonly string _fullName;
		private string _baseTypeName;

		private readonly AssemblyBrowserWindowViewModel _windowViewModel;

		public TypeViewModel(TypeDefinition typeDefinition, AssemblyViewModel assembly, AssemblyBrowserWindowViewModel windowViewModel)
		{
			_typeDefinition = typeDefinition;
			_assembly = assembly;
			_windowViewModel = windowViewModel;

			_name = MainWindow.Instance.CurrentLanguage.FormatTypeName(typeDefinition);
			_fullName = string.Format("{0}.{1}", _typeDefinition.Namespace, _name);

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
			_membersCount = Members.Count();

			VisualizeCommand = new DelegateCommand(VisualizeCommandHandler);
			NavigateCommand = new DelegateCommand(NavigateCommandHandler);
			ShowMembersCommand = new DelegateCommand(ShowMembersCommandHandler);
		}

		public ICommand VisualizeCommand { get; private set; }
		public ICommand NavigateCommand { get; private set; }
		public ICommand ShowMembersCommand { get; private set; }

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
			get { return _name; }
		}

		public string FullName
		{
			get { return _fullName; }
		}

		public int DescendantsCount
		{
			get { return _descendantsCount; }
		}

		public int DirectDescendantsCount
		{
			get { return _directDescendantsCount; }
		}

		public int MembersCount
		{
			get { return _membersCount; }
		}

		public bool IsInternal
		{
			get { return _typeDefinition.IsNotPublic; }
		}

		public bool HasBaseType
		{
			get
			{
				return _typeDefinition.BaseType != null;
			}
		}

		public bool HasDescendants
		{
			get { return _descendantsCount > 0; }
		}

		public bool IsCurrent
		{
			get { return _isCurrent; }
			set
			{
				_isCurrent = value;
				OnPropertyChanged("IsCurrent");
			}
		}

		public string ExtendedInfo
		{
			get { return FullName; }
		}

		public IEnumerable<MemberViewModel> Members { get; set; }

		public IEnumerable<TypeViewModel> DerivedTypes
		{
			get { return _derivedTypes; }
		}

		public TypeViewModel BaseType { get; private set; }

		public string BaseTypeName
		{
			get
			{
				if (!HasBaseType)
				{
					return string.Empty;
				}
				if (string.IsNullOrEmpty(_baseTypeName))
				{
					_baseTypeName = MainWindow.Instance.CurrentLanguage.FormatTypeName(_typeDefinition.BaseType.Resolve());
				}
				return _baseTypeName;
			}
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

		public bool CanVisualize
		{
			get { return DerivedTypes.Count() > 0; }
		}

		public bool CanShowMembers
		{
			get { return _membersCount > 0; }
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

		private void ShowMembersCommandHandler()
		{
			var graphScreen = _windowViewModel.Screen as GraphScreen;
			if (graphScreen == null)
			{
				return;
			}
			graphScreen.ShowDetails(this);
		}
	}
}
