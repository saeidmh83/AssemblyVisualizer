// Adopted, originally created as part of GraphSharp project
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows;

namespace ILSpyVisualizer.Controls.Graph
{
	interface ICompoundVertexControl
	{
		/// <summary>
		/// Gets the 'borderthickness' of the control around the inner canvas.
		/// </summary>
		Thickness VertexBorderThickness { get; }

		/// <summary>
		/// Gets the size of the inner canvas.
		/// </summary>
		Size InnerCanvasSize { get; }

		event RoutedEventHandler Expanded;
		event RoutedEventHandler Collapsed;
	}
}
