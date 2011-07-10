// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ILSpyVisualizer.Controls.Graph.QuickGraph.Contracts;

namespace ILSpyVisualizer.Controls.Graph.QuickGraph
{
    /// <summary>
    /// A mutable vertex set
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    [ContractClass(typeof(IMutableVertexSetContract<>))]
    public interface IMutableVertexSet<TVertex>
        : IVertexSet<TVertex>
    {
        event VertexAction<TVertex> VertexAdded;
        bool AddVertex(TVertex v);
        int AddVertexRange(IEnumerable<TVertex> vertices);

        event VertexAction<TVertex> VertexRemoved;
        bool RemoveVertex(TVertex v);
        int RemoveVertexIf(VertexPredicate<TVertex> pred);
    }
}
