using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Web.Framework.Infrastructure.Extensions;
using Nop.Data;
using BitShift.Plugin.Payments.FirstData.Data;
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

            //data context
            builder.RegisterPluginDataContext<FirstDataObjectContext>("nop_object_context_bitshift_firstdata");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<SavedCard>>()
                .As<IRepository<SavedCard>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_bitshift_firstdata"))
                .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<FirstDataStoreSetting>>()
                .As<IRepository<FirstDataStoreSetting>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_bitshift_firstdata"))
                .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
