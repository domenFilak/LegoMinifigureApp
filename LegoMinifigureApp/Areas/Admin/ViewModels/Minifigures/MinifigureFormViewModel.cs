using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Entities.Enums;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Minifigures;
using LegoMinifigureApp.Services.Minifigures.Dtos;
using LegoMinifigureApp.Services.Storage;
using LegoMinifigureApp.Services.Themes.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureFormViewModel
    {
        //hidden fields
        public int? Id { get; set; }
        public string? RowVersion { get; set; }

        //input fields
        [Required]
        [StringLength(Minifigure.NameMaxLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(Minifigure.BrickLinkIdMaxLength)]
        [Display(Name = "BrickLink ID")]
        public string BrickLinkId { get; set; } = string.Empty;

        [Range(Minifigure.FirstReleaseYear, short.MaxValue, ErrorMessage = "Release year must be {1} or later.")]
        [Display(Name = "Release year")]
        public short? ReleaseYear { get; set; }

        [Required(ErrorMessage = "Choose a theme.")]
        [Display(Name = "Theme")]
        public int? ThemeId { get; set; } //required and nullable; so null instead of 0 if nothing choosen and validation then catches this

        [Required(ErrorMessage = "Choose a condition.")]
        public MinifigureCondition? Condition { get; set; }

        [StringLength(Minifigure.DescriptionMaxLength)]
        public string? Description { get; set; }

        public IFormFile? Image { get; set; }

        [Display(Name = "Remove current image")]
        public bool RemoveImage { get; set; }

        //display only
        public string? CurrentImageUrl { get; set; }

        [ValidateNever]
        public IReadOnlyList<SelectListItem> ThemeOptions { get; set; } = [];

        [ValidateNever]
        public IReadOnlyList<SelectListItem> ConditionOptions { get; set; } = [];

        public bool IsEdit => Id.HasValue;
        public bool HasCurrentImage => !string.IsNullOrEmpty(CurrentImageUrl);
        public int MinReleaseYear => Minifigure.FirstReleaseYear;
        public int MaxReleaseYear => DateTime.UtcNow.Year + Minifigure.ReleaseYearMaxYearsAhead;


        public static MinifigureFormViewModel From(MinifigureEditDto dto)
        {
            return new MinifigureFormViewModel
            {
                Id = dto.Id,
                RowVersion = RowVersionText.Encode(dto.RowVersion),
                Name = dto.Name,
                BrickLinkId = dto.BrickLinkId,
                ReleaseYear = dto.ReleaseYear,
                ThemeId = dto.ThemeId,
                Condition = dto.Condition,
                Description = dto.Description,
                CurrentImageUrl = dto.ImagePath is null ? null : ImageUrls.ForMinifigure(dto.ImagePath)
            };
        }

        public void SetOptions(IReadOnlyList<ThemeOptionDto> themes)
        {
            ThemeOptions = themes
                .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
                .ToList();

            ConditionOptions = Enum.GetValues<MinifigureCondition>()
                .Select(c => new SelectListItem(c.ToString(), c.ToString()))
                .ToList();
        }

        public MinifigureDetailsCommand ToCommand(Stream? imageStream)
        {
            ImageUpload? image = null;
            if (Image is not null && imageStream is not null)
            {
                image = new ImageUpload(imageStream, Image.FileName, Image.Length);
            }

            return new MinifigureDetailsCommand(
                Name,
                BrickLinkId,
                ReleaseYear,
                ThemeId!.Value,
                Condition!.Value,
                Description,
                image,
                RemoveImage);
        }

        public byte[]? GetRowVersionBytes()
        {
            return RowVersionText.TryDecode(RowVersion);
        }
    }
}
