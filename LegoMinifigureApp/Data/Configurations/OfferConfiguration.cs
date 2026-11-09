using LegoMinifigureApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegoMinifigureApp.Data.Configurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.HasKey(o => o.Id);

            //visitor data
            builder.Property(o => o.Name).IsRequired().HasMaxLength(Offer.NameMaxLength);
            builder.Property(o => o.Email).IsRequired().HasMaxLength(Offer.EmailMaxLength);
            builder.Property(o => o.Message).HasMaxLength(Offer.MessageMaxLength);

            //offer data
            builder.Property(o => o.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(o => o.OfferedPrice).HasPrecision(10, 2);
            builder.Property(o => o.AskingPriceAtOffer).HasPrecision(10, 2);

            //handling
            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            //concurrency
            builder.Property(o => o.RowVersion).IsRowVersion();

            //relationship with "Minifigures" db table; figures are never hard deleted
            builder.HasOne(o => o.Minifigure)
                .WithMany()
                .HasForeignKey(o => o.MinifigureId)
                .OnDelete(DeleteBehavior.Restrict);

            //admin list: offers by status, newest first
            builder.HasIndex(o => new { o.Status, o.CreatedAt, o.Id })
                .HasDatabaseName("IX_Offers_Status_CreatedAt")
                .IsDescending(false, true, true)
                .HasFilter("[IsDeleted] = 0");

            builder.HasQueryFilter(QueryFilters.SoftDelete, o => !o.IsDeleted);

            //db level guarantees, mirror entitys rules
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Offers_OfferedPrice",
                    "([Type] = 'Buy' AND [OfferedPrice] > 0) OR ([Type] = 'Trade' AND [OfferedPrice] IS NULL)");

                t.HasCheckConstraint("CK_Offers_TradeMessage",
                    "[Type] <> 'Trade' OR ([Message] IS NOT NULL AND LEN([Message]) > 0)");

                t.HasCheckConstraint("CK_Offers_AskingPriceAtOffer",
                    "[AskingPriceAtOffer] IS NULL OR [AskingPriceAtOffer] > 0");

                t.HasCheckConstraint("CK_Offers_StatusChangedAt",
                    "([Status] = 'New' AND [StatusChangedAt] IS NULL) OR ([Status] <> 'New' AND [StatusChangedAt] IS NOT NULL)");

                t.HasCheckConstraint("CK_Offers_DeletedAt",
                    "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
            });
        }
    }
}
