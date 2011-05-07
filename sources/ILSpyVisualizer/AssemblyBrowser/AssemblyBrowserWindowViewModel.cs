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

			Types = _assemblyDefinition.MainModule.Types.Select(t => new TypeViewModel(t));
		}

		public string Title
		{
			get { return _assemblyDefinition.Name.Name; }
		}

		public IEnumerable<TypeViewModel> Types { get; private set; }
	}
}
