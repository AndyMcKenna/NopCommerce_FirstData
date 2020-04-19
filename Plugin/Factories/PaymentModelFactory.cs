using BitShift.Plugin.Payments.FirstData.Domain;
using BitShift.Plugin.Payments.FirstData.Models;
using BitShift.Plugin.Payments.FirstData.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Security;
using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace BitShift.Plugin.Payments.FirstData.Factories
{
  /// <summary>
  /// Represents the common models factory
  /// </summary>
  public partial class PaymentModelFactory : IPaymentModelFactory
  {
    #region Fields

    private FirstDataStoreSetting _firstDataStoreSetting;
    private readonly IFirstDataStoreSettingService _firstDataStoreSettingService;
    private readonly IStoreContext _storeContext;
    private readonly ILocalizationService _localizationService;
    private readonly OrderSettings _orderSettings;
    private readonly ISavedCardService _savedCardService;
    private readonly IWorkContext _workContext;
    private readonly ISavedCardModelFactory _savedCardModelFactory;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly ILogger _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEncryptionService _encryptionService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ICustomerService _customerService;
    private readonly IShoppingCartService _shoppingCartService;

    #endregion

    #region Ctor

    public PaymentModelFactory(IFirstDataStoreSettingService firstDataStoreSettingService, IPluginService pluginService,
        IStoreContext storeContext, IOrderTotalCalculationService orderTotalCalculationService,
        ILocalizationService localizationService, OrderSettings orderSettings,
        ISavedCardService savedCardService, IWorkContext workContext,
        ISavedCardModelFactory savedCardModelFactory, ILogger logger,
        IHttpContextAccessor httpContextAccessor,
        IEncryptionService encryptionService, IGenericAttributeService genericAttributeService,
        ICustomerService customerService, IShoppingCartService shoppingCartService)
    {
      _storeContext = storeContext;
      _localizationService = localizationService;
      _orderSettings = orderSettings;
      _savedCardService = savedCardService;
      _workContext = workContext;
      _savedCardModelFactory = savedCardModelFactory;
      _firstDataStoreSettingService = firstDataStoreSettingService;
      _orderTotalCalculationService = orderTotalCalculationService;
      _logger = logger;
      _httpContextAccessor = httpContextAccessor;
      _encryptionService = encryptionService;
      _genericAttributeService = genericAttributeService;
      _customerService = customerService;
      _shoppingCartService = shoppingCartService;

      var pluginDescriptor = pluginService.GetPluginDescriptorBySystemName<FirstDataPaymentProcessor>("BitShift.Payments.FirstData", LoadPluginsMode.All);
      if (pluginDescriptor != null && pluginDescriptor.Installed)
      {
        _firstDataStoreSetting = _firstDataStoreSettingService.GetByStore(_storeContext.CurrentStore.Id);
        if (_firstDataStoreSetting == null)
          throw new Exception("First Data plugin not configured");
      }
    }

    #endregion

    #region Methods

    public PaymentInfoModel PreparePaymentInfoModel()
    {
      var model = new PaymentInfoModel();

      _firstDataStoreSetting = _firstDataStoreSettingService.GetByStore(_storeContext.CurrentStore.Id);

      if (_firstDataStoreSetting.TransactionMode == (int)TransactMode.HostedPaymentPagePostCapture ||
          _firstDataStoreSetting.TransactionMode == (int)TransactMode.HostedPaymentPageAuthOnly)
      {
        model.UseHostedPage = true;

        var shoppingCartItems = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

        var orderTotal = _orderTotalCalculationService.GetShoppingCartTotal(shoppingCartItems).GetValueOrDefault();
      }
      else
      {

        //CC types
        if (_firstDataStoreSetting.AllowVisa)
        {
          model.CreditCardTypes.Add(new SelectListItem()
          {
            Text = "Visa",
            Value = "Visa",
          });
        }

        if (_firstDataStoreSetting.AllowMastercard)
        {
          model.CreditCardTypes.Add(new SelectListItem()
          {
            Text = "Mastercard",
            Value = "MasterCard",
          });
        }

        if (_firstDataStoreSetting.AllowDiscover)
        {
          model.CreditCardTypes.Add(new SelectListItem()
          {
            Text = "Discover",
            Value = "Discover",
          });
        }

        if (_firstDataStoreSetting.AllowAmex)
        {
          model.CreditCardTypes.Add(new SelectListItem()
          {
            Text = "Amex",
            Value = "Amex",
          });
        }

        //years
        for (int i = 0; i < 15; i++)
        {
          string year = Convert.ToString(DateTime.Now.Year + i);
          model.ExpireYears.Add(new SelectListItem()
          {
            Text = year,
            Value = year,
          });
        }

        //months
        for (int i = 1; i <= 12; i++)
        {
          string text = (i < 10) ? "0" + i.ToString() : i.ToString();
          model.ExpireMonths.Add(new SelectListItem()
          {
            Text = text,
            Value = i.ToString(),
          });
        }

        if (_firstDataStoreSetting == null)
          throw new Exception("First Data plugin not configured");

        model.EnableCardSaving = _firstDataStoreSetting.EnableCardSaving;
        model.EnablePurchaseOrderNumber = _firstDataStoreSetting.EnablePurchaseOrderNumber;
        model.SavedCardsLabel = _localizationService.GetResource("BitShift.Plugin.FirstData.Payment.SavedCardsLabel");
        model.NewCardLabel = _localizationService.GetResource("BitShift.Plugin.FirstData.Payment.NewCardLabel");
        model.IsOnePageCheckout = _orderSettings.OnePageCheckoutEnabled;

        if (_firstDataStoreSetting.EnableCardSaving)
        {
          var savedCards = _savedCardService.GetByCustomer(_workContext.CurrentCustomer.Id);
          foreach (var savedCard in savedCards)
          {
            model.SavedCards.Add(_savedCardModelFactory.PrepareSavedCardModel(savedCard));
          }
        }

      }

      return model;
    }

    public PaymentInfoModel PrepareHostedModel(IUrlHelper urlHelper)
    {
      _firstDataStoreSetting = _firstDataStoreSettingService.GetByStore(_storeContext.CurrentStore.Id);

      //clear the authorization tag from the user
      _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, Constants.AuthorizationAttribute, "");

      var shoppingCartItems = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

      var orderTotal = _orderTotalCalculationService.GetShoppingCartTotal(shoppingCartItems).GetValueOrDefault();
      var model = new PaymentInfoModel
      {
        PaymentPageID = _encryptionService.DecryptText(_firstDataStoreSetting.PaymentPageID),
        ReferenceNumber = _workContext.CurrentCustomer.Id.ToString(),
        CurrencyCode = _workContext.WorkingCurrency.CurrencyCode,
        OrderAmount = orderTotal.ToString("0.00"),
        ResponseURL = urlHelper.Link("Plugin.Payments.FirstData.PaymentResponse", null),
        PaymentURL = _firstDataStoreSetting.UseSandbox ? "https://demo.globalgatewaye4.firstdata.com/payment" :
                                                           "https://checkout.globalgatewaye4.firstdata.com/payment"
      };

      _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, Constants.OrderTotalAttribute, model.OrderAmount);

      var billingAddress = _workContext.CurrentCustomer.BillingAddress;
      if (billingAddress != null)
      {
        model.Address1 = billingAddress.Address1;
        model.City = billingAddress.City;
        model.State = billingAddress.StateProvince?.Abbreviation;
        model.Country = billingAddress.Country?.ThreeLetterIsoCode;
        model.Zip = billingAddress.ZipPostalCode;
      }

      DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      TimeSpan diff = DateTime.UtcNow - origin;
      var ticks = Math.Floor(diff.TotalSeconds);
      model.Ticks = Convert.ToInt32(ticks);

      // Convert string to array of bytes.
      byte[] data = Encoding.UTF8.GetBytes($"{model.PaymentPageID}^{model.ReferenceNumber}^{ticks}^{model.OrderAmount}^{model.CurrencyCode}");

      // key
      var transactionKey = _encryptionService.DecryptText(_firstDataStoreSetting.TransactionKey);
      byte[] key = Encoding.UTF8.GetBytes(transactionKey);

      // Create HMAC-MD5 Algorithm;
      HMACMD5 hmac = new HMACMD5(key);

      // Compute hash.
      byte[] hashBytes = hmac.ComputeHash(data);

      // Convert to HEX string.
      model.HashCode = BitConverter.ToString(hashBytes).ToLower().Replace("-", "");

      return model;
    }

    public PaymentResponseModel PreparePaymentResponseModel(IFormCollection formCollection)
    {
      var model = new PaymentResponseModel();

      //check for valid info
      var customer = _customerService.GetCustomerById(Convert.ToInt32(formCollection["x_cust_id"]));
      var orderTotal = _genericAttributeService.GetAttribute<string>(customer, Constants.OrderTotalAttribute);

      var hashPattern = $"{_encryptionService.DecryptText(_firstDataStoreSetting.ResponseKey)}{_encryptionService.DecryptText(_firstDataStoreSetting.PaymentPageID)}{formCollection["x_trans_id"]}{orderTotal}";
      StringBuilder hash = new StringBuilder();
      MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
      byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(hashPattern));

      for (int i = 0; i < bytes.Length; i++)
      {
        hash.Append(bytes[i].ToString("x2"));
      }

      var hashedSignature = hash.ToString();

      if (formCollection["x_MD5_hash"] != hashedSignature)
      {
        model.IsSuccess = false;
        model.ErrorMessage = "Invalid Response Key";
      }
      else if (formCollection["Transaction_Approved"] != "YES")
      {
        model.IsSuccess = false;
        model.ErrorMessage = formCollection["EXact_Message"];
      }
      else
      {
        //valid, set authorization attribute to the auth transaction #
        model.IsSuccess = true;
        _genericAttributeService.SaveAttribute(customer, Constants.AuthorizationAttribute, $"{formCollection["Authorization_Num"]}|{formCollection["Transaction_Tag"]}");
      }

      return model;
    }

    #endregion
  }
}
