using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace BitShift.Plugin.Payments.FirstData.Models
{
  public class SavedCardModel : BaseNopEntityModel
  {
    public virtual string CardholderName { get; set; }
    public virtual string Last4Digits { get; set; }
    public virtual int ExpireMonth { get; set; }
    public virtual int ExpireYear { get; set; }
    public virtual string CardType { get; set; }
    public virtual bool IsExpired { get; set; }

    public virtual string UseCardLabel { get; set; }
    public virtual string CardDescription { get; set; }
    public virtual string ExpirationDescription { get; set; }
    public virtual string ExpiredLabel { get; set; }
  }
}
