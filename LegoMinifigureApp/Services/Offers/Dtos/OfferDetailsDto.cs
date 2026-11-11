using LegoMinifigureApp.Entities.Enums;

namespace LegoMinifigureApp.Services.Offers.Dtos
{
    public record OfferDetailsDto
    (
        int Id,
        DateTime CreatedAt,
        OfferType Type,
        OfferStatus Status,
        DateTime? StatusChangedAt,
        string Name,
        string Email,
        decimal? OfferedPrice,
        decimal? AskingPriceAtOffer,
        string? Message,
        byte[] RowVersion,
        //the figure, as it is now
        int MinifigureId,
        string MinifigureName,
        string BrickLinkId,
        string? MinifigureImagePath,
        MinifigureStatus MinifigureStatus,
        bool MinifigureIsDeleted,
        bool MinifigureIsForSale,
        decimal? MinifigureAskingPrice,
        bool MinifigureIsForTrade
    );
}
