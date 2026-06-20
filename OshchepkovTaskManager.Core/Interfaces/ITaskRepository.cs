using OshchepkovTaskManager.Core.Models;

namespace OshchepkovTaskManager.Core.Interfaces
{
    public interface ITaskRepository
    {
        IReadOnlyList<TaskItem> GetAll();
        TaskItem? GetById(Guid id);
        void Add(TaskItem task);
        void Update(TaskItem task);
        void Delete(Guid id);
        void SaveToFile(string filePath);
        void LoadFromFile(string filePath);
    }
}
