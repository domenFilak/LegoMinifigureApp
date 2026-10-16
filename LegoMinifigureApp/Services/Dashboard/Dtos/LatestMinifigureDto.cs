namespace LegoMinifigureApp.Services.Dashboard.Dtos
{
    public record LatestMinifigureDto
    (
        int Id, 
        string Name, 
        string? ImagePath
    );
}
