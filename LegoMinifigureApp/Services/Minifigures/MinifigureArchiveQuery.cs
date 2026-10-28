namespace LegoMinifigureApp.Services.Minifigures
{
    public record MinifigureArchiveQuery
    (
        string? Search,
        int? ThemeId,
        ArchiveStatusFilter Status,
        int Page
    );
}