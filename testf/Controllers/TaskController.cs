using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using ToDoApp.Hubs;
using ToDoApp.Models;
using ToDoApp.Providers;

namespace ToDoApp.Controllers
{
    public class TasksController : EntitySetControllerWithHub<TasksHub>
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
                Hub.Clients.All.sendNotification();
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

            Hub.Clients.All.sendNotification();

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        [HttpGet]
        [CustomAuthorization]
        [Route("api/tasks/indirectory/{directoryId}")]
        public HttpResponseMessage GetTasksInDirectory(int directoryId)
        {
            ApplicationUser user = GetCurrentUser();
            if (!(user is null))
            {
                MySqlParameter userIdParam = new MySqlParameter("@User_Id", user.Id);
                MySqlParameter directoryIdParam = new MySqlParameter("@Directory_Id", directoryId);
                List<ToDoTask> toDoTasksInDirectory = DbContext.Tasks.SqlQuery("Call ToDoTask_TasksInDirectory(@User_Id, @Directory_Id)", userIdParam, directoryIdParam).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, toDoTasksInDirectory);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        [HttpPut]
        [CustomAuthorization]
        public HttpResponseMessage Put([FromBody] UpdatingTaskModel updatingTaskModel)
        {
            ApplicationUser user = GetCurrentUser();
            if (!(user is null))
            {
                ToDoTask toDoTask = DbContext.Tasks.FirstOrDefault(t => t.Id == updatingTaskModel.Id);
                MySqlParameter taskIdParam = new MySqlParameter("@Id", toDoTask.Id);
                MySqlParameter userIdParam = new MySqlParameter("@User_Id", user.Id);
                MySqlParameter directoryIdParam;
                if (!updatingTaskModel.Directory_Id.Equals(0))
                {
                    directoryIdParam = new MySqlParameter("@Directory_Id", updatingTaskModel.Directory_Id);
                }
                else
                {
                    directoryIdParam = new MySqlParameter("@Directory_Id", null);
                }
                MySqlParameter headerParam = new MySqlParameter("@Header", updatingTaskModel.Header);
                MySqlParameter descriptionParam = new MySqlParameter("@Description", updatingTaskModel.Description);
                MySqlParameter isDoneParam = new MySqlParameter("@IsDone", updatingTaskModel.IsDone);
                MySqlParameter isFavoriteParam = new MySqlParameter("@IsFavorite", updatingTaskModel.IsFavorite);
                MySqlParameter isOverdueParam = new MySqlParameter("@IsOverdue", updatingTaskModel.IsOverdue);
                MySqlParameter dateOfTaskParam = new MySqlParameter("@DateOfTask", updatingTaskModel.DateOfTask);
                DbContext.Database.ExecuteSqlCommand("Call ToDoTask_Update(@Id, @User_Id, @Directory_Id, @Header, @Description, @IsDone, @IsOverdue, @IsFavorite, @DateOfTask)", taskIdParam, userIdParam, directoryIdParam, headerParam, descriptionParam, isDoneParam, isOverdueParam, isFavoriteParam, dateOfTaskParam);

                Hub.Clients.All.sendNotification();
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
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


                Hub.Clients.All.sendNotification();
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