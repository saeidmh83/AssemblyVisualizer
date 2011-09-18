// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

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
using System.Windows.Media.Animation;
using AssemblyVisualizer.Controls.ZoomControl;
using AssemblyVisualizer.Behaviors;

namespace AssemblyVisualizer.AssemblyBrowser.Screens
{	
	internal partial class GraphScreenView : UserControl
	{
		public event Action LayoutFinished;

		public GraphScreenView()
		{
			InitializeComponent();

			Loaded += LoadedHandler;
            Unloaded += UnloadedHandler;
		}        

		private GraphScreen ViewModel
		{
			get { return DataContext as GraphScreen; }
		}

		private void GraphChangedHandler()
		{
			zoomControl.ZoomToFill();
		}

		private void LoadedHandler(object sender, RoutedEventArgs e)
		{
			ViewModel.GraphChanged += GraphChangedHandler;
			ViewModel.ShowDetailsRequest += ShowDetailsRequestHandler;
			ViewModel.HideDetailsRequest += HideDetailsRequestHandler;
			ViewModel.FillGraphRequest += FillGraphRequestHandler;
			ViewModel.OriginalSizeRequest += OriginalSizeRequestHandler;
			ViewModel.FocusSearchRequest += FocusSearchRequestHandler;
           
            brdSearch.SetValue(VisibilityAnimation.AnimationTypeProperty, VisibilityAnimation.AnimationType.Fade);
            assemblyList.SetValue(VisibilityAnimation.AnimationTypeProperty, VisibilityAnimation.AnimationType.Fade);
		}

        private void UnloadedHandler(object sender, RoutedEventArgs e)
        {           
            brdSearch.SetValue(VisibilityAnimation.AnimationTypeProperty, VisibilityAnimation.AnimationType.None);
            assemblyList.SetValue(VisibilityAnimation.AnimationTypeProperty, VisibilityAnimation.AnimationType.None);
        }

		private void HideSearchAnimationCompletedHandler(object sender, EventArgs e)
		{
			brdSearch.Visibility = Visibility.Collapsed;
		}

		private void FocusSearchRequestHandler()
		{
			txtSearch.Focus();
		}

		private void ShowDetailsRequestHandler()
		{
			detailsPopup.MaxHeight = ActualHeight - 38;
			detailsPopup.IsOpen = true;
		}

		private void HideDetailsRequestHandler()
		{
			detailsPopup.IsOpen = false;
		}

		private void FillGraphRequestHandler()
		{
			zoomControl.ZoomToFill();
		}

		private void OriginalSizeRequestHandler()
		{
			var animation = new DoubleAnimation(1, TimeSpan.FromSeconds(1));
			zoomControl.BeginAnimation(ZoomControl.ZoomProperty, animation);
		}

		private void SearchPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				ViewModel.HideSearchCommand.Execute(null);
			}
		}

		private void LayoutFinishedHandler()
		{
			OnLayoutFinished();
		}

		private void OnLayoutFinished()
		{
			var handler = LayoutFinished;
			if (handler != null)
			{
				handler();
			}
		}
	}
}
