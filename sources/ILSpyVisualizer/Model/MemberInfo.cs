// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ILSpyVisualizer.Model
{
    class MemberInfo
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Text { get; set; }
        public TypeInfo DeclaringType { get; set; }
        public bool IsPublic { get; set; }
        public bool IsInternal { get; set; }
        public bool IsProtected { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsProtectedOrInternal { get; set; }
        public bool IsProtectedAndInternal { get; set; }
        public ImageSource Icon { get; set; }
        public object MemberReference { get; set; }
    }
}
