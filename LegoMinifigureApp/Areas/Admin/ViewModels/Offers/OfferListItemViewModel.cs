using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Offers.Dtos;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Offers
{
    public class OfferListItemViewModel
    {
        public const int MessagePreviewLength = 60;

        public int Id { get; init; }
        public DateTime CreatedAt { get; init; }
        //badges
        public string TypeText { get; init; } = string.Empty;
        public string TypeBadgeClass { get; init; } = string.Empty;
        public string StatusText { get; init; } = string.Empty;
        public string StatusBadgeClass { get; init; } = string.Empty;
        //sender
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        //what is offered
        public string? OfferedPriceText { get; init; }
        public string? AskingPriceAtOfferText { get; init; }
        public string? MessagePreview { get; init; }
        //figure
        public int MinifigureId { get; init; }
        public string MinifigureName { get; init; } = string.Empty;
        public string MinifigureImageUrl { get; init; } = string.Empty;
        public string? FigureNote { get; init; }

        public static OfferListItemViewModel From(OfferListItemDto dto)
        {
            return new OfferListItemViewModel
            {
                Id = dto.Id,
                CreatedAt = dto.CreatedAt,
                TypeText = dto.Type.ToString(),
                TypeBadgeClass = OfferBadges.TypeClass(dto.Type),
                StatusText = dto.Status.ToString(),
                StatusBadgeClass = OfferBadges.StatusClass(dto.Status),
                Name = dto.Name,
                Email = dto.Email,
                OfferedPriceText = PriceText.Format(dto.OfferedPrice),
                AskingPriceAtOfferText = PriceText.Format(dto.AskingPriceAtOffer),
                MessagePreview = CreatePreview(dto.Message),
                MinifigureId = dto.MinifigureId,
                MinifigureName = dto.MinifigureName,
                MinifigureImageUrl = ImageUrls.ForMinifigureThumbnail(dto.MinifigureImagePath),
                FigureNote = OfferBadges.FigureNote(dto.MinifigureStatus, dto.MinifigureIsDeleted)
            };
        }

        //one line, at most MessagePreviewLength characters
        public static string? CreatePreview(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return null;
            }

            var oneLine = message.ReplaceLineEndings(" ");

            if (oneLine.Length <= MessagePreviewLength)
            {
                return oneLine;
            }

            return oneLine.Substring(0, MessagePreviewLength).TrimEnd() + "…";
        }
    }
}