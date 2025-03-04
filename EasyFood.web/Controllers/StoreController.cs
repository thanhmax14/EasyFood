using Microsoft.AspNetCore.Mvc;

namespace EasyFood.web.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
