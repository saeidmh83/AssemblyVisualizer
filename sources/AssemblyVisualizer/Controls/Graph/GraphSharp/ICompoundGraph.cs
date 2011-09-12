// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using AssemblyVisualizer.Controls.Graph.QuickGraph;

namespace AssemblyVisualizer.Controls.Graph.GraphSharp
{
	public interface ICompoundGraph<TVertex, TEdge> : IBidirectionalGraph<TVertex, TEdge>
		where TEdge : IEdge<TVertex>
	{
		bool AddChildVertex( TVertex parent, TVertex child );
		int AddChildVertexRange( TVertex parent, IEnumerable<TVertex> children );
		TVertex GetParent( TVertex vertex );
		bool IsChildVertex( TVertex vertex );
		IEnumerable<TVertex> GetChildrenVertices( TVertex vertex );
		int GetChildrenCount( TVertex vertex );
		bool IsCompoundVertex( TVertex vertex );

		IEnumerable<TVertex> CompoundVertices { get; }
        IEnumerable<TVertex> SimpleVertices { get; }
	}
}
