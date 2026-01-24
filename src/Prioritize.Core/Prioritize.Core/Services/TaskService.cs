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
    public class TaskService
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
                DueDate = x.DueDate.ToString(),
                IsCompleted = x.IsCompleted,
                Notes = x.Notes,
                Due = EstablishDueCategory(x)
            });

            return taskList;
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
