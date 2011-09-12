// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.Common
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
	}
}
