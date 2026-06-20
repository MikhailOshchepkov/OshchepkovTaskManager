using OshchepkovTaskManager.Core.Interfaces;
using OshchepkovTaskManager.Core.Models;
using TaskStatus = OshchepkovTaskManager.Core.Models.TaskStatus;

namespace OshchepkovTaskManager.Core.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IReadOnlyList<TaskItem> GetAllTasks() => _repository.GetAll();

        public TaskItem? GetTaskById(Guid id) => _repository.GetById(id);

        public void AddTask(TaskItem task) => _repository.Add(task);

        public void UpdateTask(TaskItem task) => _repository.Update(task);

        public void DeleteTask(Guid id) => _repository.Delete(id);


        public IReadOnlyList<TaskItem> FilterByStatus(TaskStatus status) =>
            _repository.GetAll()
                       .Where(t => t.Status == status)
                       .ToList()
                       .AsReadOnly();

        public IReadOnlyList<TaskItem> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return _repository.GetAll();

            var q = query.Trim().ToLowerInvariant();

            return _repository.GetAll()
                              .Where(t => t.Title.ToLowerInvariant().Contains(q) ||
                                          t.Description.ToLowerInvariant().Contains(q))
                              .ToList()
                              .AsReadOnly();
        }


        public IReadOnlyList<TaskItem> SortByPriority(bool descending = true)
        {
            var query = _repository.GetAll().AsEnumerable();
            return (descending
                ? query.OrderByDescending(t => t.Priority)
                : query.OrderBy(t => t.Priority))
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<TaskItem> SortByDueDate(bool descending = false)
        {
            var query = _repository.GetAll().AsEnumerable();
            return (descending
                ? query.OrderByDescending(t => t.DueDate)
                : query.OrderBy(t => t.DueDate))
                .ToList()
                .AsReadOnly();
        }


        public void SaveToFile(string filePath) => _repository.SaveToFile(filePath);

        public void LoadFromFile(string filePath) => _repository.LoadFromFile(filePath);


        public TaskStatistics GetStatistics()
        {
            var all = _repository.GetAll();
            return new TaskStatistics
            {
                Total      = all.Count,
                New        = all.Count(t => t.Status == TaskStatus.New),
                InProgress = all.Count(t => t.Status == TaskStatus.InProgress),
                Completed  = all.Count(t => t.Status == TaskStatus.Completed),
                Cancelled  = all.Count(t => t.Status == TaskStatus.Cancelled),
                Overdue    = all.Count(t => t.IsOverdue),
                Important  = all.Count(t => t.IsImportant)
            };
        }
    }
}
