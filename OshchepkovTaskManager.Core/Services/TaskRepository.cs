using System.Text.Json;
using OshchepkovTaskManager.Core.Interfaces;
using OshchepkovTaskManager.Core.Models;

namespace OshchepkovTaskManager.Core.Services
{
    public class TaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _tasks = new();

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        public IReadOnlyList<TaskItem> GetAll() => _tasks.AsReadOnly();

        public TaskItem? GetById(Guid id) =>
            _tasks.FirstOrDefault(t => t.Id == id);

        public void Add(TaskItem task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Название задачи не может быть пустым.", nameof(task));
            _tasks.Add(task);
        }

        public void Update(TaskItem task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            var index = _tasks.FindIndex(t => t.Id == task.Id);
            if (index == -1)
                throw new KeyNotFoundException($"Задача с Id={task.Id} не найдена.");
            _tasks[index] = task;
        }

        public void Delete(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
                throw new KeyNotFoundException($"Задача с Id={id} не найдена.");
            _tasks.Remove(task);
        }

        public void SaveToFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));

            var json = JsonSerializer.Serialize(_tasks, _jsonOptions);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.", filePath);

            var json = File.ReadAllText(filePath);
            var loaded = JsonSerializer.Deserialize<List<TaskItem>>(json, _jsonOptions);

            _tasks.Clear();
            if (loaded != null)
                _tasks.AddRange(loaded);
        }
    }
}
