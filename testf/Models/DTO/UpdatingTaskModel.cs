using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoApp.Models
{
    public class UpdatingTaskModel
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public string Header { get; set; }

        public DateTime? DateOfTask { get; set; }

        public bool IsFavorite { get; set; }

        public bool IsDone { get; set; }

        public int Directory_Id { get; set; }
    }
}