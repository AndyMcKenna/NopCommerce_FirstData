using Nop.Core;

namespace BitShift.Plugin.Payments.FirstData.Domain
{
    public class SavedCard : BaseEntity
    {
        public virtual int Customer_Id { get; set; }
        public virtual int BillingAddress_Id { get; set; }
        public virtual string CardholderName { get; set; }
        public virtual string Token { get; set; }
        public virtual int ExpireMonth { get; set; }
        public virtual int ExpireYear { get; set; }
        public virtual string CardType { get; set; }
    }
}
