using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Prioritize.Data
{
    public class TaskItemEntity
    {

        public int Id { get; set; }
        public string Title { get; set; }
        //public TaskFilter Due { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string? Notes { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class TaskItemEntityConfiguration : IEntityTypeConfiguration<TaskItemEntity>
    {
        public void Configure(EntityTypeBuilder<TaskItemEntity> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
