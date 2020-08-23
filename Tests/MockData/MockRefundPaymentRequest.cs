using BitShift.Plugin.Payments.FirstData.Domain;
using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitShift.Plugin.Payments.FirstData.Tests.MockData
{
  public static class MockRefundPaymentRequest
  {
    public static RefundPaymentRequest Full => new RefundPaymentRequest
    {
      Order = new Order
      {
        OrderTotal = 27,
        CaptureTransactionId = "",
        Id = 42
      },
      AmountToRefund = 27,
      IsPartialRefund = false
    };

    public static RefundPaymentRequest Partial => new RefundPaymentRequest
    {
      Order = new Order
      {
        OrderTotal = 27,
        CaptureTransactionId = "",
        Id = 42
      },
      AmountToRefund = 15,
      IsPartialRefund = true
    };

    public static RefundPaymentRequest PartialTooMuch => new RefundPaymentRequest
    {
      Order = new Order
      {
        OrderTotal = 27,
        CaptureTransactionId = "",
        Id = 42
      },
      AmountToRefund = 50,
      IsPartialRefund = true
    };

    public static Dictionary<string, object> CustomValues => new Dictionary<string, object>
    {
      { Constants.TransactionTag, "FakeTag" },
      { "Refund Tag 10", "" }
    };
  }
}
