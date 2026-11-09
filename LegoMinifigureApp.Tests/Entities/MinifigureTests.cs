using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Entities
{
    public class MinifigureTests
    {
        private static readonly DateTime SoldAt = new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime RestoredAt = new DateTime(2026, 11, 5, 9, 30, 0, DateTimeKind.Utc);

        //new figure
        [Fact]
        public void NewFigure_IsOwnedAndNotListed()
        {
            var figure = CreateOwnedFigure();

            Assert.Equal(MinifigureStatus.Owned, figure.Status);
            Assert.False(figure.IsArchived);
            Assert.False(figure.IsForSale);
            Assert.Null(figure.AskingPrice);
            Assert.False(figure.IsForTrade);
            Assert.Null(figure.TradeNotes);
            Assert.Null(figure.StatusChangedAt);
        }

        //listing for sale
        [Fact]
        public void ListForSale_OwnedFigure_SetsFlagAndPrice()
        {
            var figure = CreateOwnedFigure();

            figure.ListForSale(25.50m);

            Assert.True(figure.IsForSale);
            Assert.Equal(25.50m, figure.AskingPrice);
        }

        [Fact]
        public void ListForSale_AlreadyListed_UpdatesPrice()
        {
            var figure = CreateOwnedFigure();
            figure.ListForSale(25.50m);

            figure.ListForSale(30m);

            Assert.True(figure.IsForSale);
            Assert.Equal(30m, figure.AskingPrice);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void ListForSale_PriceNotPositive_Throws(double price)
        {
            var figure = CreateOwnedFigure();

            Assert.Throws<ArgumentOutOfRangeException>(() => figure.ListForSale((decimal)price));
        }

        [Fact]
        public void ListForSale_SoldFigure_Throws()
        {
            var figure = CreateSoldFigure();

            Assert.Throws<InvalidOperationException>(() => figure.ListForSale(10m));
        }

        [Fact]
        public void RemoveFromSale_ClearsFlagAndPrice()
        {
            var figure = CreateOwnedFigure();
            figure.ListForSale(25.50m);

            figure.RemoveFromSale();

            Assert.False(figure.IsForSale);
            Assert.Null(figure.AskingPrice);
        }

        //listing for trade
        [Fact]
        public void ListForTrade_WithNotes_SetsFlagAndTrimmedNotes()
        {
            var figure = CreateOwnedFigure();

            figure.ListForTrade("  Looking for Space figures  ");

            Assert.True(figure.IsForTrade);
            Assert.Equal("Looking for Space figures", figure.TradeNotes);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ListForTrade_EmptyNotes_StoresNull(string? notes)
        {
            var figure = CreateOwnedFigure();

            figure.ListForTrade(notes);

            Assert.True(figure.IsForTrade);
            Assert.Null(figure.TradeNotes);
        }

        [Fact]
        public void ListForTrade_TradedFigure_Throws()
        {
            var figure = CreateTradedFigure();

            Assert.Throws<InvalidOperationException>(() => figure.ListForTrade("anything"));
        }

        [Fact]
        public void RemoveFromTrade_ClearsFlagAndNotes()
        {
            var figure = CreateOwnedFigure();
            figure.ListForTrade("Looking for Space figures");

            figure.RemoveFromTrade();

            Assert.False(figure.IsForTrade);
            Assert.Null(figure.TradeNotes);
        }

        //marking as sold
        [Fact]
        public void MarkAsSold_OwnedFigure_SetsStatusPriceDateAndNote()
        {
            var figure = CreateOwnedFigure();

            figure.MarkAsSold(40m, "  Sold at a fair  ", SoldAt);

            Assert.Equal(MinifigureStatus.Sold, figure.Status);
            Assert.True(figure.IsArchived);
            Assert.Equal(40m, figure.SalePrice);
            Assert.Equal(SoldAt, figure.StatusChangedAt);
            Assert.Equal("Sold at a fair", figure.ArchiveNote);
        }

        [Fact]
        public void MarkAsSold_ListedFigure_RemovesBothListings()
        {
            var figure = CreateOwnedFigure();
            figure.ListForSale(25m);
            figure.ListForTrade("Looking for Space figures");

            figure.MarkAsSold(40m, null, SoldAt);

            Assert.False(figure.IsForSale);
            Assert.Null(figure.AskingPrice);
            Assert.False(figure.IsForTrade);
            Assert.Null(figure.TradeNotes);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void MarkAsSold_EmptyNote_StoresNull(string? note)
        {
            var figure = CreateOwnedFigure();

            figure.MarkAsSold(40m, note, SoldAt);

            Assert.Null(figure.ArchiveNote);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void MarkAsSold_PriceNotPositive_Throws(double price)
        {
            var figure = CreateOwnedFigure();

            Assert.Throws<ArgumentOutOfRangeException>(() => figure.MarkAsSold((decimal)price, null, SoldAt));
        }

        [Fact]
        public void MarkAsSold_PriceNotPositive_LeavesFigureUnchanged()
        {
            var figure = CreateOwnedFigure();
            figure.ListForSale(25m);

            Assert.Throws<ArgumentOutOfRangeException>(() => figure.MarkAsSold(0m, null, SoldAt));

            Assert.Equal(MinifigureStatus.Owned, figure.Status);
            Assert.True(figure.IsForSale); //listing still there
            Assert.Null(figure.SalePrice);
        }

        [Fact]
        public void MarkAsSold_AlreadySold_Throws()
        {
            var figure = CreateSoldFigure();

            Assert.Throws<InvalidOperationException>(() => figure.MarkAsSold(50m, null, SoldAt));
        }

        [Fact]
        public void MarkAsSold_AlreadyTraded_Throws()
        {
            var figure = CreateTradedFigure();

            Assert.Throws<InvalidOperationException>(() => figure.MarkAsSold(50m, null, SoldAt));
        }

        //marking as traded
        [Fact]
        public void MarkAsTraded_OwnedFigure_SetsStatusDateAndNoteWithoutPrice()
        {
            var figure = CreateOwnedFigure();

            figure.MarkAsTraded("  Swapped for a Batman  ", SoldAt);

            Assert.Equal(MinifigureStatus.Traded, figure.Status);
            Assert.True(figure.IsArchived);
            Assert.Null(figure.SalePrice);
            Assert.Equal(SoldAt, figure.StatusChangedAt);
            Assert.Equal("Swapped for a Batman", figure.ArchiveNote);
        }

        [Fact]
        public void MarkAsTraded_ListedFigure_RemovesBothListings()
        {
            var figure = CreateOwnedFigure();
            figure.ListForSale(25m);
            figure.ListForTrade("Looking for Space figures");

            figure.MarkAsTraded(null, SoldAt);

            Assert.False(figure.IsForSale);
            Assert.Null(figure.AskingPrice);
            Assert.False(figure.IsForTrade);
            Assert.Null(figure.TradeNotes);
        }

        [Fact]
        public void MarkAsTraded_AlreadySold_Throws()
        {
            var figure = CreateSoldFigure();

            Assert.Throws<InvalidOperationException>(() => figure.MarkAsTraded(null, SoldAt));
        }

        //restoring
        [Fact]
        public void Restore_SoldFigure_BecomesOwnedAndClearsSaleInfo()
        {
            var figure = CreateSoldFigure();

            figure.Restore(RestoredAt);

            Assert.Equal(MinifigureStatus.Owned, figure.Status);
            Assert.False(figure.IsArchived);
            Assert.Null(figure.SalePrice);
            Assert.Null(figure.ArchiveNote);
            Assert.Equal(RestoredAt, figure.StatusChangedAt);
        }

        [Fact]
        public void Restore_TradedFigure_BecomesOwnedAndClearsNote()
        {
            var figure = CreateTradedFigure();

            figure.Restore(RestoredAt);

            Assert.Equal(MinifigureStatus.Owned, figure.Status);
            Assert.Null(figure.ArchiveNote);
            Assert.Equal(RestoredAt, figure.StatusChangedAt);
        }

        [Fact]
        public void Restore_SoldFigure_ComesBackNotListed()
        {
            var figure = CreateOwnedFigure();
            figure.ListForSale(25m);
            figure.MarkAsSold(40m, null, SoldAt);

            figure.Restore(RestoredAt);

            Assert.False(figure.IsForSale);
            Assert.False(figure.IsForTrade);
        }

        [Fact]
        public void Restore_OwnedFigure_ChangesNothing()
        {
            var figure = CreateOwnedFigure();

            figure.Restore(RestoredAt);

            Assert.Equal(MinifigureStatus.Owned, figure.Status);
            Assert.Null(figure.StatusChangedAt); //no status change happened
        }

        [Fact]
        public void Restore_RestoredFigure_CanBeListedAgain()
        {
            var figure = CreateSoldFigure();
            figure.Restore(RestoredAt);

            figure.ListForSale(30m);

            Assert.True(figure.IsForSale);
        }

        //helpers
        private static Minifigure CreateOwnedFigure()
        {
            return new Minifigure
            {
                Name = "Luke Skywalker",
                BrickLinkId = "sw0001",
                ThemeId = 1,
                Condition = MinifigureCondition.Used
            };
        }

        private static Minifigure CreateSoldFigure()
        {
            var figure = CreateOwnedFigure();
            figure.MarkAsSold(40m, "Sold at a fair", SoldAt);
            return figure;
        }

        private static Minifigure CreateTradedFigure()
        {
            var figure = CreateOwnedFigure();
            figure.MarkAsTraded("Swapped for a Batman", SoldAt);
            return figure;
        }
    }
}
