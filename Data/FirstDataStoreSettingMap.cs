using BitShift.Plugin.Payments.FirstData.Domain;
using Nop.Data.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BitShift.Plugin.Payments.FirstData.Data
{
    public partial class FirstDataStoreSettingMap : NopEntityTypeConfiguration<FirstDataStoreSetting>
    {
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<FirstDataStoreSetting> builder)
        {
            builder.ToTable("BitShift_FirstData_StoreSetting");
            builder.HasKey(x => x.StoreId);
            builder.Ignore(x => x.Id);
            builder.Property(x => x.HMAC).HasMaxLength(128);
            builder.Property(x => x.GatewayID).HasMaxLength(128);
            builder.Property(x => x.Password).HasMaxLength(128);
            builder.Property(x => x.KeyID).HasMaxLength(128);
            builder.Property(x => x.PaymentPageID).HasMaxLength(128);
            builder.Property(x => x.TransactionKey).HasMaxLength(128);
            builder.Property(x => x.ResponseKey).HasMaxLength(128);
        }
    }
}