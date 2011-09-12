// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using System.Linq;
using AssemblyVisualizer.Controls.Graph.GraphSharp.Layout;
using AssemblyVisualizer.Controls.Graph.QuickGraph;

namespace AssemblyVisualizer.Controls.Graph.GraphSharp.EdgeRouting
{
	public class StandardEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph> : IEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
	{
		protected static readonly string[] algorithmTypes = new string[] { };
		public IEnumerable<string> AlgorithmTypes
		{
			get { return algorithmTypes; }
		}

		public IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> CreateAlgorithm( string newAlgorithmType, ILayoutContext<TVertex, TEdge, TGraph> context, IEdgeRoutingParameters parameters )
		{
			return null;
		}

		public IEdgeRoutingParameters CreateParameters( string algorithmType, IEdgeRoutingParameters oldParameters )
		{
			return null;
		}

		public bool IsValidAlgorithm( string algorithmType )
		{
			return AlgorithmTypes.Any( at => at == algorithmType );
		}

		public string GetAlgorithmType( IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> algorithm )
		{
			return string.Empty;
		}

	}
}