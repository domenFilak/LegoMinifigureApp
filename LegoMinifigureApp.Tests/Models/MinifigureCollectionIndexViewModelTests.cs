using LegoMinifigureApp.Models.ViewModels.Minifigures;
using LegoMinifigureApp.Services.Minifigures;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Models
{
    public class MinifigureCollectionIndexViewModelTests
    {
        //paging links

        [Fact]
        public void RouteValuesForPage_NoFilters_ContainsOnlyPage()
        {
            var model = new MinifigureCollectionIndexViewModel();

            var values = model.RouteValuesForPage(2);

            Assert.Single(values);
            Assert.Equal("2", values["page"]);
        }

        [Fact]
        public void RouteValuesForPage_AllFilters_KeepsEveryFilter()
        {
            var model = new MinifigureCollectionIndexViewModel
            {
                Search = "luke",
                ThemeId = 5,
                Listing = ListingFilter.ForSale,
                Sorting = SortingFilter.DateOfAddition
            };

            var values = model.RouteValuesForPage(3);

            Assert.Equal(5, values.Count);
            Assert.Equal("luke", values["search"]);
            Assert.Equal("5", values["themeId"]);
            Assert.Equal("ForSale", values["listing"]);
            Assert.Equal("DateOfAddition", values["sorting"]);
            Assert.Equal("3", values["page"]);
        }

        [Fact]
        public void RouteValuesForPage_DefaultValues_AreLeftOutOfTheUrl()
        {
            var model = new MinifigureCollectionIndexViewModel
            {
                Listing = ListingFilter.All,
                Sorting = SortingFilter.Name
            };

            var values = model.RouteValuesForPage(1);

            Assert.False(values.ContainsKey("listing"));
            Assert.False(values.ContainsKey("sorting"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void RouteValuesForPage_EmptySearch_IsLeftOutOfTheUrl(string? search)
        {
            var model = new MinifigureCollectionIndexViewModel { Search = search };

            var values = model.RouteValuesForPage(1);

            Assert.False(values.ContainsKey("search"));
        }

        //clear filters button

        [Fact]
        public void HasActiveFilters_NothingChosen_IsFalse()
        {
            var model = new MinifigureCollectionIndexViewModel();

            Assert.False(model.HasActiveFilters);
        }

        [Fact]
        public void HasActiveFilters_SearchTyped_IsTrue()
        {
            var model = new MinifigureCollectionIndexViewModel { Search = "luke" };

            Assert.True(model.HasActiveFilters);
        }

        [Fact]
        public void HasActiveFilters_ThemeChosen_IsTrue()
        {
            var model = new MinifigureCollectionIndexViewModel { ThemeId = 5 };

            Assert.True(model.HasActiveFilters);
        }

        [Fact]
        public void HasActiveFilters_ListingChosen_IsTrue()
        {
            var model = new MinifigureCollectionIndexViewModel { Listing = ListingFilter.ForTrade };

            Assert.True(model.HasActiveFilters);
        }

        [Fact]
        public void HasActiveFilters_NonDefaultSorting_IsTrue()
        {
            var model = new MinifigureCollectionIndexViewModel { Sorting = SortingFilter.DateOfAddition };

            Assert.True(model.HasActiveFilters);
        }

        [Fact]
        public void HasActiveFilters_OnlySpacesInSearch_IsFalse()
        {
            var model = new MinifigureCollectionIndexViewModel { Search = "   " };

            Assert.False(model.HasActiveFilters);
        }
    }
}
