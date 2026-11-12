using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Offers;
using LegoMinifigureApp.Services.Offers.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Offers
{
    public class OfferIndexViewModel
    {
        //filter values currently
        public OfferStatusFilter Status { get; init; }
        public OfferTypeFilter Type { get; init; }
        //all rows
        public IReadOnlyList<OfferListItemViewModel> Items { get; init; } = [];
        //paging
        public int Page { get; init; }
        public int TotalPages { get; init; }
        public int TotalCount { get; init; }
        public int FirstItemNumber { get; init; }
        public int LastItemNumber { get; init; }
        public bool HasPreviousPage { get; init; }
        public bool HasNextPage { get; init; }
        //dropdown options
        public IReadOnlyList<SelectListItem> StatusOptions { get; init; } = [];
        public IReadOnlyList<SelectListItem> TypeOptions { get; init; } = [];

        public bool HasItems => Items.Count > 0;

        //New and All are the defaults
        public bool HasActiveFilters => Status != OfferStatusFilter.New || Type != OfferTypeFilter.All;

        public Dictionary<string, string> RouteValuesForPage(int page)
        {
            var values = new Dictionary<string, string>();

            if (Status != OfferStatusFilter.New)
            {
                values["status"] = Status.ToString();
            }

            if (Type != OfferTypeFilter.All)
            {
                values["type"] = Type.ToString();
            }

            values["page"] = page.ToString();

            return values;
        }

        public static OfferIndexViewModel From(OfferFilterViewModel filter, PagedResult<OfferListItemDto> result)
        {
            return new OfferIndexViewModel
            {
                Status = filter.Status,
                Type = filter.Type,
                Items = result.Items.Select(OfferListItemViewModel.From).ToList(),
                Page = result.Page,
                TotalPages = result.TotalPages,
                TotalCount = result.TotalCount,
                FirstItemNumber = result.FirstItemNumber,
                LastItemNumber = result.LastItemNumber,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage,

                StatusOptions =
                [
                    new SelectListItem("New", nameof(OfferStatusFilter.New)),
                    new SelectListItem("Accepted", nameof(OfferStatusFilter.Accepted)),
                    new SelectListItem("Declined", nameof(OfferStatusFilter.Declined)),
                    new SelectListItem("All", nameof(OfferStatusFilter.All))
                ],

                TypeOptions =
                [
                    new SelectListItem("All types", nameof(OfferTypeFilter.All)),
                    new SelectListItem("Buy", nameof(OfferTypeFilter.Buy)),
                    new SelectListItem("Trade", nameof(OfferTypeFilter.Trade))
                ]
            };
        }
    }
}
