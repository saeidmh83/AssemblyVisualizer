// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using Mono.Cecil;

namespace ILSpyVisualizer.Common
{
    class FieldViewModel : MemberViewModel
    {
        private readonly FieldDefinition _fieldDefinition;

        public FieldViewModel(FieldDefinition propertyDefinition)
        {
            _fieldDefinition = propertyDefinition;
        }

        public override ImageSource Icon
        {
            get { return FieldTreeNode.GetIcon(_fieldDefinition); }
        }

        public override string Text
        {
            get
            {
                return _fieldDefinition.Name;
            }
        }

        public override MemberReference MemberReference
        {
            get { return _fieldDefinition; }
        }

        public override bool IsPublic
        {
            get { return _fieldDefinition.IsPublic; }
        }

        public override bool IsProtected
        {
            get { return _fieldDefinition.IsFamily; }
        }

        public override bool IsInternal
        {
            get { return _fieldDefinition.IsAssembly; }
        }

        public override bool IsPrivate
        {
            get { return _fieldDefinition.IsPrivate; }
        }

        public override bool IsProtectedInternal
        {
            get { return _fieldDefinition.IsFamilyOrAssembly; }
        }
    }
}
