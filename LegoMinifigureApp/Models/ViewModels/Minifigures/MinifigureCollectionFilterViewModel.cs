using LegoMinifigureApp.Services.Minifigures;

namespace LegoMinifigureApp.Models.ViewModels.Minifigures
{
    public class MinifigureCollectionFilterViewModel
    {
        public const int PageSize = 24; //so we have even division into rows of even number of cards

        public string? Search { get; set; }
        public int? ThemeId { get; set; }
        public ListingFilter Listing { get; set; }
        public SortingFilter Sorting { get; set; }
        public int Page { get; set; } = 1;

        public MinifigureListQuery ToQuery()
        {
            return new MinifigureListQuery(Search, ThemeId, Listing, Sorting, Page, PageSize);
        }
    }
}
