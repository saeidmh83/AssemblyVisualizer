// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Diagnostics.Contracts;
using ILSpyVisualizer.Controls.Graph.QuickGraph.Contracts;

namespace ILSpyVisualizer.Controls.Graph.QuickGraph
{
    /// <summary>
    /// A directed graph datastructure that is efficient
    /// to traverse both in and out edges.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
    /// <typeparam name="TEdge">The type of the edge.</typeparam>
    [ContractClass(typeof(IBidirectionalGraphContract<,>))]
    public interface IBidirectionalGraph<TVertex,TEdge> 
        : IVertexAndEdgeListGraph<TVertex,TEdge>
        , IBidirectionalIncidenceGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
    }
}
