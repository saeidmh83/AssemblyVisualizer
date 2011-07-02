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
using ILSpyVisualizer.AssemblyBrowser.ViewModels;
using Mono.Cecil;
using ICSharpCode.ILSpy;


namespace ILSpyVisualizer.AssemblyBrowser
{
	/// <summary>
	/// Interaction logic for AssemblyBrowserWindow.xaml
	/// </summary>
	internal partial class AssemblyBrowserWindow : Window
	{
		public AssemblyBrowserWindow(IEnumerable<AssemblyDefinition> assemblyDefinitions)
		{
			InitializeComponent();

			ViewModel = new AssemblyBrowserWindowViewModel(assemblyDefinitions, Dispatcher);

			WindowManager.AddAssemblyBrowser(this);

			CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseForward,
			                                       (s, e) => ViewModel.NavigateForwardCommand.Execute(null)));
			CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack,
												   (s, e) => ViewModel.NavigateBackCommand.Execute(null)));
		}

		internal AssemblyBrowserWindowViewModel ViewModel
		{
			get { return DataContext as AssemblyBrowserWindowViewModel; }
			set { DataContext = value; }
		}

		private void WindowDrop(object sender, DragEventArgs e)
		{
			var assemblyFilePaths = e.Data.GetData("ILSpyAssemblies") as string[];
			foreach (var assemblyFilePath in assemblyFilePaths)
			{
				var loadedAssembly =
					MainWindow.Instance.CurrentAssemblyList.OpenAssembly(assemblyFilePath);
				
				ViewModel.AddAssembly(loadedAssembly.AssemblyDefinition);
			}
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
