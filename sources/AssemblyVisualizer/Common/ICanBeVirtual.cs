using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyVisualizer.Common
{
    interface ICanBeVirtual
    {
        bool IsVirtual { get; }
        bool IsOverride { get; }
    }
}
