// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using ILSpyVisualizer.Controls.Graph.QuickGraph.Contracts;
using System.Diagnostics.Contracts;

namespace ILSpyVisualizer.Controls.Graph.QuickGraph
{
    [ContractClass(typeof(IIncidenceGraphContract<,>))]
    public interface IIncidenceGraph<TVertex, TEdge> 
        : IImplicitGraph<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
        bool ContainsEdge(TVertex source, TVertex target);
        bool TryGetEdges(
            TVertex source,
            TVertex target,
            out IEnumerable<TEdge> edges);
        bool TryGetEdge(
            TVertex source,
            TVertex target,
            out TEdge edge);
    }
}
