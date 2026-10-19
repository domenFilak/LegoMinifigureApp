using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Services.Themes.Dtos;
using System.ComponentModel.DataAnnotations;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Themes
{
    public class ThemeFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(Theme.NameMaxLength)]
        public string Name { get; set; } = string.Empty;

        public bool IsEdit => Id.HasValue;

        public static ThemeFormViewModel From(ThemeDto dto)
        {
            return new ThemeFormViewModel
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }
    }
}
