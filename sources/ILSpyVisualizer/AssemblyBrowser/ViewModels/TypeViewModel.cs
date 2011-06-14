// Copyright 2011 Denis Markelov
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

using System.Collections.Generic;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.ILSpy;
using ILSpyVisualizer.AssemblyBrowser.Screens;
using System.Windows.Media;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	class TypeViewModel : ViewModelBase
	{
		private static readonly Brush InternalBackground = new SolidColorBrush(Color.FromRgb(222, 222, 222));
		private static readonly Brush DefaultBackground = Brushes.White;
		private static readonly Brush GraphRootBackground = Brushes.Yellow;

		private readonly TypeDefinition _typeDefinition;
		private readonly AssemblyViewModel _assembly;
		private readonly IList<TypeViewModel> _derivedTypes = new List<TypeViewModel>();

		private int _descendantsCount;
		private int _directDescendantsCount;
		private readonly int _membersCount;
		
		private bool _showMembers;
		private bool _isCurrent;
		private bool _isMarked;
		private readonly bool _isBaseTypeAvailable = true;
		private readonly string _name;
		private readonly string _fullName;
		private readonly string _baseTypeName;
		private readonly string _baseTypeFullName;
		private readonly string _extendedInfo;
		private Brush _background;

		private readonly AssemblyBrowserWindowViewModel _windowViewModel;

		public TypeViewModel(TypeDefinition typeDefinition, AssemblyViewModel assembly, AssemblyBrowserWindowViewModel windowViewModel)
		{
			_typeDefinition = typeDefinition;
			_assembly = assembly;
			_windowViewModel = windowViewModel;

			_name = MainWindow.Instance.CurrentLanguage.FormatTypeName(typeDefinition);
			_fullName = GetFullName(_typeDefinition.Namespace, _name);
			_extendedInfo = IsInternal
			                	? string.Format("{0}\nInternal", FullName)
			                	: FullName;

			if (HasBaseType)
			{
				var baseType = _typeDefinition.BaseType.Resolve();
				
				_baseTypeName = baseType != null ? MainWindow.Instance.CurrentLanguage
					.FormatTypeName(baseType) : _typeDefinition.BaseType.Name;
				_baseTypeFullName = GetFullName(_typeDefinition.BaseType.Namespace, BaseTypeName);
				if (baseType == null)
				{
					_baseTypeFullName = _baseTypeFullName + "\nNot available";
					_isBaseTypeAvailable = false;
				}
			}

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
			NavigateToBaseCommand = new DelegateCommand(NavigateToBaseCommandHandler);
			ShowMembersCommand = new DelegateCommand(ShowMembersCommandHandler);

			RefreshBackground();
		}

		public ICommand VisualizeCommand { get; private set; }
		public ICommand NavigateCommand { get; private set; }
		public ICommand NavigateToBaseCommand { get; private set; }
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

		public bool IsBaseTypeAvailable
		{
			get { return _isBaseTypeAvailable; }
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
				RefreshBackground();
			}
		}

		public bool IsMarked
		{
			get { return _isMarked; }
			set
			{
				_isMarked = value;
				OnPropertyChanged("IsMarked");
			}
		}

		public Brush Background
		{
			get { return _background; }
			private set
			{
				_background = value;
				OnPropertyChanged("Background");
			}
		}

		public Brush AssignedBackground { get; set; }

		public string ExtendedInfo
		{
			get { return _extendedInfo; }
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
				return _baseTypeName;
			}
		}

		public string BaseTypeFullName
		{
			get { return _baseTypeFullName; }
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

		public void RefreshBackground()
		{
			var brush = DefaultBackground; 
			if (IsInternal)
			{
				brush = InternalBackground;
			}
			if (AssignedBackground != null)
			{
				brush = AssignedBackground;
			}
			if (IsCurrent)
			{
				brush = GraphRootBackground;
			}

			Background = brush;
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

		private void NavigateToBaseCommandHandler()
		{
			if (!IsBaseTypeAvailable)
			{
				return;
			}
			MainWindow.Instance.JumpToReference(_typeDefinition.BaseType);
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

		private static string GetFullName(string typeNamespace, string typeName)
		{
			if (string.IsNullOrEmpty(typeNamespace))
			{
				return typeName;
			}
			return string.Format("{0}.{1}", typeNamespace, typeName);
		}
	}
}
