using Microsoft.AspNetCore.Mvc;

namespace EasyFood.web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
