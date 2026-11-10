using LegoMinifigureApp.Data;
using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Offers.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace LegoMinifigureApp.Services.Offers
{
    public class OfferService(ApplicationDbContext db, TimeProvider timeProvider, ILogger<OfferService> logger) : IOfferService
    {
        //get figure details for buy form; only if still in collection and for sale
        public async Task<OfferFigureDto?> GetFigureForBuyAsync(int minifigureId, CancellationToken cancellationToken = default)
        {
            return await db.Minifigures
                .Where(m => m.Id == minifigureId && m.Status == MinifigureStatus.Owned && m.IsForSale)
                .Select(m => new OfferFigureDto(
                    m.Id,
                    m.Name,
                    m.BrickLinkId,
                    m.ImagePath,
                    m.IsForSale,
                    m.AskingPrice,
                    m.IsForTrade,
                    m.TradeNotes,
                    m.RowVersion)
                )
                .FirstOrDefaultAsync(cancellationToken);
        }

        //get figure details for trade form; only if still in the collection and for trade
        public async Task<OfferFigureDto?> GetFigureForTradeAsync(int minifigureId, CancellationToken cancellationToken = default)
        {
            return await db.Minifigures
                .Where(m => m.Id == minifigureId && m.Status == MinifigureStatus.Owned && m.IsForTrade)
                .Select(m => new OfferFigureDto(
                    m.Id,
                    m.Name,
                    m.BrickLinkId,
                    m.ImagePath,
                    m.IsForSale,
                    m.AskingPrice,
                    m.IsForTrade,
                    m.TradeNotes,
                    m.RowVersion)
                )
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Result> CreateBuyOfferAsync(BuyOfferCommand command, CancellationToken cancellationToken = default)
        {
            var figure = await db.Minifigures.FirstOrDefaultAsync(m => m.Id == command.MinifigureId, cancellationToken);
            if (figure is null)
            {
                return Result.Failure(OfferErrors.FigureNotFound(command.MinifigureId));
            }

            //checked again on send from public user: figure may have been sold or unlisted while the form was open
            if (figure.Status != MinifigureStatus.Owned || !figure.IsForSale)
            {
                return Result.Failure(OfferErrors.NotForSale);
            }

            //visitors must have seen the latest version of minifigure when sending back buy offer
            if (!figure.RowVersion.SequenceEqual(command.FigureRowVersion))
            {
                return Result.Failure(OfferErrors.ListingChanged);
            }

            var name = NormalizeText(command.Name);
            var email = NormalizeText(command.Email);

            var contactError = ValidateContact(name, email);
            if (contactError is not null)
            {
                return Result.Failure(contactError);
            }

            var price = decimal.Round(command.OfferedPrice, 2, MidpointRounding.AwayFromZero);

            if (price <= 0)
            {
                return Result.Failure(OfferErrors.OfferedPriceNotPositive);
            }

            if (price > Offer.MaxOfferedPrice)
            {
                return Result.Failure(OfferErrors.OfferedPriceTooHigh);
            }

            var message = NormalizeOptionalText(command.Message);

            if (message?.Length > Offer.MessageMaxLength)
            {
                return Result.Failure(OfferErrors.MessageTooLong);
            }

            if (await HasOpenOfferAsync(figure.Id, OfferType.Buy, email, cancellationToken))
            {
                return Result.Failure(OfferErrors.DuplicateOffer);
            }

            //asking price snapshot always comes from the database, never from the form
            var offer = Offer.CreateBuy(figure.Id, name, email, price, message, figure.AskingPrice, GetUtcNow());

            db.Offers.Add(offer);
            await db.SaveChangesAsync(cancellationToken);

            //no name or email in logs
            logger.LogInformation("Offer {OfferId} ({OfferType}) created for minifigure {MinifigureId}.", offer.Id, offer.Type, figure.Id);

            return Result.Success();
        }

        public async Task<Result> CreateTradeOfferAsync(TradeOfferCommand command, CancellationToken cancellationToken = default)
        {
            var figure = await db.Minifigures.FirstOrDefaultAsync(m => m.Id == command.MinifigureId, cancellationToken);
            if (figure is null)
            {
                return Result.Failure(OfferErrors.FigureNotFound(command.MinifigureId));
            }

            //checked again on send from public user: figure may have been sold or unlisted while the form was open
            if (figure.Status != MinifigureStatus.Owned || !figure.IsForTrade)
            {
                return Result.Failure(OfferErrors.NotForTrade);
            }

            //visitors must have seen the latest version of minifigure when sending back buy offer
            if (!figure.RowVersion.SequenceEqual(command.FigureRowVersion))
            {
                return Result.Failure(OfferErrors.ListingChanged);
            }

            var name = NormalizeText(command.Name);
            var email = NormalizeText(command.Email);

            var contactError = ValidateContact(name, email);
            if (contactError is not null)
            {
                return Result.Failure(contactError);
            }

            var message = NormalizeText(command.Message);

            if (message.Length == 0)
            {
                return Result.Failure(OfferErrors.MessageRequired);
            }

            if (message.Length > Offer.MessageMaxLength)
            {
                return Result.Failure(OfferErrors.MessageTooLong);
            }

            if (await HasOpenOfferAsync(figure.Id, OfferType.Trade, email, cancellationToken))
            {
                return Result.Failure(OfferErrors.DuplicateOffer);
            }

            //asking price snapshot always comes from the database, never from the form
            var offer = Offer.CreateTrade(figure.Id, name, email, message, figure.AskingPrice, GetUtcNow());

            db.Offers.Add(offer);
            await db.SaveChangesAsync(cancellationToken);

            //no name or email in logs
            logger.LogInformation("Offer {OfferId} ({OfferType}) created for minifigure {MinifigureId}.", offer.Id, offer.Type, figure.Id);

            return Result.Success();
        }


        private static Error? ValidateContact(string name, string email)
        {
            if (name.Length == 0)
            {
                return OfferErrors.NameRequired;
            }

            if (name.Length > Offer.NameMaxLength)
            {
                return OfferErrors.NameTooLong;
            }

            //no line breaks, tabs or other invisible control characters in a name
            if (name.Any(char.IsControl))
            {
                return OfferErrors.NameInvalid;
            }

            if (email.Length == 0)
            {
                return OfferErrors.EmailRequired;
            }

            if (email.Length > Offer.EmailMaxLength)
            {
                return OfferErrors.EmailTooLong;
            }

            if (!IsValidEmail(email))
            {
                return OfferErrors.EmailInvalid;
            }

            return null;
        }

        private static bool IsValidEmail(string email)
        {
            if (!MailAddress.TryCreate(email, out var address))
            {
                return false;
            }

            //MailAddress also accepts "Ana <ana@example.com>"; only plain address is allowed
            return address.Address == email;
        }

        //one open (New) offer per email, figure and type
        private Task<bool> HasOpenOfferAsync(int minifigureId, OfferType type, string email, CancellationToken cancellationToken)
        {
            return db.Offers.AnyAsync(o =>
                o.MinifigureId == minifigureId &&
                o.Type == type &&
                o.Status == OfferStatus.New &&
                o.Email == email,
                cancellationToken);
        }

        private static string NormalizeText(string? text)
        {
            return text?.Trim() ?? string.Empty;
        }

        private static string? NormalizeOptionalText(string? text)
        {
            return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        }

        private DateTime GetUtcNow()
        {
            return timeProvider.GetUtcNow().UtcDateTime;
        }

    }
}
