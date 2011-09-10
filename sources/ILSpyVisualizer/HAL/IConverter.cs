using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.Model;

namespace ILSpyVisualizer.HAL
{
    interface IConverter
    {
        AssemblyInfo Assembly(object assembly);
        TypeInfo Type(object type);
    }
}
