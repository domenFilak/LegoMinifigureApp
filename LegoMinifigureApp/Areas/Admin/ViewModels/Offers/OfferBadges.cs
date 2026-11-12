using LegoMinifigureApp.Entities.Enums;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Offers
{
    //badge styles and notes shared by the offers list and details page
    public static class OfferBadges
    {
        public static string StatusClass(OfferStatus status)
        {
            return status switch
            {
                OfferStatus.New => "badge-offer-new",
                OfferStatus.Accepted => "badge-offer-accepted",
                _ => "badge-offer-declined"
            };
        }

        //same colors as listing badges: buy = sale (yellow), trade = trade (blue)
        public static string TypeClass(OfferType type)
        {
            return type == OfferType.Buy ? "badge-sale" : "badge-trade";
        }

        //why the figure may no longer be available; null when it is still in the collection
        public static string? FigureNote(MinifigureStatus status, bool isDeleted)
        {
            if (isDeleted)
            {
                return "Figure deleted";
            }

            if (status == MinifigureStatus.Sold)
            {
                return "Sold";
            }

            if (status == MinifigureStatus.Traded)
            {
                return "Traded";
            }

            return null;
        }
    }
}
