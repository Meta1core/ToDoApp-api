using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Text;
using System.Data.SqlClient;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    public class ToDoController : Controller
    {
        ApplicationDbContext DbContext = new ApplicationDbContext();
        // GET: ToDo
        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId();
            var tasks = from t in DbContext.Tasks
                        where t.User.Id == currentUserId
                        select t;
            return View(tasks.ToList());
        }

        // GET: ToDo/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ToDo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ToDo/Create
        [HttpPost]
        public ActionResult Create(string description, DateTime date, DateTime time)
        {
            ToDoTask toDoTask = new ToDoTask
            {
                Description = description,
                User = GetCurrentUser(),
                DateOfTask = ProcessDate(date, time)
            };
            DbContext.Tasks.Add(toDoTask);
            DbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ToDo/Edit/5
        public ActionResult Edit(int id)
        {
            ToDoTask toDoTask = DbContext.Tasks.Find(IsRequestCorrect(id));
            return toDoTask != null ? View(toDoTask) : (ActionResult)new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }


        public int? IsRequestCorrect(int? id)
        {
            if (DbContext.Tasks.Find(id) is null)
            {
                return null;
            }
            if (id == null)
            {
                return null;
            }
            ApplicationUser currentUser = GetCurrentUser();
            if (DbContext.Tasks.Find(id).User.Id != currentUser.Id)
            {
                return null;
            }
            return id;
        }
        // POST: ToDo/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, DateTime dateOfTask, string textOfTask)
        {
            ToDoTask toDoTask = DbContext.Tasks.FirstOrDefault(t => t.Id == id);
            toDoTask.DateOfTask = dateOfTask;
            toDoTask.Description = textOfTask;
            DbContext.SaveChanges();
            return RedirectToAction("Index");
        }


        // POST: ToDo/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            ToDoTask toDoTask = DbContext.Tasks.FirstOrDefault(t => t.Id == id);
            DbContext.Tasks.Remove(toDoTask);
            DbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        private ApplicationUser GetCurrentUser()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = DbContext.Users.FirstOrDefault(x => x.Id == currentUserId);
            return currentUser;
        }
        private DateTime ProcessDate(DateTime date, DateTime time)
        {
            DateTime dateOfTask = new DateTime(date.Year,
                    date.Month,
                    date.Day,
                    time.Hour,
                    time.Minute,
                    time.Second);
            return dateOfTask;
        }
    }
}
