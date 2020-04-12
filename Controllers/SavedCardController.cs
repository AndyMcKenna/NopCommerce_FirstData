using BitShift.Plugin.Payments.FirstData.Factories;
using BitShift.Plugin.Payments.FirstData.Models;
using BitShift.Plugin.Payments.FirstData.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using System.Collections.Generic;

namespace BitShift.Plugin.Payments.FirstData.Controllers
{
    public class SavedCardController : BaseController
    {
        private readonly ISavedCardService _savedCardService;
        private readonly IWorkContext _workContext;
        private readonly ISavedCardModelFactory _savedCardModelFactory;

        public SavedCardController(ISavedCardService savedCardService,
            IWorkContext workContext, ISavedCardModelFactory savedCardModelFactory)
        {
            _savedCardService = savedCardService;
            _workContext = workContext;
            _savedCardModelFactory = savedCardModelFactory;
        }

        public ActionResult SavedCards()
        {
            return View("~/Plugins/BitShift.Payments.FirstData/Views/Payment/SavedCards.cshtml");
        }

        public ActionResult SavedCardsTable()
        {
            IList<SavedCardModel> model = new List<SavedCardModel>();
            var savedCards = _savedCardService.GetByCustomer(_workContext.CurrentCustomer.Id);
            foreach (var savedCard in savedCards)
            {
                model.Add(_savedCardModelFactory.PrepareSavedCardModel(savedCard));
            }

            return PartialView("~/Plugins/BitShift.Payments.FirstData/Views/Payment/SavedCardsTable.cshtml", model);
        }

        [HttpPost]
        public void DeleteSavedCard(int id)
        {
            var card = _savedCardService.GetById(id);
            if (card != null && card.Customer_Id == _workContext.CurrentCustomer.Id)
            {
                _savedCardService.Delete(card);
            }
        }
    }
}