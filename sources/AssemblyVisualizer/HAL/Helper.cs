// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Model;
#if ILSpy
using AssemblyVisualizer.HAL.ILSpy;
#elif Reflector
using AssemblyVisualizer.HAL.Reflector;
#endif

namespace AssemblyVisualizer.HAL
{
    static class Helper
    {
        private static ModelHelper _helper = new ModelHelper();

        public static EventInfo GetEventForBackingField(object field)
        {
            return _helper.GetEventForBackingField(field);
        }

        public static PropertyInfo GetAccessorProperty(object method)
        {
            return _helper.GetAccessorProperty(method);
        }

        public static EventInfo GetAccessorEvent(object method)
        {
            return _helper.GetAccessorEvent(method);
        }

        public static IEnumerable<MethodInfo> GetUsedMethods(object method)
        {
            return _helper.GetUsedMethods(method);
        }

        public static IEnumerable<FieldInfo> GetUsedFields(object method)
        {
            return _helper.GetUsedFields(method);
        }

        public static TypeInfo GetDeclaringType(object member)
        {
            return _helper.LoadDeclaringType(member);  
        }
    }
}
