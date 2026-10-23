using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Minifigures.Dtos;

namespace LegoMinifigureApp.Services.Minifigures
{
    public interface IMinifigureService
    {
        Task<PagedResult<MinifigureListItemDto>> GetOwnedPageAsync(MinifigureListQuery query, CancellationToken cancellationToken = default);

        Task<Result<int>> CreateAsync(MinifigureDetailsCommand command, CancellationToken cancellationToken = default);

        Task<MinifigureEditDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(int id, byte[] rowVersion, MinifigureDetailsCommand command, CancellationToken cancellationToken = default);

        Task<MinifigureDetailsDto?> GetDetailsAsync(int id, CancellationToken cancellationToken = default);

        Task<Result> ListForSaleAsync(int id, byte[] rowVersion, decimal askingPrice, CancellationToken cancellationToken = default);
        Task<Result> RemoveFromSaleAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> ListForTradeAsync(int id, byte[] rowVersion, string? tradeNotes, CancellationToken cancellationToken = default);
        Task<Result> RemoveFromTradeAsync(int id, CancellationToken cancellationToken = default);
    }
}
