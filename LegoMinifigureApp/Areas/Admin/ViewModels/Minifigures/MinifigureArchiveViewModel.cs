using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Minifigures;
using LegoMinifigureApp.Services.Minifigures.Dtos;
using LegoMinifigureApp.Services.Themes.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureArchiveViewModel
    {
        //filter values currently
        public string? Search { get; init; }
        public int? ThemeId { get; init; }
        public ArchiveStatusFilter Status { get; init; }

        //all rows
        public IReadOnlyList<MinifigureArchiveItemViewModel> Items { get; init; } = [];

        //paging
        public int Page { get; init; }
        public int TotalPages { get; init; }
        public int TotalCount { get; init; }
        public int FirstItemNumber { get; init; }
        public int LastItemNumber { get; init; }
        public bool HasPreviousPage { get; init; }
        public bool HasNextPage { get; init; }

        //dropdown options
        public IReadOnlyList<SelectListItem> ThemeOptions { get; init; } = [];
        public IReadOnlyList<SelectListItem> StatusOptions { get; init; } = [];

        public bool HasItems => Items.Count > 0;

        public bool HasActiveFilters => !string.IsNullOrWhiteSpace(Search) || ThemeId.HasValue || Status != ArchiveStatusFilter.All;

        public Dictionary<string, string> RouteValuesForPage(int page)
        {
            var values = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                values["search"] = Search;
            }

            if (ThemeId.HasValue)
            {
                values["themeId"] = ThemeId.Value.ToString();
            }

            if (Status != ArchiveStatusFilter.All)
            {
                values["status"] = Status.ToString();
            }

            values["page"] = page.ToString();

            return values;
        }

        public static MinifigureArchiveViewModel From(
            MinifigureArchiveFilterViewModel filter,
            PagedResult<MinifigureArchiveItemDto> result,
            IReadOnlyList<ThemeOptionDto> themes)
        {
            return new MinifigureArchiveViewModel
            {
                Search = filter.Search,
                ThemeId = filter.ThemeId,
                Status = filter.Status,

                Items = result.Items.Select(MinifigureArchiveItemViewModel.From).ToList(),

                Page = result.Page,
                TotalPages = result.TotalPages,
                TotalCount = result.TotalCount,
                FirstItemNumber = result.FirstItemNumber,
                LastItemNumber = result.LastItemNumber,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage,

                ThemeOptions = themes
                    .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
                    .ToList(),

                StatusOptions =
                [
                    new SelectListItem("All archived", nameof(ArchiveStatusFilter.All)),
                    new SelectListItem("Sold", nameof(ArchiveStatusFilter.Sold)),
                    new SelectListItem("Traded", nameof(ArchiveStatusFilter.Traded))
                ]
            };
        }
    }
}
