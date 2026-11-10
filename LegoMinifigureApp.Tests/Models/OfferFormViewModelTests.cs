using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Models.ViewModels.Offers;
using LegoMinifigureApp.Services.Offers.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Models
{
    public class OfferFormViewModelTests
    {
        private static readonly byte[] RowVersion = [0, 0, 0, 0, 0, 0, 7, 209];

        //figure card
        [Fact]
        public void FigureCard_ForSaleAndForTrade_ShowsBothListings()
        {
            var figure = new OfferFigureDto(10, "Luke Skywalker", "sw0001", null, true, 25m, true, "Looking for Space figures", RowVersion);

            var card = OfferFigureViewModel.From(figure);

            Assert.True(card.IsForSale);
            Assert.Equal("25.00 €", card.AskingPriceText);
            Assert.True(card.IsForTrade);
            Assert.Equal("Looking for Space figures", card.TradeNotes);
        }

        [Fact]
        public void FigureCard_ForTradeWithoutNotes_IsStillMarkedForTrade()
        {
            var figure = new OfferFigureDto(10, "Luke Skywalker", "sw0001", null, false, null, true, null, RowVersion);

            var card = OfferFigureViewModel.From(figure);

            Assert.True(card.IsForTrade);
            Assert.Null(card.TradeNotes);
        }

        [Fact]
        public void FigureCard_OnlyForSale_IsNotMarkedForTrade()
        {
            var figure = new OfferFigureDto(10, "Luke Skywalker", "sw0001", null, true, 25m, false, null, RowVersion);

            var card = OfferFigureViewModel.From(figure);

            Assert.True(card.IsForSale);
            Assert.False(card.IsForTrade);
        }

        [Fact]
        public void FigureCard_UsesThumbnailImage()
        {
            var figure = new OfferFigureDto(10, "Luke Skywalker", "sw0001", "uploads/minifigures/abc.jpg", true, 25m, false, null, RowVersion);

            var card = OfferFigureViewModel.From(figure);

            Assert.Equal("/uploads/minifigures/thumbs/abc.webp", card.ImageUrl);
        }

        //buy form

        [Fact]
        public void BuyFrom_PrefillsAskingPriceAndRowVersion()
        {
            var figure = new OfferFigureDto(10, "Luke Skywalker", "sw0001", null, true, 25m, false, null, RowVersion);

            var model = BuyOfferFormViewModel.From(figure);

            Assert.Equal(25m, model.OfferedPrice);
            Assert.Equal(RowVersionText.Encode(RowVersion), model.FigureRowVersion);
            Assert.Equal(10, model.Figure.Id);
            Assert.False(model.ListingWasUpdated);
        }

        [Fact]
        public void BuyFrom_StartsWithEmptyContactFieldsAndNoHoneypotValue()
        {
            var figure = new OfferFigureDto(10, "Luke Skywalker", "sw0001", null, true, 25m, false, null, RowVersion);

            var model = BuyOfferFormViewModel.From(figure);

            Assert.Equal(string.Empty, model.Name);
            Assert.Equal(string.Empty, model.Email);
            Assert.Null(model.Message);
            Assert.Null(model.Website);
        }

        [Fact]
        public void BuyToCommand_UsesRouteIdAndGivenRowVersion()
        {
            var model = new BuyOfferFormViewModel
            {
                Name = "Ana",
                Email = "ana@example.com",
                OfferedPrice = 20m,
                Message = "Hi"
            };

            var command = model.ToCommand(10, RowVersion);

            Assert.Equal(10, command.MinifigureId);
            Assert.Equal(RowVersion, command.FigureRowVersion);
            Assert.Equal("Ana", command.Name);
            Assert.Equal("ana@example.com", command.Email);
            Assert.Equal(20m, command.OfferedPrice);
            Assert.Equal("Hi", command.Message);
        }

        //trade form

        [Fact]
        public void TradeFrom_StartsWithEmptyMessageAndRowVersion()
        {
            var figure = new OfferFigureDto(10, "Luke Skywalker", "sw0001", null, false, null, true, "Looking for Space figures", RowVersion);

            var model = TradeOfferFormViewModel.From(figure);

            Assert.Equal(string.Empty, model.Message);
            Assert.Equal(RowVersionText.Encode(RowVersion), model.FigureRowVersion);
            Assert.Equal("Looking for Space figures", model.Figure.TradeNotes);
            Assert.False(model.ListingWasUpdated);
        }

        [Fact]
        public void TradeToCommand_UsesRouteIdAndGivenRowVersion()
        {
            var model = new TradeOfferFormViewModel
            {
                Name = "Ana",
                Email = "ana@example.com",
                Message = "My Boba Fett for your Luke"
            };

            var command = model.ToCommand(10, RowVersion);

            Assert.Equal(10, command.MinifigureId);
            Assert.Equal(RowVersion, command.FigureRowVersion);
            Assert.Equal("Ana", command.Name);
            Assert.Equal("ana@example.com", command.Email);
            Assert.Equal("My Boba Fett for your Luke", command.Message);
        }
    }
}
