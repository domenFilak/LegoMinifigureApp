namespace LegoMinifigureApp.Services.Offers.Dtos
{
    //the figure shown next to an offer form
    public record OfferFigureDto
    (
        int Id,
        string Name,
        string BrickLinkId,
        string? ImagePath,
        bool IsForSale,
        decimal? AskingPrice,
        bool IsForTrade,
        string? TradeNotes,
        byte[] RowVersion
    );
}
