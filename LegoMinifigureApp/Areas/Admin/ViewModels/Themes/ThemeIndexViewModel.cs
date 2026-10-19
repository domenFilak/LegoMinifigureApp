using LegoMinifigureApp.Services.Themes.Dtos;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Themes
{
    public class ThemeIndexViewModel
    {
        public IReadOnlyList<ThemeSummaryViewModel> Themes { get; init; } = [];

        public bool HasThemes => Themes.Count > 0;

        public static ThemeIndexViewModel From(IReadOnlyList<ThemeDto> dtos)
        {
            return new ThemeIndexViewModel
            {
                Themes = dtos.Select(ThemeSummaryViewModel.From).ToList()
            };
        }
    }
}
