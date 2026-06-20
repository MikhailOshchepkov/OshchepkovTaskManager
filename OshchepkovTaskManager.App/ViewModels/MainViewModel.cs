using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using OshchepkovTaskManager.Core.Interfaces;
using OshchepkovTaskManager.Core.Models;
using OshchepkovTaskManager.Core.Services;
using TaskStatus = OshchepkovTaskManager.Core.Models.TaskStatus;

namespace OshchepkovTaskManager.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ITaskService _service;


        private ObservableCollection<TaskItem> _tasks = new();
        private TaskItem? _selectedTask;
        private string _searchQuery = string.Empty;
        private string _filterStatus = "Все";
        private string _sortMode = "По умолчанию";
        private string _statusBarText = "Готово";

        private int _statTotal, _statNew, _statInProgress, _statCompleted, _statOverdue;

        public MainViewModel()
        {
            _service = new TaskService(new TaskRepository());
            InitCommands();
            LoadDemoTasks();
            RefreshList();
        }

        private void LoadDemoTasks()
        {
            _service.AddTask(new TaskItem
            {
                Title       = "Подготовить отчёт по практике",
                Description = "Написать сопроводительную записку, диаграмму классов и описание ПО",
                Priority    = TaskPriority.Critical,
                Status      = TaskStatus.InProgress,
                DueDate     = DateTime.Today.AddDays(3),
                IsImportant = true
            });
            _service.AddTask(new TaskItem
            {
                Title       = "Реализовать модульные тесты",
                Description = "Покрыть тестами TaskRepository и TaskService",
                Priority    = TaskPriority.High,
                Status      = TaskStatus.Completed,
                DueDate     = DateTime.Today.AddDays(-1),
                IsImportant = false
            });
            _service.AddTask(new TaskItem
            {
                Title       = "Загрузить проект на GitHub",
                Description = "Создать репозиторий, сделать коммит, отправить ссылку преподавателю",
                Priority    = TaskPriority.High,
                Status      = TaskStatus.New,
                DueDate     = DateTime.Today.AddDays(5),
                IsImportant = true
            });
            _service.AddTask(new TaskItem
            {
                Title       = "Изучить паттерн MVVM",
                Description = "Разобраться с привязками данных, командами и INotifyPropertyChanged",
                Priority    = TaskPriority.Medium,
                Status      = TaskStatus.Completed,
                DueDate     = DateTime.Today.AddDays(-5),
                IsImportant = false
            });
            _service.AddTask(new TaskItem
            {
                Title       = "Настроить сериализацию JSON",
                Description = "Проверить корректное сохранение и загрузку задач из файла",
                Priority    = TaskPriority.Medium,
                Status      = TaskStatus.New,
                DueDate     = DateTime.Today.AddDays(7),
                IsImportant = false
            });
            _service.AddTask(new TaskItem
            {
                Title       = "Просроченная задача",
                Description = "Эта задача не была выполнена вовремя — выделена красным",
                Priority    = TaskPriority.Low,
                Status      = TaskStatus.New,
                DueDate     = DateTime.Today.AddDays(-3),
                IsImportant = false
            });
        }


        public ObservableCollection<TaskItem> Tasks
        {
            get => _tasks;
            set => SetField(ref _tasks, value);
        }

        public TaskItem? SelectedTask
        {
            get => _selectedTask;
            set => SetField(ref _selectedTask, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set { SetField(ref _searchQuery, value); RefreshList(); }
        }

        public string FilterStatus
        {
            get => _filterStatus;
            set { SetField(ref _filterStatus, value); RefreshList(); }
        }

        public string SortMode
        {
            get => _sortMode;
            set { SetField(ref _sortMode, value); RefreshList(); }
        }

        public string StatusBarText
        {
            get => _statusBarText;
            set => SetField(ref _statusBarText, value);
        }

        public int StatTotal       { get => _statTotal;       set => SetField(ref _statTotal, value); }
        public int StatNew         { get => _statNew;         set => SetField(ref _statNew, value); }
        public int StatInProgress  { get => _statInProgress;  set => SetField(ref _statInProgress, value); }
        public int StatCompleted   { get => _statCompleted;   set => SetField(ref _statCompleted, value); }
        public int StatOverdue     { get => _statOverdue;     set => SetField(ref _statOverdue, value); }

        public IEnumerable<string> FilterOptions { get; } =
            new[] { "Все", "Новая", "В процессе", "Завершена", "Отменена" };

        public IEnumerable<string> SortOptions { get; } =
            new[] { "По умолчанию", "По приоритету ↓", "По приоритету ↑", "По сроку ↑", "По сроку ↓" };

        public ICommand AddTaskCommand     { get; private set; } = null!;
        public ICommand EditTaskCommand    { get; private set; } = null!;
        public ICommand DeleteTaskCommand  { get; private set; } = null!;
        public ICommand SaveFileCommand    { get; private set; } = null!;
        public ICommand LoadFileCommand    { get; private set; } = null!;
        public ICommand ClearSearchCommand { get; private set; } = null!;
        public ICommand SelectTaskCommand  { get; private set; } = null!;

        private void InitCommands()
        {
            AddTaskCommand = new RelayCommand(_ => OpenAddDialog());

            SelectTaskCommand = new RelayCommand(p =>
            {
                if (p is TaskItem t) SelectedTask = t;
            });

            EditTaskCommand = new RelayCommand(
                _ => OpenEditDialog(),
                _ => SelectedTask != null);

            DeleteTaskCommand = new RelayCommand(
                _ => DeleteSelectedTask(),
                _ => SelectedTask != null);

            SaveFileCommand  = new RelayCommand(_ => SaveToFile());
            LoadFileCommand  = new RelayCommand(_ => LoadFromFile());

            ClearSearchCommand = new RelayCommand(_ =>
            {
                SearchQuery = string.Empty;
                FilterStatus = "Все";
                SortMode = "По умолчанию";
            });
        }

        private void OpenAddDialog()
        {
            var vm = new TaskEditViewModel(null);
            var dlg = new Views.TaskEditWindow(vm) { Owner = Application.Current.MainWindow };
            if (dlg.ShowDialog() == true)
            {
                _service.AddTask(vm.ToTaskItem());
                RefreshList();
                SetStatus("Задача добавлена.");
            }
        }

        private void OpenEditDialog()
        {
            if (SelectedTask == null) return;
            var vm = new TaskEditViewModel(SelectedTask);
            var dlg = new Views.TaskEditWindow(vm) { Owner = Application.Current.MainWindow };
            if (dlg.ShowDialog() == true)
            {
                vm.ApplyTo(SelectedTask);
                _service.UpdateTask(SelectedTask);
                RefreshList();
                SetStatus("Задача обновлена.");
            }
        }

        private void DeleteSelectedTask()
        {
            if (SelectedTask == null) return;
            var result = MessageBox.Show(
                $"Удалить задачу «{SelectedTask.Title}»?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _service.DeleteTask(SelectedTask.Id);
                SelectedTask = null;
                RefreshList();
                SetStatus("Задача удалена.");
            }
        }

        private void SaveToFile()
        {
            var dlg = new SaveFileDialog
            {
                Title      = "Сохранить задачи",
                Filter     = "JSON файлы (*.json)|*.json",
                DefaultExt = "json",
                FileName   = "tasks"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    _service.SaveToFile(dlg.FileName);
                    SetStatus($"Сохранено: {dlg.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadFromFile()
        {
            var dlg = new OpenFileDialog
            {
                Title  = "Загрузить задачи",
                Filter = "JSON файлы (*.json)|*.json"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    _service.LoadFromFile(dlg.FileName);
                    RefreshList();
                    SetStatus($"Загружено: {dlg.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void RefreshList()
        {
            IReadOnlyList<TaskItem> items;

            // 1. Поиск
            if (!string.IsNullOrWhiteSpace(SearchQuery))
                items = _service.Search(SearchQuery);
            else
                items = _service.GetAllTasks();

            // 2. Фильтр по статусу
            if (FilterStatus != "Все")
            {
                var status = FilterStatus switch
                {
                    "Новая"      => TaskStatus.New,
                    "В процессе" => TaskStatus.InProgress,
                    "Завершена"  => TaskStatus.Completed,
                    "Отменена"   => TaskStatus.Cancelled,
                    _            => (TaskStatus?)null
                };
                if (status.HasValue)
                    items = items.Where(t => t.Status == status.Value).ToList();
            }

            // 3. Сортировка
            items = SortMode switch
            {
                "По приоритету ↓" => items.OrderByDescending(t => t.Priority).ToList(),
                "По приоритету ↑" => items.OrderBy(t => t.Priority).ToList(),
                "По сроку ↑"      => items.OrderBy(t => t.DueDate).ToList(),
                "По сроку ↓"      => items.OrderByDescending(t => t.DueDate).ToList(),
                _                 => items
            };

            Tasks = new ObservableCollection<TaskItem>(items);

            // Статистика
            var stat = _service.GetStatistics();
            StatTotal      = stat.Total;
            StatNew        = stat.New;
            StatInProgress = stat.InProgress;
            StatCompleted  = stat.Completed;
            StatOverdue    = stat.Overdue;
        }

        private void SetStatus(string message)
        {
            StatusBarText = $"{message}  |  {DateTime.Now:HH:mm:ss}";
        }
    }
}
