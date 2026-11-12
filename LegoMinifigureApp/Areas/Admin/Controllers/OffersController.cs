using LegoMinifigureApp.Areas.Admin.ViewModels.Offers;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Offers;
using Microsoft.AspNetCore.Mvc;

namespace LegoMinifigureApp.Areas.Admin.Controllers
{
    public class OffersController(IOfferAdminService _offerAdminService) : AdminControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] OfferFilterViewModel filter, CancellationToken cancellationToken)
        {
            var result = await _offerAdminService.GetPageAsync(filter.ToQuery(), cancellationToken);
            return View(OfferIndexViewModel.From(filter, result));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var offer = await _offerAdminService.GetDetailsAsync(id, cancellationToken);
            if (offer is null)
            {
                return NotFound();
            }

            return View(OfferDetailsViewModel.From(offer));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept([FromRoute] int id, string? rowVersion, CancellationToken cancellationToken)
        {
            var decodedRowVersion = RowVersionText.TryDecode(rowVersion);
            if (decodedRowVersion is null)
            {
                return BadRequest();
            }

            var result = await _offerAdminService.AcceptAsync(id, decodedRowVersion, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error!.Type == ErrorType.NotFound)
                {
                    return NotFound();
                }

                TempData[TempDataKeys.Error] = result.Error.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData[TempDataKeys.Success] = "Offer accepted. Reply to the sender by e-mail to arrange the deal.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline([FromRoute] int id, string? rowVersion, CancellationToken cancellationToken)
        {
            var decodedRowVersion = RowVersionText.TryDecode(rowVersion);
            if (decodedRowVersion is null)
            {
                return BadRequest();
            }

            var result = await _offerAdminService.DeclineAsync(id, decodedRowVersion, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error!.Type == ErrorType.NotFound)
                {
                    return NotFound();
                }

                TempData[TempDataKeys.Error] = result.Error.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData[TempDataKeys.Success] = "Offer declined.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var offer = await _offerAdminService.GetDetailsAsync(id, cancellationToken);
            if (offer is null)
            {
                return NotFound();
            }

            return View(OfferDetailsViewModel.From(offer));
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, string? rowVersion, CancellationToken cancellationToken)
        {
            var decodedRowVersion = RowVersionText.TryDecode(rowVersion);
            if (decodedRowVersion is null)
            {
                return BadRequest();
            }

            var result = await _offerAdminService.DeleteAsync(id, decodedRowVersion, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error!.Type == ErrorType.NotFound)
                {
                    return NotFound();
                }

                TempData[TempDataKeys.Error] = result.Error.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData[TempDataKeys.Success] = "Offer deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
