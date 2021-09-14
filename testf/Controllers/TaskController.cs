﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ToDoApp.Providers;
using ToDoApp.Models;
using System.Data.Entity;
using MySql.Data.MySqlClient;

namespace ToDoApp.Controllers
{
    public class TasksController : ApiController
    {
        ApplicationDbContext DbContext = new ApplicationDbContext();

        [HttpGet]
        [CustomAuthorization]
        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            ApplicationUser user = GetCurrentUser();
            if (!(user is null))
            {
                MySqlParameter param = new MySqlParameter("@User_Id", user.Id);
                List<ToDoTask> toDoTasks = DbContext.Tasks.SqlQuery("Call ToDoTask_GetActiveList(@User_Id)", param).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, toDoTasks);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        [CustomAuthorization]
        private ApplicationUser GetCurrentUser()
        {
            string email = "";
            ClaimsIdentity id = ((ClaimsIdentity)User.Identity);
            Claim claim = id.Claims.First();
            if (claim != null)
            {
                email = claim.Value;
            }
            ApplicationUser user = DbContext.Users.Where(t => t.UserName == email).First();
            return user;
        }

        // GET api/<controller>/5
        [HttpGet]
        [CustomAuthorization]
        [Route("api/tasks/{id}")]
        public HttpResponseMessage Get(int id)
        {
            ToDoTask toDoTask = DbContext.Tasks.Find(IsRequestCorrect(id));
            return toDoTask != null ? Request.CreateResponse(HttpStatusCode.OK, toDoTask) : Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Incorrect request");
        }

        [HttpGet]
        [CustomAuthorization]
        [Route("api/tasks/overdue")]
        public HttpResponseMessage GetOverdueList()
        {
            ApplicationUser user = GetCurrentUser();
            if (!(user is null))
            {
                MySqlParameter param = new MySqlParameter("@User_Id", user.Id);
                List<ToDoTask> toDoTasks = DbContext.Tasks.SqlQuery("Call ToDoTask_GetOverdueList(@User_Id)", param).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, toDoTasks);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [CustomAuthorization]
        [Route("api/tasks/completed")]
        public HttpResponseMessage GetCompletedList()
        {
            ApplicationUser user = GetCurrentUser();
            if (!(user is null))
            {
                MySqlParameter param = new MySqlParameter("@User_Id", user.Id);
                List<ToDoTask> toDoTasks = DbContext.Tasks.SqlQuery("Call ToDoTask_GetCompletedList(@User_Id)", param).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, toDoTasks);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [CustomAuthorization]
        [Route("api/tasks/favorite")]
        public HttpResponseMessage GetFavoriteList()
        {
            ApplicationUser user = GetCurrentUser();
            if (!(user is null))
            {
                MySqlParameter param = new MySqlParameter("@User_Id", user.Id);
                List<ToDoTask> toDoTasks = DbContext.Tasks.SqlQuery("Call ToDoTask_GetFavoriteList(@User_Id)", param).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, toDoTasks);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        // POST api/<controller>

        [HttpPost]
        [CustomAuthorization]
        public HttpResponseMessage Post([FromBody] AddingTaskModel addingTaskModel)
        {
            ToDoTask toDoTask = new ToDoTask
            {
                Description = addingTaskModel.Description,
                User = GetCurrentUser(),
                DateOfTask = addingTaskModel.DateOfTask,
                Header = addingTaskModel.Header,
                Directory = DbContext.Directories.Where(d => d.Id == addingTaskModel.Directory_Id).FirstOrDefault()
            };
            DbContext.Tasks.Add(toDoTask);
            DbContext.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        [HttpPut]
        [CustomAuthorization]
        public HttpResponseMessage Put([FromBody] UpdatingTaskModel updatingTaskModel)
        {
            ToDoTask toDoTask = DbContext.Tasks.FirstOrDefault(t => t.Id == updatingTaskModel.Id);
            toDoTask.Header = updatingTaskModel.Header;
            toDoTask.Description = updatingTaskModel.Description;
            toDoTask.IsFavorite = updatingTaskModel.IsFavorite;
            toDoTask.IsDone = updatingTaskModel.IsDone;
            toDoTask.DateOfTask = updatingTaskModel.DateOfTask;
            toDoTask.Directory = DbContext.Directories.Where(d => d.Id == updatingTaskModel.Directory_Id).FirstOrDefault();
            DbContext.Entry(toDoTask).State = EntityState.Modified;
            DbContext.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("api/tasks/{id}")]
        [CustomAuthorization]
        public HttpResponseMessage Delete(int id)
        {
            ToDoTask toDoTask = DbContext.Tasks.Find(IsRequestCorrect(id));
            if (!(toDoTask is null))
            {
                DbContext.Tasks.Attach(toDoTask);
                DbContext.Tasks.Remove(toDoTask);
                DbContext.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Incorrect request");
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
    }
}