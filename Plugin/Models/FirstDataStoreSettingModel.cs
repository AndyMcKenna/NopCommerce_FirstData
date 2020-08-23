using System.Collections.Generic;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BitShift.Plugin.Payments.FirstData.Models
{
  public class FirstDataStoreSettingModel : BaseNopModel
  {
    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.StoreID")]
    public int StoreID { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.StoreID")]
    public string StoreName { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.UseSandbox")]
    public bool UseSandbox { get; set; }

    public int TransactModeId { get; set; }
    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.TransactModeValues")]
    public IList<SelectListItem> TransactModeValues { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.HMAC")]
    public string HMAC { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.GatewayID")]
    public string GatewayID { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.Password")]
    public string Password { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.KeyID")]
    public string KeyID { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.PaymentPageID")]
    public string PaymentPageID { get; set; }
    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.TransactionKey")]
    public string TransactionKey { get; set; }
    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.ResponseKey")]
    public string ResponseKey { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.AdditionalFee")]
    public decimal AdditionalFee { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.AdditionalFeePercentage")]
    public bool AdditionalFeePercentage { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.EnableRecurringPayments")]
    public bool EnableRecurringPayments { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.EnableCardSaving")]
    public bool EnableCardSaving { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.EnablePurchaseOrderNumber")]
    public bool EnablePurchaseOrderNumber { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.AllowVisa")]
    public bool AllowVisa { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.AllowMastercard")]
    public bool AllowMastercard { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.AllowAmex")]
    public bool AllowAmex { get; set; }

    [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.AllowDiscover")]
    public bool AllowDiscover { get; set; }

    public bool UseDefaultSettings { get; set; }
  }
}