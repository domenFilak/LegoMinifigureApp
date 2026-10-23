using LegoMinifigureApp.Entities.Enums;

namespace LegoMinifigureApp.Services.Minifigures.Dtos
{
    public record MinifigureDetailsDto
    (
        int Id,
        string Name,
        string BrickLinkId,
        string ThemeName,
        MinifigureCondition Condition,
        short? ReleaseYear,
        string? Description,
        string? ImagePath,
        MinifigureStatus Status,
        DateTime? StatusChangedAt,
        bool IsForSale,
        decimal? AskingPrice,
        bool IsForTrade,
        string? TradeNotes,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        byte[] RowVersion
    );
}