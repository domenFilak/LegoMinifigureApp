using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Minifigures.Dtos;

namespace LegoMinifigureApp.Models.ViewModels.Minifigures
{
    public class MinifigureCollectionDetailsViewModel
    {
        public string Name { get; init; } = string.Empty;
        public string BrickLinkId { get; init; } = string.Empty;
        public string ThemeName { get; init; } = string.Empty;
        public string Condition { get; init; } = string.Empty;
        public short? ReleaseYear { get; init; }
        public string? Description { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
        public bool IsForSale { get; init; }
        public string? AskingPriceText { get; init; }
        public bool IsForTrade { get; init; }
        public string? TradeNotes { get; init; }

        public static MinifigureCollectionDetailsViewModel From(MinifigureDetailsDto dto)
        {
            return new MinifigureCollectionDetailsViewModel
            {
                Name = dto.Name,
                BrickLinkId = dto.BrickLinkId,
                ThemeName = dto.ThemeName,
                Condition = dto.Condition.ToString(),
                ReleaseYear = dto.ReleaseYear,
                Description = dto.Description,
                ImageUrl = ImageUrls.ForMinifigure(dto.ImagePath),
                IsForSale = dto.IsForSale,
                AskingPriceText = PriceText.Format(dto.AskingPrice),
                IsForTrade = dto.IsForTrade,
                TradeNotes = dto.TradeNotes
            };
        }
    }
}
