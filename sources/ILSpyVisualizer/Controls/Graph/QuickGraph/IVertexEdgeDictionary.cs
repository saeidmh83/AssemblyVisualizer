// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics.Contracts;
using ILSpyVisualizer.Controls.Graph.QuickGraph.Contracts;

namespace ILSpyVisualizer.Controls.Graph.QuickGraph
{
    /// <summary>
    /// A dictionary of vertices to a list of edges
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TEdge"></typeparam>
    [ContractClass(typeof(IVertexEdgeDictionaryContract<,>))]
    public interface IVertexEdgeDictionary<TVertex, TEdge>
        : IDictionary<TVertex, IEdgeList<TVertex, TEdge>>
#if !SILVERLIGHT
        , ICloneable
        , ISerializable
#endif
     where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Gets a clone of the dictionary. The vertices and edges are not cloned.
        /// </summary>
        /// <returns></returns>
#if !SILVERLIGHT
        new 
#endif
        IVertexEdgeDictionary<TVertex, TEdge> Clone();
    }
}
