using BitShift.Plugin.Payments.FirstData.Domain;

namespace BitShift.Plugin.Payments.FirstData.Services
{
    public interface IFirstDataStoreSettingService
    {
        FirstDataStoreSetting GetByStore(int storeId, bool fallbackToDefault = true);
        void Insert(FirstDataStoreSetting setting);
        void Update(FirstDataStoreSetting setting);
        void Delete(FirstDataStoreSetting setting);
    }
}
