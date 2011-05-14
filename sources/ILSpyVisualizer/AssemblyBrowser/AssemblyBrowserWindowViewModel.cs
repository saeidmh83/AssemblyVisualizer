using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;

namespace ILSpyVisualizer.AssemblyBrowser
{
	class AssemblyBrowserWindowViewModel : ViewModelBase
	{
		private readonly AssemblyDefinition _assemblyDefinition;
		private bool _showTypeHierarchies = true;

		public AssemblyBrowserWindowViewModel(AssemblyDefinition assemblyDefinition)
		{
			_assemblyDefinition = assemblyDefinition;

			var typeViewModelsDictionary = new Dictionary<TypeDefinition, TypeViewModel>();
			
			var types = _assemblyDefinition.MainModule.Types
				.Select(t =>
				        	{
				        		var viewModel = new TypeViewModel(t);
								typeViewModelsDictionary.Add(t, viewModel);
				        		return viewModel;
				        	}).ToList();

			foreach (var typeDefinition in _assemblyDefinition.MainModule.Types
				.Where(t => t.BaseType != null))
			{
				var baseType = typeDefinition.BaseType.Resolve();
				if (typeViewModelsDictionary.ContainsKey(baseType))
				{
					typeViewModelsDictionary[baseType].AddDerivedType(
						typeViewModelsDictionary[typeDefinition]);
				}
			}

			var specialTypes = new[]
			                   	{
									"System.Object", "System.ValueType", "System.Enum", "System.Attribute"
			                   	};

			var baseTypes = types.Where(t => (t.BaseType == null 
										|| specialTypes.Contains(t.BaseType.FullName))
										&& !specialTypes.Contains(t.FullName));

			HierarchyRootTypes = baseTypes.Where(t => t.DerivedTypes.Count() > 0)
				.OrderBy(t => t.Name);
			SingleTypes = baseTypes.Where(t => t.DerivedTypes.Count() == 0)
				.OrderBy(t => t.Name);
		}

		public string Title
		{
			get { return _assemblyDefinition.Name.Name; }
		}

		public IEnumerable<TypeViewModel> HierarchyRootTypes { get; private set; }

		public IEnumerable<TypeViewModel> SingleTypes { get; private set; }

		public bool ShowTypeHierarchies
		{
			get { return _showTypeHierarchies; }
			set
			{
				_showTypeHierarchies = value;
				OnPropertyChanged("ShowTypeHierarchies");
				OnPropertyChanged("ShowSingleTypes");
			}
		}

		public bool ShowSingleTypes
		{
			get { return !_showTypeHierarchies; }
			set
			{
				_showTypeHierarchies = !value;
				OnPropertyChanged("ShowTypeHierarchies");
				OnPropertyChanged("ShowSingleTypes");
			}
		}
	}
}
