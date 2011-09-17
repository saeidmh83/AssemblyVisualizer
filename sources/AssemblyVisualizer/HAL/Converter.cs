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
    class Converter
    {
        private static IConverter _converter;

        static Converter()
        {
            #if ILSpy
            _converter = new ILSpy.Converter();
            #endif
            #if Reflector
            _converter = new Reflector.Converter();
            #endif
        }

        public static AssemblyInfo Assembly(object assembly)
        {
            return _converter.Assembly(assembly);
        }

        public static TypeInfo Type(object type)
        {
            return _converter.Type(type);
        }

        public static void ClearCache()
        {
            _converter.ClearCache();
        }
    }
}
