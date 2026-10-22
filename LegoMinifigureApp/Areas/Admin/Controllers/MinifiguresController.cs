using LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Minifigures;
using LegoMinifigureApp.Services.Themes;
using Microsoft.AspNetCore.Mvc;

namespace LegoMinifigureApp.Areas.Admin.Controllers
{
    public class MinifiguresController(
        IMinifigureService _minifigureService, 
        IThemeService _themeService) : AdminControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] MinifigureFilterViewModel filter, CancellationToken cancellationToken)
        {
            var result = await _minifigureService.GetOwnedPageAsync(filter.ToQuery(), cancellationToken);
            var themes = await _themeService.GetOptionsAsync(cancellationToken);

            return View(MinifigureIndexViewModel.From(filter, result, themes));
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new MinifigureFormViewModel();
            await LoadOptionsAsync(model, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MinifigureFormViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await LoadOptionsAsync(model, cancellationToken);
                return View(model);
            }

            await using var imageStream = model.Image?.OpenReadStream();
            var result = await _minifigureService.CreateAsync(model.ToCommand(imageStream), cancellationToken);

            if (result.IsFailure)
            {
                ModelState.AddModelError(result.Error!.Field ?? string.Empty, result.Error.Message);
                await LoadOptionsAsync(model, cancellationToken);
                return View(model);
            }

            TempData[TempDataKeys.Success] = $"Minifigure '{model.Name.Trim()}' added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var figure = await _minifigureService.GetForEditAsync(id, cancellationToken);
            if (figure is null)
            {
                return NotFound();
            }

            var model = MinifigureFormViewModel.From(figure);
            await LoadOptionsAsync(model, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MinifigureFormViewModel model, CancellationToken cancellationToken)
        {
            model.Id = id; //take route id as truth

            var rowVersion = model.GetRowVersionBytes();
            if (rowVersion is null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadOptionsAsync(model, cancellationToken);
                return View(model);
            }

            await using var imageStream = model.Image?.OpenReadStream();
            var result = await _minifigureService.UpdateAsync(id, rowVersion, model.ToCommand(imageStream), cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error!.Type == ErrorType.NotFound)
                {
                    return NotFound();
                }

                ModelState.AddModelError(result.Error.Field ?? string.Empty, result.Error.Message);
                await LoadOptionsAsync(model, cancellationToken);
                return View(model);
            }

            TempData[TempDataKeys.Success] = $"Minifigure '{model.Name.Trim()}' saved.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadOptionsAsync(MinifigureFormViewModel model, CancellationToken cancellationToken)
        {
            var themes = await _themeService.GetOptionsAsync(cancellationToken);
            model.SetOptions(themes);
        }
    }
}
