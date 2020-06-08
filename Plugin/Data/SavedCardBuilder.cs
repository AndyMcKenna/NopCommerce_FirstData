using BitShift.Plugin.Payments.FirstData.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace BitShift.Plugin.Payments.FirstData.Data
{
  public class SavedCardBuilder : NopEntityBuilder<SavedCard>
  {
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
      table.WithColumn(nameof(SavedCard.Token)).AsString(64)
           .WithColumn(nameof(SavedCard.CardholderName)).AsString(256)
           .WithColumn(nameof(SavedCard.CardType)).AsString(64);
    }
  }
}