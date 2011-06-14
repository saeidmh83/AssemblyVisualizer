// Copyright 2011 Denis Markelov
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	class AssemblyViewModel : ViewModelBase
	{
		private static readonly Brush DefaultForeground = new SolidColorBrush(Color.FromRgb(85, 85, 85));

		private readonly AssemblyDefinition _assemblyDefinition;
		private readonly AssemblyBrowserWindowViewModel _windowViewModel;
		private readonly int _exportedTypesCount;
		private readonly int _internalTypesCount;
		private bool _showRemoveCommand = true;
		private Brush _foreground;
		
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

			RefreshForeground();
		}

		public string Name
		{
			get { return _assemblyDefinition.Name.Name; }
		}

		public string FullName
		{
			get { return _assemblyDefinition.FullName; }
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
			_windowViewModel.RemoveAssembly(_assemblyDefinition);
		}
	}
}
