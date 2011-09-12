// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

namespace AssemblyVisualizer.Controls.Graph.GraphSharp.OverlapRemoval
{
	public interface IOverlapRemovalParameters : IAlgorithmParameters
	{
		float VerticalGap { get; }
		float HorizontalGap { get; }
	}
}