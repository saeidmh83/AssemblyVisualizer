// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.HAL
{
    interface IConverter
    {
        AssemblyInfo Assembly(object assembly);
        TypeInfo Type(object type);
        MethodInfo Method(object method);
        FieldInfo Field(object field);
        PropertyInfo Property(object property);
        EventInfo Event(object ev);
        void ClearCache();
    }
}
