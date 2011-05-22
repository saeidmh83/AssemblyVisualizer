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

	}
}
