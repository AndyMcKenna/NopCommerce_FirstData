using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitShift.Plugin.Payments.FirstData.Tests.MockData
{
  public static class MockProcessPaymentRequest
  {
    public static ProcessPaymentRequest Standard => new ProcessPaymentRequest
    {
      CustomerId = 0,
      CreditCardNumber = CardNumbers.Real["Visa"].First(),
      CreditCardCvv2 = "123",
      CreditCardExpireMonth = 12,
      CreditCardExpireYear = DateTime.Today.Year + 1,
      CreditCardName = "John Doe",
      CreditCardType = "Visa",
      StoreId = 0,
      OrderTotal = 10,
      OrderGuid = new Guid(),
      CustomValues = new Dictionary<string, object>()
    };
  }
}
