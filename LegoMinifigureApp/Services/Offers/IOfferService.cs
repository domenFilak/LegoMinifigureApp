using LegoMinifigureApp.Services.Common;
using LegoMinifigureApp.Services.Offers.Dtos;

namespace LegoMinifigureApp.Services.Offers
{
    public interface IOfferService
    {
        Task<OfferFigureDto?> GetFigureForBuyAsync(int minifigureId, CancellationToken cancellationToken = default);
        Task<OfferFigureDto?> GetFigureForTradeAsync(int minifigureId, CancellationToken cancellationToken = default);
        Task<Result> CreateBuyOfferAsync(BuyOfferCommand command, CancellationToken cancellationToken = default);
        Task<Result> CreateTradeOfferAsync(TradeOfferCommand command, CancellationToken cancellationToken = default);
    }
}
