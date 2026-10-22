using LegoMinifigureApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegoMinifigureApp.Data.Configurations
{
    public class MinifigureConfiguration : IEntityTypeConfiguration<Minifigure>
    {
        public void Configure(EntityTypeBuilder<Minifigure> builder)
        {
            builder.HasKey(m => m.Id);

            //descriptive data
            builder.Property(m => m.Name).IsRequired().HasMaxLength(Minifigure.NameMaxLength);
            builder.Property(m => m.BrickLinkId).IsRequired().HasMaxLength(Minifigure.BrickLinkIdMaxLength);
            builder.Property(m => m.Description).HasMaxLength(Minifigure.DescriptionMaxLength);
            builder.Property(m => m.ImagePath).HasMaxLength(Minifigure.ImagePathMaxLength);

            builder.Property(m => m.Condition)
                .HasConversion<string>()
                .HasMaxLength(20);

            //lifecycle and listing
            builder.Property(m => m.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(m => m.AskingPrice).HasPrecision(10, 2);
            builder.Property(m => m.TradeNotes).HasMaxLength(500);

            //concurrency
            builder.Property(m => m.RowVersion).IsRowVersion();

            //only computed, not a column
            builder.Ignore(m => m.IsArchived);

            //relationship with "Theme" db table
            builder.HasOne(m => m.Theme)
                .WithMany(t => t.Minifigures)
                .HasForeignKey(m => m.ThemeId)
                .OnDelete(DeleteBehavior.Restrict);

            //indexes
            builder.HasIndex(m => m.BrickLinkId); //not unique, duplicates allowed
            builder.HasIndex(m => m.Status);

            builder.HasQueryFilter(QueryFilters.SoftDelete, m => !m.IsDeleted);

            //db level guarantees, mirror entitys rules
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Minifigures_ReleaseYear",
                    "[ReleaseYear] IS NULL OR [ReleaseYear] >= 1978");

                t.HasCheckConstraint("CK_Minifigures_AskingPrice",
                    "([IsForSale] = 0 AND [AskingPrice] IS NULL) OR ([IsForSale] = 1 AND [AskingPrice] > 0)");

                t.HasCheckConstraint("CK_Minifigures_TradeNotes",
                    "[IsForTrade] = 1 OR [TradeNotes] IS NULL");

                t.HasCheckConstraint("CK_Minifigures_ListingRequiresOwned",
                    "[Status] = 'Owned' OR ([IsForSale] = 0 AND [IsForTrade] = 0)");

                t.HasCheckConstraint("CK_Minifigures_StatusChangedAt",
                    "[Status] = 'Owned' OR [StatusChangedAt] IS NOT NULL");

                t.HasCheckConstraint("CK_Minifigures_DeletedAt",
                    "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
            });
        }
    }
}
