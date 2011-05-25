using System.Windows.Input;
using System.Linq;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	class AssemblyViewModel : ViewModelBase
	{
		private readonly AssemblyDefinition _assemblyDefinition;
		private readonly AssemblyBrowserWindowViewModel _windowViewModel;
		private bool _showRemoveCommand = true;
		private int _exportedTypesCount;
		private int _internalTypesCount;
		
		public AssemblyViewModel(AssemblyDefinition assemblyDefinition, 
								 AssemblyBrowserWindowViewModel windowViewModel)
		{
			_assemblyDefinition = assemblyDefinition;
			_windowViewModel = windowViewModel;

			var types = _assemblyDefinition.Modules
				.SelectMany(m => m.Types);
			_exportedTypesCount = types.Count(t => t.IsPublic);
			_internalTypesCount = types.Count(t => t.IsNotPublic);

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

		public int ExportedTypesCount
		{
			get { return _exportedTypesCount; }
		}

		public int InternalTypesCount
		{
			get { return _internalTypesCount; }
		}

		private void RemoveCommandHandler()
		{
			_windowViewModel.RemoveAssembly(_assemblyDefinition);
		}
	}
}
