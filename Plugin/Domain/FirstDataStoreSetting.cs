using System.ComponentModel.DataAnnotations.Schema;
using Nop.Core;

namespace BitShift.Plugin.Payments.FirstData.Domain
{
  public class FirstDataStoreSetting : BaseEntity
  {
    public int StoreId { get; set; }

    public bool UseSandbox { get; set; }
    public int TransactionMode { get; set; }
    public string HMAC { get; set; }
    public string GatewayID { get; set; }
    public string Password { get; set; }
    public string KeyID { get; set; }
    public bool EnableRecurringPayments { get; set; }
    public bool EnableCardSaving { get; set; }
    public bool EnablePurchaseOrderNumber { get; set; }

    public string PaymentPageID { get; set; }
    public string TransactionKey { get; set; }
    public string ResponseKey { get; set; }

    public bool AllowVisa { get; set; }
    public bool AllowMastercard { get; set; }
    public bool AllowAmex { get; set; }
    public bool AllowDiscover { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
    /// </summary>
    public bool AdditionalFeePercentage { get; set; }
    /// <summary>
    /// Additional fee
    /// </summary>
    public decimal AdditionalFee { get; set; }
  }
}
