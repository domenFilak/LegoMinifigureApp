using LegoMinifigureApp.Data;
using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Minifigures.Dtos;
using LegoMinifigureApp.Services.Storage;
using Microsoft.EntityFrameworkCore;

namespace LegoMinifigureApp.Services.Minifigures
{
    public class MinifigureService(
        ApplicationDbContext db,
        TimeProvider timeProvider,
        IImageStorageService _imageStorageService) : IMinifigureService
    {
        public const int PageSize = 20;
        private const string ImageFolder = "minifigures";

        public async Task<PagedResult<MinifigureListItemDto>> GetOwnedPageAsync(MinifigureListQuery query, CancellationToken cancellationToken = default)
        {
            //build query start - owned figures only, soft deleted filtered out automatically
            IQueryable<Minifigure> figures = db.Minifigures.Where(m => m.Status == MinifigureStatus.Owned);

            //apply search
            var search = query.Search?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                figures = figures.Where(m => m.Name.Contains(search) || m.BrickLinkId.Contains(search));
            }

            //apply theme
            if (query.ThemeId.HasValue)
            {
                var themeId = query.ThemeId.Value;
                figures = figures.Where(m => m.ThemeId == themeId);
            }

            //apply listing choice
            figures = query.Listing switch
            {
                ListingFilter.ForSale => figures.Where(m => m.IsForSale),
                ListingFilter.ForTrade => figures.Where(m => m.IsForTrade),
                ListingFilter.NotListed => figures.Where(m => !m.IsForSale && !m.IsForTrade),
                _ => figures //all, nothing applied
            };

            //count everything that matches, then fix the page number
            var totalCount = await figures.CountAsync(cancellationToken);
            var totalPages = PagedResult<MinifigureListItemDto>.CalculateTotalPages(totalCount, PageSize);
            var page = Math.Clamp(query.Page, 1, totalPages);

            //load only this page and needed columns
            var items = await figures
                .OrderBy(m => m.Name)
                .ThenBy(m => m.Id)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(m => new MinifigureListItemDto(
                    m.Id,
                    m.Name,
                    m.BrickLinkId,
                    m.Theme.Name,
                    m.Condition,
                    m.ImagePath,
                    m.IsForSale,
                    m.AskingPrice,
                    m.IsForTrade,
                    m.CreatedAt))
                .ToListAsync(cancellationToken);

            return new PagedResult<MinifigureListItemDto>(items, page, PageSize, totalCount);
        }

        public async Task<MinifigureEditDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
        {
            return await db.Minifigures
                .Where(m => m.Id == id)
                .Select(m => new MinifigureEditDto(
                    m.Id,
                    m.Name,
                    m.BrickLinkId,
                    m.ReleaseYear,
                    m.ThemeId,
                    m.Condition,
                    m.Description,
                    m.ImagePath,
                    m.RowVersion))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Result> UpdateAsync(int id, byte[] rowVersion, MinifigureDetailsCommand command, CancellationToken cancellationToken = default)
        {
            var figure = await db.Minifigures.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
            if (figure is null)
            {
                return Result.Failure(MinifigureErrors.NotFound(id));
            }

            //check if figure was changed since the form was opened
            if (!figure.RowVersion.SequenceEqual(rowVersion))
            {
                return Result.Failure(MinifigureErrors.ConcurrencyConflict);
            }

            var details = Normalize(command);

            var validationError = await ValidateAsync(details, cancellationToken);
            if (validationError is not null)
            {
                return Result.Failure(validationError);
            }

            var oldImagePath = figure.ImagePath;
            string? newImagePath = null;
            if (details.Image is not null)
            {
                newImagePath = await _imageStorageService.SaveAsync(details.Image, ImageFolder, cancellationToken);
            }

            ApplyDetails(figure, details);

            if (newImagePath is not null)
            {
                figure.ImagePath = newImagePath; //new upload over remove image
            }
            else if (details.RemoveImage)
            {
                figure.ImagePath = null;
            }

            figure.MarkUpdated(GetUtcNow());

            db.Entry(figure).Property(m => m.RowVersion).OriginalValue = rowVersion;

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _imageStorageService.Delete(newImagePath);
                return Result.Failure(MinifigureErrors.ConcurrencyConflict);
            }
            catch
            {
                _imageStorageService.Delete(newImagePath);
                throw;
            }

            //after successful save, remove image that is no longer used
            if (oldImagePath is not null && oldImagePath != figure.ImagePath)
            {
                _imageStorageService.Delete(oldImagePath);
            }

            return Result.Success();
        }

