// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using ICSharpCode.ILSpy.TreeNodes;
using Mono.Cecil;
using ICSharpCode.ILSpy;
using ILSpyVisualizer.Model;

namespace ILSpyVisualizer.Common
{
	class MethodViewModel : MemberViewModel, ICanBeVirtual
	{
		private readonly MethodInfo _methodInfo;

		public MethodViewModel(MethodInfo methodInfo) : base(methodInfo)
		{
			_methodInfo = methodInfo;           
		}

		public override ImageSource Icon
		{
			get { return _methodInfo.Icon; }
		}

		public override string Text
		{
			get
			{
                return _methodInfo.Text;
			}
		}

        public bool IsVirtual
        {
            get
            {
                return _methodInfo.IsVirtual;
            }
        }

        public bool IsOverride
        {
            get
            {
                return _methodInfo.IsOverride;           
            }
        }

        public override object MemberReference
        {
            get { return _methodInfo.MemberReference; }
        }

        public override bool IsPublic
        {
            get { return _methodInfo.IsPublic; }
        }

        public override bool IsProtected
        {
            get { return _methodInfo.IsProtected; }
        }

        public override bool IsInternal
        {
            get { return _methodInfo.IsInternal; }
        }

        public override bool IsPrivate
        {
            get { return _methodInfo.IsPrivate; }
        }

        public override bool IsProtectedInternal
        {
            get { return _methodInfo.IsProtectedOrInternal; }
        }        
	}
}
