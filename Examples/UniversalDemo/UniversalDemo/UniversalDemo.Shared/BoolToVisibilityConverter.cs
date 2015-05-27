//-----------------------------------------------------------------------------
// <copyright file="BoolToVisibilityConverter.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace UniversalDemo
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Converts a <see cref="Boolean"/> value to a <see cref="Visibility"/> value.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolToVisibilityConverter"/> class.
        /// </summary>
        public BoolToVisibilityConverter()
        {
            this.DefaultValue = Visibility.Visible;
        }

        /// <summary>
        /// Gets or sets the default value when the value to be converted
        /// is not a <see cref="Boolean"/>.
        /// </summary>
        public Visibility DefaultValue { get; set; }

        /// <summary>Converts the specified value to a visibility.</summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns><see cref="Visibility.Visible"/> if value is <c>true</c>;
        /// <see cref="Visibility.Collapsed"/> if value is <c>false</c>;
        /// otherwise returns the value of <see cref="DefaultValue"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }

            return this.DefaultValue;
        }

        /// <summary>Back conversion is not supported.</summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Nothing is returned</returns>
        /// <exception cref="System.NotSupportedException">Back conversion is
        /// not supported.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
