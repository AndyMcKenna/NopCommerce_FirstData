using BitShift.Plugin.Payments.FirstData.Domain;
using BitShift.Plugin.Payments.FirstData.Factories;
using BitShift.Plugin.Payments.FirstData.Models;
using BitShift.Plugin.Payments.FirstData.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitShift.Plugin.Payments.FirstData.Controllers
{
  [AuthorizeAdmin]
  [Area(AreaNames.Admin)]
  public class FirstDataController : BasePaymentController
  {
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;
    private readonly FirstDataSettings _firstDataSettings;
    private readonly IEncryptionService _encryptionService;
    private readonly IFirstDataStoreSettingService _firstDataStoreSettingService;
    private readonly IStoreService _storeService;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly OrderSettings _orderSettings;
    private readonly PaymentSettings _paymentSettings;
    private readonly ILogger _logger;
    private readonly IPaymentModelFactory _paymentModelFactory;

    public FirstDataController(ISettingService settingService, ILocalizationService localizationService,
        FirstDataSettings firstDataSettings, IEncryptionService encryptionService,
        IFirstDataStoreSettingService firstDataStoreSettingService, IStoreContext storeContext,
        IStoreService storeService, ILogger logger,
        IWorkContext workContext, OrderSettings orderSettings,
        PaymentSettings paymentSettings, IPaymentModelFactory paymentModelFactory)
    {
      _settingService = settingService;
      _localizationService = localizationService;
      _firstDataSettings = firstDataSettings;
      _encryptionService = encryptionService;
      _workContext = workContext;
      _orderSettings = orderSettings;
      _storeContext = storeContext;
      _storeService = storeService;
      _firstDataStoreSettingService = firstDataStoreSettingService;
      _logger = logger;
      _paymentSettings = paymentSettings;
      _paymentModelFactory = paymentModelFactory;
    }
    #region Configure
    public ActionResult Configure()
    {
      var model = new ConfigurationModel
      {
        SandboxURL = _firstDataSettings.SandboxURL,
        ProductionURL = _firstDataSettings.ProductionURL,
        Stores = _storeService.GetAllStores().Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList()
      };
      model.Stores.Insert(0, new SelectListItem { Value = "0", Text = "Default Store Settings" });

      return View("~/Plugins/BitShift.Payments.FirstData/Views/Configure/Configure.cshtml", model);
    }

    [HttpPost]
    public ActionResult Configure(ConfigurationModel model)
    {
      if (!ModelState.IsValid)
        return Configure();

      model.SavedSuccessfully = true;
      if (ModelState.IsValid)
      {
        try
        {
          _firstDataSettings.SandboxURL = model.SandboxURL;
          _firstDataSettings.ProductionURL = model.ProductionURL;
          _settingService.SaveSetting(_firstDataSettings);

          model.SaveMessage = "Settings saved";
        }
        catch (Exception ex)
        {
          model.SavedSuccessfully = false;
          model.SaveMessage = ex.Message;
        }
      }
      else
      {
        model.SavedSuccessfully = false;
        model.SaveMessage = ModelState.Values.First().Errors.First().ErrorMessage;
      }

      model.Stores = _storeService.GetAllStores().Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList();
      model.Stores.Insert(0, new SelectListItem { Value = "0", Text = "Default Store Settings" });

      return View("~/Plugins/BitShift.Payments.FirstData/Views/Configure/Configure.cshtml", model);
    }

    public ActionResult GetStoreSettings(int storeId)
    {
      var model = new FirstDataStoreSettingModel
      {
        StoreID = storeId,
        TransactModeValues = new List<SelectListItem>
                {
                    new SelectListItem { Text = _localizationService.GetLocalizedEnum(TransactMode.Authorize), Value = ((int)TransactMode.Authorize).ToString() },
                    new SelectListItem { Text = _localizationService.GetLocalizedEnum(TransactMode.AuthorizeAndCapture), Value = ((int)TransactMode.AuthorizeAndCapture).ToString() },
                    new SelectListItem { Text = _localizationService.GetLocalizedEnum(TransactMode.AuthorizeAndCaptureAfterOrder), Value = ((int)TransactMode.AuthorizeAndCaptureAfterOrder).ToString() },
                    new SelectListItem { Text = _localizationService.GetLocalizedEnum(TransactMode.HostedPaymentPageAuthOnly), Value = ((int)TransactMode.HostedPaymentPageAuthOnly).ToString() },
                    new SelectListItem { Text = _localizationService.GetLocalizedEnum(TransactMode.HostedPaymentPagePostCapture), Value = ((int)TransactMode.HostedPaymentPagePostCapture).ToString() }
                }
      };

      var store = _storeService.GetStoreById(storeId);
      if (store == null)
      {
        model.StoreName = "Default Settings";
      }
      else
      {
        model.StoreName = store.Name;
      }

      var setting = _firstDataStoreSettingService.GetByStore(storeId, false);
      if (setting == null)
      {
        model.UseDefaultSettings = true;
      }
      else
      {
        model.UseDefaultSettings = false;
        model.UseSandbox = setting.UseSandbox;
        model.TransactModeId = setting.TransactionMode;
        if (!string.IsNullOrEmpty(setting.HMAC))
          model.HMAC = _encryptionService.DecryptText(setting.HMAC);
        if (!string.IsNullOrEmpty(setting.GatewayID))
          model.GatewayID = _encryptionService.DecryptText(setting.GatewayID);
        if (!string.IsNullOrEmpty(setting.Password))
          model.Password = _encryptionService.DecryptText(setting.Password);
        if (!string.IsNullOrEmpty(setting.KeyID))
          model.KeyID = _encryptionService.DecryptText(setting.KeyID);
        if (!string.IsNullOrEmpty(setting.PaymentPageID))
          model.PaymentPageID = _encryptionService.DecryptText(setting.PaymentPageID);
        if (!string.IsNullOrEmpty(setting.TransactionKey))
          model.TransactionKey = _encryptionService.DecryptText(setting.TransactionKey);
        if (!string.IsNullOrEmpty(setting.ResponseKey))
          model.ResponseKey = _encryptionService.DecryptText(setting.ResponseKey);
        model.AdditionalFee = setting.AdditionalFee;
        model.AdditionalFeePercentage = setting.AdditionalFeePercentage;
        model.EnableRecurringPayments = setting.EnableRecurringPayments;
        model.EnableCardSaving = setting.EnableCardSaving;
        model.EnablePurchaseOrderNumber = setting.EnablePurchaseOrderNumber;
        model.AllowVisa = setting.AllowVisa;
        model.AllowMastercard = setting.AllowMastercard;
        model.AllowAmex = setting.AllowAmex;
        model.AllowDiscover = setting.AllowDiscover;
      }

      return PartialView("~/Plugins/BitShift.Payments.FirstData/Views/Configure/_StoreSettings.cshtml", model);
    }

    [HttpPost]
    public string SaveStoreSettings(FirstDataStoreSettingModel model)
    {
      try
      {
        bool alreadyExists = true;
        var setting = _firstDataStoreSettingService.GetByStore(model.StoreID, false);
        if (setting == null)
        {
          setting = new FirstDataStoreSetting
          {
            StoreId = model.StoreID
          };
          alreadyExists = false;
        }

        setting.UseSandbox = model.UseSandbox;
        setting.TransactionMode = model.TransactModeId;
        setting.HMAC = _encryptionService.EncryptText(model.HMAC);
        setting.GatewayID = _encryptionService.EncryptText(model.GatewayID);
        setting.Password = _encryptionService.EncryptText(model.Password);
        setting.KeyID = _encryptionService.EncryptText(model.KeyID);
        setting.PaymentPageID = _encryptionService.EncryptText(model.PaymentPageID);
        setting.TransactionKey = _encryptionService.EncryptText(model.TransactionKey);
        setting.ResponseKey = _encryptionService.EncryptText(model.ResponseKey);
        setting.AdditionalFee = model.AdditionalFee;
        setting.AdditionalFeePercentage = model.AdditionalFeePercentage;
        setting.EnableRecurringPayments = model.EnableRecurringPayments;
        setting.EnableCardSaving = model.EnableCardSaving;
        setting.EnablePurchaseOrderNumber = model.EnablePurchaseOrderNumber;
        setting.AllowVisa = model.AllowVisa;
        setting.AllowMastercard = model.AllowMastercard;
        setting.AllowAmex = model.AllowAmex;
        setting.AllowDiscover = model.AllowDiscover;

        if (alreadyExists)
        {
          _firstDataStoreSettingService.Update(setting);
        }
        else
        {
          _firstDataStoreSettingService.Insert(setting);
        }

        return "success";
      }
      catch (Exception ex)
      {
        _logger.Error("Error updating store specific settings", ex, _workContext.CurrentCustomer);
        return ex.Message;
      }
    }

    [HttpPost]
    public void RevertStoreSettings(int storeId)
    {
      var setting = _firstDataStoreSettingService.GetByStore(storeId, false);
      if (setting == null)
      {
        return;
      }

      _firstDataStoreSettingService.Delete(setting);
    }

    #endregion

    #region Checkout



    #endregion
  }
}