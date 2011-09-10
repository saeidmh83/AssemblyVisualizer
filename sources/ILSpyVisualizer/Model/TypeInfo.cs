// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILSpyVisualizer.Model
{
    class TypeInfo : MemberInfo
    {
        private TypeInfo _baseType;        

        public TypeInfo BaseType 
        {
            get
            {
                if (_baseType == null)
                {
                    if (BaseTypeRetriever == null)
                    {
                        return null;
                    }
                    _baseType = BaseTypeRetriever();
                    BaseTypeRetriever = null;
                }
                return _baseType;
            }
        }

        public Func<TypeInfo> BaseTypeRetriever { get; set; }
        public ModuleInfo Module { get; set; }

        public IEnumerable<MethodInfo> Methods { get; set; }
        public IEnumerable<PropertyInfo> Properties { get; set; }
        public IEnumerable<EventInfo> Events { get; set; }
        public IEnumerable<FieldInfo> Fields { get; set; }

        public bool IsEnum { get; set; }
        public bool IsInterface { get; set; }
        public bool IsValueType { get; set; }

        public int MembersCount { get; set; }
    }
}
