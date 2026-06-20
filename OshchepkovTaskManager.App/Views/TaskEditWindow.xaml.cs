using System.Windows;
using OshchepkovTaskManager.App.ViewModels;

namespace OshchepkovTaskManager.App.Views
{
    public partial class TaskEditWindow : Window
    {
        private readonly TaskEditViewModel _vm;

        public TaskEditWindow(TaskEditViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = vm;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_vm.IsValid)
            {
                MessageBox.Show("Пожалуйста, введите название задачи.",
                                "Ошибка валидации",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
        }
    }
}
