using OshchepkovTaskManager.Core.Models;
using TaskStatus = OshchepkovTaskManager.Core.Models.TaskStatus;
using OshchepkovTaskManager.Core.Services;
using Xunit;

namespace OshchepkovTaskManager.Tests
{
    public class TaskServiceTests
    {
        private static TaskService CreateService()
        {
            var repo = new TaskRepository();
            return new TaskService(repo);
        }

        private static TaskItem MakeTask(string title,
            TaskStatus status = TaskStatus.New,
            TaskPriority priority = TaskPriority.Medium,
            int dueDaysFromNow = 5,
            bool important = false) => new()
        {
            Title       = title,
            Description = $"Описание: {title}",
            Status      = status,
            Priority    = priority,
            DueDate     = DateTime.Today.AddDays(dueDaysFromNow),
            IsImportant = important
        };


        [Fact]
        public void Constructor_NullRepository_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TaskService(null!));
        }


        [Fact]
        public void AddAndGet_ReturnsTask()
        {
            var svc = CreateService();
            var t = MakeTask("Задача 1");
            svc.AddTask(t);
            Assert.Single(svc.GetAllTasks());
            Assert.Equal(t.Id, svc.GetTaskById(t.Id)!.Id);
        }

        [Fact]
        public void DeleteTask_RemovesIt()
        {
            var svc = CreateService();
            var t = MakeTask("Удалить");
            svc.AddTask(t);
            svc.DeleteTask(t.Id);
            Assert.Empty(svc.GetAllTasks());
        }

        [Fact]
        public void UpdateTask_ChangesTitle()
        {
            var svc = CreateService();
            var t = MakeTask("Старый");
            svc.AddTask(t);
            t.Title = "Новый";
            svc.UpdateTask(t);
            Assert.Equal("Новый", svc.GetTaskById(t.Id)!.Title);
        }

        [Fact]
        public void FilterByStatus_ReturnsOnlyMatchingTasks()
        {
            var svc = CreateService();
            svc.AddTask(MakeTask("A", TaskStatus.New));
            svc.AddTask(MakeTask("B", TaskStatus.InProgress));
            svc.AddTask(MakeTask("C", TaskStatus.New));

            var result = svc.FilterByStatus(TaskStatus.New);
            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal(TaskStatus.New, t.Status));
        }

        [Fact]
        public void FilterByStatus_NoMatches_ReturnsEmpty()
        {
            var svc = CreateService();
            svc.AddTask(MakeTask("A", TaskStatus.New));
            Assert.Empty(svc.FilterByStatus(TaskStatus.Completed));
        }


        [Fact]
        public void Search_ByTitle_FindsTask()
        {
            var svc = CreateService();
            svc.AddTask(MakeTask("Купить продукты"));
            svc.AddTask(MakeTask("Сделать отчёт"));

            var result = svc.Search("купить");
            Assert.Single(result);
            Assert.Contains("Купить", result[0].Title);
        }

        [Fact]
        public void Search_ByDescription_FindsTask()
        {
            var svc = CreateService();
            svc.AddTask(MakeTask("Задача X"));
            svc.AddTask(MakeTask("Задача Y"));

            var result = svc.Search("Задача Y");
            Assert.Single(result);
        }

        [Fact]
        public void Search_EmptyQuery_ReturnsAll()
        {
            var svc = CreateService();
            svc.AddTask(MakeTask("A"));
            svc.AddTask(MakeTask("B"));
            Assert.Equal(2, svc.Search("").Count);
        }

        [Fact]
        public void Search_CaseInsensitive()
        {
            var svc = CreateService();
            svc.AddTask(MakeTask("ВАЖНАЯ задача"));
            Assert.Single(svc.Search("важная"));
        }


        [Fact]
        public void SortByPriority_Descending_HighestFirst()
        {
            var svc = CreateService();
            svc.AddTask(MakeTask("Low",      priority: TaskPriority.Low));
            svc.AddTask(MakeTask("Critical", priority: TaskPriority.Critical));
            svc.AddTask(MakeTask("Medium",   priority: TaskPriority.Medium));

            var sorted = svc.SortByPriority(descending: true);
            Assert.Equal(TaskPriority.Critical, sorted[0].Priority);
            Assert.Equal(TaskPriority.Low,      sorted[2].Priority);
        }

        [Fact]
        public void SortByPriority_Ascending_LowestFirst()
        {
            var svc = CreateService();
            svc.AddTask(MakeTask("High",   priority: TaskPriority.High));
            svc.AddTask(MakeTask("Low",    priority: TaskPriority.Low));

            var sorted = svc.SortByPriority(descending: false);
            Assert.Equal(TaskPriority.Low, sorted[0].Priority);
        }


        [Fact]
        public void SortByDueDate_Ascending_EarliestFirst()
        {
            var svc = CreateService();
            svc.AddTask(MakeTask("Late",  dueDaysFromNow: 10));
            svc.AddTask(MakeTask("Early", dueDaysFromNow: 1));

            var sorted = svc.SortByDueDate(descending: false);
            Assert.Equal("Early", sorted[0].Title);
        }

        [Fact]
        public void GetStatistics_EmptyRepo_AllZero()
        {
            var svc = CreateService();
            var s = svc.GetStatistics();
            Assert.Equal(0, s.Total);
            Assert.Equal(0, s.Overdue);
        }

        [Fact]
        public void GetStatistics_CountsCorrectly()
        {
            var svc = CreateService();
            svc.AddTask(MakeTask("New1",       TaskStatus.New));
            svc.AddTask(MakeTask("InProg",     TaskStatus.InProgress));
            svc.AddTask(MakeTask("Done",       TaskStatus.Completed));
            svc.AddTask(MakeTask("Overdue",    TaskStatus.New,       dueDaysFromNow: -3));
            svc.AddTask(MakeTask("Important",  important: true));

            var s = svc.GetStatistics();
            Assert.Equal(5, s.Total);
            Assert.Equal(1, s.InProgress);
            Assert.Equal(1, s.Completed);
            Assert.Equal(1, s.Overdue);
            Assert.Equal(1, s.Important);
        }

        [Fact]
        public void IsOverdue_CompletedPastDue_NotOverdue()
        {
            var t = MakeTask("Done", TaskStatus.Completed, dueDaysFromNow: -5);
            Assert.False(t.IsOverdue);
        }

        [Fact]
        public void IsOverdue_NewPastDue_IsOverdue()
        {
            var t = MakeTask("Old", TaskStatus.New, dueDaysFromNow: -1);
            Assert.True(t.IsOverdue);
        }
    }
}
