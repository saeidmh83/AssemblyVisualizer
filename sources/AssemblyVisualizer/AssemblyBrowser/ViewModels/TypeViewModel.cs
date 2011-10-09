// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using AssemblyVisualizer.Infrastructure;
using System.Linq;
using System.Windows.Input;
using AssemblyVisualizer.AssemblyBrowser.Screens;
using AssemblyVisualizer.Common;
using System.Windows.Media;
using AssemblyVisualizer.AncestryBrowser;
using AssemblyVisualizer.Properties;
using AssemblyVisualizer.Model;
using AssemblyVisualizer.HAL;
using System.Windows;
using AssemblyVisualizer.InteractionBrowser;

namespace AssemblyVisualizer.AssemblyBrowser.ViewModels
{
	class TypeViewModel : ViewModelBase
	{
		private static readonly Brush InternalBackground = new SolidColorBrush(Color.FromRgb(222, 222, 222));
		private static readonly Brush DefaultBackground = Brushes.White;
		private static readonly Brush GraphRootBackground = Brushes.Yellow;

		private readonly TypeInfo _typeInfo;
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
        private readonly AssemblyViewModel _assemblyViewModel;

		public TypeViewModel(TypeInfo typeInfo, AssemblyViewModel assemblyViewModel, AssemblyBrowserWindowViewModel windowViewModel)
		{
			_typeInfo = typeInfo;
			_windowViewModel = windowViewModel;
            _assemblyViewModel = assemblyViewModel;

            _name = typeInfo.Name;
            _fullName = typeInfo.FullName;
			_extendedInfo = IsInternal
			                	? string.Format("{0}\n{1}", FullName, Resources.Internal)
			                	: FullName;
            if (_typeInfo.IsEnum)
            {
                var enumValues = _typeInfo.Fields.Where(f => f.Name != "value__").Select(f => f.Name);
                if (enumValues.Count() > 0)
                {
                    var values = string.Join("\n", _typeInfo.Fields.Where(f => f.Name != "value__").Select(f => f.Name));
                    _extendedInfo = string.Format("{0}\n\n{1}", _extendedInfo, values);
                }
            }
            else if (_typeInfo.IsInterface)
            {
                var members = _typeInfo.Events.Select(m => m.Text)
                    .Concat(_typeInfo.Properties.Select(p => p.Text))
                    .Concat(_typeInfo.Methods.Select(p => p.Text));
                if (members.Count() > 0)
                {
                    var values = string.Join("\n", members);
                    _extendedInfo = string.Format("{0}\n\n{1}", _extendedInfo, values);
                }
            }

			if (HasBaseType)
			{
                var baseType = _typeInfo.BaseType;

                _baseTypeName = baseType.Name;
                _baseTypeFullName = baseType.FullName;
				if (baseType == null)
				{
					_baseTypeFullName = _baseTypeFullName + "\n" + Resources.NotAvailable;
					_isBaseTypeAvailable = false;
				}
			}

			var properties = typeInfo.Properties
				.Where(p => p.IsPublic)
				.Select(p => new PropertyViewModel(p))
				.OfType<MemberViewModel>();

			var events = typeInfo.Events
				.Where(e => e.IsPublic)
				.Select(e => new EventViewModel(e))
				.OfType<MemberViewModel>();

			var methods = typeInfo.Methods
				.Where(m => m.IsPublic)
				.Select(m => new MethodViewModel(m))
				.OfType<MemberViewModel>();

			Members = properties.Concat(events).Concat(methods);
            _membersCount = typeInfo.MembersCount;

			VisualizeCommand = new DelegateCommand(VisualizeCommandHandler);
			NavigateCommand = new DelegateCommand(NavigateCommandHandler);
			NavigateToBaseCommand = new DelegateCommand(NavigateToBaseCommandHandler);
			ShowMembersCommand = new DelegateCommand(ShowMembersCommandHandler);
            BrowseAncestryCommand = new DelegateCommand(BrowseAncestryCommandHandler);
            BrowseInteractionsCommand = new DelegateCommand(BrowseInteractionsCommandHandler);

			RefreshBackground();
            ResetName();
		}

		public ICommand VisualizeCommand { get; private set; }
		public ICommand NavigateCommand { get; private set; }
		public ICommand NavigateToBaseCommand { get; private set; }
		public ICommand ShowMembersCommand { get; private set; }
        public ICommand BrowseAncestryCommand { get; private set; }
        public ICommand BrowseInteractionsCommand { get; private set; }

		public TypeInfo TypeInfo
		{
			get { return _typeInfo; }
		}

        public AssemblyViewModel AssemblyViewModel
        {
            get { return _assemblyViewModel; }
        }

        public string NameStart { get; set; }
        public string NameMiddle { get; set; }
        public string NameEnd { get; set; }       

        public Thickness NameMargin
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NameMiddle))
                {
                    return new Thickness(0, 0, -4, 0);
                }
                if (string.IsNullOrWhiteSpace(NameStart)
                    && string.IsNullOrWhiteSpace(NameEnd))
                {
                    return new Thickness(-4, 0, -4, 0);
                }
                if (string.IsNullOrWhiteSpace(NameStart))
                {
                    return new Thickness(-4, 0, 0, 0);
                }
                if (string.IsNullOrWhiteSpace(NameEnd))
                {
                    return new Thickness(0, 0, -4, 0);
                }

                return new Thickness();
            }
        }

        public bool IsNameMiddleEmpty { get { return string.IsNullOrWhiteSpace(NameMiddle); } }

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

        public ImageSource Icon { get { return _typeInfo.Icon; } }

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
			get { return _typeInfo.IsInternal; }
		}

		public bool HasBaseType
		{
			get
			{
				return _typeInfo.BaseType != null;
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

        public void ResetName()
        {
            NameStart = Name;
            NameMiddle = string.Empty;
            NameEnd = string.Empty;
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
			Services.JumpTo(_typeInfo.MemberReference);
		}

		private void NavigateToBaseCommandHandler()
		{
			if (!IsBaseTypeAvailable)
			{
				return;
			}
			Services.JumpTo(_typeInfo.BaseType.MemberReference);
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

        private void BrowseAncestryCommandHandler()
        {
            Services.BrowseAncestry(TypeInfo);
        }

        private void BrowseInteractionsCommandHandler()
        {
            Services.BrowseInteractions(new[] { TypeInfo }, true);
        }
	}
}
