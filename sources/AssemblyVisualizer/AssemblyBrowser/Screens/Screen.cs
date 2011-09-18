// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AssemblyVisualizer.AssemblyBrowser.ViewModels;
using AssemblyVisualizer.Infrastructure;

namespace AssemblyVisualizer.AssemblyBrowser.Screens
{
	class Screen : ViewModelBase
	{
		public Screen(AssemblyBrowserWindowViewModel windowViewModel)
		{
			WindowViewModel = windowViewModel;
		}

		public virtual bool AllowAssemblyDrop
		{
			get { return true; }
		}

		public IEnumerable<AssemblyViewModel> Assemblies
		{
			get { return WindowViewModel.Assemblies; }
		}

		protected AssemblyBrowserWindowViewModel WindowViewModel { get; private set; }

		public virtual void NotifyAssembliesChanged()
		{
		}

		public virtual void ShowInnerSearch()
		{
		}

        public virtual void ToggleAssembliesVisibility()
        { 
        }
	}
}
