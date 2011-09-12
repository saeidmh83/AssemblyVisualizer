// Adopted, originally created as part of GraphSharp project
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Controls;
using System;

namespace AssemblyVisualizer.Controls.Graph
{
    public interface ITransition
    {
        /// <summary>
        /// Runs the transition.
        /// </summary>
        /// <param name="context">The context of the transition.</param>
        /// <param name="control">The control which the transition should be run on.</param>
        /// <param name="duration">The duration of the transition.</param>
        void Run( IAnimationContext context, Control control, TimeSpan duration );

        /// <summary>
        /// Runs the transition.
        /// </summary>
        /// <param name="context">The context of the transition.</param>
        /// <param name="control">The control which the transition should be run on.</param>
        /// <param name="duration">The duration of the transition.</param>
        /// <param name="endMethod">The method that should be called when the transition finished.</param>
        void Run( IAnimationContext context, Control control, TimeSpan duration, Action<Control> endMethod );
    }
}
