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

			RootTypes = types.Where(t => t.BaseType == null);
		}

		public string Title
		{
			get { return _assemblyDefinition.Name.Name; }
		}

		public IEnumerable<TypeViewModel> RootTypes { get; private set; }
	}
}
