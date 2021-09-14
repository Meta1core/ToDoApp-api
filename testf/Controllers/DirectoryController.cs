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

namespace ToDoApp.Controllers
{
    [RoutePrefix("api/directory")]
    public class DirectoryController : ApiController
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();

        [CustomAuthorization]
        public List<Directory> Get()
        {
            ApplicationUser user = GetCurrentUser();
            var tasks = dbContext.Directories.Where(t => t.User.Id == user.Id).ToList();
            return tasks.ToList();
        }

        [HttpPost]
        [CustomAuthorization]
        public HttpResponseMessage Post(AddingDirectoryModel addingDirectoryModel)
        {
            if (!String.IsNullOrEmpty(addingDirectoryModel.directoryName))
            {
                Directory directory = new Directory() { DirectoryName = addingDirectoryModel.directoryName, User = GetCurrentUser() };
                dbContext.Directories.Add(directory);
                dbContext.SaveChanges();
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
            ApplicationUser user = dbContext.Users.Where(t => t.UserName == email).First();
            return user;
        }
    }

}