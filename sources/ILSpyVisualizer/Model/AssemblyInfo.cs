using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILSpyVisualizer.Model
{
    class AssemblyInfo
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public IEnumerable<ModuleInfo> Modules { get; set; }
        public int ExportedTypesCount { get; set; }
        public int InternalTypesCount { get; set; }
    }
}
