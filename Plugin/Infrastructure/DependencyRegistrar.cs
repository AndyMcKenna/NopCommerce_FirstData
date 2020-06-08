using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Web.Framework.Infrastructure.Extensions;
using Nop.Data;
using BitShift.Plugin.Payments.FirstData.Domain;
using BitShift.Plugin.Payments.FirstData.Services;
using BitShift.Plugin.Payments.FirstData.Factories;

namespace BitShift.Plugin.Payments.FirstData.Infrastructure
{
  public class DependencyRegistrar : IDependencyRegistrar
  {
    public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
    {
      builder.RegisterType<SavedCardService>().As<ISavedCardService>().InstancePerLifetimeScope();
      builder.RegisterType<FirstDataStoreSettingService>().As<IFirstDataStoreSettingService>().InstancePerLifetimeScope();
      builder.RegisterType<PaymentModelFactory>().As<IPaymentModelFactory>().InstancePerLifetimeScope();
      builder.RegisterType<SavedCardModelFactory>().As<ISavedCardModelFactory>().InstancePerLifetimeScope();
      builder.RegisterType<FDWebRequest>().As<IWebRequest>().InstancePerLifetimeScope();
    }

    public int Order
    {
      get { return 1; }
    }
  }
}
