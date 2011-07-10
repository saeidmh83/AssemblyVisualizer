// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using ILSpyVisualizer.Controls.Graph.QuickGraph;
using System.Windows;

namespace ILSpyVisualizer.Controls.Graph.GraphSharp.Layout
{
    public class ContextualLayoutContext<TVertex, TEdge, TGraph> : LayoutContext<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        public TVertex SelectedVertex { get; private set; }

        public ContextualLayoutContext( TGraph graph, TVertex selectedVertex, IDictionary<TVertex, Point> positions, IDictionary<TVertex, Size> sizes )
            : base( graph, positions, sizes, global::ILSpyVisualizer.Controls.Graph.GraphSharp.Layout.LayoutMode.Simple )
        {
            SelectedVertex = selectedVertex;
        }
    }
}