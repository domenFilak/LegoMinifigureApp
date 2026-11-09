namespace LegoMinifigureApp.Services.Offers
{
    public record TradeOfferCommand
    (
        int MinifigureId,
        byte[] FigureRowVersion, //version of the figure the visitor saw
        string Name,
        string Email,
        string Message
    );
}