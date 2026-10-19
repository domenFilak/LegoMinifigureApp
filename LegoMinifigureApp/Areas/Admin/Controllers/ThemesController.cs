using LegoMinifigureApp.Areas.Admin.ViewModels.Themes;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Themes;
using Microsoft.AspNetCore.Mvc;

namespace LegoMinifigureApp.Areas.Admin.Controllers
{
    public class ThemesController(IThemeService _themeService) : AdminControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var themes = await _themeService.GetAllAsync(cancellationToken);
            return View(ThemeIndexViewModel.From(themes));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ThemeFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThemeFormViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _themeService.CreateAsync(model.Name, cancellationToken);
            if (result.IsFailure)
            {
                ModelState.AddModelError(nameof(model.Name), result.Error!.Message);
                return View(model);
            }

            TempData[TempDataKeys.Success] = $"Theme '{model.Name.Trim()}' created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var theme = await _themeService.GetByIdAsync(id, cancellationToken);
            if (theme is null)
            {
                return NotFound();
            }

            return View(ThemeFormViewModel.From(theme));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ThemeFormViewModel model, CancellationToken cancellationToken)
        {
            model.Id = id; //routes id is source of truth, if id mangled with in model etc.

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _themeService.RenameAsync(id, model.Name, cancellationToken);
            if (result.IsFailure)
            {
                if (result.Error!.Type == ErrorType.NotFound)
                {
                    return NotFound();
                }

                ModelState.AddModelError(nameof(model.Name), result.Error.Message);
                return View(model);
            }

            TempData[TempDataKeys.Success] = $"Theme renamed to '{model.Name.Trim()}'.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var theme = await _themeService.GetByIdAsync(id, cancellationToken);
            if (theme is null)
            {
                return NotFound();
            }

            return View(ThemeSummaryViewModel.From(theme));
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            var result = await _themeService.DeleteAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                if (result.Error!.Type == ErrorType.NotFound)
                {
                    return NotFound();
                }

                TempData[TempDataKeys.Error] = result.Error.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData[TempDataKeys.Success] = "Theme deleted.";
            return RedirectToAction(nameof(Index));
        }

    }
}
