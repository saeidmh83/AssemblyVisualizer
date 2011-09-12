// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Windows;
using AssemblyVisualizer.Controls.Graph;
using AssemblyVisualizer.Controls.Graph.GraphSharp.Layout;
using AssemblyVisualizer.Controls.Graph.QuickGraph;

namespace AssemblyVisualizer.DependencyBrowser
{
    class AssemblyGraph : BidirectionalGraph<AssemblyViewModel, Edge<AssemblyViewModel>>
    {
        public AssemblyGraph(bool allowParallelEdges)
            : base(allowParallelEdges)
        {
        }
    }

    class AssemblyGraphLayout : GraphLayout<AssemblyViewModel, Edge<AssemblyViewModel>, AssemblyGraph>
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
