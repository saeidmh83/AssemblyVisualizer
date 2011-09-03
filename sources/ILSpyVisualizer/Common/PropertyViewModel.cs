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
            get { return _propertyDefinition.GetMethod.IsPublic; }
        }

        public override bool IsProtected
        {
            get { return _propertyDefinition.GetMethod.IsFamily; }
        }

        public override bool IsInternal
        {
            get { return _propertyDefinition.GetMethod.IsAssembly; }
        }

        public override bool IsPrivate
        {
            get { return _propertyDefinition.GetMethod.IsPrivate; }
        }

        public override bool IsProtectedInternal
        {
            get { return _propertyDefinition.GetMethod.IsFamilyOrAssembly; }
        }
	}
}
