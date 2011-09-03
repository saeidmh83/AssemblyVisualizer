// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using Mono.Cecil;

namespace ILSpyVisualizer.Common
{
	class PropertyViewModel : MemberViewModel
	{
		private readonly PropertyDefinition _propertyDefinition;

		public PropertyViewModel(PropertyDefinition propertyDefinition)
		{
			_propertyDefinition = propertyDefinition;
		}

		public override ImageSource Icon
		{
			get { return PropertyTreeNode.GetIcon(_propertyDefinition); }
		}

		public override string Text
		{
			get
			{
				return PropertyTreeNode
					.GetText(_propertyDefinition, MainWindow.Instance.CurrentLanguage) as string;
			}
		}

        public override MemberReference MemberReference
        {
            get { return _propertyDefinition; }
        }

        public override bool IsPublic
        {
            get
            {
                return _propertyDefinition.GetMethod != null
                     && _propertyDefinition.GetMethod.IsPublic
                     || _propertyDefinition.SetMethod != null
                     && _propertyDefinition.SetMethod.IsPublic;
            }
        }

        public override bool IsProtected
        {
            get
            {
                return _propertyDefinition.GetMethod != null
                     && _propertyDefinition.GetMethod.IsFamily
                     || _propertyDefinition.SetMethod != null
                     && _propertyDefinition.SetMethod.IsFamily;
            }
        }

        public override bool IsInternal
        {
            get
            {
                return _propertyDefinition.GetMethod != null
                       && _propertyDefinition.GetMethod.IsAssembly
                       || _propertyDefinition.SetMethod != null
                       && _propertyDefinition.SetMethod.IsAssembly;
            }
        }

        public override bool IsPrivate
        {
            get
            {
                return _propertyDefinition.GetMethod != null
                       && _propertyDefinition.GetMethod.IsPrivate
                       || _propertyDefinition.SetMethod != null
                       && _propertyDefinition.SetMethod.IsPrivate;
            }
        }

        public override bool IsProtectedInternal
        {
            get
            {
                return _propertyDefinition.GetMethod != null
                     && _propertyDefinition.GetMethod.IsFamilyOrAssembly
                     || _propertyDefinition.SetMethod != null
                     && _propertyDefinition.SetMethod.IsFamilyOrAssembly;
            }
        }
	}
}
