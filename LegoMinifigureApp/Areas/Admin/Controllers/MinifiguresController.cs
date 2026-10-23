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
            return RedirectToAction(nameof(Details), new { id = result.Value });
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
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var figure = await _minifigureService.GetDetailsAsync(id, cancellationToken);
            if (figure is null)
            {
                return NotFound();
            }

            return View(MinifigureDetailsViewModel.From(figure));
        }

        [HttpGet]
        public async Task<IActionResult> Sale(int id, CancellationToken cancellationToken)
        {
            var figure = await _minifigureService.GetDetailsAsync(id, cancellationToken);
            if (figure is null)
            {
                return NotFound();
            }

            if (figure.Status != MinifigureStatus.Owned)
            {
                TempData[TempDataKeys.Error] = "Only owned figures can be listed.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(MinifigureSaleFormViewModel.From(figure));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sale(int id, MinifigureSaleFormViewModel model, CancellationToken cancellationToken)
        {
            model.Id = id;

            var rowVersion = RowVersionText.TryDecode(model.RowVersion);
            if (rowVersion is null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _minifigureService.ListForSaleAsync(id, rowVersion, model.AskingPrice!.Value, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error!.Type == ErrorType.NotFound)
                {
                    return NotFound();
                }

                ModelState.AddModelError(result.Error.Field ?? string.Empty, result.Error.Message);
                return View(model);
            }

            var price = decimal.Round(model.AskingPrice.Value, 2, MidpointRounding.AwayFromZero);
            TempData[TempDataKeys.Success] = $"Listed for sale at {PriceText.Format(price)}.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromSale(int id, CancellationToken cancellationToken)
        {
            var result = await _minifigureService.RemoveFromSaleAsync(id, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error!.Type == ErrorType.NotFound)
                {
                    return NotFound();
                }

                TempData[TempDataKeys.Error] = result.Error.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData[TempDataKeys.Success] = "Removed from sale.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Trade(int id, CancellationToken cancellationToken)
        {
            var figure = await _minifigureService.GetDetailsAsync(id, cancellationToken);
            if (figure is null)
            {
                return NotFound();
            }

            if (figure.Status != MinifigureStatus.Owned)
            {
                TempData[TempDataKeys.Error] = "Only owned figures can be listed.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(MinifigureTradeFormViewModel.From(figure));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Trade(int id, MinifigureTradeFormViewModel model, CancellationToken cancellationToken)
        {
            model.Id = id;

            var rowVersion = RowVersionText.TryDecode(model.RowVersion);
            if (rowVersion is null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _minifigureService.ListForTradeAsync(id, rowVersion, model.TradeNotes, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error!.Type == ErrorType.NotFound)
                {
                    return NotFound();
                }

                ModelState.AddModelError(result.Error.Field ?? string.Empty, result.Error.Message);
                return View(model);
            }

            TempData[TempDataKeys.Success] = "Listed for trade.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromTrade(int id, CancellationToken cancellationToken)
        {
            var result = await _minifigureService.RemoveFromTradeAsync(id, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error!.Type == ErrorType.NotFound)
                {
                    return NotFound();
                }

                TempData[TempDataKeys.Error] = result.Error.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData[TempDataKeys.Success] = "Removed from trade.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task LoadOptionsAsync(MinifigureFormViewModel model, CancellationToken cancellationToken)
        {
            var themes = await _themeService.GetOptionsAsync(cancellationToken);
            model.SetOptions(themes);
        }
    }
}
