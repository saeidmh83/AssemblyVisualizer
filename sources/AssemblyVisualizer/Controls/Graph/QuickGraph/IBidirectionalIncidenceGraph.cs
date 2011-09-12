// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
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
    [ContractClass(typeof(IBidirectionalIncidenceGraphContract<,>))]
    public interface IBidirectionalIncidenceGraph<TVertex, TEdge>
        : IIncidenceGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Determines whether <paramref name="v"/> has no in-edges.
        /// </summary>
        /// <param name="v">The vertex</param>
        /// <returns>
        /// 	<c>true</c> if <paramref name="v"/> has no in-edges; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        bool IsInEdgesEmpty(TVertex v);

        /// <summary>
        /// Gets the number of in-edges of <paramref name="v"/>
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns>The number of in-edges pointing towards <paramref name="v"/></returns>
        [Pure]
        int InDegree(TVertex v);

        /// <summary>
        /// Gets the collection of in-edges of <paramref name="v"/>.
        /// </summary>
        /// <param name="v">The vertex</param>
        /// <returns>The collection of in-edges of <paramref name="v"/></returns>
        [Pure]
        IEnumerable<TEdge> InEdges(TVertex v);

        /// <summary>
        /// Tries to get the in-edges of <paramref name="v"/>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="edges"></param>
        /// <returns></returns>
        [Pure]
        bool TryGetInEdges(TVertex v, out IEnumerable<TEdge> edges);

        /// <summary>
        /// Gets the in-edge at location <paramref name="index"/>.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        [Pure]
        TEdge InEdge(TVertex v, int index);

        /// <summary>
        /// Gets the degree of <paramref name="v"/>, i.e.
        /// the sum of the out-degree and in-degree of <paramref name="v"/>.
        /// </summary>
        /// <param name="v">The vertex</param>
        /// <returns>The sum of OutDegree and InDegree of <paramref name="v"/></returns>
        [Pure]
        int Degree(TVertex v);
    }
}
