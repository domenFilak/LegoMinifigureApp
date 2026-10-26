using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Minifigures.Dtos;
using System.ComponentModel.DataAnnotations;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureSoldFormViewModel
    {
        public int Id { get; set; }
        public string? RowVersion { get; set; }

        //display
        public string Name { get; set; } = string.Empty;
        public string? CurrentListingText { get; set; }

        [Required(ErrorMessage = "Enter the sale price.")]
        [Range(0.01, Minifigure.MaxSalePrice, ErrorMessage = "Sale price must be between {1} and {2} €.")]
        [Display(Name = "Sale price (€)")]
        public decimal? SalePrice { get; set; }

        [StringLength(Minifigure.ArchiveNoteMaxLength)]
        [Display(Name = "Note")]
        public string? ArchiveNote { get; set; }

        public static MinifigureSoldFormViewModel From(MinifigureDetailsDto dto)
        {
            return new MinifigureSoldFormViewModel
            {
                Id = dto.Id,
                RowVersion = RowVersionText.Encode(dto.RowVersion),
                Name = dto.Name,
                CurrentListingText = ListingText.Describe(dto.IsForSale, dto.AskingPrice, dto.IsForTrade),
                SalePrice = dto.AskingPrice
            };
        }
    }
}
