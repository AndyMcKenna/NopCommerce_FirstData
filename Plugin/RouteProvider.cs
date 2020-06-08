using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace BitShift.Plugin.Payments.FirstData
{
  public partial class RouteProvider : IRouteProvider
  {
    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
      routeBuilder.MapControllerRoute("Plugin.Payments.FirstData.PaymentInfo",
           "Plugins/FirstData/PaymentInfo",
           new { controller = "FirstData", action = "PaymentInfo" }
      );

      routeBuilder.MapControllerRoute("Plugin.Payments.FirstData.SavedCards",
          "Plugins/FirstData/SavedCards",
          new { controller = "SavedCard", action = "SavedCards" }
      );

      routeBuilder.MapControllerRoute("Plugin.Payments.FirstData.SavedCardsTable",
          "Plugins/FirstData/SavedCardsTable",
          new { controller = "SavedCard", action = "SavedCardsTable" }
      );

      routeBuilder.MapControllerRoute("Plugin.Payments.FirstData.DeleteSavedCard",
          "Plugins/FirstData/DeleteSavedCard",
          new { controller = "SavedCard", action = "DeleteSavedCard" }
      );

      routeBuilder.MapControllerRoute("Plugin.Payments.FirstData.GetHostedPaymentForm",
          "Plugins/FirstData/GetHostedPaymentForm",
          new { controller = "PublicView", action = "GetHostedPaymentForm" }
      );

      routeBuilder.MapControllerRoute("Plugin.Payments.FirstData.PaymentResponse",
          "Plugins/FirstData/PaymentResponse",
          new { controller = "PublicView", action = "PaymentResponse" }
      );
    }

    public int Priority
    {
      get
      {
        return -1;
      }
    }
  }
}
