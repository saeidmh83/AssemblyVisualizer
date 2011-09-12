// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using ILSpyVisualizer.Controls.Graph.QuickGraph;
using System.Windows;

namespace ILSpyVisualizer.Controls.Graph.GraphSharp.Layout
{
    public interface ILayoutContext<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        IDictionary<TVertex, Point> Positions { get; }
        IDictionary<TVertex, Size> Sizes { get; }

        TGraph Graph { get; }

        global::ILSpyVisualizer.Controls.Graph.GraphSharp.Layout.LayoutMode Mode { get; }
    }
}