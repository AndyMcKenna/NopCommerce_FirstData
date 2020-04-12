using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using BitShift.Plugin.Payments.FirstData.Domain;
using BitShift.Plugin.Payments.FirstData.Models;
using BitShift.Plugin.Payments.FirstData.Validators;
using Moq;
using Nop.Services.Localization;
using Nop.Tests;
using Nop.Web.MVC.Tests.Public.Validators;
using NUnit.Framework;

namespace BitShift.Plugin.Payments.FirstData.Tests
{
  [TestFixture]
  public class PaymentInfoValidatorTests : BaseValidatorTests
  {
    private PaymentInfoValidator _validator;
    private Mock<ILocalizationService> _localizationService;
    private FirstDataStoreSetting _storeSetting;

    private Dictionary<string, List<string>> _testCardNumbers = new Dictionary<string, List<string>>
    {
      { "Amex", new List<string>
        {
          "378282246310005",
          "371449635398431",
          "378734493671000"
        }
      },
      { "Discover", new List<string>
        {
          "6011000990139424",
          "6011111111111117"
        }
      },
      { "Mastercard", new List<string>
        {
          "5555555555554444",
          "5105105105105100"
        }
      },
      { "Visa", new List<string>
        {
          "4111111111111111",
          "4012888888881881",
          "4222222222222"
        }
      }
    };

    [SetUp]
    public void Setup()
    {
      Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

      _storeSetting = new FirstDataStoreSetting
      {
        AllowAmex = true,
        AllowDiscover = true,
        AllowMastercard = true,
        AllowVisa = true
      };

      _localizationService = new Mock<ILocalizationService>();
      _localizationService.Setup(x => x.GetResource(It.IsAny<string>())).Returns<string>(x => x);

      _validator = new PaymentInfoValidator(_localizationService.Object, _storeSetting);
      base.Setup();
    }

    [Test]
    public void Success()
    {
      var paymentInfoModel = new PaymentInfoModel
      {
        CardholderName = "John Doe",
        CardNumber = _testCardNumbers["Visa"].First(),
        CardCode = "123",
        ExpireMonth = "01",
        ExpireYear = (DateTime.Today.Year + 1).ToString()
      };

      _validator.Validate(paymentInfoModel).IsValid.ShouldBeTrue();
    }

    [Test]
    public void CardBrands_AllNumbers()
    {
      var paymentInfoModel = new PaymentInfoModel
      {
        CardholderName = "John Doe",
        CardNumber = "",
        CardCode = "123",
        ExpireMonth = "01",
        ExpireYear = (DateTime.Today.Year + 1).ToString()
      };

      foreach (var brand in _testCardNumbers.Keys)
      {
        foreach (var number in _testCardNumbers[brand])
        {
          paymentInfoModel.CardNumber = number;
          if (!_validator.Validate(paymentInfoModel).IsValid)
          {
            Assert.Fail($"Card Failed: {brand}: {number}");
          }
        }
      }
    }

    [Test]
    public void CardholderName_Invalid()
    {
      var paymentInfoModel = new PaymentInfoModel
      {
        CardholderName = "",
        CardNumber = "4111111111111111",
        CardCode = "123",
        ExpireMonth = "01",
        ExpireYear = (DateTime.Today.Year + 1).ToString()
      };

      var result = _validator.Validate(paymentInfoModel);
      result.IsValid.ShouldBeFalse();
      result.Errors
          .Any(e => e.PropertyName == "CardholderName" && e.ErrorMessage == "Payment.CardholderName.Required")
          .ShouldBeTrue();
    }

    [Test]
    public void CardCode_Invalid()
    {
      var paymentInfoModel = new PaymentInfoModel
      {
        CardholderName = "John Doe",
        CardNumber = "4111111111111111",
        CardCode = "",
        ExpireMonth = "01",
        ExpireYear = (DateTime.Today.Year + 1).ToString()
      };

      var result = _validator.Validate(paymentInfoModel);
      result.IsValid.ShouldBeFalse();
      result.Errors
          .Any(e => e.PropertyName == "CardCode" && e.ErrorMessage == "Payment.CardCode.Wrong")
          .ShouldBeTrue();

      paymentInfoModel.CardCode = "12345";
      result = _validator.Validate(paymentInfoModel);
      result.IsValid.ShouldBeFalse();
      result.Errors
          .Any(e => e.PropertyName == "CardCode" && e.ErrorMessage == "Payment.CardCode.Wrong")
          .ShouldBeTrue();

      paymentInfoModel.CardCode = "abc";
      result = _validator.Validate(paymentInfoModel);
      result.IsValid.ShouldBeFalse();
      result.Errors
          .Any(e => e.PropertyName == "CardCode" && e.ErrorMessage == "Payment.CardCode.Wrong")
          .ShouldBeTrue();
    }

    [Test]
    public void ExpireMonth_Invalid()
    {
      var paymentInfoModel = new PaymentInfoModel
      {
        CardholderName = "John Doe",
        CardNumber = "4111111111111111",
        CardCode = "123",
        ExpireMonth = "",
        ExpireYear = (DateTime.Today.Year + 1).ToString()
      };

      var result = _validator.Validate(paymentInfoModel);
      result.IsValid.ShouldBeFalse();
      result.Errors
          .Any(e => e.PropertyName == "ExpireMonth" && e.ErrorMessage == "Payment.ExpireMonth.Required")
          .ShouldBeTrue();
    }

