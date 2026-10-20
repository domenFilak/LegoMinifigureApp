using LegoMinifigureApp.Data;
using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Minifigures.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LegoMinifigureApp.Services.Minifigures
{
    public class MinifigureService(ApplicationDbContext db) : IMinifigureService
    {
        public const int PageSize = 20;

        public async Task<PagedResult<MinifigureListItemDto>> GetOwnedPageAsync(MinifigureListQuery query, CancellationToken cancellationToken = default)
        {
            //build query start - owned figures only, soft deleted filtered out automatically
            IQueryable<Minifigure> figures = db.Minifigures.Where(m => m.Status == MinifigureStatus.Owned);

            //apply search
            var search = query.Search?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                figures = figures.Where(m => m.Name.Contains(search) || m.BrickLinkId.Contains(search));
            }

            //apply theme
            if (query.ThemeId.HasValue)
            {
                var themeId = query.ThemeId.Value;
                figures = figures.Where(m => m.ThemeId == themeId);
            }

            //apply listing choice
            figures = query.Listing switch
            {
                ListingFilter.ForSale => figures.Where(m => m.IsForSale),
                ListingFilter.ForTrade => figures.Where(m => m.IsForTrade),
                ListingFilter.NotListed => figures.Where(m => !m.IsForSale && !m.IsForTrade),
                _ => figures //all, nothing applied
            };

            //count everything that matches, then fix the page number
            var totalCount = await figures.CountAsync(cancellationToken);
            var totalPages = PagedResult<MinifigureListItemDto>.CalculateTotalPages(totalCount, PageSize);
            var page = Math.Clamp(query.Page, 1, totalPages);

            //load only this page and needed columns
            var items = await figures
                .OrderBy(m => m.Name)
                .ThenBy(m => m.Id)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(m => new MinifigureListItemDto(
                    m.Id,
                    m.Name,
                    m.BrickLinkId,
                    m.Theme.Name,
                    m.Condition,
                    m.ImagePath,
                    m.IsForSale,
                    m.AskingPrice,
                    m.IsForTrade,
                    m.CreatedAt))
                .ToListAsync(cancellationToken);

            return new PagedResult<MinifigureListItemDto>(items, page, PageSize, totalCount);
        }
    }
}
