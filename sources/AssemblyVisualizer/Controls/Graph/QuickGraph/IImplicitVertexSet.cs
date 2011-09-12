// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Diagnostics.Contracts;
using ILSpyVisualizer.Controls.Graph.QuickGraph.Contracts;

namespace ILSpyVisualizer.Controls.Graph.QuickGraph
{
    /// <summary>
    /// An implicit set of vertices
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    [ContractClass(typeof(IImplicitVertexSetContract<>))]
    public interface IImplicitVertexSet<TVertex>
    {
        /// <summary>
        /// Determines whether the specified vertex contains vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>
        /// 	<c>true</c> if the specified vertex contains vertex; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        bool ContainsVertex(TVertex vertex);
    }
}
