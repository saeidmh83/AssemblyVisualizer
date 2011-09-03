// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using Mono.Cecil;

namespace ILSpyVisualizer.Common
{
	class EventViewModel : MemberViewModel
	{
		private readonly EventDefinition _eventDefinition;

		public EventViewModel(EventDefinition eventDefinition)
		{
			_eventDefinition = eventDefinition;
		}

		public override ImageSource Icon
		{
			get { return EventTreeNode.GetIcon(_eventDefinition); }
		}

		public override string Text
		{
			get
			{
				return EventTreeNode
					.GetText(_eventDefinition, MainWindow.Instance.CurrentLanguage) as string;
			}
		}

        public override MemberReference MemberReference
        {
            get { return _eventDefinition; }
        }

        public override bool IsPublic
        {
            get { return _eventDefinition.AddMethod.IsPublic; }
        }

        public override bool IsProtected
        {
            get { return _eventDefinition.AddMethod.IsFamily; }
        }

        public override bool IsInternal
        {
            get { return _eventDefinition.AddMethod.IsAssembly; }
        }

        public override bool IsPrivate
        {
            get { return _eventDefinition.AddMethod.IsPrivate; }
        }

        public override bool IsProtectedInternal
        {
            get { return _eventDefinition.AddMethod.IsFamilyOrAssembly; }
        }
	}
}
