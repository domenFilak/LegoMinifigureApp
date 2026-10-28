using LegoMinifigureApp.Services.Minifigures;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureArchiveFilterViewModel
    {
        public string? Search { get; set; }
        public int? ThemeId { get; set; }
        public ArchiveStatusFilter Status { get; set; }
        public int Page { get; set; } = 1;

        public MinifigureArchiveQuery ToQuery()
        {
            return new MinifigureArchiveQuery(Search, ThemeId, Status, Page);
        }
    }
}
