namespace LegoMinifigureApp.Entities.Common
{
    public abstract class AuditableEntity
    {
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        public void MarkCreated(DateTime nowUtc) 
        {
            CreatedAt = nowUtc;
        }

        public void MarkUpdated(DateTime nowUtc)
        {
            UpdatedAt = nowUtc;
        }

        public void SoftDelete(DateTime nowUtc)
        {
            if (IsDeleted)
            {
                return;
            }
            IsDeleted = true;
            DeletedAt = nowUtc;
        }

        public void RestoreFromTrash()
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}
