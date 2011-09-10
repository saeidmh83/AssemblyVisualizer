// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using ILSpyVisualizer.AssemblyBrowser.ViewModels;
using ILSpyVisualizer.Infrastructure;

using System.Windows.Input;
using ILSpyVisualizer.Common.CommandsGroup;
using ILSpyVisualizer.Properties;

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

		private enum TypeVisibility
		{
			Any,
			Public,
			Internal
		}

		private enum SortingMode
		{
			Name,
			DescendantsCount,
			MembersCount
		}

		private const string HomePageUri = @"http://ilspyvisualizer.denismarkelov.com";

		private string _searchTerm = string.Empty;
		private bool _isSearchPerformed = true;
		private DispatcherTimer _searchTimer;
		private SearchMode _searchMode = SearchMode.All;
		private SortingMode _sortingMode = SortingMode.DescendantsCount;
		private TypeVisibility _typeVisibilityFilter = TypeVisibility.Any;
		
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
					Resources.SortBy,
				    new List<GroupedUserCommand>
				    	{
				    		new GroupedUserCommand(Resources.Name, SortByName),
				    		new GroupedUserCommand(Resources.DescendantsCount, SortByDescendantsCount, true),
							new GroupedUserCommand(Resources.MembersCount, SortByMembersCount)
				    	});

			var filteringByTypeGroup = new CommandsGroupViewModel(
					Resources.Types,
					new List<GroupedUserCommand>
			         	{
			            	new GroupedUserCommand(Resources.All, ShowAllTypes, true),
			            	new GroupedUserCommand(Resources.Interfaces, ShowInterfaces),
							new GroupedUserCommand(Resources.ValueTypes, ShowValueTypes),
							new GroupedUserCommand(Resources.Enums, ShowEnums)
			            });

			var filteringByVisibilityGroup = new CommandsGroupViewModel(
					Resources.Visibility,
					new List<GroupedUserCommand>
			         	{
			            	new GroupedUserCommand(Resources.Any, ShowAnyVisibility, true),
			            	new GroupedUserCommand(Resources.Public, ShowPublicTypes),
							new GroupedUserCommand(Resources.Internal, ShowInternalTypes)
			            });

			SearchControlGroups = new ObservableCollection<CommandsGroupViewModel>
			                      	{
			                      		sortingGroup,
										filteringByTypeGroup,
										filteringByVisibilityGroup
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

				OnPropertyChanged("IsSearchTermEmpty");
			}
		}

		public bool IsSearchTermEmpty
		{
			get { return string.IsNullOrEmpty(SearchTerm); }
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
						results = results.Where(t => t.TypeInfo.IsInterface);
						break;
					case SearchMode.ValueTypes:
						results = results.Where(t => t.TypeInfo.IsValueType);
						break;
					case SearchMode.Enums:
						results = results.Where(t => t.TypeInfo.IsEnum);
						break;
				}

				switch (_typeVisibilityFilter)
				{
					case TypeVisibility.Internal:
						results = results.Where(t => t.TypeInfo.IsInternal);
						break;
					case TypeVisibility.Public:
						results = results.Where(t => t.TypeInfo.IsPublic);
						break;
				}

				switch (_sortingMode)
				{
					case SortingMode.DescendantsCount:
						results = results.OrderByDescending(t => t.DescendantsCount);
						break;
					case SortingMode.MembersCount:
						results = results.OrderByDescending(t => t.MembersCount);
						break;
					case SortingMode.Name:
						results = results.OrderBy(t => t.Name);
						break;
				}

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

		public override void ShowInnerSearch()
		{
			OnSearchFocusRequested();
		}

		private bool SatisfiesSearchTerm(TypeViewModel typeViewModel)
		{
			return typeViewModel
				.Name.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0;

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
			_sortingMode = SortingMode.Name;
			TriggerSearch();
		}

		private void SortByDescendantsCount()
		{
			_sortingMode = SortingMode.DescendantsCount;
			TriggerSearch();
		}

		private void SortByMembersCount()
		{
			_sortingMode = SortingMode.MembersCount;
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

		private void ShowAllTypes()
		{
			_searchMode = SearchMode.All;
			TriggerSearch();
		}

		private void ShowAnyVisibility()
		{
			_typeVisibilityFilter = TypeVisibility.Any;
			TriggerSearch();
		}

		private void ShowPublicTypes()
		{
			_typeVisibilityFilter = TypeVisibility.Public;
			TriggerSearch();
		}

		private void ShowInternalTypes()
		{
			_typeVisibilityFilter = TypeVisibility.Internal;
			TriggerSearch();
		}

		#endregion
	}
}
