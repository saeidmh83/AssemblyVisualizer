// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILSpyVisualizer.Model
{
    class FieldInfo : MemberInfo
    {
        public bool IsInitOnly { get; set; }
        public bool IsSpecialName { get; set; }
        public bool IsLiteral { get; set; }
    }
}
