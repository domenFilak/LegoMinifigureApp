using LegoMinifigureApp.Services.Minifigures;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureFilterViewModel
    {
        public string? Search { get; set; }
        public int? ThemeId { get; set; }
        public ListingFilter Listing { get; set; }
        public int Page { get; set; } = 1;

        public MinifigureListQuery ToQuery()
        {
            return new MinifigureListQuery(Search, ThemeId, Listing, Page);
        }
    }
}
