using System.Text.Json.Serialization;

namespace OshchepkovTaskManager.Core.Models
{
    public enum TaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    public enum TaskStatus
    {
        New = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }

    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

        public TaskStatus Status { get; set; } = TaskStatus.New;

        public bool IsImportant { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public bool IsOverdue =>
            Status != TaskStatus.Completed &&
            Status != TaskStatus.Cancelled &&
            DueDate.Date < DateTime.Today;

        public override string ToString() =>
            $"[{Status}] {Title} (Приоритет: {Priority}, Срок: {DueDate:dd.MM.yyyy})";
    }
}
