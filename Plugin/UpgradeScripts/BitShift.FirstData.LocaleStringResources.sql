/******************************************************************************************/
/*  Locale String Resources															      */
/*                                                                                        */
/*	Summary: This will insert any missing resources for the BitShift FirstData plugin     */
/*  This will only insert a record if it doesn't already exist                            */
/******************************************************************************************/

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Notes' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Notes', 'If you''re using this gateway, ensure that your primary store currency is supported by FirstData Global Gateway e4.  Recurring Payments and Card Saving require the TransArmor service on your merchant account.  Read more <a href=\') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.UseSandbox' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.UseSandbox', 'Use Sandbox') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.UseSandbox.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.UseSandbox.Hint', 'Check to enable Sandbox (testing environment).') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.TransactModeValues' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.TransactModeValues', 'Transaction mode') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.TransactModeValues.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.TransactModeValues.Hint', 'Choose transaction mode') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.HMAC' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.HMAC', 'HMAC') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.HMAC.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.HMAC.Hint', 'The HMAC for your terminal') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.GatewayID' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.GatewayID', 'Gateway ID') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.GatewayID.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.GatewayID.Hint', 'Specify gateway identifier.') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.Password' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.Password', 'Password') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.Password.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.Password.Hint', 'Specify terminal password.') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.KeyID' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.KeyID', 'Key ID') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.KeyID.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.KeyID.Hint', 'Specify key identifier.') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AdditionalFee' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AdditionalFee', 'Additional fee') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AdditionalFee.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AdditionalFee.Hint', 'Enter additional fee to charge your customers.') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AdditionalFeePercentage' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AdditionalFeePercentage', 'Additinal fee. Use percentage') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AdditionalFeePercentage.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AdditionalFeePercentage.Hint', 'Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.ExpiryDateError' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.ExpiryDateError', 'Card expired') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.TechnicalError' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.TechnicalError', 'There has been an unexpected error while processing your payment.') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Payment.SaveCard' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Payment.SaveCard', 'Save Card') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Payment.SaveCard.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Payment.SaveCard.Hint', 'Save card for future use.  Your card number is tokenized on First Data''s servers and not stored on our site.') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.EnableRecurringPayments' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.EnableRecurringPayments', 'Enable Recurring Payments') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.EnableRecurringPayments.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.EnableRecurringPayments.Hint', 'Allows manual recurring payments by using the TransArmor Token') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.EnableCardSaving' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.EnableCardSaving', 'Enable Card Saving') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.EnableCardSaving.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.EnableCardSaving.Hint', 'Allows customers to choose to save a card when they use it.  The TransArmor Token is saved instead of the CC number') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Payment.UseCardLabel' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Payment.UseCardLabel', 'Use') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Payment.CardDescription' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Payment.CardDescription', '{0} ending in {1}') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Payment.ExpirationDescription' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Payment.ExpirationDescription', 'Expires {0}/{1}') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Payment.ExpiredLabel' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Payment.ExpiredLabel', 'Expired') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Payment.SavedCardsLabel' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Payment.SavedCardsLabel', 'Saved Cards') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Payment.NewCardLabel' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Payment.NewCardLabel', 'Enter new card') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Payment.PurchaseOrder' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Payment.PurchaseOrder', 'Purchase Order') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.EnablePurchaseOrderNumber' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.EnablePurchaseOrderNumber', 'Enable Purchase Order Number') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.EnablePurchaseOrderNumber.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.EnablePurchaseOrderNumber.Hint', 'Will optionally capture a purchase order number and append it to the Authorization Transaction ID in the Order Details') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.SandboxURL' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.SandboxURL', 'Sandbox URL') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.SandboxURL.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.SandboxURL.Hint', 'Where to send sandbox transactions') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.ProductionURL' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.ProductionURL', 'Production URL') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.ProductionURL.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.ProductionURL.Hint', 'Where to send real transactions') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.StoreID' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.StoreID', 'Store') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.StoreID.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.StoreID.Hint', 'The store that these settings apply to.  The Default Store settings will be used for any store that doesn''t have settings.') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Configure' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Configure', 'Global Settings') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Stores' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Stores', 'Store Specific Settings') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'Bitshift.Plugin.FirstData.Storenotes' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Storenotes', 'Set your store specific settings here.  A store without it''s own entry will use the Default Store Settings') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Stores.Revert' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Stores.Revert', 'Revert to Default Settings') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.StoreSettingsSaved' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.StoreSettingsSaved', 'Store settings saved successfully') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.SavedCards' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.SavedCards', 'Saved Cards') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.SavedCards.NoCards' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.SavedCards.NoCards', 'You don''t have any cards saved.  Complete an order and check the \') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.SavedCards.Fields.Type' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.SavedCards.Fields.Type', 'Type') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.SavedCards.Fields.Name' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.SavedCards.Fields.Name', 'Cardholder Name') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.SavedCards.Fields.Last4' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.SavedCards.Fields.Last4', 'Number') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.SavedCards.Fields.Expires' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.SavedCards.Fields.Expires', 'Expires') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'bitshift.plugin.firstdata.savedcards.description' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.plugin.firstdata.savedcards.description', 'Review your saved cards here.  You can save a new card during checkout.') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.PaymentPageID' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.PaymentPageID', 'Payment Page ID') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.PaymentPageID.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.PaymentPageID.Hint', 'The payment page to use during checkout') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.TransactionKey' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.TransactionKey', 'Transaction Key') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.TransactionKey.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.TransactionKey.Hint', 'The payment page''s transaction key') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.ResponseKey' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.ResponseKey', 'Response Key') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.ResponseKey.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.ResponseKey.Hint', 'The response key First Data will send back to the plugin') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.HostedPaymentPage.AuthorizationEmpty' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.HostedPaymentPage.AuthorizationEmpty', 'Error processing payment, Authorization is empty') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AllowVisa' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AllowVisa', 'Allow Visa') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AllowVisa.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AllowVisa.Hint', 'Check to allow Visa payments') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AllowMastercard' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AllowMastercard', 'Allow Mastercard') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AllowMastercard.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AllowMastercard.Hint', 'Check to allow Mastercard payments') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AllowAmex' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AllowAmex', 'Allow Amex') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AllowAmex.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AllowAmex.Hint', 'Check to allow American Express payments') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AllowDiscover' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AllowDiscover', 'Allow Discover') END

IF NOT EXISTS (SELECT * FROM LocaleStringResource WHERE ResourceName = 'BitShift.Plugin.FirstData.Fields.AllowDiscover.Hint' and LanguageId = 1) BEGIN  
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'BitShift.Plugin.FirstData.Fields.AllowDiscover.Hint', 'Check to allow Discover payments') END

IF NOT EXISTS (select * from LocaleStringResource where ResourceName = 'bitshift.plugin.firstdata.paymentmethoddescription' and LanguageId = 1) BEGIN
INSERT INTO LocaleStringResource (LanguageId, ResourceName, ResourceValue) VALUES (1, 'bitshift.plugin.firstdata.paymentmethoddescription', 'Credit card') END