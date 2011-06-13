// Copyright 2011 Denis Markelov
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ILSpyVisualizer.AssemblyBrowser.ViewModels;
using ILSpyVisualizer.Infrastructure;

namespace ILSpyVisualizer.AssemblyBrowser.Screens
{
	class Screen : ViewModelBase
	{
		public Screen(AssemblyBrowserWindowViewModel windowViewModel)
		{
			WindowViewModel = windowViewModel;

			Commands = new ObservableCollection<UserCommand>();
		}

		public virtual bool AllowAssemblyDrop
		{
			get { return true; }
		}

		public IEnumerable<AssemblyViewModel> Assemblies
		{
			get { return WindowViewModel.Assemblies; }
		}

		public ObservableCollection<UserCommand> Commands { get; private set; }

		protected AssemblyBrowserWindowViewModel WindowViewModel { get; private set; }

		public virtual void NotifyAssembliesChanged()
		{
		}

		public virtual void ShowInnerSearch()
		{
		}
	}
}
