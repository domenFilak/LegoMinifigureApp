using LegoMinifigureApp.Data;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Services.Dashboard.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LegoMinifigureApp.Services.Dashboard
{
    public class DashboardService(ApplicationDbContext db) : IDashboardService
    {
        private const int LatestFiguresCount = 3;

        public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
        {
            //first query, all counts
            var counts = await db.Minifigures
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Owned = g.Count(m => m.Status == MinifigureStatus.Owned),
                    ForSale = g.Count(m => m.IsForSale),
                    ForTrade = g.Count(m => m.IsForTrade),
                    Sold = g.Count(m => m.Status == MinifigureStatus.Sold),
                    Traded = g.Count(m => m.Status == MinifigureStatus.Traded)
                })
                .FirstOrDefaultAsync(cancellationToken);

            //second query, latest owned figures
            var latestFigures = await db.Minifigures
                .Where(m => m.Status == MinifigureStatus.Owned)
                .OrderByDescending(m => m.CreatedAt)
                .ThenByDescending(m => m.Id)
                .Take(LatestFiguresCount)
                .Select(m => new LatestMinifigureDto(m.Id, m.Name, m.ImagePath))
                .ToListAsync(cancellationToken);

            return new DashboardSummaryDto(
                OwnedCount: counts?.Owned ?? 0,
                ForSaleCount: counts?.ForSale ?? 0,
                ForTradeCount: counts?.ForTrade ?? 0,
                SoldCount: counts?.Sold ?? 0,
                TradedCount: counts?.Traded ?? 0,
                LatestFigures: latestFigures);
        }
    }
}
