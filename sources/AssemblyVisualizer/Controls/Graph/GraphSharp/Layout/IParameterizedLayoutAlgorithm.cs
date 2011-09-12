// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

namespace AssemblyVisualizer.Controls.Graph.GraphSharp.Layout
{
	public interface IParameterizedLayoutAlgorithm
	{
		ILayoutParameters GetParameters();
	}

	public interface IParameterizedLayoutAlgorithm<TParam> : IParameterizedLayoutAlgorithm
		where TParam : ILayoutParameters
	{
		TParam Parameters { get; }
	}
}