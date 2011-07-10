// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Diagnostics.Contracts;

namespace ILSpyVisualizer.Controls.Graph.QuickGraph.Contracts
{
    [ContractClassFor(typeof(IImplicitVertexSet<>))]
    abstract class IImplicitVertexSetContract<TVertex>
        : IImplicitVertexSet<TVertex>
    {
        [Pure]
        bool IImplicitVertexSet<TVertex>.ContainsVertex(TVertex vertex)
        {
            IImplicitVertexSet<TVertex> ithis = this;
            Contract.Requires(vertex != null);

            return default(bool);
        }
    }
}
