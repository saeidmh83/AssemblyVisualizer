using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILSpyVisualizer.Model
{
    class MethodInfo : MemberInfo
    {
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
    }
}
