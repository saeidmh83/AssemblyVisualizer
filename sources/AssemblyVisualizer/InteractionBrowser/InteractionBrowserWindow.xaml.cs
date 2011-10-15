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
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using AssemblyVisualizer.Controls.ZoomControl;
using AssemblyVisualizer.Model;
using AssemblyVisualizer.Behaviors;

namespace AssemblyVisualizer.InteractionBrowser
{   
    partial class InteractionBrowserWindow : Window
    {  
        public InteractionBrowserWindow(IEnumerable<TypeInfo> types, bool drawGraph)
        {
            InitializeComponent();            

            ViewModel = new InteractionBrowserWindowViewModel(types, drawGraph);
            ViewModel.FillGraphRequest += FillGraphRequestHandler;
            ViewModel.OriginalSizeRequest += OriginalSizeRequestHandler;
            ViewModel.FocusSearchRequest += FocusSearchRequestHandler;

            Loaded += new RoutedEventHandler(LoadedHandler);
            Unloaded += new RoutedEventHandler(UnloadedHandler);     

            WindowManager.AddInteractionBrowser(this); 
        }
        
        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            gridTypeSelector.SetValue(VisibilityAnimation.AnimationTypeProperty, VisibilityAnimation.AnimationType.Fade);
            typeList.SetValue(VisibilityAnimation.AnimationTypeProperty, VisibilityAnimation.AnimationType.Fade);
            brdSearch.SetValue(VisibilityAnimation.AnimationTypeProperty, VisibilityAnimation.AnimationType.Fade);
        }

        private void UnloadedHandler(object sender, RoutedEventArgs e)
        {
            gridTypeSelector.SetValue(VisibilityAnimation.AnimationTypeProperty, VisibilityAnimation.AnimationType.None);
            typeList.SetValue(VisibilityAnimation.AnimationTypeProperty, VisibilityAnimation.AnimationType.None);
            brdSearch.SetValue(VisibilityAnimation.AnimationTypeProperty, VisibilityAnimation.AnimationType.None);
        }

        private void SearchPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                ViewModel.HideSearchCommand.Execute(null);
            }
        }

        public InteractionBrowserWindowViewModel ViewModel
        {
            get
            {
                return DataContext as InteractionBrowserWindowViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            WindowManager.RemoveInteractionBrowser(this);
        }        
                
        private void FillGraphRequestHandler()
        {
            zoomControl.ZoomToFill();
        }

        private void OriginalSizeRequestHandler()
        {
            var animation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
            zoomControl.BeginAnimation(ZoomControl.ZoomProperty, animation);
        }

        private void TypeListMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewModel.IsTypeListVisible = false;
        }

        private void FocusSearchRequestHandler()
        {
            txtSearch.Focus();
        }

    }
}