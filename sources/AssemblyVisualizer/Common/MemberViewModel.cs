// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using AssemblyVisualizer.Infrastructure;

using System.Windows.Input;

using AssemblyVisualizer.Model;
using AssemblyVisualizer.HAL;

namespace AssemblyVisualizer.Common
{
	abstract class MemberViewModel : ViewModelBase
	{
        private string _toolTip;
        private MemberInfo _memberInfo;
        private SolidColorBrush _background;
        private SolidColorBrush _foreground;

        public MemberViewModel(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;

            JumpCommand = new DelegateCommand(JumpCommandHandler);
        }

        public bool IsMarked { get; set; }        
        public ICommand JumpCommand { get; private set; }

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

        public virtual string Text
        {
            get
            {
                return _memberInfo.Text;
            }
        }

        public virtual ImageSource Icon
        {
            get
            {
                return _memberInfo.Icon;
            }
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

        public SolidColorBrush Background 
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                OnPropertyChanged("Background");
            }
        }

        public SolidColorBrush Foreground
        {
            get
            {
                return _foreground;
            }
            set
            {
                _foreground = value;
                OnPropertyChanged("Foreground");
            }
        }
        private void JumpCommandHandler()
        {
            Services.JumpTo(MemberReference);
        }
	}
}
