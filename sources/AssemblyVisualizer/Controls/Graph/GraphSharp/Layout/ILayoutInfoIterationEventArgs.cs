// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using AssemblyVisualizer.Controls.Graph.QuickGraph;

namespace AssemblyVisualizer.Controls.Graph.GraphSharp.Layout
{
    public interface ILayoutInfoIterationEventArgs<TVertex, TEdge>
        : ILayoutIterationEventArgs<TVertex>
        where TVertex : class
        where TEdge : IEdge<TVertex>
    {
        object GetVertexInfo(TVertex vertex);

        object GetEdgeInfo(TEdge edge);
    }

    public interface ILayoutInfoIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>
        : ILayoutInfoIterationEventArgs<TVertex, TEdge>
        where TVertex : class
        where TEdge : IEdge<TVertex>
    {
        IDictionary<TVertex, TVertexInfo> VertexInfos { get; }
        IDictionary<TEdge, TEdgeInfo> EdgeInfos { get; }
    }
}
