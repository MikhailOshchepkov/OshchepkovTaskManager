using OshchepkovTaskManager.Core.Models;
using TaskStatus = OshchepkovTaskManager.Core.Models.TaskStatus;

namespace OshchepkovTaskManager.App.ViewModels
{

    public class TaskEditViewModel : ViewModelBase
    {
        private string _title       = string.Empty;
        private string _description = string.Empty;
        private TaskPriority _priority = TaskPriority.Medium;
        private DateTime _dueDate   = DateTime.Today.AddDays(7);
        private TaskStatus _status  = TaskStatus.New;
        private bool _isImportant;

        public TaskEditViewModel(TaskItem? task)
        {
            if (task != null)
            {
                _title       = task.Title;
                _description = task.Description;
                _priority    = task.Priority;
                _dueDate     = task.DueDate;
                _status      = task.Status;
                _isImportant = task.IsImportant;
                IsEdit       = true;
            }
        }

        public bool IsEdit { get; }

        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        public TaskPriority Priority
        {
            get => _priority;
            set => SetField(ref _priority, value);
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set => SetField(ref _dueDate, value);
        }

        public TaskStatus Status
        {
            get => _status;
            set => SetField(ref _status, value);
        }

        public bool IsImportant
        {
            get => _isImportant;
            set => SetField(ref _isImportant, value);
        }

        public IEnumerable<TaskPriority> Priorities { get; } =
            Enum.GetValues<TaskPriority>();

        public IEnumerable<TaskStatus> Statuses { get; } =
            Enum.GetValues<TaskStatus>();

        public bool IsValid => !string.IsNullOrWhiteSpace(Title);

        public TaskItem ToTaskItem() => new()
        {
            Title       = Title.Trim(),
            Description = Description.Trim(),
            Priority    = Priority,
            DueDate     = DueDate,
            Status      = Status,
            IsImportant = IsImportant
        };

        public void ApplyTo(TaskItem task)
        {
            task.Title       = Title.Trim();
            task.Description = Description.Trim();
            task.Priority    = Priority;
            task.DueDate     = DueDate;
            task.Status      = Status;
            task.IsImportant = IsImportant;
        }
    }
}
