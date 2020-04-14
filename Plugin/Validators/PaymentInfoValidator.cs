using System;
using FluentValidation;
using BitShift.Plugin.Payments.FirstData.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using BitShift.Plugin.Payments.FirstData.Services;
using BitShift.Plugin.Payments.FirstData.Domain;
using Nop.Core;

namespace BitShift.Plugin.Payments.FirstData.Validators
{
    public class PaymentInfoValidator : AbstractValidator<PaymentInfoModel>
    {
        private FirstDataStoreSetting _firstDataStoreSetting;

        public PaymentInfoValidator(ILocalizationService localizationService, FirstDataStoreSetting storeSetting)
        {
            //useful links:
            //http://fluentvalidation.codeplex.com/wikipage?title=Custom&referringTitle=Documentation&ANCHOR#CustomValidator
            //http://benjii.me/2010/11/credit-card-validator-attribute-for-asp-net-mvc-3/

            //RuleFor(x => x.CardNumber).NotEmpty().WithMessage(localizationService.GetResource("Payment.CardNumber.Required"));
            //RuleFor(x => x.CardCode).NotEmpty().WithMessage(localizationService.GetResource("Payment.CardCode.Required"));

            RuleFor(x => x.CardholderName).NotEmpty().WithMessage(localizationService.GetResource("Payment.CardholderName.Required"));
            RuleFor(x => x.CardNumber).CreditCard().WithMessage(localizationService.GetResource("Payment.CardNumber.Wrong"));
            RuleFor(x => x).Must(AcceptCardBrand).WithMessage(localizationService.GetResource("Payment.CardNumber.NotAccepted"));
            RuleFor(x => x.CardCode).Matches(@"^[0-9]{3,4}$").WithMessage(localizationService.GetResource("Payment.CardCode.Wrong"));
            RuleFor(x => x.ExpireMonth).NotEmpty().WithMessage(localizationService.GetResource("Payment.ExpireMonth.Required"));
            RuleFor(x => x.ExpireYear).NotEmpty().WithMessage(localizationService.GetResource("Payment.ExpireYear.Required"));
            RuleFor(x => x.ExpiryDate).Must(BeLaterThanToday).WithMessage(localizationService.GetResource("BitShift.Plugin.FirstData.ExpiryDateError"));

            _firstDataStoreSetting = storeSetting;
        }

        private bool AcceptCardBrand(PaymentInfoModel model)
        {
            if (string.IsNullOrEmpty(model.CardNumber) || model.CardNumber.Length < 6)
                return false;

            if (!int.TryParse(model.CardNumber.Substring(0, 6), out var first6))
            {
                return false;
            }

            //Amex
            if (model.CardNumber.StartsWith("34") ||
                model.CardNumber.StartsWith("37"))
            {
                return _firstDataStoreSetting.AllowAmex;
            }

            //Mastercard
            if (model.CardNumber.StartsWith("51") ||
                model.CardNumber.StartsWith("52") ||
                model.CardNumber.StartsWith("53") ||
                model.CardNumber.StartsWith("54") ||
                model.CardNumber.StartsWith("55") ||
                (first6 >= 222100 && first6 <= 272099))
            {
                return _firstDataStoreSetting.AllowMastercard;
            }

            //Visa
            if (model.CardNumber.StartsWith("4"))
            {
                return _firstDataStoreSetting.AllowVisa;
            }

            //Discover
            if (model.CardNumber.StartsWith("6011") ||
                model.CardNumber.StartsWith("644") ||
                model.CardNumber.StartsWith("645") ||
                model.CardNumber.StartsWith("646") ||
                model.CardNumber.StartsWith("647") ||
                model.CardNumber.StartsWith("648") ||
                model.CardNumber.StartsWith("649") ||
                model.CardNumber.StartsWith("65") ||
                (first6 >= 622126 && first6 <= 622925))
            {
                return _firstDataStoreSetting.AllowDiscover;
            }

            //some other card type, allow it
            return true;
        }

        private bool BeLaterThanToday(string expiryDate)
        {
            if (expiryDate.Length < 6)
                return false;

            int month = Convert.ToInt32(expiryDate.Substring(0, 2));
            int year = Convert.ToInt32(expiryDate.Substring(2, 4));

            if (year < DateTime.Today.Year || (year == DateTime.Today.Year && month < DateTime.Today.Month))
                return false;
            else
                return true;
        }
    }
}