using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BCS.CADs.Synchronization.Utility
{
    public class ItemPickerConveter : IValueConverter
    {




        #region IValueConverter Members

        /// <summary>
        /// Implement the ConvertBack method of IValueConverter. Converts DateTime object to specified format
        /// </summary>
        /// <param name="value">The DateTime value we're converting</param>
        /// <param name="targetType">Not used</param>
        /// <param name="parameter">String format to convert to (optional)</param>
        /// <param name="culture">Not used</param>
        /// <returns>Collapsed if value is true, else Visible</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            dynamic instance = Activator.CreateInstance(value.GetType());
            instance = value;
            string name = instance.Label;
            return name;
        }

        /// <summary>
        /// Implement the Convert method of IValueConverter. Converts a string representation of a date to DateTime
        /// </summary>
        /// <param name="value">The visibility value to convert to a boolean.</param>
        /// <param name="targetType">Not used</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Not used</param>
        /// <returns>false if Visible, else true</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string txtString = (string)value;
            ListViewItem lvItem = new ListViewItem();

            if (!String.IsNullOrEmpty(txtString))
            {
                lvItem.Content = txtString;
                return lvItem;
            }

            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}
