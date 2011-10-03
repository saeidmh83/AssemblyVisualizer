// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using AssemblyVisualizer.HAL;

namespace AssemblyVisualizer.Model
{
    class MemberInfo
    {
        private TypeInfo _declaringType;

        public string Name { get; set; }
        public string FullName { get; set; }
        public string Text { get; set; }        
        public bool IsPublic { get; set; }
        public bool IsInternal { get; set; }
        public bool IsProtected { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsProtectedOrInternal { get; set; }
        public bool IsProtectedAndInternal { get; set; }
        public bool IsStatic { get; set; }
        public ImageSource Icon { get; set; }
        public object MemberReference { get; set; }
        public TypeInfo DeclaringType
        {
            get
            {
                if (_declaringType == null)
                {
                    _declaringType = Helper.GetDeclaringType(MemberReference);
                }
                return _declaringType;
            }
            set
            {
                _declaringType = value;
            }
        }
    }
}
