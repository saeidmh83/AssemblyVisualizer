using System.Windows.Input;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	class AssemblyViewModel : ViewModelBase
	{
		private readonly AssemblyDefinition _assemblyDefinition;
		private readonly AssemblyBrowserWindowViewModel _windowViewModel;
		private bool _showRemoveCommand = true;
		
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

		public bool ShowRemoveCommand
		{
			get { return _showRemoveCommand; }
			set
			{
				_showRemoveCommand = value;
				OnPropertyChanged("ShowRemoveCommand");
			}
		}

		public AssemblyDefinition AssemblyDefinition
		{
			get { return _assemblyDefinition; }
		}

		private void RemoveCommandHandler()
		{
			_windowViewModel.RemoveAssembly(_assemblyDefinition);
		}
	}
}
