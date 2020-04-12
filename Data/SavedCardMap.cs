using BitShift.Plugin.Payments.FirstData.Domain;
using Nop.Data.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BitShift.Plugin.Payments.FirstData.Data
{
    public partial class SavedCardMap : NopEntityTypeConfiguration<SavedCard>
    {
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<SavedCard> builder)
        {
            builder.ToTable("BitShift_FirstData_SavedCard");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Token).HasMaxLength(64);
            builder.Property(x => x.CardholderName).HasMaxLength(256);
            builder.Property(x => x.CardType).HasMaxLength(64);
        }
    }
}