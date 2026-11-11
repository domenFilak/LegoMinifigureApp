using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Offers.Dtos;

namespace LegoMinifigureApp.Services.Offers
{
    public interface IOfferAdminService
    {
        Task<PagedResult<OfferListItemDto>> GetPageAsync(OfferListQuery query, CancellationToken cancellationToken = default);
        Task<OfferDetailsDto?> GetDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> AcceptAsync(int id, byte[] rowVersion, CancellationToken cancellationToken = default);
        Task<Result> DeclineAsync(int id, byte[] rowVersion, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, byte[] rowVersion, CancellationToken cancellationToken = default);
        Task<int> CountNewAsync(CancellationToken cancellationToken = default);
        Task<int> DeleteExpiredAsync(CancellationToken cancellationToken = default);
    }
}