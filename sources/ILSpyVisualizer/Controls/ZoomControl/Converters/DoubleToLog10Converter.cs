// Copyright 2011 Denis Markelov
// Adopted, originally created as part of WPFExtensions library
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

using System.Windows.Data;
using System;

namespace ILSpyVisualizer.Controls.ZoomControl.Converters
{
    public class DoubleToLog10Converter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double val = (double) value;
            return Math.Log10(val);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double val = (double) value;
            return Math.Pow(10, val);
        }

        #endregion
    }
}
