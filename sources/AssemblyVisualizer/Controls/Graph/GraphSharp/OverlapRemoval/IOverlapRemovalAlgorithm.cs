// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using System.Windows;

namespace AssemblyVisualizer.Controls.Graph.GraphSharp.OverlapRemoval
{
	public interface IOverlapRemovalAlgorithm<TObject> : IAlgorithm
	{
		IDictionary<TObject, Rect> Rectangles { get; }

		IOverlapRemovalParameters GetParameters();
	}

	public interface IOverlapRemovalAlgorithm<TObject, TParam> : IOverlapRemovalAlgorithm<TObject>
		where TParam : IOverlapRemovalParameters
	{
		TParam Parameters { get; }
	}
}