    [Test]
    public void ExpireYear_Invalid()
    {
      var paymentInfoModel = new PaymentInfoModel
      {
        CardholderName = "John Doe",
        CardNumber = "4111111111111111",
        CardCode = "123",
        ExpireMonth = "01",
        ExpireYear = ""
      };

      var result = _validator.Validate(paymentInfoModel);
      result.IsValid.ShouldBeFalse();
      result.Errors
          .Any(e => e.PropertyName == "ExpireYear" && e.ErrorMessage == "Payment.ExpireYear.Required")
          .ShouldBeTrue();
    }

    [Test]
    public void ExpiryDate_Invalid()
    {
      var paymentInfoModel = new PaymentInfoModel
      {
        CardholderName = "John Doe",
        CardNumber = "4111111111111111",
        CardCode = "123",
        ExpireMonth = "01",
        ExpireYear = (DateTime.Today.Year - 1).ToString()
      };

      var result = _validator.Validate(paymentInfoModel);
      result.IsValid.ShouldBeFalse();
      result.Errors
          .Any(e => e.PropertyName == "ExpiryDate" && e.ErrorMessage == "BitShift.Plugin.FirstData.ExpiryDateError")
          .ShouldBeTrue();

      if (DateTime.Today.Month > 1)
      {
        paymentInfoModel.ExpireMonth = (DateTime.Today.Month - 1).ToString();
        paymentInfoModel.ExpireYear = DateTime.Today.Year.ToString();
        result = _validator.Validate(paymentInfoModel);
        result.IsValid.ShouldBeFalse();
        result.Errors
            .Any(e => e.PropertyName == "ExpiryDate" && e.ErrorMessage == "BitShift.Plugin.FirstData.ExpiryDateError")
            .ShouldBeTrue();
      }
    }

    [Test]
    public void Cardbrand_Amex_Blocked()
    {
      var paymentInfoModel = new PaymentInfoModel
      {
        CardholderName = "John Doe",
        CardCode = "123",
        ExpireMonth = "01",
        ExpireYear = (DateTime.Today.Year + 1).ToString()
      };

      var storeSetting = new FirstDataStoreSetting
      {
        AllowAmex = false,
        AllowDiscover = true,
        AllowMastercard = true,
        AllowVisa = true
      };

      OnlySpecifiedBrandsBlocked("Amex", paymentInfoModel, storeSetting);
    }

    [Test]
    public void Cardbrand_Discover_Blocked()
    {
      var paymentInfoModel = new PaymentInfoModel
      {
        CardholderName = "John Doe",
        CardCode = "123",
        ExpireMonth = "01",
        ExpireYear = (DateTime.Today.Year + 1).ToString()
      };

      var storeSetting = new FirstDataStoreSetting
      {
        AllowAmex = true,
        AllowDiscover = false,
        AllowMastercard = true,
        AllowVisa = true
      };

      OnlySpecifiedBrandsBlocked("Discover", paymentInfoModel, storeSetting);
    }

    [Test]
    public void Cardbrand_Mastercard_Blocked()
    {
      var paymentInfoModel = new PaymentInfoModel
      {
        CardholderName = "John Doe",
        CardCode = "123",
        ExpireMonth = "01",
        ExpireYear = (DateTime.Today.Year + 1).ToString()
      };

      var storeSetting = new FirstDataStoreSetting
      {
        AllowAmex = true,
        AllowDiscover = true,
        AllowMastercard = false,
        AllowVisa = true
      };

      OnlySpecifiedBrandsBlocked("Mastercard", paymentInfoModel, storeSetting);
    }

    [Test]
    public void Cardbrand_Visa_Blocked()
    {
      var paymentInfoModel = new PaymentInfoModel
      {
        CardholderName = "John Doe",
        CardCode = "123",
        ExpireMonth = "01",
        ExpireYear = (DateTime.Today.Year + 1).ToString()
      };

      var storeSetting = new FirstDataStoreSetting
      {
        AllowAmex = true,
        AllowDiscover = true,
        AllowMastercard = true,
        AllowVisa = false
      };

      OnlySpecifiedBrandsBlocked("Visa", paymentInfoModel, storeSetting);
    }

    private void OnlySpecifiedBrandsBlocked(string targettedBrand, PaymentInfoModel paymentInfoModel, FirstDataStoreSetting setting)
    {
      var validator = new PaymentInfoValidator(_localizationService.Object, setting);

      foreach (var brand in _testCardNumbers.Keys)
      {
        foreach (var number in _testCardNumbers[brand])
        {
          paymentInfoModel.CardNumber = number;
          var result = validator.Validate(paymentInfoModel);
          if (brand == targettedBrand)
          {
            result.IsValid.ShouldBeFalse();
          }
          else
          {
            result.IsValid.ShouldBeTrue();
          }
        }
      }
    }
  }
}
