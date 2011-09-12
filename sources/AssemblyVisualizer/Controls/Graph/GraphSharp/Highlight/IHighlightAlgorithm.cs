// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using AssemblyVisualizer.Controls.Graph.QuickGraph;

namespace AssemblyVisualizer.Controls.Graph.GraphSharp.Highlight
{
	public interface IHighlightAlgorithm<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
	{
		IHighlightParameters Parameters { get; }

		/// <summary>
		/// Reset the whole highlight.
		/// </summary>
		void ResetHighlight();

		bool OnVertexHighlighting( TVertex vertex );
		bool OnVertexHighlightRemoving( TVertex vertex );
		bool OnEdgeHighlighting( TEdge edge );
		bool OnEdgeHighlightRemoving( TEdge edge );

		bool IsParametersSettable(IHighlightParameters parameters);
		bool TrySetParameters(IHighlightParameters parameters);
	}
}