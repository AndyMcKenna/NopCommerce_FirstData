using BitShift.Plugin.Payments.FirstData.Domain;
using BitShift.Plugin.Payments.FirstData.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Web.Framework.Components;
using Nop.Services.Plugins;
using System;
using Nop.Core.Plugins;

namespace BitShift.Plugin.Payments.FirstData.Components
{
  public class SavedCardsLinkViewComponent : NopViewComponent
  {
    private FirstDataStoreSetting _firstDataStoreSetting;
    private readonly PaymentSettings _paymentSettings;

    public SavedCardsLinkViewComponent(IFirstDataStoreSettingService firstDataStoreSettingService, IPluginFinder pluginService,
        IFirstDataStoreSettingService firstDateStoreSettingService, IStoreContext storeContext,
        PaymentSettings paymentSettings)
    {
      _paymentSettings = paymentSettings;

      var pluginDescriptor = pluginService.GetPluginDescriptorBySystemName<FirstDataPaymentProcessor>("BitShift.Payments.FirstData", LoadPluginsMode.All);
      if (pluginDescriptor != null && pluginDescriptor.Installed)
      {
        _firstDataStoreSetting = firstDataStoreSettingService.GetByStore(storeContext.CurrentStore.Id);
        if (_firstDataStoreSetting == null)
          throw new Exception("First Data plugin not configured");
      }
    }

    public IViewComponentResult Invoke()
    {
      if (_firstDataStoreSetting != null && _firstDataStoreSetting.EnableCardSaving && _paymentSettings.ActivePaymentMethodSystemNames.Contains(Constants.SystemName))
      {
        bool model = false;
        return View("~/Plugins/BitShift.Payments.FirstData/Views/Payment/SavedCardsLink.cshtml", model);
      }
      else
      {
        return Content("");
      }
    }
  }
}
