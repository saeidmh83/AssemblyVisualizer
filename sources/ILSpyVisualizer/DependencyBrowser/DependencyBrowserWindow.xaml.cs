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
    }
}
