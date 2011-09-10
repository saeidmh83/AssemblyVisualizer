using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;
using ILSpyVisualizer.Model;
using Mono.Cecil;

namespace ILSpyVisualizer.HAL
{
    class Services
    {
        public static void JumpTo(object memberReference)
        {
            MainWindow.Instance.JumpToReference(memberReference);
        }

        public static bool MethodsMatch(MethodInfo method1, MethodInfo method2)
        { 
            var md1 = method1.MemberReference as MethodDefinition;
            var md2 = method2.MemberReference as MethodDefinition;

            return md1.Name == md2.Name && ParametersMatch(md1, md2);
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
    }
}
