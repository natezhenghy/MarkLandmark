using System;
using System.Globalization;
using System.Windows.Data;

namespace MarkLandmark
{
    class LandmarkNameToVisualStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Landmark.LandmarkName name = (Landmark.LandmarkName)value;
            string localizedMessage = (string)App.Current.FindResource("Landmark_" + name);

            return localizedMessage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
