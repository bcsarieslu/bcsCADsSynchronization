using System;
using System.Windows;
using System.Windows.Data;

namespace BCS.CADs.Synchronization.Utility
{
    /// <summary>
    /// Value converter to a string to a double value
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Implement the Convert method of IValueConverter. Convert a string to a double value.
        /// </summary>
        /// <param name="value">The string value we're testing</param>
        /// <param name="targetType">Not used</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Not used</param>
        /// <returns>Collapsed if value is true, else Visible</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double dValue = 0.0; // initial value
            string sValue = value as string;

            if (!string.IsNullOrEmpty(sValue))
                double.TryParse(sValue, out dValue);

            return dValue;
        }

        /// <summary>
        /// Implement the ConvertBack method of IValueConverter. Converts a double value to string.
        /// </summary>
        /// <param name="value">The double value to convert to a string.</param>
        /// <param name="targetType">Not used</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Not used</param>
        /// <returns>false if Visible, else true</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string calculatorString = (string)value;
            double calculatorValue;

            if (Double.TryParse(calculatorString, out calculatorValue))
                return calculatorValue;

            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}
