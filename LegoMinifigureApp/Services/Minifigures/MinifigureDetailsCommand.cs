using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Services.Storage;

namespace LegoMinifigureApp.Services.Minifigures
{
    public record MinifigureDetailsCommand
    (
        string Name,
        string BrickLinkId,
        short? ReleaseYear,
        int ThemeId,
        MinifigureCondition Condition,
        string? Description,
        ImageUpload? Image,
        bool RemoveImage
    );
}
