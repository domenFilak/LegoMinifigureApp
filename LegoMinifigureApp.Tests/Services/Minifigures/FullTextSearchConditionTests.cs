using LegoMinifigureApp.Services.Minifigures;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Services.Minifigures
{
    public class FullTextSearchConditionTests
    {
        //normal searches
        [Fact]
        public void Build_OneWord_ReturnsPrefixTerm()
        {
            var condition = FullTextSearchCondition.Build("luke");

            Assert.Equal("\"luke*\"", condition);
        }

        [Fact]
        public void Build_TwoWords_JoinsWithAnd()
        {
            var condition = FullTextSearchCondition.Build("luke hoth");

            Assert.Equal("\"luke*\" AND \"hoth*\"", condition);
        }

        [Fact]
        public void Build_ExtraSpaces_AreIgnored()
        {
            var condition = FullTextSearchCondition.Build("   luke    hoth   ");

            Assert.Equal("\"luke*\" AND \"hoth*\"", condition);
        }

        [Fact]
        public void Build_Numbers_AreKept()
        {
            var condition = FullTextSearchCondition.Build("12");

            Assert.Equal("\"12*\"", condition);
        }

        [Fact]
        public void Build_LettersWithAccents_AreKept()
        {
            var condition = FullTextSearchCondition.Build("Čebelica");

            Assert.Equal("\"Čebelica*\"", condition);
        }

        //punctuation and special characters
        [Fact]
        public void Build_Punctuation_SplitsWords()
        {
            var condition = FullTextSearchCondition.Build("Darth vad!");

            Assert.Equal("\"Darth*\" AND \"vad*\"", condition);
        }

        [Fact]
        public void Build_Hyphen_SplitsWords()
        {
            var condition = FullTextSearchCondition.Build("R2-D2");

            Assert.Equal("\"R2*\" AND \"D2*\"", condition);
        }

        [Fact]
        public void Build_QuotesAndOperators_CannotBreakTheSyntax()
        {
            //someone trying to inject their own full-text syntax
            var condition = FullTextSearchCondition.Build("luke\" OR \"x");

            //quotes are gone, "OR" is just a quoted word now, single letter "x" is dropped
            Assert.Equal("\"luke*\" AND \"OR*\"", condition);
        }

        //nothing usable
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("!!")]
        [InlineData("\"()&|*")]
        public void Build_NoLettersOrDigits_ReturnsNull(string search)
        {
            var condition = FullTextSearchCondition.Build(search);

            Assert.Null(condition);
        }

        //limits
        [Fact]
        public void Build_OneLetterWords_AreDropped()
        {
            var condition = FullTextSearchCondition.Build("a luke");

            Assert.Equal("\"luke*\"", condition);
        }

        [Fact]
        public void Build_OnlyOneLetter_ReturnsNull()
        {
            var condition = FullTextSearchCondition.Build("a");

            Assert.Null(condition);
        }

        [Fact]
        public void Build_MoreThanMaxWords_KeepsOnlyFirstMaxWords()
        {
            var condition = FullTextSearchCondition.Build("one two three four five six seven");

            Assert.Equal("\"one*\" AND \"two*\" AND \"three*\" AND \"four*\" AND \"five*\"", condition);
            Assert.DoesNotContain("six", condition);
        }
    }
}
