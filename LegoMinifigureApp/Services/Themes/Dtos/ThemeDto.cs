namespace LegoMinifigureApp.Services.Themes.Dtos
{
    public record ThemeDto
    (
        int Id, 
        string Name,
        DateTime CreatedAt,
        int OwnedCount, 
        int ArchivedCount
    );
}
