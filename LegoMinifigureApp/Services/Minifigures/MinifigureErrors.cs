using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Services.Common;

namespace LegoMinifigureApp.Services.Minifigures
{
    public static class MinifigureErrors
    {
        public static Error NotFound(int id) =>
            Error.NotFound($"Minifigure {id} was not found.");

        public static Error NameRequired =>
            Error.Validation("Name is required.", nameof(MinifigureDetailsCommand.Name));

        public static Error NameTooLong =>
            Error.Validation($"Name can be at most {Minifigure.NameMaxLength} characters.", nameof(MinifigureDetailsCommand.Name));

        public static Error BrickLinkIdRequired =>
            Error.Validation("BrickLink ID is required.", nameof(MinifigureDetailsCommand.BrickLinkId));

        public static Error BrickLinkIdTooLong =>
            Error.Validation($"BrickLink ID can be at most {Minifigure.BrickLinkIdMaxLength} characters.", nameof(MinifigureDetailsCommand.BrickLinkId));

        public static Error ReleaseYearOutOfRange(int maxYear) =>
            Error.Validation($"Release year must be between {Minifigure.FirstReleaseYear} and {maxYear}.", nameof(MinifigureDetailsCommand.ReleaseYear));

        public static Error InvalidCondition =>
            Error.Validation("Choose a valid condition.", nameof(MinifigureDetailsCommand.Condition));

        public static Error DescriptionTooLong =>
            Error.Validation($"Description can be at most {Minifigure.DescriptionMaxLength} characters.", nameof(MinifigureDetailsCommand.Description));

        public static Error ThemeNotFound =>
            Error.Validation("The selected theme doesn't exist.", nameof(MinifigureDetailsCommand.ThemeId));

        public static Error ConcurrencyConflict =>
            Error.Conflict("This figure was already changed elsewhere. Reload the page and make your changes again.");

        public static Error NotOwned(MinifigureStatus status) =>
            Error.Conflict($"This figure is {status} and can't be listed.");

        public static Error AskingPriceNotPositive =>
            Error.Validation("Asking price must be greater than 0.", nameof(Minifigure.AskingPrice));

        public static Error AskingPriceTooHigh =>
            Error.Validation($"Asking price can be at most {Minifigure.MaxAskingPrice} €.", nameof(Minifigure.AskingPrice));

        public static Error TradeNotesTooLong =>
            Error.Validation($"Trade notes can be at most {Minifigure.TradeNotesMaxLength} characters.", nameof(Minifigure.TradeNotes));
    }
}
