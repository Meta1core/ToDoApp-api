using System.Web.Mvc;

namespace ToDoApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NewTask()
        {
            return View();
        }
        public ActionResult EditTask()
        {
            return View();
        }
        public ActionResult DirectoriesPage()
        {
            return View();
        }
    }
}