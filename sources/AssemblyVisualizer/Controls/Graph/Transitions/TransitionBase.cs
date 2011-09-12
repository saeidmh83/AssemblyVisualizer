// Adopted, originally created as part of GraphSharp project
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;

namespace AssemblyVisualizer.Controls.Graph.Transitions
{
    public abstract class TransitionBase : ITransition
    {
        #region ITransition Members

        public void Run( IAnimationContext context, System.Windows.Controls.Control control, TimeSpan duration )
        {
            Run( context, control, duration, null );
        }

        public abstract void Run( IAnimationContext context,
            System.Windows.Controls.Control control,
            TimeSpan duration,
            Action<System.Windows.Controls.Control> endMethod );

        #endregion
    }
}
