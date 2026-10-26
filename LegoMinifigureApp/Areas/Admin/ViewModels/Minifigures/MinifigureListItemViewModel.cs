using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Minifigures.Dtos;
using System.Globalization;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureListItemViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string BrickLinkId { get; init; } = string.Empty;
        public string ThemeName { get; init; } = string.Empty;
        public string Condition { get; init; } = string.Empty;
        public string ImageUrl { get; init; } = string.Empty;
        public bool IsForSale { get; init; }
        public string? AskingPriceText { get; init; }
        public bool IsForTrade { get; init; }
        public DateTime CreatedAt { get; init; }
        public string SaleTooltip { get; init; } = string.Empty;
        public string TradeTooltip { get; init; } = string.Empty;

        public static MinifigureListItemViewModel From(MinifigureListItemDto dto)
        {
            return new MinifigureListItemViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                BrickLinkId = dto.BrickLinkId,
                ThemeName = dto.ThemeName,
                Condition = dto.Condition.ToString(),
                ImageUrl = ImageUrls.ForMinifigure(dto.ImagePath),
                IsForSale = dto.IsForSale,
                AskingPriceText = PriceText.Format(dto.AskingPrice),
                IsForTrade = dto.IsForTrade,
                CreatedAt = dto.CreatedAt,
                SaleTooltip = TooltipText.AskingPrice(dto.AskingPrice),
                TradeTooltip = TooltipText.TradeNotes(dto.TradeNotes)
            };
        }
    }
}
