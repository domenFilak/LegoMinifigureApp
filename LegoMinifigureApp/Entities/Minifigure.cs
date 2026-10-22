using LegoMinifigureApp.Entities.Common;
using LegoMinifigureApp.Entities.Enums;

namespace LegoMinifigureApp.Entities
{
    public class Minifigure : AuditableEntity
    {
        public const int NameMaxLength = 100;
        public const int BrickLinkIdMaxLength = 20;
        public const int DescriptionMaxLength = 2000;
        public const int ImagePathMaxLength = 260;
        public const short FirstReleaseYear = 1978;
        public const int ReleaseYearMaxYearsAhead = 2;

        public int Id { get; private set; }
        //description of the minifigure, editable
        public string Name { get; set; } = null!;
        public string BrickLinkId { get; set; } = null!;
        public short? ReleaseYear { get; set; }
        public MinifigureCondition Condition { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public int ThemeId { get; set; }
        public Theme Theme { get; private set; } = null!;

        //lifecycle, changed only through belonging methods
        public MinifigureStatus Status { get; private set; } = MinifigureStatus.Owned;
        public DateTime? StatusChangedAt { get; private set; }
        public bool IsForSale { get; private set; }
        public decimal? AskingPrice { get; private set; }
        public bool IsForTrade { get; private set; }
        public string? TradeNotes { get; private set; }

        //concurency
        public byte[] RowVersion { get; private set; } = null!;

        public bool IsArchived => Status != MinifigureStatus.Owned;

        public void ListForSale(decimal askingPrice)
        {
            EnsureOwned();
            if (askingPrice <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(askingPrice), "Asking price must be greater than zero.");
            }
            
            IsForSale = true;
            AskingPrice = askingPrice;
        }

        public void RemoveFromSale()
        {
            IsForSale = false;
            AskingPrice = null;
        }

        public void ListForTrade(string? tradeNotes)
        {
            EnsureOwned();
            IsForTrade = true;
            TradeNotes = string.IsNullOrWhiteSpace(tradeNotes) ? null : tradeNotes.Trim();
        }

        public void RemoveFromTrade()
        {
            IsForTrade = false;
            TradeNotes = null;
        }

        public void Archive(MinifigureStatus newStatus, DateTime changedAtUtc)
        {
            if (newStatus == MinifigureStatus.Owned)
            {
                throw new ArgumentException("Use Restore() to return a figure to the collection.", nameof(newStatus));
            }
            
            EnsureOwned();
            RemoveFromSale();
            RemoveFromTrade();
            Status = newStatus;
            StatusChangedAt = changedAtUtc;
        }

        public void Restore(DateTime changedAtUtc)
        {
            if (Status == MinifigureStatus.Owned)
            {
                return;
            }

            Status = MinifigureStatus.Owned;
            StatusChangedAt = changedAtUtc;
        }

        private void EnsureOwned()
        {
            if (IsArchived)
            {
                throw new InvalidOperationException($"Minifigure {Id} is {Status} and cannot be listed.");
            }   
        }
    }
}
