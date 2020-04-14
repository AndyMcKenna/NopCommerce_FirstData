using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Messages;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Services.Security;

namespace BitShift.Plugin.Payments.FirstData.Services
{
    public class TokenEventConsumer : IConsumer<EntityTokensAddedEvent<Order, Token>>
    {
        private IEncryptionService _encryptionService;

        public TokenEventConsumer(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public void HandleEvent(EntityTokensAddedEvent<Order, Token> eventMessage)
        {
            eventMessage.Tokens.Add(new Token("Bitshift.Order.PaymentMethodSystemName", (eventMessage.Entity.PaymentMethodSystemName == "BitShift.Payments.FirstData" ? "Credit Card - (First Data)" : eventMessage.Entity.PaymentMethodSystemName)));
            eventMessage.Tokens.Add(new Token("Bitshift.Order.MaskedCreditCardNumber", _encryptionService.DecryptText(eventMessage.Entity.MaskedCreditCardNumber)));
            eventMessage.Tokens.Add(new Token("Bitshift.Order.AuthorizationTransactionId", eventMessage.Entity.AuthorizationTransactionId));
            eventMessage.Tokens.Add(new Token("Bitshift.Order.AuthorizationTransactionResult", eventMessage.Entity.AuthorizationTransactionResult));
            //eventMessage.Tokens.Add(new Token("Bitshift.Order.CaptureTransactionId", eventMessage.Entity.CaptureTransactionId));
            //eventMessage.Tokens.Add(new Token("Bitshift.Order.CaptureTransactionResult", eventMessage.Entity.CaptureTransactionResult));
        }
    }
}
