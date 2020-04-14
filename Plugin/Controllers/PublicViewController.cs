using BitShift.Plugin.Payments.FirstData.Factories;
using BitShift.Plugin.Payments.FirstData.Models;
using BitShift.Plugin.Payments.FirstData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using System.Collections.Generic;
using System.Net;

namespace BitShift.Plugin.Payments.FirstData.Controllers
{
    public class PublicViewController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly IPaymentModelFactory _paymentModelFactory;

        public PublicViewController(IWorkContext workContext, IPaymentModelFactory paymentModelFactory)
        {
            _workContext = workContext;
            _paymentModelFactory = paymentModelFactory;
        }

        public ActionResult GetHostedPaymentForm()
        {
            var model = _paymentModelFactory.PrepareHostedModel(Url);

            return View("~/Plugins/BitShift.Payments.FirstData/Views/Payment/HostedPayment.cshtml", model);
        }

        public ActionResult PaymentResponse(IFormCollection form)
        {
            var model = _paymentModelFactory.PreparePaymentResponseModel(form);

            return PartialView("~/Plugins/BitShift.Payments.FirstData/Views/Payment/_PaymentResponse.cshtml", model);
        }
    }
}