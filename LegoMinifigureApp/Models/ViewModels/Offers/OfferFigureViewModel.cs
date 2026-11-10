using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Offers.Dtos;

namespace LegoMinifigureApp.Models.ViewModels.Offers
{
    //the figure card next to an offer form
    public class OfferFigureViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string BrickLinkId { get; init; } = string.Empty;
        public string ImageUrl { get; init; } = string.Empty;
        public bool IsForSale { get; init; }
        public string? AskingPriceText { get; init; }
        public bool IsForTrade { get; init; }
        public string? TradeNotes { get; init; }

        public static OfferFigureViewModel From(OfferFigureDto dto)
        {
            return new OfferFigureViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                BrickLinkId = dto.BrickLinkId,
                ImageUrl = ImageUrls.ForMinifigureThumbnail(dto.ImagePath),
                IsForSale = dto.IsForSale,
                AskingPriceText = PriceText.Format(dto.AskingPrice),
                IsForTrade = dto.IsForTrade,
                TradeNotes = dto.TradeNotes
            };
        }
    }
}