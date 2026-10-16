using LegoMinifigureApp.Services.Dashboard.Dtos;

namespace LegoMinifigureApp.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    }
}
