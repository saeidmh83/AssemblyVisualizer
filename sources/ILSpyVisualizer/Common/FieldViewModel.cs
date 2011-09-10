// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
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
    }
}
