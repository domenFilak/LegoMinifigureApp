namespace LegoMinifigureApp.Services.Offers
{
    public record OfferListQuery
    (
        OfferStatusFilter Status,
        OfferTypeFilter Type,
        int Page
    );
}