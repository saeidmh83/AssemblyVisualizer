using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;

namespace ILSpyVisualizer.AssemblyBrowser
{
	class AssemblyViewModel : ViewModelBase
	{
		private readonly AssemblyDefinition _assemblyDefinition;
		private readonly AssemblyBrowserWindowViewModel _windowViewModel;
		
		public AssemblyViewModel(AssemblyDefinition assemblyDefinition, 
								 AssemblyBrowserWindowViewModel windowViewModel)
		{
			_assemblyDefinition = assemblyDefinition;
			_windowViewModel = windowViewModel;

			RemoveCommand = new DelegateCommand(RemoveCommandHandler);
		}

		public string Name
		{
			get { return _assemblyDefinition.Name.Name; }
		}

		public ICommand RemoveCommand { get; private set; }

		public AssemblyDefinition AssemblyDefinition
		{
			get { return _assemblyDefinition; }
		}

		private void RemoveCommandHandler()
		{
			_windowViewModel.RemoveAssemblyDefinition(_assemblyDefinition);
		}
	}
}
