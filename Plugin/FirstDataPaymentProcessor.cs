using BitShift.Plugin.Payments.FirstData.Controllers;
using BitShift.Plugin.Payments.FirstData.Domain;
using BitShift.Plugin.Payments.FirstData.Models;
using BitShift.Plugin.Payments.FirstData.Services;
using BitShift.Plugin.Payments.FirstData.Validators;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Cms;
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
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BitShift.Plugin.Payments.FirstData
{
  /// <summary>
  /// FirstData payment processor
  /// </summary>
  public class FirstDataPaymentProcessor : BasePlugin, IPaymentMethod, IWidgetPlugin
  {
    #region Fields

    private readonly FirstDataSettings _firstDataSettings;
    private FirstDataStoreSetting _firstDataStoreSetting;
    private readonly ISettingService _settingService;
    private readonly ICurrencyService _currencyService;
    private readonly ICustomerService _customerService;
    private readonly CurrencySettings _currencySettings;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly IEncryptionService _encryptionService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger _logger;
    private readonly ISavedCardService _savedCardService;
    private readonly IOrderService _orderService;
    private readonly IFirstDataStoreSettingService _firstDataStoreSettingService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IPaymentService _paymentService;
    private readonly IWebRequest _webRequest;
    private readonly IAddressService _addressService;
    private readonly IStateProvinceService _stateProvinceService;
    private readonly ICountryService _countryService;
    private readonly WidgetSettings _widgetSettings;

    public const string SYSTEM_NAME = "BitShift.Payments.FirstData";
    #endregion

    #region Ctor

    public FirstDataPaymentProcessor(FirstDataSettings firstDataSettings,
        ISettingService settingService, ICurrencyService currencyService,
        ICustomerService customerService, CurrencySettings currencySettings,
        IWebHelper webHelper, IWorkContext workContext,
        IEncryptionService encryptionService, ILocalizationService localizationService,
        ILogger logger, IWebRequest webRequest,
        ISavedCardService savedCardService, IOrderService orderService,
        IStoreContext storeContext, IAddressService addressService,
        IFirstDataStoreSettingService firstDataStoreSettingService, IPluginService pluginService,
        IHttpContextAccessor httpContextAccessor, IGenericAttributeService genericAttributeService,
        IPaymentService paymentService, IStateProvinceService stateProvinceService,
        ICountryService countryService,
        WidgetSettings widgetSettings)
    {
      _firstDataSettings = firstDataSettings;
      _settingService = settingService;
      _currencyService = currencyService;
      _customerService = customerService;
      _currencySettings = currencySettings;
      _webHelper = webHelper;
      _workContext = workContext;
      _encryptionService = encryptionService;
      _localizationService = localizationService;
      _logger = logger;
      _webRequest = webRequest;
      _savedCardService = savedCardService;
      _orderService = orderService;
      _storeContext = storeContext;
      _firstDataStoreSettingService = firstDataStoreSettingService;
      _httpContextAccessor = httpContextAccessor;
      _genericAttributeService = genericAttributeService;
      _paymentService = paymentService;
      _widgetSettings = widgetSettings;
      _addressService = addressService;
      _stateProvinceService = stateProvinceService;
      _countryService = countryService;

      var pluginDescriptor = pluginService.GetPluginDescriptorBySystemName<FirstDataPaymentProcessor>(SYSTEM_NAME, LoadPluginsMode.All);
      if (pluginDescriptor != null && pluginDescriptor.Installed)
      {
        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(SYSTEM_NAME))
        {
          _widgetSettings.ActiveWidgetSystemNames.Add(SYSTEM_NAME);
          _settingService.SaveSetting(_widgetSettings);
        }

        _firstDataStoreSetting = _firstDataStoreSettingService.GetByStore(_storeContext.CurrentStore.Id);
        if (_firstDataStoreSetting == null)
          throw new Exception("First Data plugin not configured");
      }
    }

    #endregion

    #region Utilities

    private XmlNode SendFDRequest(string transaction)
    {
      //SHA1 hash on XML string
      UTF8Encoding encoder = new UTF8Encoding();
      byte[] xml_byte = encoder.GetBytes(transaction);
      SHA1CryptoServiceProvider sha1_crypto = new SHA1CryptoServiceProvider();
      string hash = BitConverter.ToString(sha1_crypto.ComputeHash(xml_byte)).Replace("-", "");
      string hashed_content = hash.ToLower();

      //assign values to hashing and header variables
      string method = "POST\n";
      string type = "application/xml\n";//REST XML
      string time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
      string url = "/transaction/v12";
      string keyID = _encryptionService.DecryptText(_firstDataStoreSetting.KeyID);//key ID
      string key = _encryptionService.DecryptText(_firstDataStoreSetting.HMAC);//Hmac key
      string hash_data = method + type + hashed_content + "\n" + time + "\n" + url;
      //hmac sha1 hash with key + hash_data
      HMAC hmac_sha1 = new HMACSHA1(Encoding.UTF8.GetBytes(key)); //key
      byte[] hmac_data = hmac_sha1.ComputeHash(Encoding.UTF8.GetBytes(hash_data)); //data
                                                                                   //base64 encode on hmac_data
      string base64_hash = Convert.ToBase64String(hmac_data);

      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

      string destination = (_firstDataStoreSetting.UseSandbox ? _firstDataSettings.SandboxURL : _firstDataSettings.ProductionURL);
      var webRequest = _webRequest.Create(destination);
      webRequest.Method = "POST";
      webRequest.Accept = "application/xml";
      webRequest.Headers.Add("x-gge4-date", time);
      webRequest.Headers.Add("x-gge4-content-sha1", hashed_content);
      webRequest.ContentLength = Encoding.UTF8.GetByteCount(transaction);
      webRequest.ContentType = "application/xml";
      webRequest.Headers["Authorization"] = "GGE4_API " + keyID + ":" + base64_hash;

      // send request as stream
      StreamWriter xml = new StreamWriter(webRequest.GetRequestStream());
      xml.Write(transaction);
      xml.Flush();
      xml.Close();

      //get response and read into string
      string response_string;
      try
      {
        HttpWebResponse web_response = (HttpWebResponse)webRequest.GetResponse();
        using (StreamReader response_stream = new StreamReader(web_response.GetResponseStream()))
        {
          response_string = response_stream.ReadToEnd();
          response_stream.Close();
        }
        //load xml
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(response_string);
        XmlNodeList nodelist = xmldoc.SelectNodes("TransactionResult");

        return nodelist[0];
      }
      catch (WebException ex)
      {
        using (StreamReader response_stream = new StreamReader(ex.Response.GetResponseStream()))
        {
          response_string = response_stream.ReadToEnd();
          response_stream.Close();
          throw new Exception(response_string);
        }
      }
    }

    public string SerializeCustomValues(Dictionary<string, object> customValues)
    {
      if (customValues.Count == 0)
        return null;

      //XmlSerializer won't serialize objects that implement IDictionary by default.
      //http://msdn.microsoft.com/en-us/magazine/cc164135.aspx 

      //also see http://ropox.ru/tag/ixmlserializable/ (Russian language)

      var ds = new DictionarySerializer(customValues);
      var xs = new XmlSerializer(typeof(DictionarySerializer));

      using (var textWriter = new StringWriter())
      {
        using (var xmlWriter = XmlWriter.Create(textWriter))
        {
          xs.Serialize(xmlWriter, ds);
        }
        var result = textWriter.ToString();
        return result;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a view component for displaying plugin in public store ("payment info" checkout step)
    /// </summary>
    public string GetPublicViewComponentName()
    {
      return "PaymentInfo";
    }

    /// <summary>
    /// Gets a name of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component name</returns>
    public string GetWidgetViewComponentName(string widgetZone)
    {
      return "SavedCardsLink";
    }

    /// <summary>
    /// Process a payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>Process payment result</returns>
    public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
    {
      var result = new ProcessPaymentResult();

      var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
      if (!customer.BillingAddressId.HasValue)
      {
        throw new ArgumentException("customer must have a billing address");
      }
      var billingAddress = _addressService.GetAddressById(customer.BillingAddressId.Value);
      var billingState = _stateProvinceService.GetStateProvinceById(billingAddress.StateProvinceId.GetValueOrDefault());
      var billingCountry = _countryService.GetCountryById(billingAddress.CountryId.GetValueOrDefault());

      if (_firstDataStoreSetting.TransactionMode == (int)TransactMode.HostedPaymentPagePostCapture ||
          _firstDataStoreSetting.TransactionMode == (int)TransactMode.HostedPaymentPageAuthOnly)
      {
        var authAttribute = _genericAttributeService.GetAttribute<string>(customer, Constants.AuthorizationAttribute);
        if (!string.IsNullOrEmpty(authAttribute))
        {
          var authSplit = authAttribute.Split(new[] { '|' });
          result.AuthorizationTransactionId = authSplit[0];
          processPaymentRequest.CustomValues.Add(Constants.TransactionTag, authSplit[1]);
          result.AuthorizationTransactionResult = string.Format("Approved on Payment Page");
          result.AllowStoringCreditCardNumber = false;
          result.NewPaymentStatus = PaymentStatus.Authorized;
        }
        else
        {
          _logger.Error("BitShift.Plugin.FirstData.HostedPaymentPage.AuthorizationEmpty");
          result.AddError(_localizationService.GetResource("BitShift.Plugin.FirstData.TechnicalError"));
        }
      }
      else
      {
        var cardNumber = processPaymentRequest.CreditCardNumber;
        bool saveCard = processPaymentRequest.CustomValues.ContainsKey(Constants.SaveCard) && Convert.ToBoolean(processPaymentRequest.CustomValues[Constants.SaveCard]);
        bool useSavedCard = processPaymentRequest.CustomValues.ContainsKey(Constants.SavedCardId);

        StringBuilder string_builder = new StringBuilder();
        try
        {
          using (StringWriter string_writer = new StringWriter(string_builder))
          {
            using (XmlTextWriter xml_writer = new XmlTextWriter(string_writer))
            {     //build XML string
              xml_writer.Formatting = Formatting.Indented;
              xml_writer.WriteStartElement("Transaction");
              xml_writer.WriteElementString("ExactID", _encryptionService.DecryptText(_firstDataStoreSetting.GatewayID));//Gateway ID
              xml_writer.WriteElementString("Password", _encryptionService.DecryptText(_firstDataStoreSetting.Password));//Password
              xml_writer.WriteElementString("Transaction_Type", (_firstDataStoreSetting.TransactionMode == (int)TransactMode.Authorize ||
                                                                 _firstDataStoreSetting.TransactionMode == (int)TransactMode.AuthorizeAndCaptureAfterOrder ?
                                                                 TransactionType.PreAuth : TransactionType.Purchase));
              xml_writer.WriteElementString("DollarAmount", processPaymentRequest.OrderTotal.ToString());

              if (useSavedCard)
              {
                int savedCardId = Convert.ToInt32(processPaymentRequest.CustomValues[Constants.SavedCardId]);
                SavedCard card = _savedCardService.GetById(savedCardId);
                if (card == null)
                {
                  throw new NullReferenceException("Saved Card #" + savedCardId.ToString() + " is null");
                }

                xml_writer.WriteElementString("CardHoldersName", card.CardholderName);
                xml_writer.WriteElementString("TransarmorToken", card.Token);
                xml_writer.WriteElementString("CardType", card.CardType);
                xml_writer.WriteElementString("Expiry_Date", card.ExpireMonth.ToString("00") + card.ExpireYear.ToString().Substring(2, 2));
              }
              else
              {
                xml_writer.WriteElementString("Expiry_Date", processPaymentRequest.CreditCardExpireMonth.ToString("00") + processPaymentRequest.CreditCardExpireYear.ToString().Substring(2, 2));
                xml_writer.WriteElementString("CardHoldersName", processPaymentRequest.CreditCardName);
                xml_writer.WriteElementString("Card_Number", cardNumber);
                
                xml_writer.WriteElementString("VerificationStr1", (billingAddress.Address1 ?? "") + "|"
                                                                + (billingAddress.ZipPostalCode ?? "") + "|"
                                                                + (billingState != null ? $"{billingState.Name}|" : "")
                                                                + billingCountry?.ThreeLetterIsoCode);
                
                xml_writer.WriteElementString("VerificationStr2", processPaymentRequest.CreditCardCvv2);
                xml_writer.WriteElementString("CVD_Presence_Ind", "1");
              }

              xml_writer.WriteElementString("Client_Email", customer.Email);
              xml_writer.WriteElementString("Currency", _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
              xml_writer.WriteElementString("Customer_Ref", customer.CustomerGuid.ToString());
              xml_writer.WriteElementString("Reference_No", processPaymentRequest.OrderGuid.ToString());
              xml_writer.WriteElementString("Client_IP", _webHelper.GetCurrentIpAddress());
              xml_writer.WriteElementString("ZipCode", billingAddress.ZipPostalCode ?? "");
              xml_writer.WriteEndElement();
              string xml_string = string_builder.ToString();

              try
              {
                var response = SendFDRequest(xml_string);
                if (response.SelectSingleNode("Transaction_Approved").InnerText == "true")
                {
                  result.AuthorizationTransactionId = response.SelectSingleNode("Authorization_Num").InnerText;
                  processPaymentRequest.CustomValues.Add(Constants.TransactionTag, response.SelectSingleNode("Transaction_Tag").InnerText);
                  result.AuthorizationTransactionResult = string.Format("Approved ({0}: {1})", response.SelectSingleNode("Bank_Resp_Code").InnerText, response.SelectSingleNode("Bank_Message").InnerText);
                  if (_firstDataStoreSetting.TransactionMode == (int)TransactMode.AuthorizeAndCapture)
                  {
                    result.CaptureTransactionId = response.SelectSingleNode("Authorization_Num").InnerText;
                  }

                  result.AvsResult = response.SelectSingleNode("AVS").InnerText;
                  result.AllowStoringCreditCardNumber = false;

                  result.NewPaymentStatus = (_firstDataStoreSetting.TransactionMode == (int)TransactMode.Authorize || _firstDataStoreSetting.TransactionMode == (int)TransactMode.AuthorizeAndCaptureAfterOrder ?
                                              PaymentStatus.Authorized :
                                              PaymentStatus.Paid);

                  if (saveCard && !useSavedCard)
                  {
                    var token = response.SelectSingleNode("TransarmorToken").InnerText;

                    var existingCard = _savedCardService.GetByToken(customer.Id, token);
                    if (existingCard == null) //don't save the same card twice
                    {
                      SavedCard card = new SavedCard
                      {
                        BillingAddress_Id = customer.BillingAddressId.Value,
                        CardholderName = processPaymentRequest.CreditCardName,
                        CardType = response.SelectSingleNode("CardType").InnerText,
                        Customer_Id = customer.Id,
                        ExpireMonth = processPaymentRequest.CreditCardExpireMonth,
                        ExpireYear = processPaymentRequest.CreditCardExpireYear,
                        Token = token
                      };
                      _savedCardService.Insert(card);
                    }
                  }
                }
                else
                {
                  var errorCode = response.SelectSingleNode("Bank_Resp_Code").InnerText == "100" ?
                                  response.SelectSingleNode("EXact_Resp_Code").InnerText :
                                  response.SelectSingleNode("Bank_Resp_Code").InnerText;
                  var defaultMessage = response.SelectSingleNode("Bank_Resp_Code").InnerText == "100" ?
                                  response.SelectSingleNode("EXact_Message").InnerText :
                                  response.SelectSingleNode("Bank_Message").InnerText;
                  var errorMessage = _localizationService.GetLocaleStringResourceByName($"BitShift.Plugin.FirstData.CustomError.{errorCode}");
                  if (errorMessage != null)
                  {
                    result.AddError(errorMessage.ResourceValue);
                  }
                  else
                  {
                    result.AddError($"{errorCode}: {defaultMessage}");
                  }
                }
              }
              catch (Exception ex)
              {
                result.AddError(_localizationService.GetResource("BitShift.Plugin.FirstData.TechnicalError"));
                _logger.Error("Error processing payment", ex, _workContext.CurrentCustomer);
              }
            }
          }

          processPaymentRequest.CustomValues.Remove(Constants.SaveCard);
          if (processPaymentRequest.CustomValues.ContainsKey(Constants.PO) && processPaymentRequest.CustomValues[Constants.PO].ToString() == "")
            processPaymentRequest.CustomValues.Remove(Constants.PO);
        }
        catch (Exception ex)
        {
          result.AddError(_localizationService.GetResource("BitShift.Plugin.FirstData.TechnicalError"));
          _logger.Error("Error processing payment.", ex, _workContext.CurrentCustomer);
        }
      }

      return result;
    }

    /// <summary>
    /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
    /// </summary>
    /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
    public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
    {
      //nothing
    }

    /// <summary>
    /// Gets additional handling fee
    /// </summary>
    /// <param name="cart">Shoping cart</param>
    /// <returns>Additional handling fee</returns>
    public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
    {
      var result = _paymentService.CalculateAdditionalFee(cart,
          _firstDataStoreSetting.AdditionalFee, _firstDataStoreSetting.AdditionalFeePercentage);
      return result;
    }

    /// <summary>
    /// Captures payment
    /// </summary>
    /// <param name="capturePaymentRequest">Capture payment request</param>
    /// <returns>Capture payment result</returns>
    public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
    {
      var result = new CapturePaymentResult();
      var customValues = _paymentService.DeserializeCustomValues(capturePaymentRequest.Order);
      var customer = _customerService.GetCustomerById(capturePaymentRequest.Order.CustomerId);

      StringBuilder string_builder = new StringBuilder();
      using (StringWriter string_writer = new StringWriter(string_builder))
      {
        using (XmlTextWriter xml_writer = new XmlTextWriter(string_writer))
        {     //build XML string
          xml_writer.Formatting = Formatting.Indented;
          xml_writer.WriteStartElement("Transaction");
          xml_writer.WriteElementString("ExactID", _encryptionService.DecryptText(_firstDataStoreSetting.GatewayID));//Gateway ID
          xml_writer.WriteElementString("Password", _encryptionService.DecryptText(_firstDataStoreSetting.Password));//Password
          xml_writer.WriteElementString("Transaction_Type", TransactionType.TaggedPreAuthCompletion);
          xml_writer.WriteElementString("DollarAmount", capturePaymentRequest.Order.OrderTotal.ToString());
          xml_writer.WriteElementString("Authorization_Num", capturePaymentRequest.Order.AuthorizationTransactionId);
          xml_writer.WriteElementString("Reference_No", capturePaymentRequest.Order.Id.ToString());
          xml_writer.WriteElementString("Transaction_Tag", customValues[Constants.TransactionTag].ToString());
          xml_writer.WriteEndElement();
          string xml_string = string_builder.ToString();

          try
          {
            var response = SendFDRequest(xml_string);
            if (response.SelectSingleNode("Transaction_Approved").InnerText == "true")
            {
              result.CaptureTransactionId = response.SelectSingleNode("Authorization_Num").InnerText;
              customValues[Constants.TransactionTag] = response.SelectSingleNode("Transaction_Tag").InnerText;
              result.CaptureTransactionResult = string.Format("Approved ({0}: {1})", response.SelectSingleNode("Bank_Resp_Code").InnerText, response.SelectSingleNode("Bank_Message").InnerText);
              result.NewPaymentStatus = PaymentStatus.Paid;

              capturePaymentRequest.Order.CustomValuesXml = SerializeCustomValues(customValues);
              _orderService.UpdateOrder(capturePaymentRequest.Order);
            }
            else
            {
              var errorMessage = _localizationService.GetLocaleStringResourceByName($"BitShift.Plugin.FirstData.CustomError.{response.SelectSingleNode("Bank_Resp_Code").InnerText}");
              if (errorMessage != null)
              {
                result.AddError(errorMessage.ResourceValue);
              }
              else
              {
                result.AddError($"{response.SelectSingleNode("Bank_Resp_Code").InnerText}: {response.SelectSingleNode("Bank_Message").InnerText}");
              }
            }
          }
          catch (Exception ex)
          {
            result.AddError(ex.Message);
            _logger.Error("Error capturing payment", ex, customer);
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Refunds a payment
    /// </summary>
    /// <param name="refundPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
    {
      var result = new RefundPaymentResult();
      var customValues = _paymentService.DeserializeCustomValues(refundPaymentRequest.Order);
      var customer = _customerService.GetCustomerById(refundPaymentRequest.Order.CustomerId);
      var latestRefundTag = customValues.Keys.Where(k => k.StartsWith("Refund Tag")).OrderByDescending(k => k).FirstOrDefault();
      var newRefundNumber = latestRefundTag == null ? 1 : Convert.ToInt32(latestRefundTag.Substring(10)) + 1;
      var newRefundTag = $"Refund Tag {newRefundNumber:D2}";

      if (refundPaymentRequest.Order.CaptureTransactionId != null && refundPaymentRequest.Order.CaptureTransactionId.Contains("|"))
      {
        if (customValues.ContainsKey(Constants.TransactionTag))
          customValues.Remove(Constants.TransactionTag);

        customValues.Add(Constants.TransactionTag, refundPaymentRequest.Order.CaptureTransactionId.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[1]);
        refundPaymentRequest.Order.CaptureTransactionId = refundPaymentRequest.Order.CaptureTransactionId.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[0];
        refundPaymentRequest.Order.CustomValuesXml = SerializeCustomValues(customValues);
      }

      StringBuilder string_builder = new StringBuilder();
      try
      {
        using (StringWriter string_writer = new StringWriter(string_builder))
        {
          using (XmlTextWriter xml_writer = new XmlTextWriter(string_writer))
          {     //build XML string
            xml_writer.Formatting = Formatting.Indented;
            xml_writer.WriteStartElement("Transaction");
            xml_writer.WriteElementString("ExactID", _encryptionService.DecryptText(_firstDataStoreSetting.GatewayID));//Gateway ID
            xml_writer.WriteElementString("Password", _encryptionService.DecryptText(_firstDataStoreSetting.Password));//Password
            xml_writer.WriteElementString("Transaction_Type", TransactionType.TaggedRefund);
            xml_writer.WriteElementString("DollarAmount", refundPaymentRequest.AmountToRefund.ToString());
            xml_writer.WriteElementString("Authorization_Num", refundPaymentRequest.Order.CaptureTransactionId);
            xml_writer.WriteElementString("Transaction_Tag", customValues[Constants.TransactionTag].ToString());
            xml_writer.WriteEndElement();
            string xml_string = string_builder.ToString();

            try
            {
              var response = SendFDRequest(xml_string);
              string_builder.Append("TransactionSent-Response:");
              string_builder.Append(response.ToString());
              if (response.SelectSingleNode("Transaction_Approved").InnerText == "true")
              {
                result.NewPaymentStatus = refundPaymentRequest.IsPartialRefund ? PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded;
                customValues.Add(newRefundTag, response.SelectSingleNode("Transaction_Tag").InnerText);
                refundPaymentRequest.Order.CustomValuesXml = SerializeCustomValues(customValues);

                _orderService.UpdateOrder(refundPaymentRequest.Order);
              }
              else
              {
                var errorMessage = _localizationService.GetLocaleStringResourceByName($"BitShift.Plugin.FirstData.CustomError.{response.SelectSingleNode("Bank_Resp_Code").InnerText}");
                if (errorMessage != null)
                {
                  result.AddError(errorMessage.ResourceValue);
                }
                else
                {
                  result.AddError($"{response.SelectSingleNode("Bank_Resp_Code").InnerText}: {response.SelectSingleNode("Bank_Message").InnerText}");
                }
              }
            }
            catch (Exception ex)
            {
              result.AddError(ex.Message);
              _logger.Error("Error refunding payment - Sending to FD or reading response - " + string_builder.ToString(), ex, customer);
            }
          }
        }
      }
      catch (Exception ex)
      {
        result.AddError("Error refunding payment.  See Log for more details");
        _logger.Error("Error refunding payment - Building request - " + string_builder.ToString(), ex, customer);
      }

      return result;
    }

    /// <summary>
    /// Voids a payment
    /// </summary>
    /// <param name="voidPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
    {
      var result = new VoidPaymentResult();
      var customValues = _paymentService.DeserializeCustomValues(voidPaymentRequest.Order);
      var customer = _customerService.GetCustomerById(voidPaymentRequest.Order.CustomerId);

      StringBuilder string_builder = new StringBuilder();
      using (StringWriter string_writer = new StringWriter(string_builder))
      {
        using (XmlTextWriter xml_writer = new XmlTextWriter(string_writer))
        {     //build XML string
          xml_writer.Formatting = Formatting.Indented;
          xml_writer.WriteStartElement("Transaction");
          xml_writer.WriteElementString("ExactID", _encryptionService.DecryptText(_firstDataStoreSetting.GatewayID));//Gateway ID
          xml_writer.WriteElementString("Password", _encryptionService.DecryptText(_firstDataStoreSetting.Password));//Password
          xml_writer.WriteElementString("Transaction_Type", TransactionType.TaggedVoid);
          xml_writer.WriteElementString("DollarAmount", voidPaymentRequest.Order.OrderTotal.ToString());
          xml_writer.WriteElementString("Authorization_Num", voidPaymentRequest.Order.AuthorizationTransactionId);
          xml_writer.WriteElementString("Transaction_Tag", customValues[Constants.TransactionTag].ToString());
          xml_writer.WriteEndElement();
          string xml_string = string_builder.ToString();

          try
          {
            var response = SendFDRequest(xml_string);
            if (response.SelectSingleNode("Transaction_Approved").InnerText == "true")
            {
              result.NewPaymentStatus = PaymentStatus.Voided;
            }
            else
            {
              var errorMessage = _localizationService.GetLocaleStringResourceByName($"BitShift.Plugin.FirstData.CustomError.{response.SelectSingleNode("Bank_Resp_Code").InnerText}");
              if (errorMessage != null)
              {
                result.AddError(errorMessage.ResourceValue);
              }
              else
              {
                result.AddError($"{response.SelectSingleNode("Bank_Resp_Code").InnerText}: {response.SelectSingleNode("Bank_Message").InnerText}");
              }
            }
          }
          catch (Exception ex)
          {
            result.AddError(ex.Message);
            _logger.Error("Error voiding payment", ex, customer);
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Process recurring payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>Process payment result</returns>
    public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
    {
      var result = new ProcessPaymentResult();

      var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
      if(!customer.BillingAddressId.HasValue)
      {
        throw new ArgumentException("customer must have a billing address");
      }
      var billingAddress = _addressService.GetAddressById(customer.BillingAddressId.Value);
      var billingState = _stateProvinceService.GetStateProvinceById(billingAddress.StateProvinceId.Value);
      var billingCountry = _countryService.GetCountryById(billingAddress.CountryId.Value);
      var cardNumber = processPaymentRequest.CreditCardNumber.Split('|')[0];
      bool useSavedCard = (processPaymentRequest.CreditCardNumber.StartsWith("T"));

      StringBuilder string_builder = new StringBuilder();
      try
      {
        using (StringWriter string_writer = new StringWriter(string_builder))
        {
          using (XmlTextWriter xml_writer = new XmlTextWriter(string_writer))
          {     //build XML string
            xml_writer.Formatting = Formatting.Indented;
            xml_writer.WriteStartElement("Transaction");
            xml_writer.WriteElementString("ExactID", _encryptionService.DecryptText(_firstDataStoreSetting.GatewayID));//Gateway ID
            xml_writer.WriteElementString("Password", _encryptionService.DecryptText(_firstDataStoreSetting.Password));//Password
            xml_writer.WriteElementString("Transaction_Type", (_firstDataStoreSetting.TransactionMode == (int)TransactMode.Authorize ? TransactionType.PreAuth : TransactionType.Purchase));  //check settings
            xml_writer.WriteElementString("DollarAmount", processPaymentRequest.OrderTotal.ToString());

            if (processPaymentRequest.InitialOrder.Id == 0)
            {
              if (useSavedCard)
              {
                int savedCardId = Convert.ToInt32(processPaymentRequest.CreditCardNumber.Substring(1));
                SavedCard card = _savedCardService.GetById(savedCardId);
                if (card == null)
                {
                  throw new NullReferenceException("Saved Card #" + savedCardId.ToString() + " is null");
                }
                result.SubscriptionTransactionId = "T" + card.Id.ToString();
                xml_writer.WriteElementString("CardHoldersName", card.CardholderName);
                xml_writer.WriteElementString("TransarmorToken", card.Token);
                xml_writer.WriteElementString("CardType", card.CardType);
                xml_writer.WriteElementString("Expiry_Date", card.ExpireMonth.ToString("00") + card.ExpireYear.ToString().Substring(2, 2));
              }
              else
              {
                xml_writer.WriteElementString("Expiry_Date", processPaymentRequest.CreditCardExpireMonth.ToString("00") + processPaymentRequest.CreditCardExpireYear.ToString().Substring(2, 2));
                xml_writer.WriteElementString("CardHoldersName", processPaymentRequest.CreditCardName);
                xml_writer.WriteElementString("Card_Number", cardNumber);
                xml_writer.WriteElementString("VerificationStr1", (billingAddress.Address1 ?? "") + "|"
                                                                + (billingAddress.ZipPostalCode ?? "") + "|"
                                                                + (billingState?.Name ?? "") + "|"
                                                                + billingCountry?.ThreeLetterIsoCode);                
                xml_writer.WriteElementString("VerificationStr2", processPaymentRequest.CreditCardCvv2);
                xml_writer.WriteElementString("CVD_Presence_Ind", "1");
              }
            }
            else
            {
              var initialOrder = _orderService.GetOrderById(processPaymentRequest.InitialOrder.Id);
              int savedCardId = Convert.ToInt32(initialOrder.SubscriptionTransactionId.Substring(1));
              SavedCard card = _savedCardService.GetById(savedCardId);
              if (card == null)
              {
                throw new NullReferenceException("Saved Card #" + savedCardId.ToString() + " is null");
              }

              xml_writer.WriteElementString("CardHoldersName", card.CardholderName);
              xml_writer.WriteElementString("TransarmorToken", card.Token);
              xml_writer.WriteElementString("CardType", card.CardType);
              xml_writer.WriteElementString("Expiry_Date", card.ExpireMonth.ToString("00") + card.ExpireYear.ToString().Substring(2, 2));
            }

            xml_writer.WriteElementString("Client_Email", customer.Email);
            xml_writer.WriteElementString("Currency", _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
            xml_writer.WriteElementString("Customer_Ref", customer.CustomerGuid.ToString());
            xml_writer.WriteElementString("Reference_No", processPaymentRequest.OrderGuid.ToString());
            xml_writer.WriteElementString("Client_IP", _webHelper.GetCurrentIpAddress());
            xml_writer.WriteElementString("ZipCode", billingAddress.ZipPostalCode ?? "");
            xml_writer.WriteEndElement();
            string xml_string = string_builder.ToString();

            try
            {
              var response = SendFDRequest(xml_string);
              if (response.SelectSingleNode("Transaction_Approved").InnerText == "true")
              {
                result.AuthorizationTransactionId = response.SelectSingleNode("Authorization_Num").InnerText;
                processPaymentRequest.CustomValues.Add(Constants.TransactionTag, response.SelectSingleNode("Transaction_Tag").InnerText);

                result.AuthorizationTransactionResult = string.Format("Approved ({0}: {1})", response.SelectSingleNode("Bank_Resp_Code").InnerText, response.SelectSingleNode("Bank_Message").InnerText);
                if (_firstDataStoreSetting.TransactionMode == (int)TransactMode.AuthorizeAndCapture)
                {
                  result.CaptureTransactionId = response.SelectSingleNode("Authorization_Num").InnerText;
                }

                result.AvsResult = response.SelectSingleNode("AVS").InnerText;
                result.AllowStoringCreditCardNumber = false;

                result.NewPaymentStatus = (_firstDataStoreSetting.TransactionMode == (int)TransactMode.Authorize ? PaymentStatus.Authorized : PaymentStatus.Paid);

                if (!useSavedCard)
                {
                  var token = response.SelectSingleNode("TransarmorToken").InnerText;

                  var existingCard = _savedCardService.GetByToken(customer.Id, token);
                  if (existingCard == null) //don't save the same card twice
                  {
                    SavedCard card = new SavedCard
                    {
                      BillingAddress_Id = customer.BillingAddressId.Value,
                      CardholderName = processPaymentRequest.CreditCardName,
                      CardType = response.SelectSingleNode("CardType").InnerText,
                      Customer_Id = customer.Id,
                      ExpireMonth = processPaymentRequest.CreditCardExpireMonth,
                      ExpireYear = processPaymentRequest.CreditCardExpireYear,
                      Token = token
                    };
                    _savedCardService.Insert(card);

                    result.SubscriptionTransactionId = "T" + card.Id.ToString();
                  }
                }
              }
              else
              {
                result.AddError(string.Format("Error {0}: {1}", response.SelectSingleNode("Bank_Resp_Code").InnerText, response.SelectSingleNode("Bank_Message").InnerText));
              }
            }
            catch (Exception ex)
            {
              result.AddError(_localizationService.GetResource("BitShift.Plugin.FirstData.TechnicalError"));
              _logger.Error("Error processing payment", ex, _workContext.CurrentCustomer);
            }
          }
        }
      }
      catch (Exception ex)
      {
        result.AddError(_localizationService.GetResource("BitShift.Plugin.FirstData.TechnicalError"));
        _logger.Error("Error processing payment", ex, _workContext.CurrentCustomer);
      }

      return result;
    }

    /// <summary>
    /// Cancels a recurring payment
    /// </summary>
    /// <param name="cancelPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
    {
      var result = new CancelRecurringPaymentResult();
      //nothing special needed from First Data
      return result;
    }

    /// <summary>
    /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>Result</returns>
    public bool CanRePostProcessPayment(Order order)
    {
      if (order == null)
        throw new ArgumentNullException("order");

      //it's not a redirection payment method. So we always return false
      return false;
    }

    public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
    {
      _firstDataStoreSetting = _firstDataStoreSettingService.GetByStore(_storeContext.CurrentStore.Id);
      var paymentInfo = new ProcessPaymentRequest();
      if (_firstDataStoreSetting.TransactionMode != (int)TransactMode.HostedPaymentPageAuthOnly &&
          _firstDataStoreSetting.TransactionMode != (int)TransactMode.HostedPaymentPagePostCapture)
      {
        paymentInfo.CreditCardType = form["CreditCardType"];
        paymentInfo.CreditCardName = form["CardholderName"];
        paymentInfo.CreditCardNumber = form["CardNumber"];
        paymentInfo.CreditCardExpireMonth = int.Parse(form["ExpireMonth"]);
        paymentInfo.CreditCardExpireYear = int.Parse(form["ExpireYear"]);
        paymentInfo.CreditCardCvv2 = form["CardCode"];
        if (_firstDataStoreSetting.EnablePurchaseOrderNumber)
        {
          paymentInfo.CustomValues.Add(Constants.PO, form["PurchaseOrderNumber"]);
        }

        if (!string.IsNullOrEmpty(form["SaveCard"]))
        {
          paymentInfo.CustomValues.Add(Constants.SaveCard, form["SaveCard"][0].Split(',')[0].ToString());
        }

        if (!string.IsNullOrEmpty(form["savedCardId"]) && int.TryParse(form["savedCardId"], out int savedCardId))
        {
          paymentInfo.CustomValues.Add(Constants.SavedCardId, savedCardId.ToString());
        }
      }

      return paymentInfo;
    }

    public IList<string> ValidatePaymentForm(IFormCollection form)
    {

      var warnings = new List<string>();

      if (_firstDataStoreSetting.TransactionMode == (int)TransactMode.HostedPaymentPagePostCapture ||
          _firstDataStoreSetting.TransactionMode == (int)TransactMode.HostedPaymentPageAuthOnly)
      {
        return warnings;
      }

      if (!string.IsNullOrEmpty(form["savedCardId"]) && int.TryParse(form["savedCardId"], out int savedCardId))
      {
        return warnings;
      }

      //validate
      var validator = new PaymentInfoValidator(_localizationService, _firstDataStoreSetting);
      var model = new PaymentInfoModel()
      {
        CardholderName = form["CardholderName"],
        CardNumber = form["CardNumber"],
        CardCode = form["CardCode"],
        ExpireMonth = form["ExpireMonth"],
        ExpireYear = form["ExpireYear"],
      };
      var validationResult = validator.Validate(model);
      if (!validationResult.IsValid)
        foreach (var error in validationResult.Errors)
          warnings.Add(error.ErrorMessage);
      return warnings;
    }

    /// <summary>
    /// Returns a value indicating whether payment method should be hidden during checkout
    /// </summary>
    /// <param name="cart">Shoping cart</param>
    /// <returns>true - hide; false - display.</returns>
    public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
    {
      //you can put any logic here
      //for example, hide this payment method if all products in the cart are downloadable
      //or hide this payment method if current customer is from certain country
      return false;
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
      return $"{_webHelper.GetStoreLocation()}Admin/FirstData/Configure";
    }

    public Type GetControllerType()
    {
      return typeof(FirstDataController);
    }

    public override void Install()
    {
      //settings
      var settings = new FirstDataSettings
      {
        SandboxURL = "https://api.demo.globalgatewaye4.firstdata.com/transaction/v12",
        ProductionURL = "https://api.globalgatewaye4.firstdata.com/transaction/v12"
      };
      _settingService.SaveSetting(settings);

      var defaultStoreSetting = new FirstDataStoreSetting
      {
        UseSandbox = true,
        TransactionMode = (int)TransactMode.AuthorizeAndCapture,
        StoreId = 0
      };
      _firstDataStoreSettingService.Insert(defaultStoreSetting);

      //locales
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Notes", "If you're using this gateway, ensure that your primary store currency is supported by FirstData Global Gateway e4.  Recurring Payments and Card Saving require the TransArmor service on your merchant account.  Read more <a href=\"http://bitshiftweb.com/transarmor-tokenization\">here</a>.");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.UseSandbox", "Use Sandbox");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.TransactModeValues", "Transaction mode");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.TransactModeValues.Hint", "Choose transaction mode");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.HMAC", "HMAC");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.HMAC.Hint", "The HMAC for your terminal");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.GatewayID", "Gateway ID");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.GatewayID.Hint", "Specify gateway identifier.");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.Password", "Password");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.Password.Hint", "Specify terminal password.");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.KeyID", "Key ID");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.KeyID.Hint", "Specify key identifier.");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AdditionalFee", "Additional fee");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AdditionalFeePercentage", "Additinal fee. Use percentage");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.ExpiryDateError", "Card expired");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.TechnicalError", "There has been an unexpected error while processing your payment.");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Payment.SaveCard", "Save Card");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Payment.SaveCard.Hint", "Save card for future use.  Your card number is tokenized on First Data's servers and not stored on our site.");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnableRecurringPayments", "Enable Recurring Payments");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnableRecurringPayments.Hint", "Allows manual recurring payments by using the TransArmor Token");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnableCardSaving", "Enable Card Saving");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnableCardSaving.Hint", "Allows customers to choose to save a card when they use it.  The TransArmor Token is saved instead of the CC number");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Payment.UseCardLabel", "Use");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Payment.CardDescription", "{0} ending in {1}");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Payment.ExpirationDescription", "Expires {0}/{1}");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Payment.ExpiredLabel", "Expired");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Payment.SavedCardsLabel", "Saved Cards");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Payment.NewCardLabel", "Enter new card");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Payment.PurchaseOrder", "Purchase Order");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnablePurchaseOrderNumber", "Enable Purchase Order Number");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnablePurchaseOrderNumber.Hint", "Will optionally capture a purchase order number and append it to the Authorization Transaction ID in the Order Details");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.SandboxURL", "Sandbox URL");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.SandboxURL.Hint", "Where to send sandbox transactions");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.ProductionURL", "Production URL");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.ProductionURL.Hint", "Where to send real transactions");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.StoreID", "Store");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.StoreID.Hint", "The store that these settings apply to.  The Default Store settings will be used for any store that doesn't have settings.");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Configure", "Global Settings");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Stores", "Store Specific Settings");
      _localizationService.AddOrUpdatePluginLocaleResource("Bitshift.Plugin.FirstData.Storenotes", "Set your store specific settings here.  A store without it's own entry will use the Default Store Settings");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Stores.Revert", "Revert to Default Settings");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.StoreSettingsSaved", "Store settings saved successfully");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards", "Saved Cards");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards.NoCards", "You don't have any cards saved.  Complete an order and check the \"Save Card\" box during checkout.");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards.Fields.Type", "Type");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards.Fields.Name", "Cardholder Name");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards.Fields.Last4", "Number");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards.Fields.Expires", "Expires");
      _localizationService.AddOrUpdatePluginLocaleResource("bitshift.plugin.firstdata.savedcards.description", "Review your saved cards here.  You can save a new card during checkout.");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.PaymentPageID", "Payment Page ID");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.PaymentPageID.Hint", "The payment page to use during checkout");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.TransactionKey", "Transaction Key");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.TransactionKey.Hint", "The payment page's transaction key");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.ResponseKey", "Response Key");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.ResponseKey.Hint", "The response key First Data will send back to the plugin");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.HostedPaymentPage.AuthorizationEmpty", "Error processing payment, Authorization is empty");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowVisa", "Allow Visa");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowVisa.Hint", "Check to allow Visa payments");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowMastercard", "Allow Mastercard");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowMastercard.Hint", "Check to allow Mastercard payments");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowAmex", "Allow Amex");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowAmex.Hint", "Check to allow American Express payments");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowDiscover", "Allow Discover");
      _localizationService.AddOrUpdatePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowDiscover.Hint", "Check to allow Discover payments");
      _localizationService.AddOrUpdatePluginLocaleResource("bitshift.plugin.firstdata.paymentmethoddescription", "Credit card");
      base.Install();
    }

    public override void Uninstall()
    {
      //settings
      _settingService.DeleteSetting<FirstDataSettings>();

      //locales
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Notes");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.UseSandbox");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.UseSandbox.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.TransactModeValues");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.TransactModeValues.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.HMAC");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.HMAC.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.GatewayID");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.GatewayID.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.Password");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.Password.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.KeyID");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.KeyID.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AdditionalFee");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AdditionalFee.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AdditionalFeePercentage");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AdditionalFeePercentage.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.ExpiryDateError");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.TechnicalError");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Payment.SaveCard");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnableRecurringPayments");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnableRecurringPayments.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnableCardSaving");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnableCardSaving.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Payment.UseCardLabel");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Payment.CardDescription");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Payment.ExpirationDescription");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Payment.ExpiredLabel");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Payment.SavedCardsLabel");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Payment.NewCardLabel");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnablePurchaseOrderNumber");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.EnablePurchaseOrderNumber.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.SandboxURL");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.SandboxURL.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.ProductionURL");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.ProductionURL.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.StoreID");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.StoreID.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Configure");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Stores");
      _localizationService.DeletePluginLocaleResource("Bitshift.Plugin.FirstData.Storenotes");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Stores.Revert");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.StoreSettingsSaved");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards.NoCards");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards.Fields.Type");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards.Fields.Name");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards.Fields.Last4");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.SavedCards.Fields.Expires");
      _localizationService.DeletePluginLocaleResource("bitshift.plugin.firstdata.savedcards.description");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.PaymentPageID");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.PaymentPageID.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.TransactionKey");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.TransactionKey.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.ResponseKey");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.ResponseKey.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.HostedPaymentPage.AuthorizationEmpty");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowVisa");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowVisa.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowMastercard");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowMastercard.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowAmex");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowAmex.Hint");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowDiscover");
      _localizationService.DeletePluginLocaleResource("BitShift.Plugin.FirstData.Fields.AllowDiscover.Hint");
      _localizationService.DeletePluginLocaleResource("bitshift.plugin.firstdata.paymentmethoddescription");

      base.Uninstall();
    }

    #endregion

    #region Saved Cards Widget

    public void GetPublicViewComponent(string widgetZone, out string viewComponentName) => viewComponentName = "SavedCardsLink";

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    /// <returns>Widget zones</returns>
    public IList<string> GetWidgetZones()
    {
      return new List<string> { PublicWidgetZones.AccountNavigationAfter };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether capture is supported
    /// </summary>
    public bool SupportCapture => true;

    /// <summary>
    /// Gets a value indicating whether partial refund is supported
    /// </summary>
    public bool SupportPartiallyRefund => true;

    /// <summary>
    /// Gets a value indicating whether refund is supported
    /// </summary>
    public bool SupportRefund => true;

    /// <summary>
    /// Gets a value indicating whether void is supported
    /// </summary>
    public bool SupportVoid => true;

    /// <summary>
    /// Gets a recurring payment type of payment method
    /// </summary>
    public RecurringPaymentType RecurringPaymentType
    {
      get
      {
        if (_firstDataStoreSetting.EnableRecurringPayments)
        {
          return RecurringPaymentType.Manual;
        }
        else
        {
          return RecurringPaymentType.NotSupported;
        }
      }
    }

    /// <summary>
    /// Gets a payment method type
    /// </summary>
    public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

    /// <summary>
    /// Gets a value indicating whether we should display a payment information page for this plugin
    /// </summary>
    public bool SkipPaymentInfo => false;

    public string PaymentMethodDescription => _localizationService.GetResource("BitShift.Plugin.FirstData.PaymentMethodDescription");

    public bool HideInWidgetList => true;
    #endregion
  }
}
