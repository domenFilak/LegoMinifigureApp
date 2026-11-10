using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Models.ViewModels.Offers;
using LegoMinifigureApp.Security;
using LegoMinifigureApp.Services.Offers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LegoMinifigureApp.Controllers
{
    public class OffersController(IOfferService _offerService) : Controller
    {
        private const int MaxFormSize = 32 * 1024;

        //buy
        [HttpGet]
        public async Task<IActionResult> Buy(int id, CancellationToken cancellationToken)
        {
            var figure = await _offerService.GetFigureForBuyAsync(id, cancellationToken);
            if (figure is null)
            {
                return NotFound();
            }

            return View(BuyOfferFormViewModel.From(figure));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting(RateLimitPolicies.Offers)]
        [RequestSizeLimit(MaxFormSize)]
        public async Task<IActionResult> Buy(int id, BuyOfferFormViewModel model, CancellationToken cancellationToken)
        {
            //spam trap filled in => a bot; act as if it worked but save nothing
            if (!string.IsNullOrEmpty(model.Website))
            {
                return RedirectToAction(nameof(Sent), new { id });
            }

            var figure = await _offerService.GetFigureForBuyAsync(id, cancellationToken);
            if (figure is null)
            {
                return Unavailable();
            }

            model.Figure = OfferFigureViewModel.From(figure);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //missing or tampered version counts as "listing changed"
            var rowVersion = RowVersionText.TryDecode(model.FigureRowVersion) ?? [];

            var result = await _offerService.CreateBuyOfferAsync(model.ToCommand(id, rowVersion), cancellationToken);

            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(Sent), new { id });
            }

            if (result.Error!.Type == ErrorType.NotFound || result.Error == OfferErrors.NotForSale)
            {
                return Unavailable();
            }

            if (result.Error == OfferErrors.ListingChanged)
            {
                //show the current listing; the next send uses its new version
                model.FigureRowVersion = RowVersionText.Encode(figure.RowVersion);
                ModelState.Remove(nameof(model.FigureRowVersion));
                model.ListingWasUpdated = true;
                return View(model);
            }

            ModelState.AddModelError(result.Error.Field ?? string.Empty, result.Error.Message);
            return View(model);
        }

        //trade
        [HttpGet]
        public async Task<IActionResult> Trade(int id, CancellationToken cancellationToken)
        {
            var figure = await _offerService.GetFigureForTradeAsync(id, cancellationToken);
            if (figure is null)
            {
                return NotFound();
            }

            return View(TradeOfferFormViewModel.From(figure));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting(RateLimitPolicies.Offers)]
        [RequestSizeLimit(MaxFormSize)]
        public async Task<IActionResult> Trade(int id, TradeOfferFormViewModel model, CancellationToken cancellationToken)
        {
            //spam trap filled in => a bot; act as if it worked, save nothing
            if (!string.IsNullOrEmpty(model.Website))
            {
                return RedirectToAction(nameof(Sent), new { id });
            }

            var figure = await _offerService.GetFigureForTradeAsync(id, cancellationToken);
            if (figure is null)
            {
                return Unavailable();
            }

            model.Figure = OfferFigureViewModel.From(figure);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //missing or tampered version counts as "listing changed"
            var rowVersion = RowVersionText.TryDecode(model.FigureRowVersion) ?? [];

            var result = await _offerService.CreateTradeOfferAsync(model.ToCommand(id, rowVersion), cancellationToken);

            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(Sent), new { id });
            }

            if (result.Error!.Type == ErrorType.NotFound || result.Error == OfferErrors.NotForTrade)
            {
                return Unavailable();
            }

            if (result.Error == OfferErrors.ListingChanged)
            {
                //show the current listing; the next send uses its new version
                model.FigureRowVersion = RowVersionText.Encode(figure.RowVersion);
                ModelState.Remove(nameof(model.FigureRowVersion));
                model.ListingWasUpdated = true;
                return View(model);
            }

            ModelState.AddModelError(result.Error.Field ?? string.Empty, result.Error.Message);
            return View(model);
        }

        //after sending
        [HttpGet]
        public IActionResult Sent(int id)
        {
            return View(id);
        }

        //figure was sold, traded or unlisted while the form was open
        private ViewResult Unavailable()
        {
            Response.StatusCode = StatusCodes.Status409Conflict;
            return View("Unavailable");
        }
    }
}
