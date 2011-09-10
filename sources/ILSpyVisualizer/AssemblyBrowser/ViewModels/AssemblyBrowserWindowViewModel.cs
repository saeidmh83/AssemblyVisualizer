// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using System.Windows.Threading;
using System.Windows.Input;
using ILSpyVisualizer.AssemblyBrowser.Screens;
using ILSpyVisualizer.Common;
using ILSpyVisualizer.Properties;
using ILSpyVisualizer.Model;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	class AssemblyBrowserWindowViewModel : ViewModelBase
	{
		#region // Nested types

		class NavigationItem
		{
			public NavigationItem(Screen screen)
			{
				Screen = screen;
			}

			public NavigationItem(TypeViewModel type)
			{
				Type = type;
			}

			public Screen Screen { get; private set; }
			public TypeViewModel Type { get; private set; }

			public bool IsScreen
			{
				get { return Screen != null; }
			}

			public string Hint
			{
				get
				{
					if (IsScreen)
					{
                        return Resources.Search;
					}
					return Type.FullName;
				}
			}
		}

		#endregion

		#region // Private fields

		private readonly Dispatcher _dispatcher;
		private readonly ObservableCollection<AssemblyViewModel> _assemblies;
		private IEnumerable<TypeInfo> _allTypeInfo;
		private IEnumerable<TypeViewModel> _types;
		private Screen _screen;
		private readonly SearchScreen _searchScreen;
		private bool _isColorized;

		private readonly Stack<NavigationItem> _previousNavigationItems = new Stack<NavigationItem>();
		private readonly Stack<NavigationItem> _nextNavigationItems = new Stack<NavigationItem>();

		#endregion

		#region // .ctor

		public AssemblyBrowserWindowViewModel(IEnumerable<AssemblyInfo> assemblyDefinitions,
											  Dispatcher dispatcher)
		{
			_dispatcher = dispatcher;

			_assemblies = new ObservableCollection<AssemblyViewModel>(
								assemblyDefinitions.Select(a => new AssemblyViewModel(a, this)));

			_searchScreen = new SearchScreen(this);
			Screen = _searchScreen;

			OnAssembliesChanged();

			NavigateBackCommand = new DelegateCommand(NavigateBackCommandHandler);
			NavigateForwardCommand = new DelegateCommand(NavigateForwardCommandHandler);
			ShowInnerSearchCommand = new DelegateCommand(ShowInnerSearchCommandHandler);

			RefreshNavigationCommands();			

			IsColorized = true;
		}

		#endregion

		#region // Public properties

		public ICommand NavigateBackCommand { get; private set; }
		public ICommand NavigateForwardCommand { get; private set; }
		public ICommand ShowInnerSearchCommand { get; private set; }

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

		public SearchScreen SearchScreen
		{
			get { return _searchScreen; }
		}

		public IEnumerable<TypeInfo> AllTypeInfo
		{
			get
			{
				if (_allTypeInfo == null)
				{
					_allTypeInfo = _assemblies
						.Select(a => a.AssemblyInfo)
						.SelectMany(a => a.Modules)
						.SelectMany(m => m.Types)
						.ToList();
				}
				return _allTypeInfo;
			}
		}

		public bool IsColorized
		{	
			get { return _isColorized; }
			set
			{
				_isColorized = value;
				RefreshColorization();
			}
		}

		public string Title
		{
            get { return Resources.AssemblyBrowser; }
		}

		public bool ShowNavigationArrows
		{
			get { return true; }
		}

		public bool CanNavigateBack
		{
			get { return _previousNavigationItems.Count > 0; }
		}

		public bool CanNavigateForward
		{
			get { return _nextNavigationItems.Count > 0; }
		}

		public string NavigateForwardHint
		{
			get
			{
				if (!CanNavigateForward)
				{
					return Resources.CannotNavigateForward;
				}
				return _nextNavigationItems.Peek().Hint;
			}
		}

		public string NavigateBackHint
		{
			get
			{
				if (!CanNavigateBack)
				{
                    return Resources.CannotNavigateBack;
				}
				return _previousNavigationItems.Peek().Hint;
			}
		}

		public IEnumerable<TypeViewModel> Types
		{
			get
			{
				if (_types == null)
				{
					var types = new List<TypeViewModel>();

					foreach (var assembly in _assemblies)
					{
						var currentAssembly = assembly;

						var assemblyTypes = currentAssembly.AssemblyInfo.Modules
							.SelectMany(m => m.Types)
							.Select(t => new TypeViewModel(t, this))
							.ToList();
						
						currentAssembly.Types = assemblyTypes;
						types.AddRange(assemblyTypes);
					}

					_types = types;
				}
				return _types;
			}
		}

		public Dispatcher Dispatcher
		{
			get { return _dispatcher; }
		}

		public ObservableCollection<AssemblyViewModel> Assemblies
		{
			get { return _assemblies; }
		}

		public UserCommand ShowSearchUserCommand
		{
			get { return new UserCommand(Resources.BackToSearch, new DelegateCommand(ShowSearch)); }
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

		private NavigationItem CurrentNavigationItem
		{
			get
			{
				var graphScreen = Screen as GraphScreen;
				if (graphScreen != null)
				{
					return new NavigationItem(graphScreen.Type);
				}
				return new NavigationItem(Screen);
			}
			set
			{
				var graphScreen = Screen as GraphScreen;
				if (graphScreen != null && value.IsScreen)
				{
					graphScreen.ClearSearch();
                    graphScreen.Type.IsCurrent = false;
				}

				if (value.IsScreen)
				{                    
					Screen = value.Screen;
				}
				else
				{
					if (graphScreen == null)
					{
						graphScreen = new GraphScreen(this);
						Screen = graphScreen;
					}
					graphScreen.Show(value.Type);
				}
			}
		}

		#endregion

		#region // Public methods

		public void ShowSearch()
		{
			if (Screen == _searchScreen)
			{
				_searchScreen.FocusSearchField();
				return;
			}

			Navigate(new NavigationItem(_searchScreen));
		}

		public void ShowGraph(TypeViewModel type)
		{
			Navigate(new NavigationItem(type));
		}

		public void AddAssemblies(IEnumerable<AssemblyInfo> assemblies)
		{
			var newAssemblies = assemblies.Except(_assemblies.Select(a => a.AssemblyInfo));
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

		public void AddAssembly(AssemblyInfo assemblyDefinition)
		{
			if (_assemblies.Any(vm => vm.AssemblyInfo == assemblyDefinition))
			{
				return;
			}

			_assemblies.Add(new AssemblyViewModel(assemblyDefinition, this));

			OnAssembliesChanged();
		}

		public void RemoveAssembly(AssemblyInfo assemblyDefinition)
		{
			var assemblyViewModel = _assemblies
				.FirstOrDefault(vm => vm.AssemblyInfo == assemblyDefinition);
			if (assemblyViewModel != null)
			{
				_assemblies.Remove(assemblyViewModel);
				OnAssembliesChanged();
			}
		}

		#endregion

		#region // Private methods

		private void OnAssembliesChanged()
		{
			UpdateInternalTypeCollections();
			_searchScreen.NotifyAssembliesChanged();
			RefreshColorization();
			CleanUpNavigationHistory();
		}

		private void UpdateInternalTypeCollections()
		{
			_allTypeInfo = null;
			_types = null;

			var typesDictionary = Types.ToDictionary(type => type.TypeInfo);

			foreach (var typeDefinition in AllTypeInfo
				.Where(t => t.BaseType != null))
			{
				var baseType = typeDefinition.BaseType;
				if (baseType != null && typesDictionary.ContainsKey(baseType))
				{
					typesDictionary[baseType].AddDerivedType(
						typesDictionary[typeDefinition]);
				}
			}

			foreach (var type in Types)
			{
				type.CountDescendants();
			}
		}

		private void Navigate(NavigationItem item)
		{
			if (Screen != null)
			{
				_previousNavigationItems.Push(CurrentNavigationItem);
			}

			CurrentNavigationItem = item;
			_nextNavigationItems.Clear();

			RefreshNavigationCommands();
		}

		private void RefreshNavigationCommands()
		{
			OnPropertyChanged("CanNavigateBack");
			OnPropertyChanged("CanNavigateForward");
			OnPropertyChanged("NavigateBackHint");
			OnPropertyChanged("NavigateForwardHint");
		}

		private void RefreshColorization()
		{
			if (IsColorized)
			{
				int currentIndex = 0;
				foreach (var assembly in Assemblies)
				{
					assembly.Colorize(
                        BrushProvider.BrushPairs[currentIndex].Caption, BrushProvider.BrushPairs[currentIndex].Background);
					currentIndex++;
                    if (currentIndex == BrushProvider.BrushPairs.Count)
					{
						currentIndex = 0;
					}
				}
			}
			else
			{
				foreach (var assembly in Assemblies)
				{
					assembly.Decolorize();
				}
			}
		}

		private void CleanUpNavigationHistory()
		{
			CleanUpNavigationStack(_previousNavigationItems);
			CleanUpNavigationStack(_nextNavigationItems);

			RefreshNavigationCommands();
		}

		private void CleanUpNavigationStack(Stack<NavigationItem> navigationStack)
		{
			var stack = new Stack<NavigationItem>();

			while (navigationStack.Count > 0)
			{
				stack.Push(navigationStack.Pop());
			}
			while (stack.Count > 0)
			{
				var item = stack.Pop();
				if (item.IsScreen)
				{
					navigationStack.Push(item);
					continue;
				}
				var sameType = Types.SingleOrDefault(t => t.TypeInfo == item.Type.TypeInfo);
				if (sameType != null)
				{
					navigationStack.Push(new NavigationItem(sameType));
				}
			}
		}

		#endregion

		#region // Command handlers

		private void NavigateBackCommandHandler()
		{
			if (_previousNavigationItems.Count == 0)
			{
				return;
			}
			_nextNavigationItems.Push(CurrentNavigationItem);
			CurrentNavigationItem = _previousNavigationItems.Pop();

			RefreshNavigationCommands();
		}

		private void NavigateForwardCommandHandler()
		{
			if (_nextNavigationItems.Count == 0)
			{
				return;
			}
			
			_previousNavigationItems.Push(CurrentNavigationItem);
			CurrentNavigationItem = _nextNavigationItems.Pop();

			RefreshNavigationCommands();
		}

		private void ShowInnerSearchCommandHandler()
		{
			if (Screen != null)
			{
				Screen.ShowInnerSearch();
			}
		}

		#endregion
	}
}
