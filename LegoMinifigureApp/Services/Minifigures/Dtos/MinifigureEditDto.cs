using LegoMinifigureApp.Entities.Enums;

namespace LegoMinifigureApp.Services.Minifigures.Dtos
{
    public record MinifigureEditDto
    (
        int Id,
        string Name,
        string BrickLinkId,
        short? ReleaseYear,
        int ThemeId,
        MinifigureCondition Condition,
        string? Description,
        string? ImagePath,
        byte[] RowVersion
    );
}
