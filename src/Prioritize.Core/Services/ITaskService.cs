using Prioritize.Core.Models;

namespace Prioritize.Core.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetTasks();
        Task AddTask(TaskItem task);
        Task RemoveTask(TaskItem task);
        Task ToggleComplete(TaskItem task);

    }
}