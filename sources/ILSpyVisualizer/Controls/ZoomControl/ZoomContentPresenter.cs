// Copyright 2011 Denis Markelov
// Adopted, originally created as part of WPFExtensions library
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ILSpyVisualizer.Controls.ZoomControl
{
    public class ZoomContentPresenter : ContentPresenter
    {
        public event ContentSizeChangedHandler ContentSizeChanged;

        private Size _contentSize;

        public Size ContentSize
        {
            get { return _contentSize; }
            private set {
                if (value == _contentSize)
                    return;

                _contentSize = value;
                if (ContentSizeChanged != null)
                    ContentSizeChanged(this, _contentSize);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var max = 1000000000;
            var x = double.IsInfinity(constraint.Width) ? max : constraint.Width;
            var y = double.IsInfinity(constraint.Height) ? max : constraint.Height;
            return new Size(x, y);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            UIElement child = VisualChildrenCount > 0
                                  ? VisualTreeHelper.GetChild(this, 0) as UIElement
                                  : null;
            if (child == null)
                return arrangeBounds;

            //set the ContentSize
            ContentSize = child.DesiredSize;
            child.Arrange(new Rect(child.DesiredSize));

            return arrangeBounds;
        }
    }
}
