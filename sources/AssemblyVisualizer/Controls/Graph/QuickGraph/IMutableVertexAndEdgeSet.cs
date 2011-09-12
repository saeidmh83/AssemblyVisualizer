// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using ILSpyVisualizer.Controls.Graph.QuickGraph.Contracts;
using System.Diagnostics.Contracts;

namespace ILSpyVisualizer.Controls.Graph.QuickGraph
{
    /// <summary>
    /// A mutable vertex and edge set
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TEdge"></typeparam>
    [ContractClass(typeof(IMutableVertexAndEdgeSetContract<,>))]
    public interface IMutableVertexAndEdgeSet<TVertex,TEdge>
        : IEdgeListGraph<TVertex, TEdge>
        , IMutableVertexSet<TVertex>
        , IMutableEdgeListGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Adds the vertices and edge to the graph.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns>true if the edge was added, otherwise false.</returns>
        bool AddVerticesAndEdge(TEdge edge);

        /// <summary>
        /// Adds a set of edges (and it's vertices if necessary)
        /// </summary>
        /// <param name="edges"></param>
        /// <returns>the number of edges added.</returns>
        int AddVerticesAndEdgeRange(IEnumerable<TEdge> edges);
    }

}
