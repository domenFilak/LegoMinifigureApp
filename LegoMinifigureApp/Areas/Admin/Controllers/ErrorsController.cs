using LegoMinifigureApp.Models.ViewModels.Errors;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LegoMinifigureApp.Areas.Admin.Controllers
{
    //error pages inside admin area; they use admin layout and are admin only
    public class ErrorsController : AdminControllerBase
    {
        //crashes (500) in admin area
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;

            var referenceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var model = ErrorPageViewModel.ForServerError(referenceId);

            return View("ErrorPage", model);
        }

        //404 and other error status codes in admin area; called by UseStatusCodePagesWithReExecute
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Status(int id)
        {
            var model = ErrorPageViewModel.ForStatusCode(id);

            Response.StatusCode = model.StatusCode;

            return View("ErrorPage", model);
        }
    }
}
