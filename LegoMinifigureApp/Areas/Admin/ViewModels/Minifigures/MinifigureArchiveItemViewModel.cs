using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Minifigures.Dtos;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureArchiveItemViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string BrickLinkId { get; init; } = string.Empty;
        public string ThemeName { get; init; } = string.Empty;
        public string ImageUrl { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public bool IsSold { get; init; }
        public string? SalePriceText { get; init; }
        public string? ArchiveNote { get; init; }
        public DateTime? StatusChangedAt { get; init; }

        public static MinifigureArchiveItemViewModel From(MinifigureArchiveItemDto dto)
        {
            return new MinifigureArchiveItemViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                BrickLinkId = dto.BrickLinkId,
                ThemeName = dto.ThemeName,
                ImageUrl = ImageUrls.ForMinifigure(dto.ImagePath),
                Status = dto.Status.ToString(),
                IsSold = dto.Status == MinifigureStatus.Sold,
                SalePriceText = PriceText.Format(dto.SalePrice),
                ArchiveNote = dto.ArchiveNote,
                StatusChangedAt = dto.StatusChangedAt
            };
        }
    }
}
