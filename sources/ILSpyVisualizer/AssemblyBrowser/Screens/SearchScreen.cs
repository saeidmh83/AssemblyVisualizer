using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using ILSpyVisualizer.AssemblyBrowser.ViewModels;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;

namespace ILSpyVisualizer.AssemblyBrowser.Screens
{
	class SearchScreen : Screen
	{
		private string _searchTerm = string.Empty;
		private bool _isSearchPerformed = true;
		private DispatcherTimer _searchTimer;

		public SearchScreen(AssemblyBrowserWindowViewModel windowViewModel) : base(windowViewModel)
		{
			InitializeSearchTimer();
		}

		public event Action SearchFocusRequested;

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
				if (string.IsNullOrWhiteSpace(SearchTerm))
				{
					return WindowViewModel.Types;
				}

				return WindowViewModel.Types.Where(SatisfiesSearchTerm);
			}
		}

		public override void NotifyAssembliesChanged()
		{
			OnPropertyChanged("SearchResults");
		}

		public void FocusSearchField()
		{
			OnSearchFocusRequested();
		}

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
	}
}
