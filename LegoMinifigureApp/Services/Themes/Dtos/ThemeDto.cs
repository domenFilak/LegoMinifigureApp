namespace LegoMinifigureApp.Services.Themes.Dtos
{
    public record ThemeDto
    (
        int Id, 
        string Name, 
        int OwnedCount, 
        int ArchivedCount
    );
}
