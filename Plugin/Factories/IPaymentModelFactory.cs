using BitShift.Plugin.Payments.FirstData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BitShift.Plugin.Payments.FirstData.Factories
{
    /// <summary>
    /// Represents the interface of the common models factory
    /// </summary>
    public partial interface IPaymentModelFactory
    {
        /// <summary>
        /// Prepare the payment info model
        /// </summary>
        /// <returns>Payment info model</returns>
        PaymentInfoModel PreparePaymentInfoModel();

        PaymentInfoModel PrepareHostedModel(IUrlHelper urlHelper);

        PaymentResponseModel PreparePaymentResponseModel(IFormCollection formCollection);
    }
}
