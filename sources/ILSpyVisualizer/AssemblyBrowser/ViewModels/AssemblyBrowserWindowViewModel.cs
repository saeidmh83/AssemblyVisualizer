using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using System.Windows.Threading;
using QuickGraph;
using System.Windows.Input;
using ILSpyVisualizer.AssemblyBrowser.Screens;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	class AssemblyBrowserWindowViewModel : ViewModelBase
	{
		#region // Private fields

		private readonly Dispatcher _dispatcher;
		private readonly ObservableCollection<AssemblyViewModel> _assemblies;
		private IEnumerable<TypeDefinition> _allTypeDefinitions;
		private IEnumerable<TypeViewModel> _types;
		private Screen _screen;
		private readonly SearchScreen _searchScreen;

		#endregion

		#region // .ctor

		public AssemblyBrowserWindowViewModel(IEnumerable<AssemblyDefinition> assemblyDefinitions,
											  Dispatcher dispatcher)
		{
			_dispatcher = dispatcher;

			_assemblies = new ObservableCollection<AssemblyViewModel>(
								assemblyDefinitions.Select(a => new AssemblyViewModel(a, this)));

			_searchScreen = new SearchScreen(this);
			Screen = _searchScreen;
			
			OnAssembliesChanged();
		}

		#endregion

		#region // Public properties

		public Screen Screen
		{
			get { return _screen; }
			set
			{
				_screen = value;
				AreAssembliesEditable = value.AllowAssemblyDrop;
				OnPropertyChanged("Screen");
			}
		}

		public IEnumerable<TypeDefinition> AllTypeDefinitions
		{
			get
			{
				if (_allTypeDefinitions == null)
				{
					_allTypeDefinitions = _assemblies
						.Select(a => a.AssemblyDefinition)
						.SelectMany(a => a.Modules)
						.SelectMany(m => m.Types)
						.ToList();
				}
				return _allTypeDefinitions;
			}
		}

		public string Title
		{
			get { return "Assembly Browser"; }
		}

		public IEnumerable<TypeViewModel> Types
		{
			get { return _types; }
		}

		public Dispatcher Dispatcher
		{
			get { return _dispatcher; }
		}

		public ObservableCollection<AssemblyViewModel> Assemblies
		{
			get { return _assemblies; }
		}

		#endregion

		#region // Private properties

		private bool AreAssembliesEditable
		{	
			set
			{
				foreach (var assembly in Assemblies)
				{
					assembly.ShowRemoveCommand = value;
				}
			}
		}

		#endregion

		#region // Public methods

		public void ShowSearch()
		{
			if (Screen != _searchScreen)
			{
				Screen = _searchScreen;
			}
			else
			{
				_searchScreen.FocusSearchField();
			}
		}

		public void AddAssemblies(IEnumerable<AssemblyDefinition> assemblies)
		{
			var newAssemblies = assemblies.Except(_assemblies.Select(a => a.AssemblyDefinition));
			if (newAssemblies.Count() == 0)
			{
				return;
			}

			foreach (var assembly in newAssemblies)
			{
				_assemblies.Add(new AssemblyViewModel(assembly, this));
			}
			
			OnAssembliesChanged();
		}

		public void AddAssembly(AssemblyDefinition assemblyDefinition)
		{
			if (_assemblies.Any(vm => vm.AssemblyDefinition == assemblyDefinition))
			{
				return;
			}

			_assemblies.Add(new AssemblyViewModel(assemblyDefinition, this));

			OnAssembliesChanged();
		}

		public void RemoveAssembly(AssemblyDefinition assemblyDefinition)
		{
			var assemblyViewModel = _assemblies
				.FirstOrDefault(vm => vm.AssemblyDefinition == assemblyDefinition);
			if (assemblyViewModel != null)
			{
				_assemblies.Remove(assemblyViewModel);
				OnAssembliesChanged();
			}
		}

		public void ShowGraph(TypeViewModel type)
		{
			if (!(Screen is GraphScreen))
			{
				Screen = new GraphScreen(this);
			}
			var graphScreen = Screen as GraphScreen;
			graphScreen.Show(type);

		}

		#endregion

		#region // Private methods

		private void OnAssembliesChanged()
		{
			UpdateInternalTypeCollections();
			Screen.NotifyAssembliesChanged();
		}

		private void UpdateInternalTypeCollections()
		{
			_allTypeDefinitions = null;
			_types = AllTypeDefinitions
				.OrderBy(t => t.Name)
				.Select(t => new TypeViewModel(t, this))
				.ToList();
			var typesDictionary = _types.ToDictionary(type => type.TypeDefinition);

			foreach (var typeDefinition in AllTypeDefinitions
				.Where(t => t.BaseType != null))
			{
				var baseType = typeDefinition.BaseType.Resolve();
				if (typesDictionary.ContainsKey(baseType))
				{
					typesDictionary[baseType].AddDerivedType(
						typesDictionary[typeDefinition]);
				}
			}
		}

		#endregion
	}
}
