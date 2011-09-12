// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

namespace ILSpyVisualizer.Controls.Graph.QuickGraph
{
    /// <summary>
    /// A directed graph where vertices and edges can be enumerated efficiently.
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
    public interface IVertexAndEdgeListGraph<TVertex,TEdge> 
        : IVertexListGraph<TVertex,TEdge>
        , IEdgeListGraph<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {}
}
