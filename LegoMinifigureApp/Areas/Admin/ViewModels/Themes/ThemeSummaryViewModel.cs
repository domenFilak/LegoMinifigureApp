using LegoMinifigureApp.Services.Themes.Dtos;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Themes
{
    public class ThemeSummaryViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int OwnedCount { get; init; }
        public int ArchivedCount { get; init; }

        public int FigureCount => OwnedCount + ArchivedCount;
        public bool CanDelete => FigureCount == 0;

        public static ThemeSummaryViewModel From(ThemeDto dto)
        {
            return new ThemeSummaryViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                OwnedCount = dto.OwnedCount,
                ArchivedCount = dto.ArchivedCount
            };
        }
    }
}
