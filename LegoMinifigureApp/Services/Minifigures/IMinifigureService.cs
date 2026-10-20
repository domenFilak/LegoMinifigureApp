using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Minifigures.Dtos;

namespace LegoMinifigureApp.Services.Minifigures
{
    public interface IMinifigureService
    {
        Task<PagedResult<MinifigureListItemDto>> GetOwnedPageAsync(MinifigureListQuery query, CancellationToken cancellationToken = default);
    }
}
