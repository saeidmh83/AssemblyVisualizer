using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.Model;

#if ILSpy
using ICSharpCode.ILSpy;
using Mono.Cecil;
using System.Windows;
#endif

namespace ILSpyVisualizer.HAL
{
    class Services
    {
        public static void JumpTo(object memberReference)
        {
            #if ILSpy
            MainWindow.JumpToReference(memberReference);
            #endif
        }

        public static bool MethodsMatch(MethodInfo method1, MethodInfo method2)
        { 
            #if ILSpy
            var md1 = method1.MemberReference as MethodDefinition;
            var md2 = method2.MemberReference as MethodDefinition;

            return md1.Name == md2.Name && ParametersMatch(md1, md2);
            #else

            return method1.Text == method2.Text;

            #endif
        }

        #if ILSpy

        public static MainWindow MainWindow
        {
            get
            {
                return MainWindow.Instance;
            }
        }

        private static bool ParametersMatch(MethodDefinition method1, MethodDefinition method2)
        {
            if (method1.Parameters.Count != method2.Parameters.Count)
            {
                return false;
            }

            for (int i = 0; i < method1.Parameters.Count; i++)
            {
                if (method1.Parameters[i].ParameterType.FullName != method2.Parameters[i].ParameterType.FullName)
                {
                    return false;
                }
            }
            return true;
        }
        #endif
    }
}
