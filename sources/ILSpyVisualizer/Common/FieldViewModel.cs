// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using Mono.Cecil;
using ILSpyVisualizer.Model;

namespace ILSpyVisualizer.Common
{
    class FieldViewModel : MemberViewModel
    {
        private readonly FieldInfo _fieldInfo;

        public FieldViewModel(FieldInfo fieldInfo) : base(fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public override ImageSource Icon
        {
            get { return _fieldInfo.Icon; }
        }

        public override string Text
        {
            get
            {
                return _fieldInfo.Text;
            }
        }

        public override object MemberReference
        {
            get { return _fieldInfo.MemberReference; }
        }

        public override bool IsPublic
        {
            get { return _fieldInfo.IsPublic; }
        }

        public override bool IsProtected
        {
            get { return _fieldInfo.IsProtected; }
        }

        public override bool IsInternal
        {
            get { return _fieldInfo.IsInternal; }
        }

        public override bool IsPrivate
        {
            get { return _fieldInfo.IsPrivate; }
        }

        public override bool IsProtectedInternal
        {
            get { return _fieldInfo.IsProtectedOrInternal; }
        }
    }
}
