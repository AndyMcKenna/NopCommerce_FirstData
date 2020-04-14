using System.Collections.Generic;
using BitShift.Plugin.Payments.FirstData.Domain;

namespace BitShift.Plugin.Payments.FirstData.Services
{
    public interface ISavedCardService
    {
        SavedCard GetById(int id);
        SavedCard GetByToken(int customerId, string token);
        IList<SavedCard> GetByCustomer(int customerId);
        void Delete(SavedCard card);
        void Insert(SavedCard card);
        void Update(SavedCard card);
    }
}
