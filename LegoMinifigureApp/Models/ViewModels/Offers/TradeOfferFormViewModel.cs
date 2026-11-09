using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Offers;
using LegoMinifigureApp.Services.Offers.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace LegoMinifigureApp.Models.ViewModels.Offers
{
    public class TradeOfferFormViewModel
    {
        //hidden field; version of the figure the visitor saw
        public string? FigureRowVersion { get; set; }

        [Required(ErrorMessage = "Enter your name.")]
        [StringLength(Offer.NameMaxLength)]
        [Display(Name = "Your name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your e-mail.")]
        [StringLength(Offer.EmailMaxLength)]
        [EmailAddress(ErrorMessage = "Enter a valid e-mail address.")]
        [Display(Name = "Your e-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tell what you offer in exchange.")]
        [StringLength(Offer.MessageMaxLength)]
        [Display(Name = "What do you offer in exchange?")]
        public string Message { get; set; } = string.Empty;

        //spam trap; hidden from people, bots fill it in
        public string? Website { get; set; }

        //display only; never read from the form
        [BindNever]
        [ValidateNever]
        public OfferFigureViewModel Figure { get; set; } = null!;

        [BindNever]
        public bool ListingWasUpdated { get; set; }

        public static TradeOfferFormViewModel From(OfferFigureDto figure)
        {
            return new TradeOfferFormViewModel
            {
                FigureRowVersion = RowVersionText.Encode(figure.RowVersion),
                Figure = OfferFigureViewModel.From(figure)
            };
        }

        public TradeOfferCommand ToCommand(int minifigureId, byte[] figureRowVersion)
        {
            return new TradeOfferCommand(minifigureId, figureRowVersion, Name, Email, Message);
        }
    }
}
