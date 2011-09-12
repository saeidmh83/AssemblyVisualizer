// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using AssemblyVisualizer.Controls.Graph.QuickGraph;

namespace AssemblyVisualizer.Controls.Graph.GraphSharp.Highlight
{
	public class HighlightContext<TVertex, TEdge, TGraph> : IHighlightContext<TVertex, TEdge, TGraph>
		where TVertex : class 
		where TEdge : IEdge<TVertex>
		where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
	{
		public TGraph Graph
		{
			get; private set;
		}

		public HighlightContext(TGraph graph)
		{
			this.Graph = graph;
		}
	}
}