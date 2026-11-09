namespace LegoMinifigureApp.Services.Offers
{
    public record BuyOfferCommand
    (
        int MinifigureId,
        byte[] FigureRowVersion, //version of the figure the visitor saw
        string Name,
        string Email,
        decimal OfferedPrice,
        string? Message
    );
}
