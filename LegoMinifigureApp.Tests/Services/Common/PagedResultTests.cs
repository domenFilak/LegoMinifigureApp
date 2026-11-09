using LegoMinifigureApp.Services.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Services.Common
{
    public class PagedResultTests
    {
        private const int PageSize = 24;

        [Theory]
        [InlineData(0, 1)]   //empty list is still "page 1 of 1"
        [InlineData(1, 1)]
        [InlineData(24, 1)]  //exactly one full page
        [InlineData(25, 2)]  //one more item starts a new page
        [InlineData(87, 4)]
        public void CalculateTotalPages_ReturnsCorrectPageCount(int totalCount, int expectedPages)
        {
            var pages = PagedResult<int>.CalculateTotalPages(totalCount, PageSize);

            Assert.Equal(expectedPages, pages);
        }

        [Fact]
        public void FirstPage_ShowsItemsOneToPageSize_AndHasOnlyNext()
        {
            var result = new PagedResult<int>(CreateItems(24), page: 1, PageSize, totalCount: 87);

            Assert.Equal(1, result.FirstItemNumber);
            Assert.Equal(24, result.LastItemNumber);
            Assert.False(result.HasPreviousPage);
            Assert.True(result.HasNextPage);
        }

        [Fact]
        public void MiddlePage_ShowsCorrectRange_AndHasBothDirections()
        {
            var result = new PagedResult<int>(CreateItems(24), page: 2, PageSize, totalCount: 87);

            Assert.Equal(25, result.FirstItemNumber);
            Assert.Equal(48, result.LastItemNumber);
            Assert.True(result.HasPreviousPage);
            Assert.True(result.HasNextPage);
        }

        [Fact]
        public void LastPage_ShowsRemainingItems_AndHasOnlyPrevious()
        {
            //87 items: pages of 24, 24, 24 and 15
            var result = new PagedResult<int>(CreateItems(15), page: 4, PageSize, totalCount: 87);

            Assert.Equal(4, result.TotalPages);
            Assert.Equal(73, result.FirstItemNumber);
            Assert.Equal(87, result.LastItemNumber);
            Assert.True(result.HasPreviousPage);
            Assert.False(result.HasNextPage);
        }

        [Fact]
        public void EmptyResult_IsPageOneOfOne_WithNoItemNumbers()
        {
            var result = new PagedResult<int>(CreateItems(0), page: 1, PageSize, totalCount: 0);

            Assert.Equal(1, result.TotalPages);
            Assert.Equal(0, result.FirstItemNumber);
            Assert.Equal(0, result.LastItemNumber);
            Assert.False(result.HasPreviousPage);
            Assert.False(result.HasNextPage);
        }

        //the content does not matter for paging, only how many items there are
        private static List<int> CreateItems(int count)
        {
            return Enumerable.Range(1, count).ToList();
        }
    }
}
