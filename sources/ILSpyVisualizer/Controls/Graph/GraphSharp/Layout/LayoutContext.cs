// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using ILSpyVisualizer.Controls.Graph.QuickGraph;
using System.Windows;

namespace ILSpyVisualizer.Controls.Graph.GraphSharp.Layout
{
    public class LayoutContext<TVertex, TEdge, TGraph> : ILayoutContext<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        public IDictionary<TVertex, Point> Positions { get; private set; }

        public IDictionary<TVertex, Size> Sizes { get; private set; }

        public TGraph Graph { get; private set; }

        public LayoutMode Mode { get; private set; }

        public LayoutContext( TGraph graph, IDictionary<TVertex, Point> positions, IDictionary<TVertex, Size> sizes, LayoutMode mode )
        {
            Graph = graph;
            Positions = positions;
            Sizes = sizes;
            Mode = mode;
        }
    }
}