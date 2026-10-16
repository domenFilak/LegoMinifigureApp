using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Dashboard.Dtos;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Dashboard
{
    public class DashboardFigureViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string ImageUrl { get; init; } = string.Empty;

        public static DashboardFigureViewModel From(LatestMinifigureDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            ImageUrl = ImageUrls.ForMinifigure(dto.ImagePath)
        };
    }
}
