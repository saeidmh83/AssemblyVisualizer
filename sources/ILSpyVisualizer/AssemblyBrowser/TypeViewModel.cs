using System.Collections.Generic;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using System.Collections.ObjectModel;
using System.Linq;
using ICSharpCode.ILSpy.TreeNodes;

namespace ILSpyVisualizer.AssemblyBrowser
{
	class TypeViewModel : ViewModelBase
	{
		private readonly TypeDefinition _typeDefinition;
		
		public TypeViewModel(TypeDefinition typeDefinition)
		{
			_typeDefinition = typeDefinition;

			var properties = typeDefinition.Properties
				.Where(p => p.GetMethod.IsPublic)
				.Select(p => new PropertyViewModel(p))
				.OfType<MemberViewModel>();

			var events = typeDefinition.Events
				.Where(e => e.AddMethod.IsPublic)
				.Select(e => new EventViewModel(e))
				.OfType<MemberViewModel>();

			var methods = typeDefinition.Methods
				.Where(m => m.IsPublic && !m.IsGetter && !m.IsSetter && !m.IsAddOn && !m.IsRemoveOn)
				.Select(m => new MethodViewModel(m))
				.OfType<MemberViewModel>();

			Members = properties.Concat(events).Concat(methods);
		}

		public string Name
		{
			get { return _typeDefinition.Name; }
		}

		public string FullName
		{
			get { return _typeDefinition.FullName; }
		}

		public IEnumerable<MemberViewModel> Members { get; set; }
	}
}
