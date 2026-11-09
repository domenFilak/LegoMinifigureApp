using LegoMinifigureApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Helpers
{
    public class UtcDatesTests
    {
        [Fact]
        public void ToIso_UtcDate_EndsWithZ()
        {
            var date = new DateTime(2026, 11, 5, 9, 30, 0, DateTimeKind.Utc);

            var text = UtcDates.ToIso(date);

            Assert.Equal("2026-11-05T09:30:00.0000000Z", text);
        }

        [Fact]
        public void ToIso_DateWithUnspecifiedKind_IsStillTreatedAsUtc()
        {
            //dates read from the database come back with Kind = Unspecified
            var date = new DateTime(2026, 11, 5, 9, 30, 0, DateTimeKind.Unspecified);

            var text = UtcDates.ToIso(date);

            Assert.Equal("2026-11-05T09:30:00.0000000Z", text);
        }

        [Fact]
        public void ToFallbackText_ShowsDayMonthYearTimeAndUtc()
        {
            var date = new DateTime(2026, 11, 5, 9, 30, 0, DateTimeKind.Utc);

            var text = UtcDates.ToFallbackText(date);

            Assert.Equal("05.11.2026 09:30 UTC", text);
        }
    }
}
