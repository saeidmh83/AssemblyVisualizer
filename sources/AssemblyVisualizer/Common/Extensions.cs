using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.Common
{
    static class Extensions
    {
        public static bool IsVisibleOutside(this MemberInfo methodInfo)
        {
            return methodInfo.IsPublic || methodInfo.IsInternal || methodInfo.IsProtectedOrInternal;
        }
    }
}
