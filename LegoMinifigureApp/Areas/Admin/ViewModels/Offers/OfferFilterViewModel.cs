using LegoMinifigureApp.Services.Offers;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Offers
{
    public class OfferFilterViewModel
    {
        public OfferStatusFilter Status { get; set; }
        public OfferTypeFilter Type { get; set; }
        public int Page { get; set; } = 1;

        public OfferListQuery ToQuery()
        {
            return new OfferListQuery(Status, Type, Page);
        }
    }
}
