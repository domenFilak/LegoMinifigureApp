namespace LegoMinifigureApp.Services.Minifigures
{
    public record MinifigureListQuery
    (
        string? Search,
        int? ThemeId,
        ListingFilter Listing,
        int Page
    );
}
