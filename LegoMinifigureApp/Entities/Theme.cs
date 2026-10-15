using LegoMinifigureApp.Entities.Common;

namespace LegoMinifigureApp.Entities
{
    public class Theme : AuditableEntity
    {
        public int Id { get; private set; }
        public string Name { get; set; } = null!;
        public ICollection<Minifigure> Minifigures { get; private set; } = new List<Minifigure>();

    }
}
