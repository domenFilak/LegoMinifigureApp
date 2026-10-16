using LegoMinifigureApp.Services.Dashboard.Dtos;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int OwnedCount { get; init; }
        public int ForSaleCount { get; init; }
        public int ForTradeCount { get; init; }
        public int SoldCount { get; init; }
        public int TradedCount { get; init; }
        public IReadOnlyList<DashboardFigureViewModel> LatestFigures { get; init; } = [];

        public int ArchivedCount => SoldCount + TradedCount;
        public bool HasFigures => OwnedCount > 0;

        public static DashboardViewModel From(DashboardSummaryDto dto) => new()
        {
            OwnedCount = dto.OwnedCount,
            ForSaleCount = dto.ForSaleCount,
            ForTradeCount = dto.ForTradeCount,
            SoldCount = dto.SoldCount,
            TradedCount = dto.TradedCount,
            LatestFigures = dto.LatestFigures.Select(DashboardFigureViewModel.From).ToList()
        };
    }
}
