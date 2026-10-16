using LegoMinifigureApp.Areas.Admin.ViewModels.Dashboard;
using LegoMinifigureApp.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace LegoMinifigureApp.Areas.Admin.Controllers
{
    public class DashboardController(IDashboardService _dashboardService) : AdminControllerBase
    {
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var summary = await _dashboardService.GetSummaryAsync(cancellationToken);
            return View(DashboardViewModel.From(summary));
        }
    }
}