        public async Task<Result> CreateAsync(MinifigureDetailsCommand command, CancellationToken cancellationToken = default)
        {
            var details = Normalize(command);

            var validationError = await ValidateAsync(details, cancellationToken);
            if (validationError is not null)
            {
                return Result.Failure(validationError);
            }

            string? newImagePath = null;
            if (details.Image is not null)
            {
                newImagePath = await _imageStorageService.SaveAsync(details.Image, ImageFolder, cancellationToken);
            }

            var figure = new Minifigure();
            ApplyDetails(figure, details);
            figure.ImagePath = newImagePath;
            figure.MarkCreated(GetUtcNow());

            db.Minifigures.Add(figure);

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                _imageStorageService.Delete(newImagePath); //figure was not saved, image should not be saved on disk as well
                throw;
            }

            return Result.Success();
        }

        private static MinifigureDetailsCommand Normalize(MinifigureDetailsCommand command)
        {
            return command with
            {
                Name = command.Name?.Trim() ?? string.Empty,
                BrickLinkId = command.BrickLinkId?.Trim().ToLowerInvariant() ?? string.Empty,
                Description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim()
            };
        }

        private async Task<Error?> ValidateAsync(MinifigureDetailsCommand details, CancellationToken cancellationToken)
        {
            if (details.Name.Length == 0)
            {
                return MinifigureErrors.NameRequired;
            }

            if (details.Name.Length > Minifigure.NameMaxLength)
            {
                return MinifigureErrors.NameTooLong;
            }

            if (details.BrickLinkId.Length == 0)
            {
                return MinifigureErrors.BrickLinkIdRequired;
            }

            if (details.BrickLinkId.Length > Minifigure.BrickLinkIdMaxLength)
            {
                return MinifigureErrors.BrickLinkIdTooLong;
            }

            if (details.ReleaseYear.HasValue)
            {
                var maxYear = GetUtcNow().Year + Minifigure.ReleaseYearMaxYearsAhead;
                if (details.ReleaseYear < Minifigure.FirstReleaseYear || details.ReleaseYear > maxYear)
                {
                    return MinifigureErrors.ReleaseYearOutOfRange(maxYear);
                }
            }

            if (!Enum.IsDefined(details.Condition))
            {
                return MinifigureErrors.InvalidCondition;
            }

            if (details.Description?.Length > Minifigure.DescriptionMaxLength)
            {
                return MinifigureErrors.DescriptionTooLong;
            }

            var themeExists = await db.Themes.AnyAsync(t => t.Id == details.ThemeId, cancellationToken);
            if (!themeExists)
            {
                return MinifigureErrors.ThemeNotFound;
            }

            if (details.Image is not null)
            {
                var imageError = await _imageStorageService.ValidateAsync(details.Image, cancellationToken);
                if (imageError is not null)
                {
                    return imageError with { Field = nameof(MinifigureDetailsCommand.Image) };
                }
            }

            return null;
        }

        private static void ApplyDetails(Minifigure figure, MinifigureDetailsCommand details)
        {
            figure.Name = details.Name;
            figure.BrickLinkId = details.BrickLinkId;
            figure.ReleaseYear = details.ReleaseYear;
            figure.ThemeId = details.ThemeId;
            figure.Condition = details.Condition;
            figure.Description = details.Description;
        }

        private DateTime GetUtcNow()
        {
            return timeProvider.GetUtcNow().UtcDateTime;
        }
    }
}
