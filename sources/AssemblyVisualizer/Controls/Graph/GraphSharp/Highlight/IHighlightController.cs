// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using AssemblyVisualizer.Controls.Graph.QuickGraph;

namespace AssemblyVisualizer.Controls.Graph.GraphSharp.Highlight
{
	public interface IHighlightController<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
	{
		TGraph Graph { get; }

		IEnumerable<TVertex> HighlightedVertices { get; }
		IEnumerable<TVertex> SemiHighlightedVertices { get; }
		IEnumerable<TEdge> HighlightedEdges { get; }
		IEnumerable<TEdge> SemiHighlightedEdges { get; }

		bool IsHighlightedVertex( TVertex vertex );
		bool IsHighlightedVertex( TVertex vertex, out object highlightInfo );
		bool IsSemiHighlightedVertex( TVertex vertex );
		bool IsSemiHighlightedVertex( TVertex vertex, out object semiHighlightInfo );
		bool IsHighlightedEdge( TEdge edge );
		bool IsHighlightedEdge( TEdge edge, out object highlightInfo );
		bool IsSemiHighlightedEdge( TEdge edge );
		bool IsSemiHighlightedEdge( TEdge edge, out object semiHighlightInfo );

		void HighlightVertex( TVertex vertex, object highlightInfo );
		void SemiHighlightVertex( TVertex vertex, object semiHighlightInfo );
		void HighlightEdge( TEdge edge, object highlightInfo );
		void SemiHighlightEdge( TEdge edge, object semiHighlightInfo );

		void RemoveHighlightFromVertex( TVertex vertex );
		void RemoveSemiHighlightFromVertex( TVertex vertex );
		void RemoveHighlightFromEdge( TEdge edge );
		void RemoveSemiHighlightFromEdge( TEdge edge );
	}
}