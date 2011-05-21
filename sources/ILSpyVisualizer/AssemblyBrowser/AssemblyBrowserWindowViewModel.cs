using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using System.Windows.Threading;
using ICSharpCode.ILSpy;
using System.Diagnostics;
using QuickGraph;
using System.Windows.Input;

namespace ILSpyVisualizer.AssemblyBrowser
{
	class AssemblyBrowserWindowViewModel : ViewModelBase
	{
		#region // Private fields

		private readonly ObservableCollection<AssemblyViewModel> _assemblies;
		private IEnumerable<TypeViewModel> _types;
		private IEnumerable<TypeDefinition> _allTypeDefinitions;
		private TypeGraph _graph;
		private bool _showGraph;

		private string _searchTerm = string.Empty;
		private bool _isSearchPerformed = true;

		private readonly Dispatcher _dispatcher;
		private DispatcherTimer _searchTimer;

		#endregion

		#region // .ctor

		public AssemblyBrowserWindowViewModel(IEnumerable<AssemblyDefinition> assemblyDefinitions, Dispatcher dispatcher)
		{
			_assemblies = new ObservableCollection<AssemblyViewModel>(
								assemblyDefinitions.Select(a => new AssemblyViewModel(a, this)));

			_dispatcher = dispatcher;

			NavigateToSearchViewCommand = new DelegateCommand(NavigateToSearchViewCommandHandler);

			OnAssembliesChanged();

			InitializeSearchTimer();
		}

		#endregion

		#region // Public properties

		public event Action GraphChanged;

		public ICommand NavigateToSearchViewCommand { get; private set; }

		public string Title
		{
			get { return "Assembly Browser"; }
		}

		public bool IsSearchPerformed
		{
			get { return _isSearchPerformed; }
			set
			{
				_isSearchPerformed = value;
				OnPropertyChanged("IsSearchPerformed");
			}
		}

		public string SearchTerm
		{
			get { return _searchTerm; }
			set
			{
				_searchTerm = value;
				if (_searchTimer.IsEnabled)
				{
					_searchTimer.Stop();
				}
				_searchTimer.Start();
				IsSearchPerformed = false;
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

		public bool ShowGraph
		{
			get { return _showGraph; }
			set
			{
				_showGraph = value;
				OnPropertyChanged("ShowGraph");
				OnPropertyChanged("IsInSearchView");
			}
		}

		public bool IsInSearchView
		{
			get { return !_showGraph; }
		}

		public IEnumerable<TypeViewModel> Types
		{
			get
			{
				if (string.IsNullOrWhiteSpace(SearchTerm))
				{
					return _types;
				}
				return _types.Where(SatisfiesSearchTerm);
			}
		}

		public ObservableCollection<AssemblyViewModel> Assemblies
		{
			get { return _assemblies; }
		}

		#endregion

		#region // Private properties

		private IEnumerable<TypeDefinition> AllTypeDefinitions
		{
			get
			{
				if (_allTypeDefinitions == null)
				{
					_allTypeDefinitions = _assemblies
						.Select(a => a.AssemblyDefinition)
						.SelectMany(a => a.Modules)
						.SelectMany(m => m.Types)
						.ToList();
				}
				return _allTypeDefinitions;
			}
		}

		private bool AreRemoveAssemblyButtonsVisible
		{	
			set
			{
				foreach (var assembly in _assemblies)
				{
					assembly.ShowRemoveCommand = value;
				}
			}
		}

		#endregion

		#region // Public methods

		public void ShowHierarchy(TypeViewModel type)
		{
			Graph = CreateGraph(type);
			ShowGraph = true;
			AreRemoveAssemblyButtonsVisible = false;
			OnGraphChanged();
		}

		public void AddAssemblies(IEnumerable<AssemblyDefinition> assemblies)
		{
			var newAssemblies = assemblies.Except(_assemblies.Select(a => a.AssemblyDefinition));
			foreach (var assembly in newAssemblies)
			{
				_assemblies.Add(new AssemblyViewModel(assembly, this));
			}

			OnAssembliesChanged();
		}

		public void AddAssembly(AssemblyDefinition assemblyDefinition)
		{
			if (_assemblies.Any(vm => vm.AssemblyDefinition == assemblyDefinition))
			{
				return;
			}

			_assemblies.Add(new AssemblyViewModel(assemblyDefinition, this));

			OnAssembliesChanged();
		}

		public void RemoveAssembly(AssemblyDefinition assemblyDefinition)
		{
			var assemblyViewModel = _assemblies
				.FirstOrDefault(vm => vm.AssemblyDefinition == assemblyDefinition);
			if (assemblyViewModel != null)
			{
				_assemblies.Remove(assemblyViewModel);
				OnAssembliesChanged();
			}
		}

		#endregion

		#region // Private methods

		private void OnAssembliesChanged()
		{
			UpdateInternalTypeCollections();
				RefreshTypesView();
		}

		private void RefreshTypesView()
		{
			OnPropertyChanged("Types");
		}

		private void UpdateInternalTypeCollections()
		{
			_allTypeDefinitions = null;
			_types = AllTypeDefinitions
				.OrderBy(t => t.Name)
				.Select(t => new TypeViewModel(t, this))
				.ToList();
			var typesDictionary = _types.ToDictionary(type => type.TypeDefinition);

			foreach (var typeDefinition in AllTypeDefinitions
				.Where(t => t.BaseType != null))
			{
				var baseType = typeDefinition.BaseType.Resolve();
				if (typesDictionary.ContainsKey(baseType))
				{
					typesDictionary[baseType].AddDerivedType(
						typesDictionary[typeDefinition]);
				}
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
			return graph;
		}

		private void OnGraphChanged()
		{
			var handler = GraphChanged;

			if (handler != null)
			{
				GraphChanged();
			}
		}

		#endregion

		#region // Private methods related to search

		private bool SatisfiesSearchTerm(TypeViewModel typeViewModel)
		{
			return typeViewModel
				.Name.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0;
			
		}

		private void InitializeSearchTimer()
		{
			_searchTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(300),
											   DispatcherPriority.Normal,
											   SearchTimerTick,
											   _dispatcher);
		}

		private void SearchTimerTick(object sender, EventArgs e)
		{
			_searchTimer.Stop();
			RefreshTypesView();
			IsSearchPerformed = true;
		}

		#endregion

		#region // Command handlers

		private void NavigateToSearchViewCommandHandler()
		{
			ShowGraph = false;
			Graph = null;
			AreRemoveAssemblyButtonsVisible = true;
		}

		#endregion
	}
}
