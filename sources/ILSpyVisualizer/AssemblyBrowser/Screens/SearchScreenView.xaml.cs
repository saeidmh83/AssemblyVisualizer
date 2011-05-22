using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ILSpyVisualizer.AssemblyBrowser.Screens
{
	/// <summary>
	/// Interaction logic for SearchScreenView.xaml
	/// </summary>
	public partial class SearchScreenView : UserControl
	{
		private bool _isInitialized;

		public SearchScreenView()
		{
			InitializeComponent();
			Loaded += LoadedHandler;
		}

		private SearchScreen ViewModel
		{
			get { return DataContext as SearchScreen; }
		}

		private void LoadedHandler(object sender, RoutedEventArgs e)
		{
			if (!_isInitialized)
			{
				ViewModel.SearchFocusRequested += SearchFocusRequestedHandler;
				_isInitialized = true;
			}
			txtSearch.Focus();
		}

		private void SearchFocusRequestedHandler()
		{
			txtSearch.Focus();
		}
	}
}
