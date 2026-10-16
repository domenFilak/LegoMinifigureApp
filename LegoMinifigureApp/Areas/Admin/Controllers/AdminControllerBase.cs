using LegoMinifigureApp.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegoMinifigureApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public abstract class AdminControllerBase : Controller
    {
    }
}
