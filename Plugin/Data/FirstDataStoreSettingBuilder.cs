using BitShift.Plugin.Payments.FirstData.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace BitShift.Plugin.Payments.FirstData.Data
{
    public class FirstDataStoreSettingBuilder : NopEntityBuilder<FirstDataStoreSetting>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
          table.WithColumn(nameof(FirstDataStoreSetting.StoreId)).AsInt32().PrimaryKey()
               .WithColumn(nameof(FirstDataStoreSetting.HMAC)).AsString(128).Nullable()
               .WithColumn(nameof(FirstDataStoreSetting.GatewayID)).AsString(128).Nullable()
               .WithColumn(nameof(FirstDataStoreSetting.Password)).AsString(128).Nullable()
               .WithColumn(nameof(FirstDataStoreSetting.KeyID)).AsString(128).Nullable()
               .WithColumn(nameof(FirstDataStoreSetting.PaymentPageID)).AsString(128).Nullable()
               .WithColumn(nameof(FirstDataStoreSetting.TransactionKey)).AsString(128).Nullable()
               .WithColumn(nameof(FirstDataStoreSetting.ResponseKey)).AsString(128).Nullable();
        }
    }
}
 