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
using AssemblyVisualizer.AssemblyBrowser.ViewModels;
using AssemblyVisualizer.Model;
using AssemblyVisualizer.HAL;


namespace AssemblyVisualizer.AssemblyBrowser
{
	/// <summary>
	/// Interaction logic for AssemblyBrowserWindow.xaml
	/// </summary>
	partial class AssemblyBrowserWindow : Window
	{
		public AssemblyBrowserWindow(IEnumerable<AssemblyInfo> assemblyDefinitions)
		{
			InitializeComponent();

			ViewModel = new AssemblyBrowserWindowViewModel(assemblyDefinitions, Dispatcher);			

			CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseForward,
			                                       (s, e) => ViewModel.NavigateForwardCommand.Execute(null)));
			CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack,
												   (s, e) => ViewModel.NavigateBackCommand.Execute(null)));

            WindowManager.AddAssemblyBrowser(this);
		}

		public AssemblyBrowserWindowViewModel ViewModel
		{
			get { return DataContext as AssemblyBrowserWindowViewModel; }
			set { DataContext = value; }
		}

		private void WindowDrop(object sender, DragEventArgs e)
		{
            #if ILSpy

			var assemblyFilePaths = e.Data.GetData("ILSpyAssemblies") as string[];
			foreach (var assemblyFilePath in assemblyFilePaths)
			{
				var loadedAssembly =
					Services.MainWindow.CurrentAssemblyList.OpenAssembly(assemblyFilePath);
				
				ViewModel.AddAssembly(Converter.Assembly(loadedAssembly.AssemblyDefinition));
			}

            #endif
		}

		private void SearchExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ShowSearch();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			WindowManager.RemoveAssemblyBrowser(this);
		}
	}
}
