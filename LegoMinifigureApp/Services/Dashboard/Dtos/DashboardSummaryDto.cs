namespace LegoMinifigureApp.Services.Dashboard.Dtos
{
    public record DashboardSummaryDto
    (
        int OwnedCount,
        int ForSaleCount,
        int ForTradeCount,
        int SoldCount,
        int TradedCount,
        IReadOnlyList<LatestMinifigureDto> LatestFigures
    );
}
