using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using OshchepkovTaskManager.Core.Models;
using TaskStatus = OshchepkovTaskManager.Core.Models.TaskStatus;

namespace OshchepkovTaskManager.App.Converters
{
    public class TaskRowColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => Brushes.Transparent;

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ImportanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? "★" : string.Empty;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ImportanceColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true
                ? new SolidColorBrush(Color.FromRgb(224, 160, 64))
                : Brushes.Transparent;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class StatusDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is TaskStatus s ? s switch
            {
                TaskStatus.New        => "Новая",
                TaskStatus.InProgress => "В процессе",
                TaskStatus.Completed  => "Завершена",
                TaskStatus.Cancelled  => "Отменена",
                _                     => s.ToString()
            } : value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class StatusBadgeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is TaskStatus s ? s switch
            {
                TaskStatus.New        => new SolidColorBrush(Color.FromRgb(50, 50, 96)),
                TaskStatus.InProgress => new SolidColorBrush(Color.FromRgb(80, 56, 16)),
                TaskStatus.Completed  => new SolidColorBrush(Color.FromRgb(22, 64, 42)),
                TaskStatus.Cancelled  => new SolidColorBrush(Color.FromRgb(48, 48, 58)),
                _                     => new SolidColorBrush(Color.FromRgb(50, 50, 60))
            } : Brushes.Transparent;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class PriorityDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is TaskPriority p ? p switch
            {
                TaskPriority.Low      => "Низкий",
                TaskPriority.Medium   => "Средний",
                TaskPriority.High     => "Высокий",
                TaskPriority.Critical => "Критический",
                _                     => p.ToString()
            } : value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class PriorityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is TaskPriority p ? p switch
            {
                TaskPriority.Critical => new SolidColorBrush(Color.FromRgb(90, 28, 28)),
                TaskPriority.High     => new SolidColorBrush(Color.FromRgb(88, 44, 12)),
                TaskPriority.Medium   => new SolidColorBrush(Color.FromRgb(36, 36, 90)),
                TaskPriority.Low      => new SolidColorBrush(Color.FromRgb(40, 40, 52)),
                _                     => new SolidColorBrush(Color.FromRgb(50, 50, 60))
            } : Brushes.Gray;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value != null ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
