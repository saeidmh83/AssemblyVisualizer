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

namespace ILSpyVisualizer.AssemblyBrowser
{
	class AssemblyBrowserWindowViewModel : ViewModelBase
	{
		#region // Private fields

		private readonly IList<AssemblyDefinition> _assemblyDefinitions;
		private IEnumerable<TypeViewModel> _types;
		private IEnumerable<TypeViewModel> _rootTypes;
		private readonly ObservableCollection<AssemblyViewModel> _assemblies;

		private readonly IEnumerable<string> _specialTypes = new[]
		                                                     	{
		                                                     		"System.Object",
		                                                     		"System.ValueType",
		                                                     		"System.Enum",
		                                                     		"System.Attribute"
		                                                     	};

		private bool _showTypeHierarchies = true;
		private string _searchTerm = string.Empty;
		private bool _isSearchPerformed = true;

		private readonly Dispatcher _dispatcher;
		private DispatcherTimer _searchTimer;

		#endregion

		#region // .ctor

		public AssemblyBrowserWindowViewModel(IEnumerable<AssemblyDefinition> assemblyDefinitions, Dispatcher dispatcher)
		{
			_assemblyDefinitions = assemblyDefinitions.ToList();
			_assemblies = new ObservableCollection<AssemblyViewModel>(
								_assemblyDefinitions.Select(a => new AssemblyViewModel(a, this)));

			_dispatcher = dispatcher;

			UpdateInternalTypeCollections();
			RefreshTypesView();

			InitializeSearchTimer();
		}

		#endregion

		#region // Public properties

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

		public IEnumerable<TypeViewModel> Types
		{
			get
			{
				if (ShowTypeHierarchies)
				{
					if (string.IsNullOrEmpty(SearchTerm))
					{
						return _rootTypes;
					}
					return _rootTypes.Where(SatisfiesSearchTerm);
				}
				if (string.IsNullOrEmpty(SearchTerm))
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

		public bool ShowTypeHierarchies
		{
			get { return _showTypeHierarchies; }
			set
			{
				if (_showTypeHierarchies == value)
				{
					return;
				}
				_showTypeHierarchies = value;
				UpdateTypesAccordingToViewMode();
				OnPropertyChanged("ShowTypeHierarchies");
				OnPropertyChanged("ShowSingleTypes");
			}
		}

		public bool ShowSingleTypes
		{
			get { return !_showTypeHierarchies; }
			set
			{
				if (_showTypeHierarchies == !value)
				{
					return;
				}
				_showTypeHierarchies = !value;
				UpdateTypesAccordingToViewMode();
				OnPropertyChanged("ShowTypeHierarchies");
				OnPropertyChanged("ShowSingleTypes");
			}
		}

		#endregion

		#region // Private properties

		private IEnumerable<TypeDefinition> AllTypeDefinitions
		{
			get
			{
				return _assemblyDefinitions.SelectMany(a => a.Modules)
										   .SelectMany(m => m.Types)
										   .ToList();
			}
		}

		#endregion

		#region // Public methods

		public void AddAssemblyDefinition(AssemblyDefinition assemblyDefinition)
		{
			if (_assemblyDefinitions.Contains(assemblyDefinition))
			{
				return;
			}

			_assemblyDefinitions.Add(assemblyDefinition);
			_assemblies.Add(new AssemblyViewModel(assemblyDefinition, this));

			UpdateInternalTypeCollections();
			RefreshTypesView();
		}

		public void RemoveAssemblyDefinition(AssemblyDefinition assemblyDefinition)
		{
			_assemblyDefinitions.Remove(assemblyDefinition);
			var assemblyViewModel = _assemblies
				.Where(a => a.AssemblyDefinition == assemblyDefinition)
				.Single();
			_assemblies.Remove(assemblyViewModel);

			UpdateInternalTypeCollections();
			RefreshTypesView();
		}

		#endregion

		#region // Private methods

		private void RefreshTypesView()
		{
			OnPropertyChanged("Types");
		}

		private void UpdateInternalTypeCollections()
		{
			var typeViewModelsDictionary = new Dictionary<TypeDefinition, TypeViewModel>();

			_types = AllTypeDefinitions
						.Select(t =>
						{
							var viewModel = new TypeViewModel(t);
							typeViewModelsDictionary.Add(t, viewModel);
							return viewModel;
						})
						.ToList();


			foreach (var typeDefinition in AllTypeDefinitions
				.Where(t => t.BaseType != null))
			{
				var baseType = typeDefinition.BaseType.Resolve();
				if (typeViewModelsDictionary.ContainsKey(baseType))
				{
					typeViewModelsDictionary[baseType].AddDerivedType(
						typeViewModelsDictionary[typeDefinition]);
				}
			}

			_rootTypes = _types.Where(t => (t.BaseType == null
											|| _specialTypes.Contains(t.BaseType.FullName))
											&& !_specialTypes.Contains(t.FullName))
							   .ToList();
		}

		private void UpdateTypesAccordingToViewMode()
		{
			foreach (var typeViewModel in _types)
			{
				typeViewModel.ShowDerivedTypes = ShowTypeHierarchies;
			}
			RefreshTypesView();
		}

		#endregion

		#region // Private methods related to search

		private bool SatisfiesSearchTerm(TypeViewModel typeViewModel)
		{
			var currentSatisfies = typeViewModel.Name.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0;
			if (currentSatisfies)
			{
				return true;
			}
			if (ShowTypeHierarchies)
			{
				return typeViewModel.DerivedTypes.Any(SatisfiesSearchTerm);
			}
			return false;
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
	}
}
