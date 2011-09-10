using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.Model;

namespace ILSpyVisualizer.HAL
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
    }
}
