using BitShift.Plugin.Payments.FirstData.Domain;
using BitShift.Plugin.Payments.FirstData.Models;
using BitShift.Plugin.Payments.FirstData.Services;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using System;

namespace BitShift.Plugin.Payments.FirstData.Factories
{
  /// <summary>
  /// Represents the common models factory
  /// </summary>
  public partial class SavedCardModelFactory : ISavedCardModelFactory
  {
    #region Fields

    private FirstDataStoreSetting _firstDataStoreSetting;
    private readonly IFirstDataStoreSettingService _firstDataStoreSettingService;
    private readonly IStoreContext _storeContext;
    private readonly ILocalizationService _localizationService;
    private readonly OrderSettings _orderSettings;
    private readonly ISavedCardService _savedCardService;
    private readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public SavedCardModelFactory(IFirstDataStoreSettingService firstDataStoreSettingService, IPluginService pluginService,
        IStoreContext storeContext,
        ILocalizationService localizationService, OrderSettings orderSettings,
        ISavedCardService savedCardService, IWorkContext workContext)
    {
      _storeContext = storeContext;
      _localizationService = localizationService;
      _orderSettings = orderSettings;
      _savedCardService = savedCardService;
      _workContext = workContext;
      _firstDataStoreSettingService = firstDataStoreSettingService;

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

    public SavedCardModel PrepareSavedCardModel(SavedCard card)
    {
      SavedCardModel model = new SavedCardModel
      {
        Id = card.Id,
        CardholderName = card.CardholderName,
        Last4Digits = (!string.IsNullOrEmpty(card.Token) && card.Token.Length > 4 ? card.Token.Substring(card.Token.Length - 4) : ""),
        ExpireMonth = card.ExpireMonth,
        ExpireYear = card.ExpireYear,
        CardType = card.CardType,
        IsExpired = (DateTime.Now > new DateTime(card.ExpireYear, card.ExpireMonth, 1).AddMonths(1)),
        CardDescription = _localizationService.GetResource("BitShift.Plugin.FirstData.Payment.CardDescription"),
        ExpirationDescription = _localizationService.GetResource("BitShift.Plugin.FirstData.Payment.ExpirationDescription"),
        ExpiredLabel = _localizationService.GetResource("BitShift.Plugin.FirstData.Payment.ExpiredLabel"),
        UseCardLabel = _localizationService.GetResource("BitShift.Plugin.FirstData.Payment.UseCardLabel")
      };

      return model;
    }

    #endregion
  }
}
