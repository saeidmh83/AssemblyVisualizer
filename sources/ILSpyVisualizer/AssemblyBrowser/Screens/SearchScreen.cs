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

namespace ILSpyVisualizer.AssemblyBrowser.Screens
{
	class SearchScreen : Screen
	{
		private string _searchTerm = string.Empty;
		private bool _isSearchPerformed = true;
		private DispatcherTimer _searchTimer;

		private bool _sortByName;
		private bool _showClasses = true;
		private bool _showStructures = true;
		private bool _showInterfaces = true;
		private bool _showEnums = true;

		public SearchScreen(AssemblyBrowserWindowViewModel windowViewModel)
			: base(windowViewModel)
		{
			InitializeSearchTimer();

			SortByNameCommand = new DelegateCommand(SortByNameCommandHandler);
			SortByDescendantsCountCommand = new DelegateCommand(SortByDescendantsCountCommandHandler);
		}

		public event Action SearchFocusRequested;

		public ICommand SortByNameCommand { get; private set; }
		public ICommand SortByDescendantsCountCommand { get; private set; }

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


		public IEnumerable<TypeViewModel> SearchResults
		{
			get
			{
				var results = WindowViewModel.Types;

				if (!string.IsNullOrWhiteSpace(SearchTerm))
				{
					results = results.Where(SatisfiesSearchTerm);
				}

				results = _sortByName ? results.OrderBy(t => t.Name)
									  : results.OrderByDescending(t => t.DescendantsCount);

				if (!ShowClasses)
				{
					results = results.Where(t => !t.TypeDefinition.IsClass);
				}
				else
				{
					if (!ShowStructures)
					{
						results = results.Where(t => !t.TypeDefinition.IsValueType);
					}
					else
					{
						if (!ShowEnums)
						{
							results = results.Where(t => !t.TypeDefinition.IsEnum);
						}
					}
				}
				if (!ShowInterfaces)
				{
					results = results.Where(t => !t.TypeDefinition.IsInterface);
				}

				return results;
			}
		}

		public bool ShowClasses
		{
			get { return _showClasses; }
			set
			{
				_showClasses = value;
				OnPropertyChanged("ShowClasses");
				TriggerSearch();
			}
		}

		public bool ShowStructures
		{
			get { return _showStructures; }
			set
			{
				_showStructures = value;
				OnPropertyChanged("ShowStructures");
				TriggerSearch();
			}
		}

		public bool ShowInterfaces
		{
			get { return _showInterfaces; }
			set
			{
				_showInterfaces = value;
				OnPropertyChanged("ShowInterfaces");
				TriggerSearch();
			}
		}

		public bool ShowEnums
		{
			get { return _showEnums; }
			set
			{
				_showEnums = value;
				OnPropertyChanged("ShowEnums");
				TriggerSearch();
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

		private void InitializeSearchTimer()
		{
			_searchTimer = new DispatcherTimer(DispatcherPriority.Normal, WindowViewModel.Dispatcher)
							{
								Interval = TimeSpan.FromMilliseconds(400)
							};
			_searchTimer.Tick += SearchTimerTick;
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

		private void SortByNameCommandHandler()
		{
			_sortByName = true;
			TriggerSearch();
		}

		private void SortByDescendantsCountCommandHandler()
		{
			_sortByName = false;
			TriggerSearch();
		}

		#endregion
	}
}
