// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Windows;
using AssemblyVisualizer.Controls.Graph;
using AssemblyVisualizer.Controls.Graph.GraphSharp.Layout;
using AssemblyVisualizer.Controls.Graph.QuickGraph;
using AssemblyVisualizer.Common;

namespace AssemblyVisualizer.TypeBrowser
{
    class MemberGraph : BidirectionalGraph<MemberViewModel, Edge<MemberViewModel>>
    {
        public MemberGraph(bool allowParallelEdges)
            : base(allowParallelEdges)
        {
        }
    }

    class MemberGraphLayout : GraphLayout<MemberViewModel, Edge<MemberViewModel>, MemberGraph>
    {
        public event Action LayoutFinished;

        protected override void OnLayoutFinished()
        {
            base.OnLayoutFinished();

            var handler = LayoutFinished;
            if (handler != null)
            {
                handler();
            }
        }
    }
}
