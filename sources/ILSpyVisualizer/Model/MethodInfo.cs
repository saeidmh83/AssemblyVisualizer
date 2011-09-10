// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILSpyVisualizer.Model
{
    class MethodInfo : MemberInfo
    {
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
    }
}
