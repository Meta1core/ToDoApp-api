using System;

namespace ToDoApp.Models
{
    public class AddingTaskModel
    {
        public string Description { get; set; }

        public string Header { get; set; }

        public DateTime? DateOfTask { get; set; }

        public int Directory_Id { get; set; }
    }
}