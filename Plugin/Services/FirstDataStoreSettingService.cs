using System;
using System.Linq;
using Nop.Core.Data;
using BitShift.Plugin.Payments.FirstData.Domain;

namespace BitShift.Plugin.Payments.FirstData.Services
{
    public class FirstDataStoreSettingService : IFirstDataStoreSettingService
    {
        readonly IRepository<FirstDataStoreSetting> _settingRepository;

        public FirstDataStoreSettingService(IRepository<FirstDataStoreSetting> settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public FirstDataStoreSetting GetByStore(int storeId, bool fallbackToDefault = true)
        {
            var setting = _settingRepository.Table.Where(s => s.StoreId == storeId).FirstOrDefault();
            if(setting == null && fallbackToDefault)
            {
                setting = _settingRepository.Table.Where(s => s.StoreId == 0).FirstOrDefault();
            }

            return setting;
        }

        public void Insert(FirstDataStoreSetting setting)
        {
            _settingRepository.Insert(setting);
        }

        public void Update(FirstDataStoreSetting setting)
        {
            _settingRepository.Update(setting);
        }

        public void Delete(FirstDataStoreSetting setting)
        {
            _settingRepository.Delete(setting);
        }
    }
}
