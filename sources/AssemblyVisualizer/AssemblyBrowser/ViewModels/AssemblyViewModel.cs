// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using AssemblyVisualizer.Infrastructure;

using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.AssemblyBrowser.ViewModels
{
	class AssemblyViewModel : ViewModelBase
	{
		private static readonly Brush DefaultForeground = new SolidColorBrush(Color.FromRgb(85, 85, 85));

		private readonly AssemblyInfo _assemblyInfo;
		private readonly AssemblyBrowserWindowViewModel _windowViewModel;
		private readonly int _exportedTypesCount;
		private readonly int _internalTypesCount;
		private bool _showRemoveCommand = true;
		private Brush _foreground;
		
		public AssemblyViewModel(AssemblyInfo assemblyInfo, 
								 AssemblyBrowserWindowViewModel windowViewModel)
		{
			_assemblyInfo = assemblyInfo;
			_windowViewModel = windowViewModel;

			var types = _assemblyInfo.Modules
				.SelectMany(m => m.Types);
			_exportedTypesCount = assemblyInfo.ExportedTypesCount;
            _internalTypesCount = assemblyInfo.InternalTypesCount;

			RemoveCommand = new DelegateCommand(RemoveCommandHandler);

			RefreshForeground();
		}

		public string Name
		{
			get { return _assemblyInfo.Name; }
		}

		public string FullName
		{
			get { return _assemblyInfo.FullName; }
		}

        public string Version
        {
            get { return _assemblyInfo.Version.ToString(); }
        }

		public Brush Foreground
		{
			get { return _foreground; }
			set
			{
				_foreground = value;
				OnPropertyChanged("Foreground");
			}
		}

		public Brush AssignedForeground { get; set; }

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

		public IEnumerable<TypeViewModel> Types { get; set; }

		public AssemblyInfo AssemblyInfo
		{
			get { return _assemblyInfo; }
		}

		public int ExportedTypesCount
		{
			get { return _exportedTypesCount; }
		}

		public int InternalTypesCount
		{
			get { return _internalTypesCount; }
		}

		public void Colorize(Brush caption, Brush background)
		{
			AssignedForeground = caption;
			RefreshForeground();

			foreach (var type in Types)
			{
				type.AssignedBackground = background;
				type.RefreshBackground();
			}
		}
		
		public void Decolorize()
		{
			AssignedForeground = null;
			RefreshForeground();

			foreach (var type in Types)
			{
				type.AssignedBackground = null;
				type.RefreshBackground();
			}
		}

		public void RefreshForeground()
		{
			var brush = DefaultForeground;
			if (AssignedForeground != null)
			{
				brush = AssignedForeground;
			}
			
			Foreground = brush;
		}

		private void RemoveCommandHandler()
		{
			_windowViewModel.RemoveAssembly(_assemblyInfo);
		}
	}
}
