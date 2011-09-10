// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using System.Windows.Input;
using ICSharpCode.ILSpy;
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

        public abstract object MemberReference { get; }
        public MemberInfo MemberInfo 
        {
            get
            {
                return _memberInfo;
            }
        }

        public abstract bool IsPublic { get; }
        public abstract bool IsProtected { get; }
        public abstract bool IsInternal { get; }
        public abstract bool IsPrivate { get; }
        public abstract bool IsProtectedInternal { get; }   

        private void JumpCommandHandler()
        {
            Services.JumpTo(MemberReference);
        }
	}
}
