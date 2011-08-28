// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using ILSpyVisualizer.Common;

namespace ILSpyVisualizer.AncestryBrowser
{
    class TypeViewModel : ViewModelBase
    {
        private TypeDefinition _typeDefinition;

        public TypeViewModel(TypeDefinition typeDefinition)
        {
            _typeDefinition = typeDefinition;
            if (_typeDefinition.BaseType != null)
            {
                BaseType = new TypeViewModel(_typeDefinition.BaseType.Resolve());
            }

            var fields = _typeDefinition.Fields                
                .Select(f => new FieldViewModel(f))
                .OfType<MemberViewModel>();

            var properties = _typeDefinition.Properties                
                .Select(p => new PropertyViewModel(p))
                .OfType<MemberViewModel>();

            var events = _typeDefinition.Events                
                .Select(e => new EventViewModel(e))
                .OfType<MemberViewModel>();

            var methods = _typeDefinition.Methods
                .Where(m => !m.IsGetter && !m.IsSetter && !m.IsAddOn && !m.IsRemoveOn)
                .Select(m => new MethodViewModel(m))
                .OfType<MemberViewModel>();

            Members = fields.Concat(properties).Concat(events).Concat(methods);
        }

        public IEnumerable<MemberViewModel> Members { get; set; }

        public TypeViewModel BaseType { get; set; }

        public string Name 
        {
            get
            {
                return _typeDefinition.Name;
            }
        }
    }
}
