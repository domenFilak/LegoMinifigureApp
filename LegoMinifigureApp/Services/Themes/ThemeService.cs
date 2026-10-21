using LegoMinifigureApp.Data;
using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Themes.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LegoMinifigureApp.Services.Themes
{
    public class ThemeService(ApplicationDbContext db, TimeProvider timeProvider) : IThemeService
    {
        public async Task<IReadOnlyList<ThemeDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await db.Themes
                .OrderBy(t => t.Name)
                .Select(t => new ThemeDto(
                    t.Id,
                    t.Name,
                    t.CreatedAt,
                    t.Minifigures.Count(m => m.Status == MinifigureStatus.Owned),
                    t.Minifigures.Count(m => m.Status != MinifigureStatus.Owned)))
                .ToListAsync(cancellationToken);
        }

        public async Task<ThemeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await db.Themes
                .Where(t => t.Id == id)
                .Select(t => new ThemeDto(
                    t.Id,
                    t.Name,
                    t.CreatedAt,
                    t.Minifigures.Count(m => m.Status == MinifigureStatus.Owned),
                    t.Minifigures.Count(m => m.Status != MinifigureStatus.Owned)))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Result> CreateAsync(string name, CancellationToken cancellationToken = default)
        {
            var normalizedName = NormalizeName(name);

            var validationError = ValidateName(normalizedName);
            if (validationError is not null)
            {
                return Result.Failure(validationError);
            }

            if (await NameExistsAsync(normalizedName, excludeId: null, cancellationToken))
            {
                return Result.Failure(ThemeErrors.DuplicateName(normalizedName));
            }

            var theme = new Theme { Name = normalizedName };
            theme.MarkCreated(GetUtcNow());

            db.Themes.Add(theme);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var theme = await db.Themes.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            if (theme is null)
            {
                return Result.Failure(ThemeErrors.NotFound(id));
            }

            //if has any figures owned or archived (once sold/traded), dont delete
            var hasFigures = await db.Minifigures.AnyAsync(m => m.ThemeId == id, cancellationToken);
            if (hasFigures)
            {
                return Result.Failure(ThemeErrors.HasFigures(theme.Name));
            }

            theme.SoftDelete(GetUtcNow());
            await db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> RenameAsync(int id, string name, CancellationToken cancellationToken = default)
        {
            var theme = await db.Themes.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            if (theme is null)
            {
                return Result.Failure(ThemeErrors.NotFound(id));
            }

            var normalizedName = NormalizeName(name);

            var validationError = ValidateName(normalizedName);
            if (validationError is not null)
            {
                return Result.Failure(validationError);
            }

            if (theme.Name == normalizedName)
            {
                return Result.Success(); //nothing changed
            }

            if (await NameExistsAsync(normalizedName, excludeId: id, cancellationToken))
            {
                return Result.Failure(ThemeErrors.DuplicateName(normalizedName));
            }

            theme.Name = normalizedName;
            theme.MarkUpdated(GetUtcNow());

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<IReadOnlyList<ThemeOptionDto>> GetOptionsAsync(CancellationToken cancellationToken = default)
        {
            return await db.Themes
                .OrderBy(t => t.Name)
                .Select(t => new ThemeOptionDto(t.Id, t.Name))
                .ToListAsync(cancellationToken);
        }

        private static string NormalizeName(string? name)
        {
            return name?.Trim() ?? string.Empty;
        }

        private static Error? ValidateName(string normalizedName)
        {
            if (normalizedName.Length == 0)
            {
                return ThemeErrors.NameRequired;
            }

            if (normalizedName.Length > Theme.NameMaxLength)
            {
                return ThemeErrors.NameTooLong;
            }

            return null;
        }

        private Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken)
        {
            return db.Themes.AnyAsync(t => t.Name == name && (excludeId == null || t.Id != excludeId), cancellationToken);
        }

        private DateTime GetUtcNow()
        {
            return timeProvider.GetUtcNow().UtcDateTime;
        }
    }
}
