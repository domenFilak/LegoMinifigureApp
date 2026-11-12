using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Offers.Dtos;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Offers
{
    public class OfferDetailsViewModel
    {
        public int Id { get; init; }
        public string RowVersion { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        //badges and status
        public string TypeText { get; init; } = string.Empty;
        public string TypeBadgeClass { get; init; } = string.Empty;
        public string StatusText { get; init; } = string.Empty;
        public string StatusBadgeClass { get; init; } = string.Empty;
        public DateTime? StatusChangedAt { get; init; }
        public bool CanBeHandled { get; init; } //only new offers can be accepted or declined
        //sender
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string MailtoUrl { get; init; } = string.Empty;
        //what is offered
        public decimal? OfferedPrice { get; init; }
        public string? OfferedPriceText { get; init; }
        public string? AskingPriceAtOfferText { get; init; } //price then
        public string? Message { get; init; }
        //figure, as it is now
        public int MinifigureId { get; init; }
        public string MinifigureName { get; init; } = string.Empty;
        public string BrickLinkId { get; init; } = string.Empty;
        public string MinifigureImageUrl { get; init; } = string.Empty;
        public bool FigureIsDeleted { get; init; }
        public string? FigureNote { get; init; }
        public string? CurrentListingText { get; init; } //price now
        public bool CanMarkFigureAsSold { get; init; }

        public static OfferDetailsViewModel From(OfferDetailsDto dto)
        {
            var isBuy = dto.Type == OfferType.Buy;
            var figureIsInCollection = !dto.MinifigureIsDeleted && dto.MinifigureStatus == MinifigureStatus.Owned;

            return new OfferDetailsViewModel
            {
                Id = dto.Id,
                RowVersion = RowVersionText.Encode(dto.RowVersion),
                CreatedAt = dto.CreatedAt,

                TypeText = dto.Type.ToString(),
                TypeBadgeClass = OfferBadges.TypeClass(dto.Type),
                StatusText = dto.Status.ToString(),
                StatusBadgeClass = OfferBadges.StatusClass(dto.Status),
                StatusChangedAt = dto.StatusChangedAt,
                CanBeHandled = dto.Status == OfferStatus.New,

                Name = dto.Name,
                Email = dto.Email,
                MailtoUrl = CreateMailtoUrl(dto.Email, dto.MinifigureName),

                OfferedPrice = dto.OfferedPrice,
                OfferedPriceText = PriceText.Format(dto.OfferedPrice),
                AskingPriceAtOfferText = PriceText.Format(dto.AskingPriceAtOffer),
                Message = dto.Message,

                MinifigureId = dto.MinifigureId,
                MinifigureName = dto.MinifigureName,
                BrickLinkId = dto.BrickLinkId,
                MinifigureImageUrl = ImageUrls.ForMinifigureThumbnail(dto.MinifigureImagePath),
                FigureIsDeleted = dto.MinifigureIsDeleted,
                FigureNote = OfferBadges.FigureNote(dto.MinifigureStatus, dto.MinifigureIsDeleted),
                CurrentListingText = ListingText.Describe(dto.MinifigureIsForSale, dto.MinifigureAskingPrice, dto.MinifigureIsForTrade),

                //accepted buy offer for a figure still in the collection
                CanMarkFigureAsSold = isBuy && dto.Status == OfferStatus.Accepted && figureIsInCollection
            };
        }

        //opens the mail program with address and subject filled in
        public static string CreateMailtoUrl(string email, string figureName)
        {
            var subject = $"Your offer for {figureName}";

            return $"mailto:{Uri.EscapeDataString(email)}?subject={Uri.EscapeDataString(subject)}";
        }
    }
}
