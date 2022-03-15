using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using ToDoApp.App_Start;
using ToDoApp.Models;
using ToDoApp.Providers;

namespace ToDoApp.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext DbContext = new ApplicationDbContext();
        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager;
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager;
            }
            private set
            {
                _userManager = value;
            }
        }

        [Route("register")]
        public HttpResponseMessage Register(RegisterViewModel model, HttpRequestMessage request)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var store = new UserStore<ApplicationUser>(DbContext);
                var manager = new ApplicationUserManager(store);
                if (manager.Users.Where(x => x.Email == model.Email).Count() != 0)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, "This email is already in use");
                }
                if (manager.Create(user, model.Password).Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    return request.CreateResponse(HttpStatusCode.OK, user);

                }
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }


        [CustomAuthorization]
        [HttpGet]
        [Route("info")]
        public ApplicationUser GetUser()
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
    }
}