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
	class EventViewModel : MemberViewModel
	{
		private readonly EventInfo _eventInfo;

		public EventViewModel(EventInfo eventInfo) : base(eventInfo)
		{
			_eventInfo = eventInfo;
		}

		public override ImageSource Icon
		{
			get { return _eventInfo.Icon; }
		}

		public override string Text
		{
			get
			{
				return _eventInfo.Text;
			}
		}

        public override object MemberReference
        {
            get { return _eventInfo.MemberReference; }
        }

        public override bool IsPublic
        {
            get { return _eventInfo.IsPublic; }
        }

        public override bool IsProtected
        {
            get { return _eventInfo.IsProtected; }
        }

        public override bool IsInternal
        {
            get { return _eventInfo.IsInternal; }
        }

        public override bool IsPrivate
        {
            get { return _eventInfo.IsPrivate; }
        }

        public override bool IsProtectedInternal
        {
            get { return _eventInfo.IsProtectedOrInternal; }
        }
	}
}
