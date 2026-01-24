using Prioritize.Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace Prioritize.Core.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public TaskFilter Due { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string? Notes { get; set; }
        public bool IsCompleted { get; set; } = false;

        // Presentation layer stuff
        public bool IsExpanded { get; set; } = false;

        // Used only for binding
        public string DueDateString
        {
            get => DueDate.ToLocalTime().DateTime.ToString();
            set
            {
                if (DateTimeOffset.TryParse(value, out var dt))
                    DueDate = dt;
            }
        }
    }
}