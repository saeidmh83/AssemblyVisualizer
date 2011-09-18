// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AssemblyVisualizer.AssemblyBrowser.ViewModels;
using AssemblyVisualizer.Controls.Graph.QuickGraph;
using AssemblyVisualizer.Infrastructure;
using System.Windows.Input;
using System.Windows;
using AssemblyVisualizer.Properties;

namespace AssemblyVisualizer.AssemblyBrowser.Screens
{
	class GraphScreen : Screen
	{	
		private TypeGraph _graph;
		private TypeViewModel _type;
		private TypeViewModel _typeForDetails;		
		private bool _isMembersPopupPinned;
		private string _searchTerm;
		private IEnumerable<TypeViewModel> _types;
		private readonly UserCommand _toggleColorizeUserCommand;
        private bool _isSearhVisible;
        private bool _isAssemblyListVisible = true;

		public GraphScreen(AssemblyBrowserWindowViewModel windowViewModel) : base(windowViewModel)
		{
			PinCommand = new DelegateCommand(PinCommandHandler);
			HideSearchCommand = new DelegateCommand(HideSearchCommandHandler);
			ShowSearchCommand = new DelegateCommand(ShowSearchCommandHandler);            

			_toggleColorizeUserCommand = new UserCommand(WindowViewModel.IsColorized
			                                             	? Resources.Decolorize
			                                             	: Resources.Colorize, ToggleColorizeCommandHandler);

			Commands = new ObservableCollection<UserCommand>
			           	{
			           		new UserCommand(Resources.FillGraph, OnFillGraphRequest),
			           		new UserCommand(Resources.OriginalSize, OnOriginalSizeRequest),
			           		                    WindowViewModel.ShowSearchUserCommand,
			           		new UserCommand(Resources.SearchInGraph, ShowSearchCommand),
			           		_toggleColorizeUserCommand
			           	};
		}

		public ICommand PinCommand { get; private set; }
		public ICommand HideSearchCommand { get; private set; }
		public ICommand ShowSearchCommand { get; private set; }        

		public ObservableCollection<UserCommand> Commands { get; private set; }

		public event Action GraphChanged;
		public event Action ShowDetailsRequest;
		public event Action HideDetailsRequest;
		public event Action FillGraphRequest;
		public event Action OriginalSizeRequest;
		public event Action FocusSearchRequest;
		
		public override bool AllowAssemblyDrop
		{
			get { return false; }
		}

        public bool IsSearchVisible
        {
            get
            {
                return _isSearhVisible;
            }
            set
            {
                _isSearhVisible = value;
                OnPropertyChanged("IsSearchVisible");
            }
        }

        public bool IsAssemblyListVisible
        {
            get
            {
                return _isAssemblyListVisible;
            }
            set
            {
                _isAssemblyListVisible = value;
                OnPropertyChanged("IsAssemblyListVisible");
            }
        }

		public TypeViewModel Type
		{
			get { return _type; }
			set
			{
                if (_type != null)
                {
                    _type.IsCurrent = false;
                }                
                value.IsCurrent = true;
				_type = value;
				OnPropertyChanged("Type");
			}
		}

		public TypeViewModel TypeForDetails
		{
			get { return _typeForDetails; }
			set
			{
				_typeForDetails = value;
				OnPropertyChanged("TypeForDetails");
			}
		}

		public TypeGraph Graph
		{
			get { return _graph; }
			set
			{
				_graph = value;
				OnPropertyChanged("Graph");
			}
		}

		public string SearchTerm
		{
			get { return _searchTerm; }
			set
			{
				_searchTerm = value;
				OnPropertyChanged("SearchTerm");
				PerformSearch();
			}
		}

		public bool IsMembersPopupPinned
		{
			get { return _isMembersPopupPinned; }
			set
			{
				_isMembersPopupPinned = value;
				OnPropertyChanged("IsMembersPopupPinned");
			}
		}		

		private IEnumerable<TypeViewModel> Types
		{
			get
			{
				if (_types == null)
				{
					_types = _type.FlattenedHierarchy;
				}
				return _types;
			}
		}

		public override void ShowInnerSearch()
		{
			ShowSearchCommand.Execute(null);
		}

        public override void ToggleAssembliesVisibility()
        {
            IsAssemblyListVisible = !IsAssemblyListVisible;
        }   

		public void ShowDetails(TypeViewModel type)
		{
			TypeForDetails = type;
			OnShowDetailsRequest();
		}

		public void Show(TypeViewModel type)
		{			
			_types = null;
			Type = type;
			Graph = CreateGraph(type);
			OnGraphChanged();
		}

		public void ClearSearch()
		{
			foreach (var type in Types)
			{
				type.IsMarked = false;
			}
		}

		private static TypeGraph CreateGraph(TypeViewModel typeViewModel)
		{
			var graph = new TypeGraph(true);
			var flattenedHierarchy = typeViewModel.FlattenedHierarchy;
			graph.AddVertexRange(flattenedHierarchy);
			foreach (var viewModel in flattenedHierarchy)
			{
				if (viewModel.BaseType == null || viewModel == typeViewModel)
				{
					continue;
				}
				graph.AddEdge(new Edge<TypeViewModel>(viewModel, viewModel.BaseType));
			}			
			return graph;
		}

		private void PerformSearch()
		{
			if (string.IsNullOrEmpty(SearchTerm) || string.IsNullOrEmpty(SearchTerm.Trim()))
			{
				ClearSearch();
				return;
			}

			foreach (var type in Types)
			{
				type.IsMarked = type.Name
                    .IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0;
			}
		}

		private void PinCommandHandler()
		{
			if (!IsMembersPopupPinned)
			{
				IsMembersPopupPinned = true;
				return;
			}
			IsMembersPopupPinned = false;
			OnHideDetailsRequest();
		}

		private void HideSearchCommandHandler()
		{
            IsSearchVisible = false;
			SearchTerm = string.Empty;
		}

		private void ShowSearchCommandHandler()
		{
            IsSearchVisible = true;
			OnFocusSearchRequest();
		}            

		private void ToggleColorizeCommandHandler()
		{
			WindowViewModel.IsColorized = !WindowViewModel.IsColorized;
			_toggleColorizeUserCommand.Text = WindowViewModel.IsColorized
			                                  	? Resources.Decolorize
			                                  	: Resources.Colorize;
		}

		private void OnGraphChanged()
		{
			var handler = GraphChanged;

			if (handler != null)
			{
				handler();
			}
		}

		private void OnShowDetailsRequest()
		{
			var handler = ShowDetailsRequest;

			if (handler != null)
			{
				handler();
			}
		}

		private void OnHideDetailsRequest()
		{
			var handler = HideDetailsRequest;

			if (handler != null)
			{
				handler();
			}
		}

		private void OnFillGraphRequest()
		{
			var handler = FillGraphRequest;

			if (handler != null)
			{
				handler();
			}
		}

		private void OnOriginalSizeRequest()
		{
			var handler = OriginalSizeRequest;

			if (handler != null)
			{
				handler();
			}
		}

		private void OnFocusSearchRequest()
		{
			var handler = FocusSearchRequest;

			if (handler != null)
			{
				handler();
			}
		}		
	}
}
