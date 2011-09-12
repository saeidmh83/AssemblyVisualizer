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
using ILSpyVisualizer.Model;
using System.Windows.Media.Animation;
using ILSpyVisualizer.Controls.ZoomControl;

namespace ILSpyVisualizer.DependencyBrowser
{
    /// <summary>
    /// Interaction logic for DependencyBrowserWindow.xaml
    /// </summary>
    partial class DependencyBrowserWindow : Window
    {
        public DependencyBrowserWindow(IEnumerable<AssemblyInfo> assemblies)
        {
            InitializeComponent();

            ViewModel = new DependencyBrowserWindowViewModel(assemblies);

            ViewModel.FillGraphRequest += FillGraphRequestHandler;
            ViewModel.OriginalSizeRequest += OriginalSizeRequestHandler;
        }

        public DependencyBrowserWindowViewModel ViewModel
        {
            get
            {
                return DataContext as DependencyBrowserWindowViewModel;
            }
            set
            {
                DataContext = value;
            }
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
    }
}
