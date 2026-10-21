using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Themes.Dtos;

namespace LegoMinifigureApp.Services.Themes
{
    public interface IThemeService
    {
        Task<IReadOnlyList<ThemeDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ThemeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> CreateAsync(string name, CancellationToken cancellationToken = default);
        Task<Result> RenameAsync(int id, string name, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ThemeOptionDto>> GetOptionsAsync(CancellationToken cancellationToken = default);
    }
}
