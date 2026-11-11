using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Services.Common;

namespace LegoMinifigureApp.Services.Offers
{
    public static class OfferErrors
    {
        public static Error FigureNotFound(int id) =>
            Error.NotFound($"Minifigure {id} was not found.");

        public static Error NotForSale =>
            Error.Conflict("This figure is no longer for sale.");

        public static Error NotForTrade =>
            Error.Conflict("This figure is no longer available for trade.");

        public static Error ListingChanged =>
            Error.Conflict("This listing was just updated. Please check the details and send your offer again.");

        public static Error DuplicateOffer =>
            Error.Conflict("You already sent an offer like this for this figure. I'll reply to your e-mail soon.");

        public static Error NameRequired =>
            Error.Validation("Enter your name.", nameof(BuyOfferCommand.Name));

        public static Error NameTooLong =>
            Error.Validation($"Your name can be at most {Offer.NameMaxLength} characters.", nameof(BuyOfferCommand.Name));

        public static Error NameInvalid =>
            Error.Validation("Your name contains characters that aren't allowed.", nameof(BuyOfferCommand.Name));

        public static Error EmailRequired =>
            Error.Validation("Enter your e-mail.", nameof(BuyOfferCommand.Email));

        public static Error EmailTooLong =>
            Error.Validation($"Your e-mail can be at most {Offer.EmailMaxLength} characters.", nameof(BuyOfferCommand.Email));

        public static Error EmailInvalid =>
            Error.Validation("Enter a valid e-mail address.", nameof(BuyOfferCommand.Email));

        public static Error OfferedPriceNotPositive =>
            Error.Validation("Your offer must be greater than 0.", nameof(BuyOfferCommand.OfferedPrice));

        public static Error OfferedPriceTooHigh =>
            Error.Validation($"Your offer can be at most {Offer.MaxOfferedPrice} €.", nameof(BuyOfferCommand.OfferedPrice));

        public static Error MessageRequired =>
            Error.Validation("Tell what you offer in exchange.", nameof(TradeOfferCommand.Message));

        public static Error MessageTooLong =>
            Error.Validation($"Your message can be at most {Offer.MessageMaxLength} characters.", nameof(TradeOfferCommand.Message));

        //admin side
        public static Error OfferNotFound(int id) =>
            Error.NotFound($"Offer {id} was not found.");

        public static Error OfferConcurrencyConflict =>
            Error.Conflict("This offer was changed in the meantime. Please check it and try again.");

        public static Error AlreadyHandled(OfferStatus status) =>
            Error.Conflict($"This offer is already {status.ToString().ToLowerInvariant()}.");
    }
}
