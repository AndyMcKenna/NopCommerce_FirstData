using FluentMigrator;
using Nop.Data.Migrations;
using BitShift.Plugin.Payments.FirstData.Domain;

namespace BitShift.Plugin.Payments.FirstData.Data.Migrations
{
  [SkipMigrationOnUpdate]
  [NopMigration("2020-06-07 00:00:00", "BitShift_FirstData base schema")]
  public class BaseMigration : AutoReversingMigration
  {
    protected IMigrationManager _migrationManager;

    public BaseMigration(IMigrationManager migrationManager)
    {
      _migrationManager = migrationManager;
    }

    public override void Up()
    {
      _migrationManager.BuildTable<FirstDataStoreSetting>(Create);
      _migrationManager.BuildTable<SavedCard>(Create);

    }
  }
}