// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.Infrastructure;

using ILSpyVisualizer.Common;

using ILSpyVisualizer.Properties;
using ILSpyVisualizer.Model;
using ILSpyVisualizer.HAL;

namespace ILSpyVisualizer.AncestryBrowser
{
    class TypeViewModel : ViewModelBase
    {
        private TypeInfo _typeInfo;
        private bool _isExpanded;        
        private IEnumerable<MemberViewModel> _members;

        private MemberViewModel[] _events;
        private MemberViewModel[] _fields;
        private MemberViewModel[] _methods;
        private MemberViewModel[] _properties;

        public TypeViewModel(TypeInfo typeInfo)
        {
            _isExpanded = true;
            _typeInfo = typeInfo;
            if (_typeInfo.BaseType != null)
            {
                BaseType = new TypeViewModel(_typeInfo.BaseType);
            }

            _fields = _typeInfo.Fields
                .OrderBy(f => f.Name)
                .Select(f => new FieldViewModel(f))
                .OfType<MemberViewModel>()
                .ToArray();
            _properties = _typeInfo.Properties
                .OrderBy(p => p.Name)
                .Select(p => new PropertyViewModel(p))
                .OfType<MemberViewModel>()
                .ToArray();
            _events = _typeInfo.Events
                .OrderBy(e => e.Name)
                .Select(e => new EventViewModel(e))
                .OfType<MemberViewModel>()
                .ToArray();
            _methods = _typeInfo.Methods
                .OrderBy(m => m.Name)               
                .Select(m => new MethodViewModel(m))
                .OfType<MemberViewModel>()
                .ToArray();   
        }

        public IEnumerable<MemberViewModel> Members 
        {
            get { return _members; }
            set 
            {
                _members = value;
                OnPropertyChanged("Members");
            }
        }
        public TypeInfo TypeInfo { get { return _typeInfo; } }
        public TypeViewModel BaseType { get; set; }
        public AssemblyViewModel Assembly { get; set; }

        public IEnumerable<TypeViewModel> Ancestry
        {
            get 
            {
                if (BaseType == null)
                {
                    return new [] { this };
                }
                return new[] { this }.Concat(BaseType.Ancestry);
            }
        }

        public bool IsLast { get; set; }

        public string Name 
        {
            get
            {
                return _typeInfo.Name;
            }
        }

        public string FullName
        {
            get
            {
                return _typeInfo.FullName;
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }       

        public void UpdateMembers(MemberOptions options)
        {
            ResetMembers(_events);
            ResetMembers(_fields);
            ResetMembers(_methods);
            ResetMembers(_properties);

            IEnumerable<MemberViewModel> members = new MemberViewModel[0]; 

            if (options.ShowFields)
            {                
                members = members.Concat(_fields);
            }
            if (options.ShowProperties)
            {                
                members = members.Concat(_properties);
            }
            if (options.ShowEvents)
            {                
                members = members.Concat(_events);
            }
            if (options.ShowMethods)
            {                
                members = members.Concat(_methods);
            }           

            if (!options.ShowPrivate)
            {
                members = members.Where(m => !m.IsPrivate);
            }
            if (!options.ShowInternal)
            {
                members = members.Where(m => !m.IsInternal);
            }
            if (!options.ShowProtected)
            {
                members = members.Where(m => !m.IsProtected);
            }
            if (!options.ShowProtectedInternal)
            {
                members = members.Where(m => !m.IsProtectedInternal);
            }
            if (!options.ShowPublic)
            {
                members = members.Where(m => !m.IsPublic);
            }
            if (!string.IsNullOrWhiteSpace(options.SearchTerm))
            { 
                members = members.Where(m => m.MemberInfo.Name.IndexOf(options.SearchTerm.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            if (options.MemberKind == MemberKind.Virtual)
            {                
                members = members.OfType<ICanBeVirtual>().Where(m => m.IsVirtual).OfType<MemberViewModel>();
                foreach (var member in members)
                {
                    var maybeVirtualMember = member as ICanBeVirtual;
                    if (maybeVirtualMember.IsOverride)
                    {
                        member.IsMarked = true;                        
                    }
                }               
            }            

            Members = members.ToArray();
        }

        public void FillToolTips()
        { 
            FillPropertiesToolTips();
            FillMethodsToolTips();
        }

        private TypeViewModel GetTypeInAncestryWithProperty(string propertyText)
        {
            if (_properties.Any(m => m.Text == propertyText))
            {
                return this;
            }
            if (BaseType != null)
            {
                return BaseType.GetTypeInAncestryWithProperty(propertyText);
            }
            throw new ArgumentException("No type with such property text in ancestry");
        }

        private TypeViewModel GetTypeInAncestryWithMethod(MethodInfo methodInfo)
        {
            if (_methods.Any(m => Services.MethodsMatch(methodInfo, m.MemberInfo as MethodInfo)))
            {
                return this;
            }
            if (BaseType != null)
            {
                return BaseType.GetTypeInAncestryWithMethod(methodInfo);
            }
            throw new ArgumentException("No type with such method text in ancestry");
        }        

        private void FillPropertiesToolTips()
        {
            foreach (var property in _properties)
            {
                var maybeVirtualMember = property as ICanBeVirtual;
                if (maybeVirtualMember == null)
                {
                    continue;
                }
                if (maybeVirtualMember.IsOverride)
                {
                    if (BaseType != null)
                    {
                        var definingType = BaseType.GetTypeInAncestryWithProperty(property.Text);
                        property.ToolTip = string.Format("{0}\n{1} {2}", property.Text, Resources.OverrideFrom, definingType.Name);
                    }
                }
            }
        }

        private void FillMethodsToolTips()
        {
            foreach (var method in _methods)
            {
                var maybeVirtualMember = method as ICanBeVirtual;
                if (maybeVirtualMember == null)
                {
                    continue;
                }
                if (maybeVirtualMember.IsOverride)
                {                    
                    if (BaseType != null)
                    {
                        var definingType = BaseType.GetTypeInAncestryWithMethod(method.MemberInfo as MethodInfo);
                        method.ToolTip = string.Format("{0}\n{1} {2}", method.Text, Resources.OverrideFrom, definingType.Name);
                    }
                }
            }         
        }

        private static string GetFullName(string typeNamespace, string typeName)
        {
            if (string.IsNullOrEmpty(typeNamespace))
            {
                return typeName;
            }
            return string.Format("{0}.{1}", typeNamespace, typeName);
        }

        private static void ResetMembers(IEnumerable<MemberViewModel> members)
        {
            foreach (var member in members)
            {
                member.IsMarked = false;
            }
        }
    }
}
