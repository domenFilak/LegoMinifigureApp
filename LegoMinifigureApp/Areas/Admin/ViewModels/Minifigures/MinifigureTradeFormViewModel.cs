using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Minifigures.Dtos;
using System.ComponentModel.DataAnnotations;

namespace LegoMinifigureApp.Areas.Admin.ViewModels.Minifigures
{
    public class MinifigureTradeFormViewModel
    {
        public int Id { get; set; }
        public string? RowVersion { get; set; }

        //display only, transfered by hidden fields
        public string Name { get; set; } = string.Empty;
        public bool IsCurrentlyForTrade { get; set; }

        [StringLength(Minifigure.TradeNotesMaxLength)]
        [Display(Name = "Trade notes")]
        public string? TradeNotes { get; set; }

        public static MinifigureTradeFormViewModel From(MinifigureDetailsDto dto)
        {
            return new MinifigureTradeFormViewModel
            {
                Id = dto.Id,
                RowVersion = RowVersionText.Encode(dto.RowVersion),
                Name = dto.Name,
                IsCurrentlyForTrade = dto.IsForTrade,
                TradeNotes = dto.TradeNotes
            };
        }
    }
}
