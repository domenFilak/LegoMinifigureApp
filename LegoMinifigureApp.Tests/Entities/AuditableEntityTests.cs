using LegoMinifigureApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegoMinifigureApp.Tests.Entities
{
    public class AuditableEntityTests
    {
        private static readonly DateTime FirstTime = new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime SecondTime = new DateTime(2026, 11, 5, 9, 30, 0, DateTimeKind.Utc);

        [Fact]
        public void MarkCreated_SetsCreatedAt()
        {
            var theme = new Theme { Name = "Star Wars" };

            theme.MarkCreated(FirstTime);

            Assert.Equal(FirstTime, theme.CreatedAt);
            Assert.Null(theme.UpdatedAt);
        }

        [Fact]
        public void MarkUpdated_SetsUpdatedAt()
        {
            var theme = new Theme { Name = "Star Wars" };
            theme.MarkCreated(FirstTime);

            theme.MarkUpdated(SecondTime);

            Assert.Equal(FirstTime, theme.CreatedAt); //created date never changes
            Assert.Equal(SecondTime, theme.UpdatedAt);
        }

        [Fact]
        public void SoftDelete_SetsDeletedFlagAndDate()
        {
            var theme = new Theme { Name = "Star Wars" };

            theme.SoftDelete(FirstTime);

            Assert.True(theme.IsDeleted);
            Assert.Equal(FirstTime, theme.DeletedAt);
        }

        [Fact]
        public void SoftDelete_AlreadyDeleted_KeepsFirstDeletedDate()
        {
            var theme = new Theme { Name = "Star Wars" };
            theme.SoftDelete(FirstTime);

            theme.SoftDelete(SecondTime);

            Assert.Equal(FirstTime, theme.DeletedAt);
        }

        [Fact]
        public void RestoreFromTrash_ClearsDeletedFlagAndDate()
        {
            var theme = new Theme { Name = "Star Wars" };
            theme.SoftDelete(FirstTime);

            theme.RestoreFromTrash();

            Assert.False(theme.IsDeleted);
            Assert.Null(theme.DeletedAt);
        }
    }
}
