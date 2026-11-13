using LegoMinifigureApp.Areas.Admin.ViewModels.Offers;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Services.Offers;
using LegoMinifigureApp.Services.Offers.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Admin
{
    public class OfferViewModelsTests
    {
        private static readonly DateTime Now = new DateTime(2026, 11, 12, 10, 0, 0, DateTimeKind.Utc);
        private static readonly byte[] RowVersion = [0, 0, 0, 0, 0, 0, 7, 209];

        //figure notes

        [Fact]
        public void FigureNote_OwnedFigure_IsNull()
        {
            Assert.Null(OfferBadges.FigureNote(MinifigureStatus.Owned, isDeleted: false));
        }

        [Fact]
        public void FigureNote_SoldFigure_SaysSold()
        {
            Assert.Equal("Sold", OfferBadges.FigureNote(MinifigureStatus.Sold, isDeleted: false));
        }

        [Fact]
        public void FigureNote_TradedFigure_SaysTraded()
        {
            Assert.Equal("Traded", OfferBadges.FigureNote(MinifigureStatus.Traded, isDeleted: false));
        }

        [Fact]
        public void FigureNote_DeletedFigure_SaysDeletedWhateverItsStatus()
        {
            Assert.Equal("Figure deleted", OfferBadges.FigureNote(MinifigureStatus.Sold, isDeleted: true));
        }

        //badge colors

        [Theory]
        [InlineData(OfferStatus.New, "badge-offer-new")]
        [InlineData(OfferStatus.Accepted, "badge-offer-accepted")]
        [InlineData(OfferStatus.Declined, "badge-offer-declined")]
        public void StatusClass_EachStatus_HasItsOwnColor(OfferStatus status, string expected)
        {
            Assert.Equal(expected, OfferBadges.StatusClass(status));
        }

        [Theory]
        [InlineData(OfferType.Buy, "badge-sale")]
        [InlineData(OfferType.Trade, "badge-trade")]
        public void TypeClass_UsesListingColors(OfferType type, string expected)
        {
            Assert.Equal(expected, OfferBadges.TypeClass(type));
        }

        //message preview

        [Fact]
        public void CreatePreview_ShortMessage_IsKept()
        {
            Assert.Equal("My Boba Fett", OfferListItemViewModel.CreatePreview("My Boba Fett"));
        }

        [Fact]
        public void CreatePreview_LongMessage_IsCutWithEllipsis()
        {
            var preview = OfferListItemViewModel.CreatePreview(new string('x', 100));

            Assert.Equal(OfferListItemViewModel.MessagePreviewLength + 1, preview!.Length);
            Assert.EndsWith("…", preview);
        }

        [Fact]
        public void CreatePreview_LineBreaks_BecomeSpaces()
        {
            Assert.Equal("Line one Line two", OfferListItemViewModel.CreatePreview("Line one\nLine two"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreatePreview_NoMessage_IsNull(string? message)
        {
            Assert.Null(OfferListItemViewModel.CreatePreview(message));
        }

        //list paging and filters

        [Fact]
        public void RouteValuesForPage_Defaults_ContainOnlyPage()
        {
            var model = new OfferIndexViewModel { Status = OfferStatusFilter.New, Type = OfferTypeFilter.All };

            var values = model.RouteValuesForPage(2);

            Assert.Single(values);
            Assert.Equal("2", values["page"]);
            Assert.False(model.HasActiveFilters);
        }

        [Fact]
        public void RouteValuesForPage_ChosenFilters_AreKept()
        {
            var model = new OfferIndexViewModel { Status = OfferStatusFilter.Accepted, Type = OfferTypeFilter.Buy };

            var values = model.RouteValuesForPage(3);

            Assert.Equal("Accepted", values["status"]);
            Assert.Equal("Buy", values["type"]);
            Assert.Equal("3", values["page"]);
            Assert.True(model.HasActiveFilters);
        }

        [Fact]
        public void HasActiveFilters_AllStatuses_IsTrue()
        {
            var model = new OfferIndexViewModel { Status = OfferStatusFilter.All, Type = OfferTypeFilter.All };

            Assert.True(model.HasActiveFilters);
        }

        //details: which buttons appear

        [Fact]
        public void Details_NewOffer_CanBeHandled()
        {
            var model = OfferDetailsViewModel.From(CreateDetails(OfferType.Buy, OfferStatus.New));

            Assert.True(model.CanBeHandled);
            Assert.False(model.CanMarkFigureAsSold);
        }

        [Fact]
        public void Details_AcceptedBuyOffer_CanMarkFigureAsSold()
        {
            var model = OfferDetailsViewModel.From(CreateDetails(OfferType.Buy, OfferStatus.Accepted));

            Assert.False(model.CanBeHandled);
            Assert.True(model.CanMarkFigureAsSold);
            Assert.Equal(20m, model.OfferedPrice);
        }

        [Fact]
        public void Details_DeclinedBuyOffer_CannotMarkFigureAsSold()
        {
            var model = OfferDetailsViewModel.From(CreateDetails(OfferType.Buy, OfferStatus.Declined));

            Assert.False(model.CanBeHandled);
            Assert.False(model.CanMarkFigureAsSold);
        }

        [Fact]
        public void Details_AcceptedTradeOffer_CannotMarkFigureAsSold()
        {
            var model = OfferDetailsViewModel.From(CreateDetails(OfferType.Trade, OfferStatus.Accepted));

            Assert.False(model.CanMarkFigureAsSold);
        }

        [Fact]
        public void Details_AcceptedBuyOfferForSoldFigure_CannotMarkFigureAsSold()
        {
            var model = OfferDetailsViewModel.From(CreateDetails(OfferType.Buy, OfferStatus.Accepted, MinifigureStatus.Sold));

            Assert.False(model.CanMarkFigureAsSold);
            Assert.Equal("Sold", model.FigureNote);
        }

        [Fact]
        public void Details_AcceptedBuyOfferForDeletedFigure_CannotMarkFigureAsSold()
        {
            var model = OfferDetailsViewModel.From(CreateDetails(OfferType.Buy, OfferStatus.Accepted, figureDeleted: true));

            Assert.False(model.CanMarkFigureAsSold);
            Assert.True(model.FigureIsDeleted);
        }

        //reply link

        [Fact]
        public void CreateMailtoUrl_EncodesAddressAndSubject()
        {
            var url = OfferDetailsViewModel.CreateMailtoUrl("ana@example.com", "Luke & Leia");

            Assert.Equal("mailto:ana%40example.com?subject=Your%20offer%20for%20Luke%20%26%20Leia", url);
        }

        //an offer in a known state
        private static OfferDetailsDto CreateDetails(
            OfferType type,
            OfferStatus status,
            MinifigureStatus figureStatus = MinifigureStatus.Owned,
            bool figureDeleted = false)
        {
            var isBuy = type == OfferType.Buy;
            var figureIsOwned = figureStatus == MinifigureStatus.Owned;

            return new OfferDetailsDto(
                Id: 15,
                CreatedAt: Now,
                Type: type,
                Status: status,
                StatusChangedAt: status == OfferStatus.New ? null : Now,
                Name: "Ana",
                Email: "ana@example.com",
                OfferedPrice: isBuy ? 20m : null,
                AskingPriceAtOffer: 25m,
                Message: isBuy ? null : "My Boba Fett",
                RowVersion: RowVersion,
                MinifigureId: 10,
                MinifigureName: "Luke Skywalker",
                BrickLinkId: "sw0001",
                MinifigureImagePath: null,
                MinifigureStatus: figureStatus,
                MinifigureIsDeleted: figureDeleted,
                MinifigureIsForSale: figureIsOwned,
                MinifigureAskingPrice: figureIsOwned ? 25m : null,
                MinifigureIsForTrade: false);
        }
    }
}
