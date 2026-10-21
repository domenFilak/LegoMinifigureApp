using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Minifigures;
using LegoMinifigureApp.Services.Minifigures.Dtos;
using LegoMinifigureApp.Services.Themes.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureIndexViewModel
    {
        //current filter values; form shows them, paging links repeat them
        public string? Search { get; init; }
        public int? ThemeId { get; init; }
        public ListingFilter Listing { get; init; }

        //rows on this page
        public IReadOnlyList<MinifigureListItemViewModel> Items { get; init; } = [];

        //info about paging
        public int Page { get; init; }
        public int TotalPages { get; init; }
        public int TotalCount { get; init; }
        public int FirstItemNumber { get; init; }
        public int LastItemNumber { get; init; }
        public bool HasPreviousPage { get; init; }
        public bool HasNextPage { get; init; }

        //dropdowns
        public IReadOnlyList<SelectListItem> ThemeOptions { get; init; } = [];
        public IReadOnlyList<SelectListItem> ListingOptions { get; init; } = [];

        public bool HasItems => Items.Count > 0;

        public bool HasActiveFilters => !string.IsNullOrWhiteSpace(Search) || ThemeId.HasValue || Listing != ListingFilter.All;

        //URL values for paging link; current filters and requested page
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

            if (Listing != ListingFilter.All)
            {
                values["listing"] = Listing.ToString();
            }

            values["page"] = page.ToString();

            return values;
        }

        public static MinifigureIndexViewModel From(
            MinifigureFilterViewModel filter,
            PagedResult<MinifigureListItemDto> result,
            IReadOnlyList<ThemeOptionDto> themes)
        {
            return new MinifigureIndexViewModel
            {
                Search = filter.Search,
                ThemeId = filter.ThemeId,
                Listing = filter.Listing,

                Items = result.Items.Select(MinifigureListItemViewModel.From).ToList(),

                Page = result.Page,
                TotalPages = result.TotalPages,
                TotalCount = result.TotalCount,
                FirstItemNumber = result.FirstItemNumber,
                LastItemNumber = result.LastItemNumber,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage,

                ThemeOptions = themes.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList(),

                ListingOptions =
                [
                    new SelectListItem("All listings", nameof(ListingFilter.All)),
                    new SelectListItem("For sale", nameof(ListingFilter.ForSale)),
                    new SelectListItem("For trade", nameof(ListingFilter.ForTrade)),
                    new SelectListItem("Not listed", nameof(ListingFilter.NotListed))
                ]
            };
        }
    }
}
