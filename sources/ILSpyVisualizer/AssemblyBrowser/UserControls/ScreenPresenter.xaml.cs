// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ILSpyVisualizer.AssemblyBrowser.Screens;

namespace ILSpyVisualizer.AssemblyBrowser.UserControls
{
	/// <summary>
	/// Interaction logic for CachingScreenPresenter.xaml
	/// </summary>
	partial class ScreenPresenter : UserControl
	{
		#region // Dependency properties

		public FrameworkElement SearchView
		{
			get { return (FrameworkElement)GetValue(SearchViewProperty); }
			set { SetValue(SearchViewProperty, value); }
		}
		
		public static readonly DependencyProperty SearchViewProperty =
			DependencyProperty.Register("SearchView", typeof(FrameworkElement), typeof(ScreenPresenter), new UIPropertyMetadata(null));
		
		public Screen Screen
		{
			get { return (Screen)GetValue(ScreenProperty); }
			set { SetValue(ScreenProperty, value); }
		}

		public static readonly DependencyProperty ScreenProperty =
			DependencyProperty.Register("Screen", typeof(Screen), typeof(ScreenPresenter), new UIPropertyMetadata(null, ScreenPropertyChangedCallback));

		private static void ScreenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var screenPresenter = dependencyObject as ScreenPresenter;
			var screenType = e.NewValue.GetType();
			
			if (screenType == typeof(GraphScreen))
			{
				var view = new GraphScreenView
				           	{
				           		DataContext = e.NewValue
				           	};
				view.LayoutFinished += screenPresenter.LayoutFinishedHandler;
				screenPresenter.cpGraph.Content = view;
			}

			if (e.NewValue == null || e.OldValue == null)
			{
				return;
			}

			if (e.NewValue.GetType() == typeof(SearchScreen)
				&& e.OldValue.GetType() == typeof(GraphScreen))
			{
				screenPresenter.ShowSearch();
			}
		}

		#endregion

		public ScreenPresenter()
		{
			InitializeComponent();
		}

		private void ShowSearch()
		{
			var animation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(350));
			animation.Completed += ShowSearchCompletedHandler;
			cpSearch.Visibility = Visibility.Visible;
			cpSearch.BeginAnimation(OpacityProperty, animation);
		}

		private void HideSearch()
		{
			var animation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(350));
			animation.Completed += HideSearchCompletedHandler;
			cpSearch.BeginAnimation(OpacityProperty, animation);
		}

		private void ShowSearchCompletedHandler(object sender, EventArgs e)
		{
			cpGraph.Content = null;
		}

		private void HideSearchCompletedHandler(object sender, EventArgs e)
		{
			if (Screen.GetType() != typeof(SearchScreen))
			{
				cpSearch.Visibility = Visibility.Collapsed;
			}
		}

		private void LayoutFinishedHandler()
		{
			HideSearch();
		}
	}
}
