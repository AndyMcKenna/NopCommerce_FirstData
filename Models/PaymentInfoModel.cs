using System.Collections.Generic;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BitShift.Plugin.Payments.FirstData.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public PaymentInfoModel()
        {
            CreditCardTypes = new List<SelectListItem>();
            ExpireMonths = new List<SelectListItem>();
            ExpireYears = new List<SelectListItem>();
            SavedCards = new List<SavedCardModel>();
        }

        [NopResourceDisplayName("Payment.SelectCreditCard")]
        public string CreditCardType { get; set; }
        [NopResourceDisplayName("Payment.SelectCreditCard")]
        public IList<SelectListItem> CreditCardTypes { get; set; }

        [NopResourceDisplayName("Payment.CardholderName")]
        public string CardholderName { get; set; }

        [NopResourceDisplayName("Payment.CardNumber")]
        public string CardNumber { get; set; }

        [NopResourceDisplayName("Payment.ExpirationDate")]
        public string ExpireMonth { get; set; }
        [NopResourceDisplayName("Payment.ExpirationDate")]
        public string ExpireYear { get; set; }
        public IList<SelectListItem> ExpireMonths { get; set; }
        public IList<SelectListItem> ExpireYears { get; set; }

        [NopResourceDisplayName("Payment.CardCode")]
        public string CardCode { get; set; }

        public string ExpiryDate
        {
            get
            {
                return ExpireMonth.PadLeft(2, '0') + ExpireYear;
            }
        }

        public bool EnablePurchaseOrderNumber { get; set; }
        [NopResourceDisplayName("BitShift.Plugin.FirstData.Payment.PurchaseOrder")]
        public string PurchaseOrderNumber { get; set; }

        public bool EnableCardSaving { get; set; }
        [NopResourceDisplayName("BitShift.Plugin.FirstData.Payment.SaveCard")]
        public bool SaveCard { get; set; }
        public int SavedCardId { get; set; }
        public string SavedCardsLabel { get; set; }
        public string NewCardLabel { get; set; }

        public bool IsOnePageCheckout { get; set; }

        public IList<SavedCardModel> SavedCards { get; set; }

        #region Payment Page fields
        public bool UseHostedPage { get; set; }
        public string PaymentPageID { get; set; }
        public string ReferenceNumber { get; set; }
        public string CurrencyCode { get; set; }
        public string HashCode { get; set; }
        public int Ticks { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string ResponseURL { get; set; }
        public string PaymentURL { get; set; }
        public string OrderAmount { get; set; }
        #endregion

    }
}