using LegoMinifigureApp.Entities.Enums;

namespace LegoMinifigureApp.Services.Minifigures.Dtos
{
    public record MinifigureListItemDto
    (
        int Id,
        string Name,
        string BrickLinkId,
        string ThemeName,
        MinifigureCondition Condition,
        string? ImagePath,
        bool IsForSale,
        decimal? AskingPrice,
        bool IsForTrade,
        string? TradeNotes,
        DateTime CreatedAt
    );
}
