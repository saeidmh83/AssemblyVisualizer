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
    }
}
