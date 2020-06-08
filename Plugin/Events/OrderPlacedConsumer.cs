using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Events;
using Nop.Core.Domain.Orders;
using BitShift.Plugin.Payments.FirstData.Domain;
using Nop.Core.Domain.Payments;
using Nop.Services.Payments;
using Nop.Services.Orders;
using Nop.Services.Logging;
using Nop.Core;
using BitShift.Plugin.Payments.FirstData.Services;
using Nop.Services.Common;
using Nop.Services.Customers;

namespace BitShift.Plugin.Payments.FirstData.Events
{
  public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
  {
    private readonly FirstDataSettings _firstDataSettings;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IOrderService _orderService;
    private readonly ILogger _logger;
    private readonly IWorkContext _workContext;
    private readonly FirstDataStoreSetting _firstDataStoreSetting;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ICustomerService _customerService;

    public OrderPlacedConsumer(FirstDataSettings firstDataSettings, IOrderProcessingService orderProcessingService,
        ILogger logger, IWorkContext workContext, IOrderService orderService,
        IStoreContext storeContext, IFirstDataStoreSettingService firstDataStoreSettingService,
        IGenericAttributeService genericAttributeService, ICustomerService customerService)
    {
      _firstDataSettings = firstDataSettings;
      _orderProcessingService = orderProcessingService;
      _orderService = orderService;
      _logger = logger;
      _workContext = workContext;
      _genericAttributeService = genericAttributeService;
      _customerService = customerService;

      _firstDataStoreSetting = firstDataStoreSettingService.GetByStore(storeContext.CurrentStore.Id);
    }

    public void HandleEvent(OrderPlacedEvent e)
    {
      if (_firstDataStoreSetting == null)
      {
        _logger.Error($"BitShift.Payments.FirstData: Plugin Store Settings not configured. Order #{e.Order.Id}", null, _workContext.CurrentCustomer);
        return;
      }

      if (e.Order.PaymentMethodSystemName == "BitShift.Payments.FirstData" &&
         e.Order.PaymentStatus == PaymentStatus.Authorized &&
         (_firstDataStoreSetting.TransactionMode == (int)TransactMode.AuthorizeAndCaptureAfterOrder ||
          _firstDataStoreSetting.TransactionMode == (int)TransactMode.HostedPaymentPagePostCapture))
      {
        try
        {
          var warnings = _orderProcessingService.Capture(e.Order);
          if (warnings.Count > 0)
          {
            var errorText = string.Join("<br />", warnings.ToArray());
            _orderService.InsertOrderNote(new OrderNote
            {
              CreatedOnUtc = DateTime.UtcNow,
              DisplayToCustomer = false,
              Note = "Error capturing payment: " + errorText,
              OrderId = e.Order.Id
            });
            _logger.Information($"BitShift.Payments.FirstData: Error capturing payment after the order was placed. Order #{e.Order.Id} {errorText}", null, _workContext.CurrentCustomer);
            _orderService.UpdateOrder(e.Order);
          }
          else
          {
            var customer = _customerService.GetCustomerById(e.Order.CustomerId);
            _genericAttributeService.SaveAttribute(customer, Constants.AuthorizationAttribute, "");
            _genericAttributeService.SaveAttribute(customer, Constants.OrderTotalAttribute, "");
          }
        }
        catch (Exception ex)
        {
          _logger.Error($"BitShift.Payments.FirstData: Error capturing payment after the order was placed. Order #{e.Order.Id} {ex.Message}", ex, _workContext.CurrentCustomer);
        }
      }
    }
  }
}
