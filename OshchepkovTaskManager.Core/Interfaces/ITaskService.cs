using OshchepkovTaskManager.Core.Models;
using TaskStatus = OshchepkovTaskManager.Core.Models.TaskStatus;

namespace OshchepkovTaskManager.Core.Interfaces
{
    public interface ITaskService
    {
        IReadOnlyList<TaskItem> GetAllTasks();
        TaskItem? GetTaskById(Guid id);
        void AddTask(TaskItem task);
        void UpdateTask(TaskItem task);
        void DeleteTask(Guid id);

        // Фильтрация и поиск
        IReadOnlyList<TaskItem> FilterByStatus(TaskStatus status);
        IReadOnlyList<TaskItem> Search(string query);

        // Сортировка
        IReadOnlyList<TaskItem> SortByPriority(bool descending = true);
        IReadOnlyList<TaskItem> SortByDueDate(bool descending = false);

        // Файловый ввод-вывод
        void SaveToFile(string filePath);
        void LoadFromFile(string filePath);

        // Статистика
        TaskStatistics GetStatistics();
    }
}
