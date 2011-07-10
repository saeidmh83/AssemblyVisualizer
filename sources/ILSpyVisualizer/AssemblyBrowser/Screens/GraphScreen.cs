// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ILSpyVisualizer.AssemblyBrowser.ViewModels;
using ILSpyVisualizer.Infrastructure;
using QuickGraph;
using System.Windows.Input;
using System.Windows;

namespace ILSpyVisualizer.AssemblyBrowser.Screens
{
	class GraphScreen : Screen
	{
		private const string ColorizeCaption = "Colorize";
		private const string DecolorizeCaption = "Decolorize";

		private TypeGraph _graph;
		private TypeViewModel _type;
		private TypeViewModel _typeForDetails;
		private TypeViewModel _currentType;
		private bool _isMembersPopupPinned;
		private string _searchTerm;
		private IEnumerable<TypeViewModel> _types;
		private readonly UserCommand _toggleColorizeUserCommand;

		public GraphScreen(AssemblyBrowserWindowViewModel windowViewModel) : base(windowViewModel)
		{
			PinCommand = new DelegateCommand(PinCommandHandler);
			HideSearchCommand = new DelegateCommand(HideSearchCommandHandler);
			ShowSearchCommand = new DelegateCommand(ShowSearchCommandHandler);

			_toggleColorizeUserCommand = new UserCommand(WindowViewModel.IsColorized
			                                             	? DecolorizeCaption
			                                             	: ColorizeCaption, ToggleColorizeCommandHandler);

			Commands = new ObservableCollection<UserCommand>
			           	{
			           		new UserCommand("Fill Graph", OnFillGraphRequest),
			           		new UserCommand("Original Size", OnOriginalSizeRequest),
			           		WindowViewModel.ShowSearchUserCommand,
			           		new UserCommand("Search in Graph", ShowSearchCommand),
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
		public event Action ShowInnerSearchRequest;
		public event Action HideInnerSearchRequest;

		public override bool AllowAssemblyDrop
		{
			get { return false; }
		}

		public TypeViewModel Type
		{
			get { return _type; }
			set
			{
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

		private TypeViewModel CurrentType
		{
			set
			{
				if (_currentType != null)
				{
					_currentType.IsCurrent = false;
				}
				_currentType = value;
				value.IsCurrent = true;
			}
		}

		private IEnumerable<TypeViewModel> Types
		{
			get
			{
				if (_types == null)
				{
					_types = _currentType.FlattenedHierarchy;
				}
				return _types;
			}
		}

		public override void ShowInnerSearch()
		{
			ShowSearchCommand.Execute(null);
		}

		public void ShowDetails(TypeViewModel type)
		{
			TypeForDetails = type;
			OnShowDetailsRequest();
		}

		public void Show(TypeViewModel type)
		{
			CurrentType = type;
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
			var flattededHierarchy = typeViewModel.FlattenedHierarchy;
			graph.AddVertexRange(flattededHierarchy);
			foreach (var viewModel in flattededHierarchy)
			{
				if (viewModel.BaseType == null || viewModel == typeViewModel)
				{
					continue;
				}
				graph.AddEdge(new Edge<TypeViewModel>(viewModel, viewModel.BaseType));
			}
			graph.Root = typeViewModel;
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
				type.IsMarked = type.Name.StartsWith(SearchTerm, StringComparison.OrdinalIgnoreCase);
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
			OnHideInnerSearchRequest();
			SearchTerm = string.Empty;
		}

		private void ShowSearchCommandHandler()
		{
			OnShowInnerSearchRequest();
			OnFocusSearchRequest();
		}

		private void ToggleColorizeCommandHandler()
		{
			WindowViewModel.IsColorized = !WindowViewModel.IsColorized;
			_toggleColorizeUserCommand.Text = WindowViewModel.IsColorized
			                                  	? DecolorizeCaption
			                                  	: ColorizeCaption;
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

		private void OnShowInnerSearchRequest()
		{
			var handler = ShowInnerSearchRequest;

			if (handler != null)
			{
				handler();
			}
		}

		private void OnHideInnerSearchRequest()
		{
			var handler = HideInnerSearchRequest;

			if (handler != null)
			{
				handler();
			}
		}
	}
}
