// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;

namespace ILSpyVisualizer.Controls.Graph.QuickGraph
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class EdgeList<TVertex, TEdge>
        : List<TEdge>
        , IEdgeList<TVertex, TEdge>
#if !SILVERLIGHT
        , ICloneable
#endif
        where TEdge : IEdge<TVertex>
    {
        public EdgeList() 
        { }

        public EdgeList(int capacity)
            : base(capacity)
        { }

        public EdgeList(EdgeList<TVertex, TEdge> list)
            : base(list)
        {}

        public EdgeList<TVertex, TEdge> Clone()
        {
            return new EdgeList<TVertex, TEdge>(this);
        }

        IEdgeList<TVertex, TEdge> IEdgeList<TVertex,TEdge>.Clone()
        {
            return this.Clone();
        }

#if !SILVERLIGHT
        object ICloneable.Clone()
        {
            return this.Clone();
        }
#endif
    }
}
