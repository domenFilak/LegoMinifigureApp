using LegoMinifigureApp.Entities.Common;
using LegoMinifigureApp.Entities.Enums;

namespace LegoMinifigureApp.Entities
{
    public class Offer : AuditableEntity
    {
        public const int NameMaxLength = 100;
        public const int EmailMaxLength = 256;
        public const int MessageMaxLength = 1000;
        public const int MaxOfferedPrice = 10000;

        public int Id { get; private set; }

        //which figure and offer type
        public int MinifigureId { get; private set; }
        public Minifigure Minifigure { get; private set; } = null!;
        public OfferType Type { get; private set; }

        //who made the offer
        public string Name { get; private set; } = null!;
        public string Email { get; private set; } = null!;

        //what is offered
        public decimal? OfferedPrice { get; private set; } //buy only
        public string? Message { get; private set; } //required for trade (what they offer in exchange), optional for buy
        public decimal? AskingPriceAtOffer { get; private set; } //figures asking price when offer was made, null if it was not for sale

        //handling by admin
        public OfferStatus Status { get; private set; } = OfferStatus.New;
        public DateTime? StatusChangedAt { get; private set; }

        public byte[] RowVersion { get; private set; } = null!;

        //private, can only be created using CreateBuy or CreateTrade
        private Offer()
        {
        }

        public static Offer CreateBuy(
            int minifigureId,
            string name,
            string email,
            decimal offeredPrice,
            string? message,
            decimal? askingPriceAtOffer,
            DateTime nowUtc)
        {
            if (offeredPrice <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offeredPrice), "Offered price must be greater than zero.");
            }

            EnsureValidAskingPriceSnapshot(askingPriceAtOffer);

            var offer = new Offer
            {
                MinifigureId = minifigureId,
                Type = OfferType.Buy,
                Name = RequireText(name, nameof(name)),
                Email = RequireText(email, nameof(email)),
                OfferedPrice = offeredPrice,
                Message = string.IsNullOrWhiteSpace(message) ? null : message.Trim(),
                AskingPriceAtOffer = askingPriceAtOffer
            };

            offer.MarkCreated(nowUtc);

            return offer;
        }

        public static Offer CreateTrade(
            int minifigureId,
            string name,
            string email,
            string message,
            decimal? askingPriceAtOffer,
            DateTime nowUtc)
        {
            EnsureValidAskingPriceSnapshot(askingPriceAtOffer);

            var offer = new Offer
            {
                MinifigureId = minifigureId,
                Type = OfferType.Trade,
                Name = RequireText(name, nameof(name)),
                Email = RequireText(email, nameof(email)),
                OfferedPrice = null,
                Message = RequireText(message, nameof(message)),
                AskingPriceAtOffer = askingPriceAtOffer
            };

            offer.MarkCreated(nowUtc);

            return offer;
        }

        private static string RequireText(string? value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("A value is required.", parameterName);
            }

            return value.Trim();
        }

        private static void EnsureValidAskingPriceSnapshot(decimal? askingPriceAtOffer)
        {
            if (askingPriceAtOffer.HasValue && askingPriceAtOffer.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(askingPriceAtOffer), "Asking price snapshot must be greater than zero.");
            }
        }
    }
}
