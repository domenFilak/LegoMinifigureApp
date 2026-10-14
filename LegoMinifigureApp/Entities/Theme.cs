using LegoMinifigureApp.Entities.Common;

namespace LegoMinifigureApp.Entities
{
    public class Theme : IAuditable, ISoftDeletable
    {
        public int Id { get; private set; }
        public string Name { get; set; } = null!;
        public ICollection<Minifigure> Minifigures { get; private set; } = new List<Minifigure>();

        //audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
