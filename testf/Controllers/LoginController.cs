using System.Web.Mvc;
using ToDoApp.Models.DTO;

namespace ToDoApp.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(RegistrationModel registrationModel)
        {
            return View();
        }
    }
}