using System;
using System.Linq;
using System.Collections.Generic;
using Nop.Core.Data;
using BitShift.Plugin.Payments.FirstData.Domain;

namespace BitShift.Plugin.Payments.FirstData.Services
{
    public class SavedCardService : ISavedCardService
    {
        private IRepository<SavedCard> _savedCardRepository;

        public SavedCardService(IRepository<SavedCard> savedCardRepository)
        {
            _savedCardRepository = savedCardRepository;
        }

        public SavedCard GetById(int id)
        {
            return _savedCardRepository.GetById(id);
        }

        public SavedCard GetByToken(int customerId, string token)
        {
            return _savedCardRepository.Table.Where(c => c.Customer_Id == customerId && c.Token == token).FirstOrDefault();
        }

        public IList<SavedCard> GetByCustomer(int customerId)
        {
            return _savedCardRepository.Table.Where(c => c.Customer_Id == customerId).ToList();
        }

        public void Delete(SavedCard card)
        {
            if (card == null)
                throw new ArgumentNullException("SavedCard");

            _savedCardRepository.Delete(card);
        }

        public void Insert(SavedCard card)
        {
            if (card == null)
                throw new ArgumentNullException("SavedCard");

            _savedCardRepository.Insert(card);
        }

        public void Update(SavedCard card)
        {
            if (card == null)
                throw new ArgumentNullException("SavedCard");

            _savedCardRepository.Update(card);
        }
    }
}
