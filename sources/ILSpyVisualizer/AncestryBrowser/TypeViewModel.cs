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

namespace ILSpyVisualizer.AncestryBrowser
{
    class TypeViewModel : ViewModelBase
    {
        private TypeDefinition _typeDefinition;
        private bool _isExpanded;
        private IEnumerable<MemberViewModel> _members;

        public TypeViewModel(TypeDefinition typeDefinition)
        {
            _isExpanded = true;
            _typeDefinition = typeDefinition;
            if (_typeDefinition.BaseType != null)
            {
                BaseType = new TypeViewModel(_typeDefinition.BaseType.Resolve());
            }   
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
            IEnumerable<MemberViewModel> members = new MemberViewModel[0];

            if (options.ShowFields)
            {
                var fields = _typeDefinition.Fields
                    .Select(f => new FieldViewModel(f))
                    .OfType<MemberViewModel>();
                members = members.Concat(fields);
            }

            if (options.ShowProperties)
            {
                var properties = _typeDefinition.Properties
                    .Select(p => new PropertyViewModel(p))
                    .OfType<MemberViewModel>();
                members = members.Concat(properties);
            }

            if (options.ShowEvents)
            {
                var events = _typeDefinition.Events
                    .Select(e => new EventViewModel(e))
                    .OfType<MemberViewModel>();
                members = members.Concat(events);
            }

            if (options.ShowMethods)
            {
                var methods = _typeDefinition.Methods
                    .Where(m => !m.IsGetter && !m.IsSetter && !m.IsAddOn && !m.IsRemoveOn)
                    .Select(m => new MethodViewModel(m))
                    .OfType<MemberViewModel>();
                members = members.Concat(methods);
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

            Members = members;
        }

        private static string GetFullName(string typeNamespace, string typeName)
        {
            if (string.IsNullOrEmpty(typeNamespace))
            {
                return typeName;
            }
            return string.Format("{0}.{1}", typeNamespace, typeName);
        }
    }
}
