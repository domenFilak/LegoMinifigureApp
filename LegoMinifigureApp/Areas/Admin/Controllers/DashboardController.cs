using LegoMinifigureApp.Areas.Admin.ViewModels.Dashboard;
using LegoMinifigureApp.Services.Dashboard;
using LegoMinifigureApp.Services.Offers;
using Microsoft.AspNetCore.Mvc;

namespace LegoMinifigureApp.Areas.Admin.Controllers
{
    public class DashboardController(IDashboardService _dashboardService, IOfferAdminService _offerAdminService) : AdminControllerBase
    {
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var summary = await _dashboardService.GetSummaryAsync(cancellationToken);
            var newOffersCount = await _offerAdminService.CountNewAsync(cancellationToken);

            return View(DashboardViewModel.From(summary, newOffersCount));
        }
    }
}
