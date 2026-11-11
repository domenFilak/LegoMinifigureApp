using LegoMinifigureApp.Data;
using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Offers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LegoMinifigureApp.Services.Offers
{
    public class OfferAdminService(ApplicationDbContext db, TimeProvider timeProvider, ILogger<OfferAdminService> logger) : IOfferAdminService
    {
        public const int PageSize = 20;
        public const int RetentionMonths = 6; //how long (months) offers stay in db before deleted

        public async Task<PagedResult<OfferListItemDto>> GetPageAsync(OfferListQuery query, CancellationToken cancellationToken = default)
        {
            //offers stay visible even when their figure was deleted,
            //soft delete filter is ignored, offers own IsDeleted is checked by hand
            IQueryable<Offer> offers = db.Offers
                .IgnoreQueryFilters([QueryFilters.SoftDelete])
                .Where(o => !o.IsDeleted);

            //status filter; New by default
            offers = query.Status switch
            {
                OfferStatusFilter.New => offers.Where(o => o.Status == OfferStatus.New),
                OfferStatusFilter.Accepted => offers.Where(o => o.Status == OfferStatus.Accepted),
                OfferStatusFilter.Declined => offers.Where(o => o.Status == OfferStatus.Declined),
                _ => offers //all
            };

            //type filter
            offers = query.Type switch
            {
                OfferTypeFilter.Buy => offers.Where(o => o.Type == OfferType.Buy),
                OfferTypeFilter.Trade => offers.Where(o => o.Type == OfferType.Trade),
                _ => offers //all
            };

            var totalCount = await offers.CountAsync(cancellationToken);
            var totalPages = PagedResult<OfferListItemDto>.CalculateTotalPages(totalCount, PageSize);
            var page = Math.Clamp(query.Page, 1, totalPages);

            //newest first
            var items = await offers
                .OrderByDescending(o => o.CreatedAt)
                .ThenByDescending(o => o.Id)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(o => new OfferListItemDto(
                    o.Id,
                    o.CreatedAt,
                    o.Type,
                    o.Status,
                    o.Name,
                    o.Email,
                    o.OfferedPrice,
                    o.AskingPriceAtOffer,
                    o.Message,
                    o.MinifigureId,
                    o.Minifigure.Name,
                    o.Minifigure.ImagePath,
                    o.Minifigure.Status,
                    o.Minifigure.IsDeleted))
                .ToListAsync(cancellationToken);

            return new PagedResult<OfferListItemDto>(items, page, PageSize, totalCount);
        }

        public async Task<OfferDetailsDto?> GetDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            //same as the list: show the offer even if its figure was deleted
            return await db.Offers
                .IgnoreQueryFilters([QueryFilters.SoftDelete])
                .Where(o => o.Id == id && !o.IsDeleted)
                .Select(o => new OfferDetailsDto(
                    o.Id,
                    o.CreatedAt,
                    o.Type,
                    o.Status,
                    o.StatusChangedAt,
                    o.Name,
                    o.Email,
                    o.OfferedPrice,
                    o.AskingPriceAtOffer,
                    o.Message,
                    o.RowVersion,
                    o.MinifigureId,
                    o.Minifigure.Name,
                    o.Minifigure.BrickLinkId,
                    o.Minifigure.ImagePath,
                    o.Minifigure.Status,
                    o.Minifigure.IsDeleted,
                    o.Minifigure.IsForSale,
                    o.Minifigure.AskingPrice,
                    o.Minifigure.IsForTrade))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Result> AcceptAsync(int id, byte[] rowVersion, CancellationToken cancellationToken = default)
        {
            var offer = await db.Offers.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (offer is null)
            {
                return Result.Failure(OfferErrors.OfferNotFound(id));
            }

            if (!offer.RowVersion.SequenceEqual(rowVersion))
            {
                return Result.Failure(OfferErrors.OfferConcurrencyConflict);
            }

            if (offer.Status != OfferStatus.New)
            {
                return Result.Failure(OfferErrors.AlreadyHandled(offer.Status));
            }

            var now = GetUtcNow();
            offer.Accept(now);
            offer.MarkUpdated(now);

            db.Entry(offer).Property(o => o.RowVersion).OriginalValue = rowVersion;

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Failure(OfferErrors.OfferConcurrencyConflict);
            }

            logger.LogInformation("Offer {OfferId} accepted.", offer.Id);

            return Result.Success();
        }

        public async Task<Result> DeclineAsync(int id, byte[] rowVersion, CancellationToken cancellationToken = default)
        {
            var offer = await db.Offers.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (offer is null)
            {
                return Result.Failure(OfferErrors.OfferNotFound(id));
            }

            if (!offer.RowVersion.SequenceEqual(rowVersion))
            {
                return Result.Failure(OfferErrors.OfferConcurrencyConflict);
            }

            if (offer.Status != OfferStatus.New)
            {
                return Result.Failure(OfferErrors.AlreadyHandled(offer.Status));
            }

            var now = GetUtcNow();
            offer.Decline(now);
            offer.MarkUpdated(now);

            db.Entry(offer).Property(o => o.RowVersion).OriginalValue = rowVersion;

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Failure(OfferErrors.OfferConcurrencyConflict);
            }

            logger.LogInformation("Offer {OfferId} declined.", offer.Id);

            return Result.Success();
        }

        //soft delete; clean up job removes forever after N months period
        public async Task<Result> DeleteAsync(int id, byte[] rowVersion, CancellationToken cancellationToken = default)
        {
            var offer = await db.Offers.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (offer is null)
            {
                return Result.Failure(OfferErrors.OfferNotFound(id));
            }

            if (!offer.RowVersion.SequenceEqual(rowVersion))
            {
                return Result.Failure(OfferErrors.OfferConcurrencyConflict);
            }

            var now = GetUtcNow();
            offer.SoftDelete(now);
            offer.MarkUpdated(now);

            db.Entry(offer).Property(o => o.RowVersion).OriginalValue = rowVersion;

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Failure(OfferErrors.OfferConcurrencyConflict);
            }

            logger.LogInformation("Offer {OfferId} deleted.", offer.Id);

            return Result.Success();
        }

        //for the dashboard card
        public async Task<int> CountNewAsync(CancellationToken cancellationToken = default)
        {
            return await db.Offers.CountAsync(o => o.Status == OfferStatus.New, cancellationToken);
        }

        //permanently removes handled offers 6 months after decision, deleted ones 6 months after deletion, never answered ones 6 months after they arrived
        public async Task<int> DeleteExpiredAsync(CancellationToken cancellationToken = default)
        {
            var cutoff = GetUtcNow().AddMonths(-RetentionMonths); //from now, retention months earlier

            return await db.Offers
                .IgnoreQueryFilters([QueryFilters.SoftDelete])
                .Where(o =>
                    (o.IsDeleted && o.DeletedAt < cutoff) ||
                    (!o.IsDeleted && o.Status != OfferStatus.New && o.StatusChangedAt < cutoff) ||
                    (!o.IsDeleted && o.Status == OfferStatus.New && o.CreatedAt < cutoff))
                .ExecuteDeleteAsync(cancellationToken);
        }

        private DateTime GetUtcNow()
        {
            return timeProvider.GetUtcNow().UtcDateTime;
        }
    }
}