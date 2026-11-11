using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Entities
{
    public class OfferTests
    {
        private static readonly DateTime Now = new DateTime(2026, 11, 9, 10, 0, 0, DateTimeKind.Utc);

        //buy offers
        [Fact]
        public void CreateBuy_ValidInput_CreatesNewBuyOffer()
        {
            var offer = Offer.CreateBuy(7, "  Ana  ", "  ana@example.com  ", 20m, "  Can pick it up  ", 25m, Now);

            Assert.Equal(7, offer.MinifigureId);
            Assert.Equal(OfferType.Buy, offer.Type);
            Assert.Equal("Ana", offer.Name);
            Assert.Equal("ana@example.com", offer.Email);
            Assert.Equal(20m, offer.OfferedPrice);
            Assert.Equal("Can pick it up", offer.Message);
            Assert.Equal(25m, offer.AskingPriceAtOffer);
            Assert.Equal(OfferStatus.New, offer.Status);
            Assert.Null(offer.StatusChangedAt);
            Assert.Equal(Now, offer.CreatedAt);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateBuy_EmptyMessage_StoresNull(string? message)
        {
            var offer = Offer.CreateBuy(7, "Ana", "ana@example.com", 20m, message, 25m, Now);

            Assert.Null(offer.Message);
        }

        [Fact]
        public void CreateBuy_FigureNotForSale_StoresNoAskingPriceSnapshot()
        {
            var offer = Offer.CreateBuy(7, "Ana", "ana@example.com", 20m, null, null, Now);

            Assert.Null(offer.AskingPriceAtOffer);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void CreateBuy_PriceNotPositive_Throws(double price)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Offer.CreateBuy(7, "Ana", "ana@example.com", (decimal)price, null, 25m, Now));
        }

        //trade offers

        [Fact]
        public void CreateTrade_ValidInput_CreatesNewTradeOfferWithoutPrice()
        {
            var offer = Offer.CreateTrade(7, "Ana", "ana@example.com", "  My Boba Fett for your Luke  ", null, Now);

            Assert.Equal(OfferType.Trade, offer.Type);
            Assert.Null(offer.OfferedPrice);
            Assert.Equal("My Boba Fett for your Luke", offer.Message);
            Assert.Equal(OfferStatus.New, offer.Status);
            Assert.Equal(Now, offer.CreatedAt);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateTrade_EmptyMessage_Throws(string? message)
        {
            Assert.Throws<ArgumentException>(() =>
                Offer.CreateTrade(7, "Ana", "ana@example.com", message!, null, Now));
        }

        //rules for both types

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_EmptyName_Throws(string? name)
        {
            Assert.Throws<ArgumentException>(() =>
                Offer.CreateBuy(7, name!, "ana@example.com", 20m, null, 25m, Now));

            Assert.Throws<ArgumentException>(() =>
                Offer.CreateTrade(7, name!, "ana@example.com", "My Boba Fett", null, Now));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_EmptyEmail_Throws(string? email)
        {
            Assert.Throws<ArgumentException>(() =>
                Offer.CreateBuy(7, "Ana", email!, 20m, null, 25m, Now));

            Assert.Throws<ArgumentException>(() =>
                Offer.CreateTrade(7, "Ana", email!, "My Boba Fett", null, Now));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_AskingPriceSnapshotNotPositive_Throws(double snapshot)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Offer.CreateBuy(7, "Ana", "ana@example.com", 20m, null, (decimal)snapshot, Now));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Offer.CreateTrade(7, "Ana", "ana@example.com", "My Boba Fett", (decimal)snapshot, Now));
        }

        //handling by admin
        [Fact]
        public void Accept_NewOffer_SetsStatusAndDate()
        {
            var offer = Offer.CreateBuy(7, "Ana", "ana@example.com", 20m, null, 25m, Now);
            var acceptedAt = Now.AddDays(1);

            offer.Accept(acceptedAt);

            Assert.Equal(OfferStatus.Accepted, offer.Status);
            Assert.Equal(acceptedAt, offer.StatusChangedAt);
        }

        [Fact]
        public void Decline_NewOffer_SetsStatusAndDate()
        {
            var offer = Offer.CreateTrade(7, "Ana", "ana@example.com", "My Boba Fett", null, Now);
            var declinedAt = Now.AddDays(1);

            offer.Decline(declinedAt);

            Assert.Equal(OfferStatus.Declined, offer.Status);
            Assert.Equal(declinedAt, offer.StatusChangedAt);
        }

        [Fact]
        public void Accept_AlreadyAccepted_Throws()
        {
            var offer = Offer.CreateBuy(7, "Ana", "ana@example.com", 20m, null, 25m, Now);
            offer.Accept(Now);

            Assert.Throws<InvalidOperationException>(() => offer.Accept(Now));
        }

        [Fact]
        public void Decline_AlreadyAccepted_Throws()
        {
            var offer = Offer.CreateBuy(7, "Ana", "ana@example.com", 20m, null, 25m, Now);
            offer.Accept(Now);

            Assert.Throws<InvalidOperationException>(() => offer.Decline(Now));
        }

        [Fact]
        public void Accept_AlreadyDeclined_Throws()
        {
            var offer = Offer.CreateBuy(7, "Ana", "ana@example.com", 20m, null, 25m, Now);
            offer.Decline(Now);

            Assert.Throws<InvalidOperationException>(() => offer.Accept(Now));
        }
    }
}
