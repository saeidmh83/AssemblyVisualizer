// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Diagnostics.Contracts;

namespace AssemblyVisualizer.Controls.Graph.QuickGraph.Contracts
{
    [ContractClassFor(typeof(IAlgorithm<>))]
    abstract class IAlgorithmContract<TGraph>
        : IAlgorithm<TGraph>
    {
        #region IAlgorithm<TGraph> Members

        TGraph IAlgorithm<TGraph>.VisitedGraph
        {
            get 
            {
                Contract.Ensures(Contract.Result<TGraph>() != null);

                return default(TGraph);
            }
        }

        #endregion

        #region IComputation Members

        object IComputation.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        ComputationState IComputation.State
        {
            get { throw new NotImplementedException(); }
        }

        void IComputation.Compute()
        {
            throw new NotImplementedException();
        }

        void IComputation.Abort()
        {
            throw new NotImplementedException();
        }

        event EventHandler IComputation.StateChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler IComputation.Started
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler IComputation.Finished
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler IComputation.Aborted
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        #endregion
    }
}