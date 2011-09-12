// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using ILSpyVisualizer.Infrastructure;

using System.Windows.Input;

using ILSpyVisualizer.Model;
using ILSpyVisualizer.HAL;

namespace ILSpyVisualizer.Common
{
	abstract class MemberViewModel : ViewModelBase
	{
        private string _toolTip;
        private MemberInfo _memberInfo;

        public bool IsMarked { get; set; }
        public virtual string ToolTip
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_toolTip))
                {
                    return Text;
                }
                return _toolTip;
            }
            set 
            {
                _toolTip = value;
            }
        }        
		public abstract ImageSource Icon { get; }
		public abstract string Text { get; }
        public ICommand JumpCommand { get; private set; }

        public MemberViewModel(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;

            JumpCommand = new DelegateCommand(JumpCommandHandler);
        }            

        public MemberInfo MemberInfo 
        {
            get
            {
                return _memberInfo;
            }
        }

        public object MemberReference
        {
            get { return _memberInfo.MemberReference; }
        } 

        public bool IsPublic
        {
            get
            {
                return _memberInfo.IsPublic;
            }
        }

        public bool IsProtected
        {
            get
            {
                return _memberInfo.IsProtected;
            }
        }

        public bool IsInternal
        {
            get
            {
                return _memberInfo.IsInternal;
            }
        }

        public bool IsPrivate
        {
            get
            {
                return _memberInfo.IsPrivate;
            }
        }

        public bool IsProtectedInternal
        {
            get
            {
                return _memberInfo.IsProtectedOrInternal;
            }
        }

        private void JumpCommandHandler()
        {
            Services.JumpTo(MemberReference);
        }
	}
}
