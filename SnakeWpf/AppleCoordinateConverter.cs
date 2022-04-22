﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SnakeWpf
{
    public class AppleCoordinateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double coordinate && values[1] is double snakeWidth)
            {
                return coordinate - snakeWidth / 2;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
