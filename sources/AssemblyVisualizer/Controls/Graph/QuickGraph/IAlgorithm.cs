// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Diagnostics.Contracts;
using AssemblyVisualizer.Controls.Graph.QuickGraph.Contracts;
//using QuickGraph.Algorithms.Services;

namespace AssemblyVisualizer.Controls.Graph.QuickGraph
{
   [ContractClass(typeof(IAlgorithmContract<>))]
    public interface IAlgorithm<TGraph> :
        IComputation
    {
        TGraph VisitedGraph { get;}
    }
}
