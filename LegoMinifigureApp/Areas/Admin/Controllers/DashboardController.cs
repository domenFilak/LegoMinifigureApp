using Microsoft.AspNetCore.Mvc;

namespace LegoMinifigureApp.Areas.Admin.Controllers
{
    public class DashboardController : AdminControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
