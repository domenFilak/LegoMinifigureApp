using LegoMinifigureApp.Models;
using LegoMinifigureApp.Models.ViewModels.Errors;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LegoMinifigureApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Minifigures");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //crashes (500); called by UseExceptionHandler
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;

            var referenceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var model = ErrorPageViewModel.ForServerError(referenceId);

            return View("ErrorPage", model);
        }

        //404 and other error status codes; called by UseStatusCodePagesWithReExecute
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Status(int id)
        {
            var model = ErrorPageViewModel.ForStatusCode(id);

            Response.StatusCode = model.StatusCode;

            return View("ErrorPage", model);
        }
    }
}
