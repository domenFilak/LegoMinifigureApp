using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Helpers;
using LegoMinifigureApp.Services.Offers;
using LegoMinifigureApp.Services.Offers.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace LegoMinifigureApp.Models.ViewModels.Offers
{
    public class BuyOfferFormViewModel
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

        [Required(ErrorMessage = "Enter your offer.")]
        [Range(0.01, Offer.MaxOfferedPrice, ErrorMessage = "Your offer must be between {1} and {2} €.")]
        [Display(Name = "Your offer (€)")]
        public decimal? OfferedPrice { get; set; }

        [StringLength(Offer.MessageMaxLength)]
        [Display(Name = "Message")]
        public string? Message { get; set; }

        //spam trap; hidden from people, bots fill it in
        public string? Website { get; set; }

        //display only; never read from the form
        [BindNever]
        [ValidateNever]
        public OfferFigureViewModel Figure { get; set; } = null!;

        [BindNever]
        public bool ListingWasUpdated { get; set; }

        public static BuyOfferFormViewModel From(OfferFigureDto figure)
        {
            return new BuyOfferFormViewModel
            {
                FigureRowVersion = RowVersionText.Encode(figure.RowVersion),
                OfferedPrice = figure.AskingPrice, //prefilled with the asking price
                Figure = OfferFigureViewModel.From(figure)
            };
        }

        public BuyOfferCommand ToCommand(int minifigureId, byte[] figureRowVersion)
        {
            return new BuyOfferCommand(minifigureId, figureRowVersion, Name, Email, OfferedPrice!.Value, Message);
        }
    }
}
