using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using ToDoApp.Models;
using ToDoApp.Providers;
using ToDoApp.Models.DTO;
using Microsoft.AspNet.SignalR;
using ToDoApp.Hubs;

namespace ToDoApp.Controllers
{
    [RoutePrefix("api/directory")]
    public class DirectoryController : EntitySetControllerWithHub<TasksHub>
    {
        ApplicationDbContext DbContext = new ApplicationDbContext();

        [CustomAuthorization]
        public List<Directory> Get()
        {
            ApplicationUser user = GetCurrentUser();
            var directories = DbContext.Directories.Where(t => t.User.Id == user.Id).ToList();
            return directories.ToList();
        }

        [CustomAuthorization]
        public HttpResponseMessage Delete(int id)
        {
            Directory directory = DbContext.Directories.Find(IsRequestCorrect(id));
            if (!(directory is null))
            {
                DbContext.Directories.Attach(directory);
                DbContext.Directories.Remove(directory);
                DbContext.SaveChanges();
                Hub.Clients.All.sendNotification();
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Incorrect request");
        }

        [HttpPost]
        [CustomAuthorization]
        public HttpResponseMessage Post(AddingDirectoryModel addingDirectoryModel)
        {
            if (!String.IsNullOrEmpty(addingDirectoryModel.directoryName))
            {
                Directory directory = new Directory() { DirectoryName = addingDirectoryModel.directoryName, User = GetCurrentUser() };
                DbContext.Directories.Add(directory);
                DbContext.SaveChanges();
                Hub.Clients.All.sendNotification();
                return new HttpResponseMessage(HttpStatusCode.Created);
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

        public int? IsRequestCorrect(int? id)
        {
            if (DbContext.Directories.Find(id) is null)
            {
                return null;
            }
            if (id == null)
            {
                return null;
            }
            ApplicationUser currentUser = GetCurrentUser();
            if (DbContext.Directories.Find(id).User.Id != currentUser.Id)
            {
                return null;
            }
            return id;
        }
    }

}