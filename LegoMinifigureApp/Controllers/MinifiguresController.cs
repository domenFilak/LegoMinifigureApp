using LegoMinifigureApp.Models.ViewModels.Minifigures;
using LegoMinifigureApp.Services.Minifigures;
using LegoMinifigureApp.Services.Themes;
using Microsoft.AspNetCore.Mvc;

namespace LegoMinifigureApp.Controllers
{
    public class MinifiguresController(IMinifigureService _minifigureService, IThemeService _themeService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] MinifigureCollectionFilterViewModel filter, CancellationToken cancellationToken)
        {
            var result = await _minifigureService.GetOwnedPageAsync(filter.ToQuery(), cancellationToken);
            var themes = await _themeService.GetOptionsAsync(cancellationToken);

            return View(MinifigureCollectionIndexViewModel.From(filter, result, themes));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var figure = await _minifigureService.GetOwnedDetailsAsync(id, cancellationToken);
            if (figure is null)
            {
                return NotFound();
            }

            return View(MinifigureCollectionDetailsViewModel.From(figure));
        }
    }
}
