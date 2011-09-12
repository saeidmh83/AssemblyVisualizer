using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILSpyVisualizer.Common
{
    interface ICanBeVirtual
    {
        bool IsVirtual { get; }
        bool IsOverride { get; }
    }
}
