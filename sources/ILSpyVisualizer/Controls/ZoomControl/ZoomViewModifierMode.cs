// Copyright 2011 Denis Markelov
// Adopted, originally created as part of WPFExtensions library
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

namespace ILSpyVisualizer.Controls.ZoomControl
{
    public enum ZoomViewModifierMode
    {
        /// <summary>
        /// It does nothing at all.
        /// </summary>
        None,

        /// <summary>
        /// You can pan the view with the mouse in this mode.
        /// </summary>
        Pan,

        /// <summary>
        /// You can zoom in with the mouse in this mode.
        /// </summary>
        ZoomIn, 

        /// <summary>
        /// You can zoom out with the mouse in this mode.
        /// </summary>
        ZoomOut,

        /// <summary>
        /// Zooming after the user has been selected the zooming box.
        /// </summary>
        ZoomBox
    }
}
