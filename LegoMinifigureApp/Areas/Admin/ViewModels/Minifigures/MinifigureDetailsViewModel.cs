using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Minifigures.Dtos;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureDetailsViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string BrickLinkId { get; init; } = string.Empty;
        public string ThemeName { get; init; } = string.Empty;
        public string Condition { get; init; } = string.Empty;
        public short? ReleaseYear { get; init; }
        public string? Description { get; init; }
        public string ImageUrl { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;
        public bool IsArchived { get; init; }
        public DateTime? StatusChangedAt { get; init; }

        public bool IsForSale { get; init; }
        public string? AskingPriceText { get; init; }
        public bool IsForTrade { get; init; }
        public string? TradeNotes { get; init; }

        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }

        public static MinifigureDetailsViewModel From(MinifigureDetailsDto dto)
        {
            return new MinifigureDetailsViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                BrickLinkId = dto.BrickLinkId,
                ThemeName = dto.ThemeName,
                Condition = dto.Condition.ToString(),
                ReleaseYear = dto.ReleaseYear,
                Description = dto.Description,
                ImageUrl = ImageUrls.ForMinifigure(dto.ImagePath),
                Status = dto.Status.ToString(),
                IsArchived = dto.Status != MinifigureStatus.Owned,
                StatusChangedAt = dto.StatusChangedAt,
                IsForSale = dto.IsForSale,
                AskingPriceText = PriceText.Format(dto.AskingPrice),
                IsForTrade = dto.IsForTrade,
                TradeNotes = dto.TradeNotes,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }
    }
}
