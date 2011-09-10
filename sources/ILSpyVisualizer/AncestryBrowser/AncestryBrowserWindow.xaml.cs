// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using ILSpyVisualizer.Common;
using ILSpyVisualizer.Model;

namespace ILSpyVisualizer.AncestryBrowser
{	
	partial class AncestryBrowserWindow : Window
	{
		public AncestryBrowserWindow(TypeInfo typeInfo)
		{
			InitializeComponent();		
			
			ViewModel = new AncestryBrowserWindowViewModel(typeInfo);
		}
		
		public AncestryBrowserWindowViewModel ViewModel
		{
			get
			{
				return DataContext as AncestryBrowserWindowViewModel;			
			}
			set
			{
				DataContext = value;
			}
		}

        private void SearchExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            txtSearch.Focus();            
        }
	}
}