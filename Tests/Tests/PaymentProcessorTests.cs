using BitShift.Plugin.Payments.FirstData.Data;
using BitShift.Plugin.Payments.FirstData.Domain;
using BitShift.Plugin.Payments.FirstData.Services;
using BitShift.Plugin.Payments.FirstData.Tests.MockData;
using Microsoft.AspNetCore.Http;
using Moq;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Stores;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Tests;
using Nop.Web.MVC.Tests.Public.Validators;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace BitShift.Plugin.Payments.FirstData.Tests
{
  [TestFixture]
  public class PaymentProcessorTests// : BaseValidatorTests
  {
    private FirstDataSettings _firstDataSettings;
    //private FirstDataStoreSetting _firstDataStoreSetting;
    private Mock<ISettingService> _settingService;
    private Mock<ILocalizationService> _localizationService;
    private Mock<ICurrencyService> _currencyService;
    private Mock<ICustomerService> _customerService;
    private Mock<IWorkContext> _workContext;
    private CurrencySettings _currencySettings;
    private Mock<IWebHelper> _webHelper;
    private Mock<IOrderTotalCalculationService> _orderTotalCalculationService;
    private StoreInformationSettings _storeInformationSettings;
    private Mock<IStoreContext> _storeContext;
    private Mock<IEncryptionService> _encryptionService;
    private Mock<ILogger> _logger;
    private Mock<ISavedCardService> _savedCardService;
    private Mock<IOrderService> _orderService;
    private FirstDataObjectContext _objectContext;
    private Mock<IFirstDataStoreSettingService> _firstDataStoreSettingService;
    private Mock<IHttpContextAccessor> _httpContextAccessor;
    private Mock<IGenericAttributeService> _genericAttributeService;
    private Mock<IPaymentService> _paymentService;
    private Mock<IPluginService> _pluginService;
    private Mock<IWebRequest> _webRequest;
    private Mock<HttpWebRequest> _httpWebRequest;
    private Mock<HttpWebResponse> _httpWebResponse;
    private WidgetSettings _widgetSettings;
    private FirstDataStoreSetting _fdStoreSetting;

    [SetUp]
    public void Setup()
    {
      _firstDataSettings = new FirstDataSettings
      {
        SandboxURL = "",
        ProductionURL = ""
      };
      _firstDataStoreSettingService = new Mock<IFirstDataStoreSettingService>();
      _settingService = new Mock<ISettingService>();
      _localizationService = new Mock<ILocalizationService>();
      _currencyService = new Mock<ICurrencyService>();
      _customerService = new Mock<ICustomerService>();
      _workContext = new Mock<IWorkContext>();
      _webHelper = new Mock<IWebHelper>();
      _orderTotalCalculationService = new Mock<IOrderTotalCalculationService>();
      _storeContext = new Mock<IStoreContext>();
      _encryptionService = new Mock<IEncryptionService>();
      _logger = new Mock<ILogger>();
      _savedCardService = new Mock<ISavedCardService>();
      _orderService = new Mock<IOrderService>();
      _httpContextAccessor = new Mock<IHttpContextAccessor>();
      _genericAttributeService = new Mock<IGenericAttributeService>();
      _paymentService = new Mock<IPaymentService>();
      _pluginService = new Mock<IPluginService>();
      _webRequest = new Mock<IWebRequest>();
      _httpWebRequest = new Mock<HttpWebRequest>();
      _httpWebResponse = new Mock<HttpWebResponse>();

      #region Settings

      _widgetSettings = new WidgetSettings
      {
        ActiveWidgetSystemNames = new List<string>
        {
          FirstDataPaymentProcessor.SYSTEM_NAME
        }
      };
      _currencySettings = new CurrencySettings
      {
        PrimaryStoreCurrencyId = 1
      };
      _fdStoreSetting = new FirstDataStoreSetting
      {
        TransactionMode = (int)TransactMode.Authorize,
        EnablePurchaseOrderNumber = true,
        UseSandbox = false
      };

      #endregion

      #region Stubs

      var pluginDescriptor = new PluginDescriptor()
      {
        Installed = true
      };
      _pluginService.Setup(p => p.GetPluginDescriptorBySystemName<FirstDataPaymentProcessor>(It.IsAny<string>(),
        It.IsAny<LoadPluginsMode>(), It.IsAny<Customer>(), It.IsAny<int>(), It.IsAny<string>())).Returns(pluginDescriptor);

      _storeContext.Setup(s => s.CurrentStore).Returns(new Store { Id = 0 });
      _webHelper.Setup(w => w.GetCurrentIpAddress()).Returns("127.0.0.1");
      _currencyService.Setup(c => c.GetCurrencyById(It.IsAny<int>(), It.IsAny<bool>())).Returns(new Currency { CurrencyCode = "USD" });
      _encryptionService.Setup(e => e.DecryptText(It.IsAny<string>(), It.IsAny<string>())).Returns("");
      _localizationService.Setup(l => l.GetResource(It.IsAny<string>())).Returns<string>(x => x);
      _genericAttributeService.Setup(g => g.GetAttribute(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns("");
      _savedCardService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new SavedCard());
      _customerService.Setup(c => c.GetCustomerById(It.IsAny<int>())).Returns(MockCustomer.JohnDoe);

      var requestMemoryStream = new MemoryStream();
      _httpWebRequest.Setup(h => h.Headers).Returns(new WebHeaderCollection());
      _httpWebRequest.Setup(h => h.GetRequestStream()).Returns(requestMemoryStream);
      _httpWebRequest.Setup(h => h.GetResponse()).Returns(_httpWebResponse.Object);
      _webRequest.Setup(w => w.Create(It.IsAny<string>())).Returns(_httpWebRequest.Object);

      #endregion
    }

    [Test]
    public void ProcessPayment_Authorize_Success()
    {
      _firstDataStoreSettingService.Setup(x => x.GetByStore(It.IsAny<int>(), It.IsAny<bool>())).Returns(_fdStoreSetting);
      _httpWebResponse.Setup(w => w.GetResponseStream()).Returns(FirstDataStreamResponses.AuthorizationSuccess);

      var processor = new FirstDataPaymentProcessor(_firstDataSettings, _settingService.Object, _currencyService.Object, _customerService.Object,
        _currencySettings, _webHelper.Object, _orderTotalCalculationService.Object, _storeInformationSettings, _workContext.Object,
        _encryptionService.Object, _localizationService.Object, _logger.Object, _webRequest.Object, _savedCardService.Object, _orderService.Object,
        _objectContext, _storeContext.Object, _firstDataStoreSettingService.Object, _pluginService.Object, _httpContextAccessor.Object,
        _genericAttributeService.Object, _paymentService.Object, _widgetSettings);

      var result = processor.ProcessPayment(MockProcessPaymentRequest.Standard);

      Assert.AreEqual(0, result.Errors.Count);
      Assert.AreNotEqual("", result.AuthorizationTransactionId);
      Assert.AreEqual(null, result.CaptureTransactionId);
      Assert.AreEqual(PaymentStatus.Authorized, result.NewPaymentStatus);
    }

    [Test]
    public void ProcessPayment_AuthorizeCapture_Success()
    {
      var fdStoreSetting = new FirstDataStoreSetting
      {
        TransactionMode = (int)TransactMode.AuthorizeAndCapture,
        EnablePurchaseOrderNumber = true,
        UseSandbox = true
      };
      _firstDataStoreSettingService.Setup(x => x.GetByStore(It.IsAny<int>(), It.IsAny<bool>())).Returns(fdStoreSetting);
      _httpWebResponse.Setup(w => w.GetResponseStream()).Returns(FirstDataStreamResponses.AuthorizationCaptureSuccess);

      var processor = new FirstDataPaymentProcessor(_firstDataSettings, _settingService.Object, _currencyService.Object, _customerService.Object,
        _currencySettings, _webHelper.Object, _orderTotalCalculationService.Object, _storeInformationSettings, _workContext.Object,
        _encryptionService.Object, _localizationService.Object, _logger.Object, _webRequest.Object, _savedCardService.Object, _orderService.Object,
        _objectContext, _storeContext.Object, _firstDataStoreSettingService.Object, _pluginService.Object, _httpContextAccessor.Object,
        _genericAttributeService.Object, _paymentService.Object, _widgetSettings);

      var result = processor.ProcessPayment(MockProcessPaymentRequest.Standard);

      Assert.AreEqual(0, result.Errors.Count);
      Assert.AreNotEqual(null, result.CaptureTransactionId);
      Assert.AreEqual(PaymentStatus.Paid, result.NewPaymentStatus);
    }

    [Test]
    public void Capture_Success()
    {
      _firstDataStoreSettingService.Setup(x => x.GetByStore(It.IsAny<int>(), It.IsAny<bool>())).Returns(_fdStoreSetting);
      _paymentService.Setup(p => p.DeserializeCustomValues(It.IsAny<Nop.Core.Domain.Orders.Order>())).Returns(MockCapturePaymentRequest.CustomValues);
      _httpWebResponse.Setup(w => w.GetResponseStream()).Returns(FirstDataStreamResponses.AuthorizationSuccess);
      _orderService.Setup(o => o.UpdateOrder(It.IsAny<Nop.Core.Domain.Orders.Order>()));

      var processor = new FirstDataPaymentProcessor(_firstDataSettings, _settingService.Object, _currencyService.Object, _customerService.Object,
        _currencySettings, _webHelper.Object, _orderTotalCalculationService.Object, _storeInformationSettings, _workContext.Object,
        _encryptionService.Object, _localizationService.Object, _logger.Object, _webRequest.Object, _savedCardService.Object, _orderService.Object,
        _objectContext, _storeContext.Object, _firstDataStoreSettingService.Object, _pluginService.Object, _httpContextAccessor.Object,
        _genericAttributeService.Object, _paymentService.Object, _widgetSettings);

      var result = processor.Capture(MockCapturePaymentRequest.Standard);

      Assert.AreEqual(0, result.Errors.Count);
      Assert.AreNotEqual(null, result.CaptureTransactionId);
      Assert.AreEqual(PaymentStatus.Paid, result.NewPaymentStatus);
    }

    [Test]
    public void Refund_Full_Success()
    {
      _firstDataStoreSettingService.Setup(x => x.GetByStore(It.IsAny<int>(), It.IsAny<bool>())).Returns(_fdStoreSetting);
      _paymentService.Setup(p => p.DeserializeCustomValues(It.IsAny<Nop.Core.Domain.Orders.Order>())).Returns(MockRefundPaymentRequest.CustomValues);
      _httpWebResponse.Setup(w => w.GetResponseStream()).Returns(FirstDataStreamResponses.RefundSuccess);
      _orderService.Setup(o => o.UpdateOrder(It.IsAny<Nop.Core.Domain.Orders.Order>()));

      var processor = new FirstDataPaymentProcessor(_firstDataSettings, _settingService.Object, _currencyService.Object, _customerService.Object,
        _currencySettings, _webHelper.Object, _orderTotalCalculationService.Object, _storeInformationSettings, _workContext.Object,
        _encryptionService.Object, _localizationService.Object, _logger.Object, _webRequest.Object, _savedCardService.Object, _orderService.Object,
        _objectContext, _storeContext.Object, _firstDataStoreSettingService.Object, _pluginService.Object, _httpContextAccessor.Object,
        _genericAttributeService.Object, _paymentService.Object, _widgetSettings);

      var result = processor.Refund(MockRefundPaymentRequest.Full);

      Assert.AreEqual(0, result.Errors.Count);
      Assert.AreEqual(PaymentStatus.Refunded, result.NewPaymentStatus);
    }

    [Test]
    public void Refund_Partial_Success()
    {
      _firstDataStoreSettingService.Setup(x => x.GetByStore(It.IsAny<int>(), It.IsAny<bool>())).Returns(_fdStoreSetting);
      _paymentService.Setup(p => p.DeserializeCustomValues(It.IsAny<Nop.Core.Domain.Orders.Order>())).Returns(MockRefundPaymentRequest.CustomValues);
      _httpWebResponse.Setup(w => w.GetResponseStream()).Returns(FirstDataStreamResponses.RefundSuccess);
      _orderService.Setup(o => o.UpdateOrder(It.IsAny<Nop.Core.Domain.Orders.Order>()));

      var processor = new FirstDataPaymentProcessor(_firstDataSettings, _settingService.Object, _currencyService.Object, _customerService.Object,
        _currencySettings, _webHelper.Object, _orderTotalCalculationService.Object, _storeInformationSettings, _workContext.Object,
        _encryptionService.Object, _localizationService.Object, _logger.Object, _webRequest.Object, _savedCardService.Object, _orderService.Object,
        _objectContext, _storeContext.Object, _firstDataStoreSettingService.Object, _pluginService.Object, _httpContextAccessor.Object,
        _genericAttributeService.Object, _paymentService.Object, _widgetSettings);

      var result = processor.Refund(MockRefundPaymentRequest.Partial);

      Assert.AreEqual(0, result.Errors.Count);
      Assert.AreEqual(PaymentStatus.PartiallyRefunded, result.NewPaymentStatus);
    }

    [Test]
    public void Refund_PartialTooMuch_Failure()
    {
      _firstDataStoreSettingService.Setup(x => x.GetByStore(It.IsAny<int>(), It.IsAny<bool>())).Returns(_fdStoreSetting);
      _paymentService.Setup(p => p.DeserializeCustomValues(It.IsAny<Nop.Core.Domain.Orders.Order>())).Returns(MockRefundPaymentRequest.CustomValues);
      _httpWebResponse.Setup(w => w.GetResponseStream()).Returns(FirstDataStreamResponses.RefundFailure);
      _orderService.Setup(o => o.UpdateOrder(It.IsAny<Nop.Core.Domain.Orders.Order>()));

      var processor = new FirstDataPaymentProcessor(_firstDataSettings, _settingService.Object, _currencyService.Object, _customerService.Object,
        _currencySettings, _webHelper.Object, _orderTotalCalculationService.Object, _storeInformationSettings, _workContext.Object,
        _encryptionService.Object, _localizationService.Object, _logger.Object, _webRequest.Object, _savedCardService.Object, _orderService.Object,
        _objectContext, _storeContext.Object, _firstDataStoreSettingService.Object, _pluginService.Object, _httpContextAccessor.Object,
        _genericAttributeService.Object, _paymentService.Object, _widgetSettings);

      var result = processor.Refund(MockRefundPaymentRequest.PartialTooMuch);

      Assert.AreEqual(1, result.Errors.Count);
      Assert.AreEqual(PaymentStatus.Pending, result.NewPaymentStatus);
    }

    [Test]
    public void Void_Success()
    {
      _firstDataStoreSettingService.Setup(x => x.GetByStore(It.IsAny<int>(), It.IsAny<bool>())).Returns(_fdStoreSetting);
      _httpWebResponse.Setup(w => w.GetResponseStream()).Returns(FirstDataStreamResponses.VoidSuccess);
      _paymentService.Setup(p => p.DeserializeCustomValues(It.IsAny<Nop.Core.Domain.Orders.Order>())).Returns(MockVoidPaymentRequest.CustomValues);

      var processor = new FirstDataPaymentProcessor(_firstDataSettings, _settingService.Object, _currencyService.Object, _customerService.Object,
        _currencySettings, _webHelper.Object, _orderTotalCalculationService.Object, _storeInformationSettings, _workContext.Object,
        _encryptionService.Object, _localizationService.Object, _logger.Object, _webRequest.Object, _savedCardService.Object, _orderService.Object,
        _objectContext, _storeContext.Object, _firstDataStoreSettingService.Object, _pluginService.Object, _httpContextAccessor.Object,
        _genericAttributeService.Object, _paymentService.Object, _widgetSettings);

      var result = processor.Void(MockVoidPaymentRequest.Standard);

      Assert.AreEqual(0, result.Errors.Count);
      Assert.AreEqual(PaymentStatus.Voided, result.NewPaymentStatus);
    }

    [Test]
    public void GetPaymentInfo_Success()
    {
      _firstDataStoreSettingService.Setup(x => x.GetByStore(It.IsAny<int>(), It.IsAny<bool>())).Returns(_fdStoreSetting);

      var form = new Mock<IFormCollection>();
      form.Setup(f => f["CreditCardType"]).Returns("Visa");
      form.Setup(f => f["CardholderName"]).Returns("John Doe");
      form.Setup(f => f["CardNumber"]).Returns(CardNumbers.Real["Visa"].First());
      form.Setup(f => f["ExpireMonth"]).Returns("12");
      form.Setup(f => f["ExpireYear"]).Returns((DateTime.Today.Year + 1).ToString());
      form.Setup(f => f["CardCode"]).Returns("123");
      form.Setup(f => f["PurchaseOrderNumber"]).Returns("test");
      form.Setup(f => f["SaveCard"]).Returns("");
      form.Setup(f => f["savedCardId"]).Returns("");

      var processor = new FirstDataPaymentProcessor(_firstDataSettings, _settingService.Object, _currencyService.Object, _customerService.Object,
        _currencySettings, _webHelper.Object, _orderTotalCalculationService.Object, _storeInformationSettings, _workContext.Object,
        _encryptionService.Object, _localizationService.Object, _logger.Object, _webRequest.Object, _savedCardService.Object, _orderService.Object, _objectContext,
        _storeContext.Object, _firstDataStoreSettingService.Object, _pluginService.Object, _httpContextAccessor.Object,
        _genericAttributeService.Object, _paymentService.Object, _widgetSettings);

      var request = processor.GetPaymentInfo(form.Object);

      Assert.AreEqual("John Doe", request.CreditCardName);
      Assert.AreEqual(1, request.CustomValues.Count);
    }
  }
}
