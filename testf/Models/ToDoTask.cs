using System;

namespace ToDoApp.Models
{
    public class ToDoTask
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public string Header { get; set; }

        public bool IsFavorite { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Directory Directory { get; set; }

        public bool IsDone { get; set; }
        public bool IsOverdue { get; set; }
        public DateTime? DateOfTask { get; set; }

        public ToDoTask()
        {
            IsDone = false;
            IsOverdue = false;
        }
    }
}