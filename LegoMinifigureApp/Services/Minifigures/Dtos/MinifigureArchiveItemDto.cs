using LegoMinifigureApp.Entities.Enums;

namespace LegoMinifigureApp.Services.Minifigures.Dtos
{
    public record MinifigureArchiveItemDto
    (
        int Id,
        string Name,
        string BrickLinkId,
        string ThemeName,
        string? ImagePath,
        MinifigureStatus Status,
        decimal? SalePrice,
        string? ArchiveNote,
        DateTime? StatusChangedAt
    );
}