using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Minifigures.Dtos;
using System.ComponentModel.DataAnnotations;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureTradedFormViewModel
    {
        public int Id { get; set; }
        public string? RowVersion { get; set; }

        //display
        public string Name { get; set; } = string.Empty;
        public string? CurrentListingText { get; set; }

        [StringLength(Minifigure.ArchiveNoteMaxLength)]
        [Display(Name = "Note")]
        public string? ArchiveNote { get; set; }

        public static MinifigureTradedFormViewModel From(MinifigureDetailsDto dto)
        {
            return new MinifigureTradedFormViewModel
            {
                Id = dto.Id,
                RowVersion = RowVersionText.Encode(dto.RowVersion),
                Name = dto.Name,
                CurrentListingText = ListingText.Describe(dto.IsForSale, dto.AskingPrice, dto.IsForTrade)
            };
        }
    }
}
