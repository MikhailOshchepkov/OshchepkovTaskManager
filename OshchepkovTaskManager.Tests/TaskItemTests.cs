using OshchepkovTaskManager.Core.Models;
using TaskStatus = OshchepkovTaskManager.Core.Models.TaskStatus;
using Xunit;

namespace OshchepkovTaskManager.Tests
{
    public class TaskItemTests
    {
        [Fact]
        public void NewTask_HasUniqueId()
        {
            var t1 = new TaskItem();
            var t2 = new TaskItem();
            Assert.NotEqual(t1.Id, t2.Id);
        }

        [Fact]
        public void NewTask_DefaultStatus_IsNew()
        {
            var t = new TaskItem();
            Assert.Equal(TaskStatus.New, t.Status);
        }

        [Fact]
        public void NewTask_DefaultPriority_IsMedium()
        {
            var t = new TaskItem();
            Assert.Equal(TaskPriority.Medium, t.Priority);
        }

        [Fact]
        public void NewTask_IsNotImportant_ByDefault()
        {
            var t = new TaskItem();
            Assert.False(t.IsImportant);
        }

        [Fact]
        public void IsOverdue_FutureDueDate_NotOverdue()
        {
            var t = new TaskItem { DueDate = DateTime.Today.AddDays(1) };
            Assert.False(t.IsOverdue);
        }

        [Fact]
        public void IsOverdue_PastDueDate_New_IsOverdue()
        {
            var t = new TaskItem
            {
                Status  = TaskStatus.New,
                DueDate = DateTime.Today.AddDays(-1)
            };
            Assert.True(t.IsOverdue);
        }

        [Fact]
        public void IsOverdue_PastDueDate_Completed_NotOverdue()
        {
            var t = new TaskItem
            {
                Status  = TaskStatus.Completed,
                DueDate = DateTime.Today.AddDays(-5)
            };
            Assert.False(t.IsOverdue);
        }

        [Fact]
        public void IsOverdue_PastDueDate_Cancelled_NotOverdue()
        {
            var t = new TaskItem
            {
                Status  = TaskStatus.Cancelled,
                DueDate = DateTime.Today.AddDays(-3)
            };
            Assert.False(t.IsOverdue);
        }

        [Fact]
        public void ToString_ContainsTitleAndStatus()
        {
            var t = new TaskItem { Title = "МояЗадача", Status = TaskStatus.InProgress };
            var s = t.ToString();
            Assert.Contains("МояЗадача", s);
            Assert.Contains("InProgress", s);
        }
    }
}
