using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using System.Windows.Threading;
using ICSharpCode.ILSpy;

namespace ILSpyVisualizer.AssemblyBrowser
{
	class AssemblyBrowserWindowViewModel : ViewModelBase
	{
		#region // Private fields

		private readonly AssemblyDefinition _assemblyDefinition;
		private readonly IEnumerable<TypeViewModel> _hierarchyRootTypes;
		private readonly IEnumerable<TypeViewModel> _singleTypes;

		private bool _showTypeHierarchies = true;
		private string _searchTerm = string.Empty;
		private bool _isSearchPerformed = true;

		private readonly Dispatcher _dispatcher;
		private DispatcherTimer _searchTimer;

		#endregion

		#region // .ctor

		public AssemblyBrowserWindowViewModel(AssemblyDefinition assemblyDefinition, Dispatcher dispatcher)
		{
			_assemblyDefinition = assemblyDefinition;
			_dispatcher = dispatcher;

			var typeViewModelsDictionary = new Dictionary<TypeDefinition, TypeViewModel>();
			
			var types = _assemblyDefinition.MainModule.Types
				.Select(t =>
				        	{
				        		var viewModel = new TypeViewModel(t);
								typeViewModelsDictionary.Add(t, viewModel);
				        		return viewModel;
				        	}).ToList();

			foreach (var typeDefinition in _assemblyDefinition.MainModule.Types
				.Where(t => t.BaseType != null))
			{
				var baseType = typeDefinition.BaseType.Resolve();
				if (typeViewModelsDictionary.ContainsKey(baseType))
				{
					typeViewModelsDictionary[baseType].AddDerivedType(
						typeViewModelsDictionary[typeDefinition]);
				}
			}

			var specialTypes = new[]
			                   	{
									"System.Object", 
									"System.ValueType", 
									"System.Enum", 
									"System.Attribute"
			                   	};

			var baseTypes = types.Where(t => (t.BaseType == null 
										|| specialTypes.Contains(t.BaseType.FullName))
										&& !specialTypes.Contains(t.FullName));

			_hierarchyRootTypes = baseTypes.Where(t => t.DerivedTypes.Count() > 0)
				.OrderBy(t => t.Name);
			_singleTypes = baseTypes.Where(t => t.DerivedTypes.Count() == 0)
				.OrderBy(t => t.Name);

			RefreshTypeCollections();

			InitializeSearchTimer();
		}

		#endregion

		#region // Public properties

		public string Title
		{
			get { return _assemblyDefinition.Name.Name; }
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

		public IEnumerable<TypeViewModel> HierarchyRootTypes
		{
			get
			{
				if (string.IsNullOrEmpty(SearchTerm))
				{
					return _hierarchyRootTypes;
				}
				return _hierarchyRootTypes.Where(SatisfiesSearchTerm);
			}
		}

		public IEnumerable<TypeViewModel> SingleTypes
		{
			get
			{
				if (string.IsNullOrEmpty(SearchTerm))
				{
					return _singleTypes;
				}
				return _singleTypes.Where(SatisfiesSearchTerm);
			}
		}

		public bool ShowTypeHierarchies
		{
			get { return _showTypeHierarchies; }
			set
			{
				_showTypeHierarchies = value;
				OnPropertyChanged("ShowTypeHierarchies");
				OnPropertyChanged("ShowSingleTypes");
			}
		}

		public bool ShowSingleTypes
		{
			get { return !_showTypeHierarchies; }
			set
			{
				_showTypeHierarchies = !value;
				OnPropertyChanged("ShowTypeHierarchies");
				OnPropertyChanged("ShowSingleTypes");
			}
		}

		#endregion

		#region // Private methods

		private void RefreshTypeCollections()
		{
			OnPropertyChanged("HierarchyRootTypes");
			OnPropertyChanged("SingleTypes");
			IsSearchPerformed = true;
		}

		#endregion

		#region // Private methods related to search

		private bool SatisfiesSearchTerm(TypeViewModel typeViewModel)
		{
			return typeViewModel.Name.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
				   || typeViewModel.DerivedTypes.Any(SatisfiesSearchTerm);
		}

		private void InitializeSearchTimer()
		{
			_searchTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(1000), 
											   DispatcherPriority.Normal,
											   SearchTimerTick,
											   _dispatcher);
		}

		private void SearchTimerTick(object sender, EventArgs e)
		{
			_searchTimer.Stop();
			RefreshTypeCollections();
		}

		#endregion
	}
}
