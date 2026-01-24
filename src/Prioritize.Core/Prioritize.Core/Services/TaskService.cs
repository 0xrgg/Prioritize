using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Prioritize.Core.Enum;
using Prioritize.Core.Models;
using Prioritize.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prioritize.Core.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _dbContext;

        public TaskService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TaskItem>> GetTasks()
        {
            var taskEntities = await _dbContext.Tasks.ToListAsync();

            var taskList = taskEntities.Select(x => new TaskItem
            {
                Id = x.Id,
                Title = x.Title,
                DueDate = x.DueDate,
                IsCompleted = x.IsCompleted,
                Notes = x.Notes,
                Due = EstablishDueCategory(x)
            });

            return taskList.OrderBy(x => x.DueDate);
        }

        public async Task AddTask(TaskItem task)
        {
            await _dbContext.AddAsync(new TaskItemEntity() 
            {
                Title = task.Title,
                Notes = task.Notes,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveTask(TaskItem task)
        {
            var entity = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == task.Id);

            if (entity is null) { return; }

            _dbContext.Tasks.Remove(entity);

            await _dbContext.SaveChangesAsync();
        }

        public async Task ToggleComplete(TaskItem task)
        {
            var entity = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == task.Id);

            if (entity is null) { return; }

            entity.IsCompleted = task.IsCompleted;

            await _dbContext.SaveChangesAsync();

        }

        private TaskFilter EstablishDueCategory(TaskItemEntity task)
        {

            var today = DateTimeOffset.UtcNow.Date;

            return task.DueDate switch
            {
                var d when d < today.AddDays(1) => TaskFilter.Tomorrow,
                var d when d < today.AddDays(7) => TaskFilter.Soon,
                var d when d < today.AddDays(30) => TaskFilter.NearFuture,
                _ => TaskFilter.All
            };
        }
    }
}
