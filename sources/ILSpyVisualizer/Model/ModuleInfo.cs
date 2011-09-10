using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILSpyVisualizer.Model
{
    class ModuleInfo
    {
        public AssemblyInfo Assembly { get; set; }
        public IEnumerable<TypeInfo> Types { get; set; }
    }
}
