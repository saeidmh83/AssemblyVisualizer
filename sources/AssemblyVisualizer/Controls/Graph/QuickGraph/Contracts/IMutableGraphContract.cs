// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Diagnostics.Contracts;

namespace AssemblyVisualizer.Controls.Graph.QuickGraph.Contracts
{
    [ContractClassFor(typeof(IMutableGraph<,>))]
    abstract class IMutableGraphContract<TVertex, TEdge>
        : IMutableGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        #region IMutableGraph<TVertex,TEdge> Members
        void IMutableGraph<TVertex, TEdge>.Clear()
        {
            IMutableGraph<TVertex, TEdge> ithis = this;
        }
        #endregion

        #region IGraph<TVertex,TEdge> Members

        bool IGraph<TVertex, TEdge>.IsDirected
        {
            get { throw new NotImplementedException(); }
        }

        bool IGraph<TVertex, TEdge>.AllowParallelEdges
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
