using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Minifigures.Dtos;
using System.ComponentModel.DataAnnotations;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureSaleFormViewModel
    {
        public int Id { get; set; }
        public string? RowVersion { get; set; }

        //display only, in hidden fields - if this set for sale/change price error still info available
        public string Name { get; set; } = string.Empty;
        public bool IsCurrentlyForSale { get; set; }

        [Required(ErrorMessage = "Enter an asking price.")]
        [Range(0.01, Minifigure.MaxAskingPrice, ErrorMessage = "Asking price must be between {1} and {2} €.")]
        [Display(Name = "Asking price (€)")]
        public decimal? AskingPrice { get; set; }

        public static MinifigureSaleFormViewModel From(MinifigureDetailsDto dto)
        {
            return new MinifigureSaleFormViewModel
            {
                Id = dto.Id,
                RowVersion = RowVersionText.Encode(dto.RowVersion),
                Name = dto.Name,
                IsCurrentlyForSale = dto.IsForSale,
                AskingPrice = dto.AskingPrice
            };
        }
    }
}
