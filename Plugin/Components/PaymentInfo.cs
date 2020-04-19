using BitShift.Plugin.Payments.FirstData.Factories;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System;
using System.Linq;
using System.Net;

namespace BitShift.Plugin.Payments.FirstData.Components
{
  public class PaymentInfoViewComponent : NopViewComponent
  {
    private readonly IPaymentModelFactory _paymentModelFactory;

    public PaymentInfoViewComponent(IPaymentModelFactory paymentModelFactory)
    {
      _paymentModelFactory = paymentModelFactory;
    }

    public IViewComponentResult Invoke()
    {
      var model = _paymentModelFactory.PreparePaymentInfoModel();

      if (Request.Method != WebRequestMethods.Http.Get)
      {
        //set postback values
        model.CardholderName = Request.Form["CardholderName"];
        model.CardNumber = Request.Form["CardNumber"];
        model.CardCode = Request.Form["CardCode"];
        model.SaveCard = (!string.IsNullOrEmpty(Request.Form["SaveCard"]) ? Request.Form["SaveCard"].ToString() == "true,false" : false);
        var selectedCcType = model.CreditCardTypes.Where(x => x.Value.Equals(Request.Form["CreditCardType"], StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        if (selectedCcType != null)
          selectedCcType.Selected = true;
        var selectedMonth = model.ExpireMonths.Where(x => x.Value.Equals(Request.Form["ExpireMonth"], StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        if (selectedMonth != null)
          selectedMonth.Selected = true;
        var selectedYear = model.ExpireYears.Where(x => x.Value.Equals(Request.Form["ExpireYear"], StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        if (selectedYear != null)
          selectedYear.Selected = true;
      }

      return View("~/Plugins/BitShift.Payments.FirstData/Views/Payment/PaymentInfo.cshtml", model);
    }
  }
}
