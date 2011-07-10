// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using ILSpyVisualizer.Controls.Graph.QuickGraph;

namespace ILSpyVisualizer.Controls.Graph.GraphSharp
{
	/// <summary>
	/// Simple algorithm interface which is not connected to any graph.
	/// </summary>
	public interface IAlgorithm
	{
		object SyncRoot { get;}
		ComputationState State { get;}

		void Compute();
		void Abort();

		event EventHandler StateChanged;
		event EventHandler Started;
		event EventHandler Finished;
		event EventHandler Aborted;
	}
}