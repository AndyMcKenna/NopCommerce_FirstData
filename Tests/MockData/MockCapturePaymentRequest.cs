using BitShift.Plugin.Payments.FirstData.Domain;
using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitShift.Plugin.Payments.FirstData.Tests.MockData
{
  public static class MockCapturePaymentRequest
  {
    public static CapturePaymentRequest Standard => new CapturePaymentRequest
    {
      Order = new Order
      {
        OrderTotal = 27,
        AuthorizationTransactionId = "",
        Id = 42
      }
    };
    public static Dictionary<string, object> CustomValues => new Dictionary<string, object>
    {
      { Constants.TransactionTag, "FakeTag" }
    };
  }
}
