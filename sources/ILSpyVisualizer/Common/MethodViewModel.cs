// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using ICSharpCode.ILSpy.TreeNodes;
using Mono.Cecil;
using ICSharpCode.ILSpy;

namespace ILSpyVisualizer.Common
{
	class MethodViewModel : MemberViewModel
	{
		private readonly MethodDefinition _methodDefinition;

		public MethodViewModel(MethodDefinition methodDefinition)
		{
			_methodDefinition = methodDefinition;           
		}

		public override ImageSource Icon
		{
			get { return MethodTreeNode.GetIcon(_methodDefinition); }
		}

		public override string Text
		{
			get
			{
				return MethodTreeNode
					.GetText(_methodDefinition, MainWindow.Instance.CurrentLanguage) as string;
			}
		}

        public override bool IsVirtual
        {
            get
            {
                return _methodDefinition.IsVirtual;
            }
        }

        public override bool IsOverridden
        {
            get
            {
                // Add proper implementation
                return base.IsOverridden;
            }
        }

        public override MemberReference MemberReference
        {
            get { return _methodDefinition; }
        }

        public override bool IsPublic
        {
            get { return _methodDefinition.IsPublic; }
        }

        public override bool IsProtected
        {
            get { return _methodDefinition.IsFamily; }
        }

        public override bool IsInternal
        {
            get { return _methodDefinition.IsAssembly; }
        }

        public override bool IsPrivate
        {
            get { return _methodDefinition.IsPrivate; }
        }

        public override bool IsProtectedInternal
        {
            get { return _methodDefinition.IsFamilyOrAssembly; }
        }
	}
}
