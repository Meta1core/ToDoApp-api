using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoApp.Models
{
    public class Directory
    {
        public int Id { get; set; }

        public string DirectoryName { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}