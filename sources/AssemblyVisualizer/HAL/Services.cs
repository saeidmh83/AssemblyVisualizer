// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.AssemblyBrowser;
using AssemblyVisualizer.Model;

#if Reflector
using AssemblyVisualizer.HAL.Reflector;
using Reflector.CodeModel;
#endif

#if ILSpy
using ICSharpCode.ILSpy;
using Mono.Cecil;
using System.Windows;
#endif

namespace AssemblyVisualizer.HAL
{
    class Services
    {
        public static bool SupportsInteractionBrowser
        {
            get
            { 
#if ILSpy
                return true;
#elif Reflector
                return false;
#endif
            }
        }

        public static void BrowseAssemblies(IEnumerable<AssemblyInfo> assemblies)
        {
            var window = new AssemblyBrowserWindow(assemblies);
#if ILSpy
            window.Owner = MainWindow;
#elif Reflector
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window);
#endif
            window.Show();
        }

        public static void JumpTo(object memberReference)
        {
#if ILSpy
            MainWindow.JumpToReference(memberReference);
#endif
#if Reflector
            Package.AssemblyBrowser.ActiveItem = memberReference;
#endif
        }

        public static bool MethodsMatch(MethodInfo method1, MethodInfo method2)
        { 
#if ILSpy
            var md1 = method1.MemberReference as MethodDefinition;
            var md2 = method2.MemberReference as MethodDefinition;

            return md1.Name == md2.Name && ParametersMatch(md1, md2);
#elif Reflector
            var md1 = method1.MemberReference as IMethodDeclaration;
            var md2 = method2.MemberReference as IMethodDeclaration;            

            //return md1.Name == md2.Name && ParametersMatch(md1, md2);           
            return method1.Text == method2.Text;
            #else

            return false;

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

#if Reflector

        private static bool ParametersMatch(IMethodDeclaration method1, IMethodDeclaration method2)
        {
            if (method1.Parameters.Count != method2.Parameters.Count)
            {
                return false;
            }            

            for (int i = 0; i < method1.Parameters.Count; i++)
            {
                if (method1.Parameters[i].ParameterType != method2.Parameters[i].ParameterType)
                {
                    return false;
                }
            }
            return true;
        }

#endif
    }
}
