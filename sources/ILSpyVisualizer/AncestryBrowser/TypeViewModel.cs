// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using ILSpyVisualizer.Common;
using ICSharpCode.ILSpy;
using ILSpyVisualizer.Properties;

namespace ILSpyVisualizer.AncestryBrowser
{
    class TypeViewModel : ViewModelBase
    {
        private TypeDefinition _typeDefinition;
        private bool _isExpanded;        
        private IEnumerable<MemberViewModel> _members;

        private IEnumerable<MemberViewModel> _events;
        private IEnumerable<MemberViewModel> _fields;
        private IEnumerable<MemberViewModel> _methods;
        private IEnumerable<MemberViewModel> _properties;

        public TypeViewModel(TypeDefinition typeDefinition)
        {
            _isExpanded = true;
            _typeDefinition = typeDefinition;
            if (_typeDefinition.BaseType != null)
            {
                BaseType = new TypeViewModel(_typeDefinition.BaseType.Resolve());
            }

            _fields = _typeDefinition.Fields
                .OrderBy(f => f.Name)
                .Select(f => new FieldViewModel(f))
                .OfType<MemberViewModel>()
                .ToArray();
            _properties = _typeDefinition.Properties
                .OrderBy(p => p.Name)
                .Select(p => new PropertyViewModel(p))
                .OfType<MemberViewModel>()
                .ToArray();
            _events = _typeDefinition.Events
                .OrderBy(e => e.Name)
                .Select(e => new EventViewModel(e))
                .OfType<MemberViewModel>()
                .ToArray();
            _methods = _typeDefinition.Methods
                .OrderBy(m => m.Name)
                .Where(m => !m.IsGetter && !m.IsSetter && !m.IsAddOn && !m.IsRemoveOn)
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
        public TypeDefinition TypeDefinition { get { return _typeDefinition; } }
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
                return MainWindow.Instance.CurrentLanguage.FormatTypeName(_typeDefinition);
            }
        }

        public string FullName
        {
            get
            {
                return GetFullName(_typeDefinition.Namespace, Name);
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
                members = members.Where(m => m.MemberReference.Name.StartsWith(options.SearchTerm.Trim(), StringComparison.InvariantCultureIgnoreCase));
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

        private TypeViewModel GetTypeInAncestryWithMethod(MethodDefinition methodDefinition)
        {
            if (_methods
                    .Select(m => m.MemberReference as MethodDefinition)
                    .Any(m => m.Name == methodDefinition.Name && ParametersMatch(m, methodDefinition)))
            {
                return this;
            }
            if (BaseType != null)
            {
                return BaseType.GetTypeInAncestryWithMethod(methodDefinition);
            }
            throw new ArgumentException("No type with such method text in ancestry");
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
                        var definingType = BaseType.GetTypeInAncestryWithMethod(method.MemberReference as MethodDefinition);
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
