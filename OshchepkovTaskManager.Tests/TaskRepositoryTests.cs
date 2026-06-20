using OshchepkovTaskManager.Core.Models;
using TaskStatus = OshchepkovTaskManager.Core.Models.TaskStatus;
using OshchepkovTaskManager.Core.Services;
using Xunit;

namespace OshchepkovTaskManager.Tests
{
    public class TaskRepositoryTests
    {
        private static TaskRepository CreateRepo() => new();

        private static TaskItem SampleTask(string title = "Тест") => new()
        {
            Title       = title,
            Description = "Описание",
            Priority    = TaskPriority.Medium,
            Status      = TaskStatus.New,
            DueDate     = DateTime.Today.AddDays(5)
        };


        [Fact]
        public void Add_ValidTask_IncreasesCount()
        {
            var repo = CreateRepo();
            repo.Add(SampleTask());
            Assert.Single(repo.GetAll());
        }

        [Fact]
        public void Add_NullTask_ThrowsArgumentNullException()
        {
            var repo = CreateRepo();
            Assert.Throws<ArgumentNullException>(() => repo.Add(null!));
        }

        [Fact]
        public void Add_EmptyTitle_ThrowsArgumentException()
        {
            var repo = CreateRepo();
            var task = SampleTask();
            task.Title = "   ";
            Assert.Throws<ArgumentException>(() => repo.Add(task));
        }

        [Fact]
        public void Add_MultipleItems_AllPresent()
        {
            var repo = CreateRepo();
            repo.Add(SampleTask("A"));
            repo.Add(SampleTask("B"));
            repo.Add(SampleTask("C"));
            Assert.Equal(3, repo.GetAll().Count);
        }


        [Fact]
        public void GetById_ExistingId_ReturnsTask()
        {
            var repo = CreateRepo();
            var task = SampleTask();
            repo.Add(task);
            var found = repo.GetById(task.Id);
            Assert.NotNull(found);
            Assert.Equal(task.Id, found!.Id);
        }

        [Fact]
        public void GetById_NonExistingId_ReturnsNull()
        {
            var repo = CreateRepo();
            Assert.Null(repo.GetById(Guid.NewGuid()));
        }

        [Fact]
        public void Update_ExistingTask_ChangesData()
        {
            var repo = CreateRepo();
            var task = SampleTask("Старое");
            repo.Add(task);
            task.Title = "Новое";
            repo.Update(task);
            Assert.Equal("Новое", repo.GetById(task.Id)!.Title);
        }

        [Fact]
        public void Update_NonExistingTask_ThrowsKeyNotFoundException()
        {
            var repo = CreateRepo();
            Assert.Throws<KeyNotFoundException>(() => repo.Update(SampleTask()));
        }


        [Fact]
        public void Delete_ExistingId_RemovesTask()
        {
            var repo = CreateRepo();
            var task = SampleTask();
            repo.Add(task);
            repo.Delete(task.Id);
            Assert.Empty(repo.GetAll());
        }

        [Fact]
        public void Delete_NonExistingId_ThrowsKeyNotFoundException()
        {
            var repo = CreateRepo();
            Assert.Throws<KeyNotFoundException>(() => repo.Delete(Guid.NewGuid()));
        }

        [Fact]
        public void SaveAndLoad_RoundTrip_PreservesData()
        {
            var repo = CreateRepo();
            var task = SampleTask("Сохранение");
            task.IsImportant = true;
            repo.Add(task);

            var path = Path.GetTempFileName();
            try
            {
                repo.SaveToFile(path);

                var repo2 = CreateRepo();
                repo2.LoadFromFile(path);

                Assert.Single(repo2.GetAll());
                var loaded = repo2.GetAll()[0];
                Assert.Equal("Сохранение", loaded.Title);
                Assert.True(loaded.IsImportant);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void LoadFromFile_NonExistingFile_ThrowsFileNotFoundException()
        {
            var repo = CreateRepo();
            Assert.Throws<FileNotFoundException>(() => repo.LoadFromFile("nonexistent.json"));
        }

        [Fact]
        public void SaveToFile_EmptyPath_ThrowsArgumentException()
        {
            var repo = CreateRepo();
            Assert.Throws<ArgumentException>(() => repo.SaveToFile(""));
        }
    }
}
