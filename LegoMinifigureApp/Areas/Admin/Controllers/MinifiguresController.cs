using LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures;
using LegoMinifigureApp.Services.Minifigures;
using LegoMinifigureApp.Services.Themes;
using Microsoft.AspNetCore.Mvc;

namespace LegoMinifigureApp.Areas.Admin.Controllers
{
    public class MinifiguresController(IMinifigureService _minifigureService, IThemeService _themeService) : AdminControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] MinifigureFilterViewModel filter, CancellationToken cancellationToken)
        {
            var result = await _minifigureService.GetOwnedPageAsync(filter.ToQuery(), cancellationToken);
            var themes = await _themeService.GetOptionsAsync(cancellationToken);

            return View(MinifigureIndexViewModel.From(filter, result, themes));
        }
    }
}
