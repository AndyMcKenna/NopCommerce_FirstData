using Nop.Web.Framework.Models;

namespace BitShift.Plugin.Payments.FirstData.Models
{
    public class PaymentResponseModel : BaseNopModel
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}