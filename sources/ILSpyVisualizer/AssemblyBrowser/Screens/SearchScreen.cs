using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using ILSpyVisualizer.AssemblyBrowser.ViewModels;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using System.Windows.Input;
using System.Diagnostics;

namespace ILSpyVisualizer.AssemblyBrowser.Screens
{
	class SearchScreen : Screen
	{
		private enum SearchMode
		{
			All,
			Interfaces,
			ValueTypes,
			Enums
		}

		private const string HomePageUri = @"http://denismarkelov.blogspot.com/p/ilspy-visualizer.html";

		private string _searchTerm = string.Empty;
		private bool _isSearchPerformed = true;
		private DispatcherTimer _searchTimer;
		private SearchMode _searchMode = SearchMode.All;

		private bool _sortByName;

		#region // .ctor

		public SearchScreen(AssemblyBrowserWindowViewModel windowViewModel)
			: base(windowViewModel)
		{
			InitializeSearchTimer();

			NavigateToHomePageCommand = new DelegateCommand(() => Process.Start(HomePageUri));

			InitializeSearchControl();
		}

		private void InitializeSearchTimer()
		{
			_searchTimer = new DispatcherTimer(DispatcherPriority.Normal, WindowViewModel.Dispatcher)
			{
				Interval = TimeSpan.FromMilliseconds(400)
			};
			_searchTimer.Tick += SearchTimerTick;
		}

		private void InitializeSearchControl()
		{
			var sortingGroup = new CommandsGroupViewModel(
					"Sort by",
				    new List<GroupedUserCommand>
				    	{
				    		new GroupedUserCommand("Name", SortByName),
				    		new GroupedUserCommand("Descendants count", SortByDescendantsCount, true)
				    	});

			var filteringGroup = new CommandsGroupViewModel(
					"Types",
					new List<GroupedUserCommand>
			         	{
			            	new GroupedUserCommand("All", ShowAll, true),
			            	new GroupedUserCommand("Interfaces", ShowInterfaces),
							new GroupedUserCommand("Value types", ShowValueTypes),
							new GroupedUserCommand("Enums", ShowEnums)
			            });
			
			SearchControlGroups = new ObservableCollection<CommandsGroupViewModel>
			                      	{
			                      		sortingGroup,
										filteringGroup
			                      	};
		}

		#endregion

		public event Action SearchFocusRequested;

		public ICommand NavigateToHomePageCommand { get; private set; }

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

		public ObservableCollection<CommandsGroupViewModel> SearchControlGroups { get; private set; }

		public IEnumerable<TypeViewModel> SearchResults
		{
			get
			{
				var results = WindowViewModel.Types;

				if (!string.IsNullOrWhiteSpace(SearchTerm))
				{
					results = results.Where(SatisfiesSearchTerm);
				}

				switch (_searchMode)
				{
					case SearchMode.Interfaces:
						results = results.Where(t => t.TypeDefinition.IsInterface);
						break;
					case SearchMode.ValueTypes:
						results = results.Where(t => t.TypeDefinition.IsValueType);
						break;
					case SearchMode.Enums:
						results = results.Where(t => t.TypeDefinition.IsEnum);
						break;
				}
				
				results = _sortByName ? results.OrderBy(t => t.Name)
									  : results.OrderByDescending(t => t.DescendantsCount);

				return results;
			}
		}

		#region // Public methods

		public override void NotifyAssembliesChanged()
		{
			TriggerSearch();
		}

		public void FocusSearchField()
		{
			OnSearchFocusRequested();
		}

		#endregion

		#region // Private methods

		private bool SatisfiesSearchTerm(TypeViewModel typeViewModel)
		{
			return typeViewModel
				.Name.StartsWith(SearchTerm, StringComparison.InvariantCultureIgnoreCase);

		}

		private void SearchTimerTick(object sender, EventArgs e)
		{
			_searchTimer.Stop();
			TriggerSearch();
			IsSearchPerformed = true;
		}

		private void TriggerSearch()
		{
			OnPropertyChanged("SearchResults");
		}

		private void OnSearchFocusRequested()
		{
			var handler = SearchFocusRequested;
			if (handler != null)
			{
				handler();
			}
		}

		#endregion

		#region // Command handlers

		private void SortByName()
		{
			_sortByName = true;
			TriggerSearch();
		}

		private void SortByDescendantsCount()
		{
			_sortByName = false;
			TriggerSearch();
		}

		private void ShowInterfaces()
		{
			_searchMode = SearchMode.Interfaces;
			TriggerSearch();
		}

		private void ShowValueTypes()
		{
			_searchMode = SearchMode.ValueTypes;
			TriggerSearch();
		}

		private void ShowEnums()
		{
			_searchMode = SearchMode.Enums;
			TriggerSearch();
		}

		private void ShowAll()
		{
			_searchMode = SearchMode.All;
			TriggerSearch();
		}

		#endregion
	}
}
