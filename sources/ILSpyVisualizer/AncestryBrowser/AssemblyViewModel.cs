// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Windows.Media;

namespace ILSpyVisualizer.AncestryBrowser
{
    class AssemblyViewModel
    {
        private AssemblyDefinition _assemblyDefinition;
        private IEnumerable<TypeViewModel> _types;

        public AssemblyViewModel(AssemblyDefinition assemblyDefinition, IEnumerable<TypeViewModel> types)
        {
            _assemblyDefinition = assemblyDefinition;
            _types = types;

            foreach (var type in _types)
            {
                type.Assembly = this;
            }
        }

        public string Name { get { return _assemblyDefinition.Name.Name; } }
        public IEnumerable<TypeViewModel> Types { get { return _types; } }
        public Brush BackgroundBrush { get; set; }
        public Brush CaptionBrush { get; set; }
    }
}
