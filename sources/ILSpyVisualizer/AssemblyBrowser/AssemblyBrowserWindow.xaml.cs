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
using Mono.Cecil;
using ICSharpCode.ILSpy;


namespace ILSpyVisualizer.AssemblyBrowser
{
	/// <summary>
	/// Interaction logic for AssemblyBrowserWindow.xaml
	/// </summary>
	internal partial class AssemblyBrowserWindow : Window
	{
		public AssemblyBrowserWindow(AssemblyDefinition assemblyDefinition)
		{
			InitializeComponent();

			ViewModel = new AssemblyBrowserWindowViewModel(assemblyDefinition, Dispatcher);
		}

		internal AssemblyBrowserWindowViewModel ViewModel
		{
			get { return DataContext as AssemblyBrowserWindowViewModel; }
			set { DataContext = value; }
		}

		private void TypeMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var frameworkElement = sender as FrameworkElement;
			var typeViewModel = frameworkElement.DataContext as TypeViewModel;
			MainWindow.Instance.JumpToReference(typeViewModel.TypeDefinition);
			e.Handled = true;
		}
	}
}
