// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Infrastructure;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.DependencyBrowser
{
    class AssemblyViewModel : ViewModelBase
    {
        private static Dictionary<AssemblyInfo, AssemblyViewModel> _correspondence = new Dictionary<AssemblyInfo, AssemblyViewModel>();

        private AssemblyInfo _assembly;
        private IList<AssemblyViewModel> _referencedAssemblies = new List<AssemblyViewModel>();
        
        private AssemblyViewModel(AssemblyInfo assembly)
        {
            _assembly = assembly;
            _correspondence.Add(assembly, this);
            foreach (var assemblyInfo in _assembly.ReferencedAssemblies)
            {
                _referencedAssemblies.Add(Create(assemblyInfo));
            }
        }

        public bool IsProcessed { get; set; }
        public bool IsMarked { get; set; }
        public string Name 
        {
            get
            {
                return _assembly.Name;
            }
        }

        public string FullName
        {
            get
            {
                return _assembly.FullName;
            }
        }

        public IEnumerable<AssemblyViewModel> ReferencedAssemblies
        {
            get
            {
                return _referencedAssemblies;
            }
        }

        public static AssemblyViewModel Create(AssemblyInfo assemblyInfo)
        {
            if (_correspondence.ContainsKey(assemblyInfo))
            {
                return _correspondence[assemblyInfo];
            }
            return new AssemblyViewModel(assemblyInfo); 
        }

        public static void ClearCache()
        {
            _correspondence.Clear();
        }
    }
}
