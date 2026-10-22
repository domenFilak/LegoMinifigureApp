using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Minifigures.Dtos;

namespace LegoMinifigureApp.Services.Minifigures
{
    public interface IMinifigureService
    {
        Task<PagedResult<MinifigureListItemDto>> GetOwnedPageAsync(MinifigureListQuery query, CancellationToken cancellationToken = default);
        
        Task<Result> CreateAsync(MinifigureDetailsCommand command, CancellationToken cancellationToken = default);

        Task<MinifigureEditDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(int id, byte[] rowVersion, MinifigureDetailsCommand command, CancellationToken cancellationToken = default);
    }
}
