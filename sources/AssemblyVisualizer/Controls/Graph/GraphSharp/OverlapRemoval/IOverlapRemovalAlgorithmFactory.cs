// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;

namespace ILSpyVisualizer.Controls.Graph.GraphSharp.OverlapRemoval
{
	public interface IOverlapRemovalAlgorithmFactory<TVertex>
		where TVertex : class
	{
		/// <summary>
		/// List of the available algorithms.
		/// </summary>
		IEnumerable<string> AlgorithmTypes { get; }

		IOverlapRemovalAlgorithm<TVertex> CreateAlgorithm( string newAlgorithmType, IOverlapRemovalContext<TVertex> context, IOverlapRemovalParameters parameters );

		IOverlapRemovalParameters CreateParameters( string algorithmType, IOverlapRemovalParameters oldParameters );

		bool IsValidAlgorithm( string algorithmType );

		string GetAlgorithmType( IOverlapRemovalAlgorithm<TVertex> algorithm );
	}
}