using LegoMinifigureApp.Entities.Enums;

namespace LegoMinifigureApp.Services.Offers.Dtos
{
    public record OfferListItemDto
    (
        int Id,
        DateTime CreatedAt,
        OfferType Type,
        OfferStatus Status,
        string Name,
        string Email,
        decimal? OfferedPrice,
        decimal? AskingPriceAtOffer,
        string? Message,
        int MinifigureId,
        string MinifigureName,
        string? MinifigureImagePath,
        MinifigureStatus MinifigureStatus,
        bool MinifigureIsDeleted
    );
}