using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ProjectAlpha.Converters
{
    public class BoolToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value != null ? parameter.ToString() == "0" ? value : !(bool)value : parameter.ToString() == "0" ? false : true;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => value != null ? parameter.ToString() == "0" ? value : !(bool)value : parameter.ToString() == "0" ? false : true;
    }

    public class ObjToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => (value == null) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => (bool)value ? parameter.ToString() == "0" ? Visibility.Visible : Visibility.Collapsed : parameter.ToString() == "0" ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusCodeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => value.ToString() == parameter.ToString() ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ObjToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => (value == null) ? false : true;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class LocationMode_IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => value.ToString() == parameter.ToString() ? true : false;

        public object ConvertBack(object value, Type targetType, object parameter, string language) => (bool)value ? int.Parse(parameter.ToString()) : -1;
    }
}
