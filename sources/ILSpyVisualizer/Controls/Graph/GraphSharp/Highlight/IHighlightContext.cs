// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using ILSpyVisualizer.Controls.Graph.QuickGraph;

namespace ILSpyVisualizer.Controls.Graph.GraphSharp.Highlight
{
	public interface IHighlightContext<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
	{
		TGraph Graph { get; }
	}
}