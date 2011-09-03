using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILSpyVisualizer.AncestryBrowser
{
    class MemberOptions
    {
        public bool ShowFields { get; set;}
        public bool ShowProperties { get; set;}
        public bool ShowEvents { get; set;}
        public bool ShowMethods { get; set;}
        public bool ShowPublic { get; set;}
        public bool ShowInternal { get; set;}
        public bool ShowProtected { get; set;}
        public bool ShowPrivate { get; set; }
        public bool ShowProtectedInternal { get; set; }
        
    }
}
