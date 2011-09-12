// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyVisualizer.Model
{
    class AssemblyInfo
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public IEnumerable<ModuleInfo> Modules { get; set; }
        public IEnumerable<AssemblyInfo> ReferencedAssemblies { get; set; }
        public int ExportedTypesCount { get; set; }
        public int InternalTypesCount { get; set; }
    }
}
