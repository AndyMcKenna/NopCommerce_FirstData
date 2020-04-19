using System.IO;

namespace BitShift.Plugin.Payments.FirstData.Tests.MockData
{
  public static class FirstDataStreamResponses
  {
    public static Stream AuthorizationSuccess => GenerateStreamFromString(FirstDataXmlResponses.AuthorizationSuccess);
    public static Stream AuthorizationCaptureSuccess => GenerateStreamFromString(FirstDataXmlResponses.AuthorizationCaptureSuccess);
    public static Stream CaptureSuccess => GenerateStreamFromString(FirstDataXmlResponses.CaptureSuccess);
    public static Stream RefundSuccess => GenerateStreamFromString(FirstDataXmlResponses.RefundSuccess);
    public static Stream RefundFailure => GenerateStreamFromString(FirstDataXmlResponses.RefundFailure);
    public static Stream VoidSuccess => GenerateStreamFromString(FirstDataXmlResponses.VoidSuccess);

    public static Stream GenerateStreamFromString(string s)
    {
      var stream = new MemoryStream();
      var writer = new StreamWriter(stream);
      writer.Write(s);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }
  }

  public static class FirstDataXmlResponses
  {
    public static string AuthorizationSuccess =>
      "<?xml version =\"1.0\" encoding=\"UTF-8\"?><TransactionResult>" +
        "<ExactID>LD6411-67</ExactID>" +
        "<Password />" +
        "<Transaction_Type>01</Transaction_Type>" +
        "<DollarAmount>27.0</DollarAmount>" +
        "<SurchargeAmount/>" +
        "<Card_Number/>" +
        "<Transaction_Tag>4492062413</Transaction_Tag>" +
        "<Track1/>" +
        "<Track2/>" +
        "<PAN/>" +
        "<Authorization_Num>ET156303</Authorization_Num>" +
        "<Expiry_Date>0121</Expiry_Date>" +
        "<CardHoldersName>test</CardHoldersName>" +
        "<VerificationStr1>21 West 52nd Street|10021|New York|USA</VerificationStr1>" +
        "<VerificationStr2>123</VerificationStr2>" +
        "<CVD_Presence_Ind>1</CVD_Presence_Ind>" +
        "<ZipCode/>" +
        "<Tax1Amount/>" +
        "<Tax1Number/>" +
        "<Tax2Amount/>" +
        "<Tax2Number/>" +
        "<Secure_AuthRequired/>" +
        "<Secure_AuthResult/>" +
        "<Ecommerce_Flag/>" +
        "<XID/>" +
        "<CAVV/>" +
        "<CAVV_Algorithm/>" +
        "<Reference_No>30cef8ec-915c-41f9-a</Reference_No>" +
        "<Customer_Ref>9cdc42a1-763b-4d39-9</Customer_Ref>" +
        "<Reference_3/>" +
        "<Language/>" +
        "<Client_IP>127.0.0.1</Client_IP>" +
        "<Client_Email>admin @yourStore.com</Client_Email>" +
        "<User_Name/>" +
        "<Transaction_Error>false</Transaction_Error>" +
        "<Transaction_Approved>true</Transaction_Approved>" +
        "<EXact_Resp_Code>00</EXact_Resp_Code>" +
        "<EXact_Message>Transaction Normal</EXact_Message>" +
        "<Bank_Resp_Code>100</Bank_Resp_Code>" +
        "<Bank_Message>Approved</Bank_Message>" +
        "<Bank_Resp_Code_2/>" +
        "<SequenceNo>000003</SequenceNo>" +
        "<AVS>2</AVS>" +
        "<CVV2>M</CVV2>" +
        "<Retrieval_Ref_No>200418</Retrieval_Ref_No>" +
        "<CAVV_Response/>" +
        "<Currency>USD</Currency>" +
        "<AmountRequested/>" +
        "<PartialRedemption>false</PartialRedemption>" +
        "<MerchantName>Fake Merchant</MerchantName>" +
        "<MerchantAddress>54321 Main St</MerchantAddress>" +
        "<MerchantCity>Anytown</MerchantCity>" +
        "<MerchantProvince>MI</MerchantProvince>" +
        "<MerchantCountry>United States</MerchantCountry>" +
        "<MerchantPostal>55555</MerchantPostal>" +
        "<MerchantURL>http://www.github.com</MerchantURL>" +
        "<TransarmorToken>9050655610261111</TransarmorToken>" +
        "<CardType>Visa</CardType>" +
        "<CurrentBalance/>" +
        "<PreviousBalance/>" +
        "<EAN/>" +
        "<CardCost/>" +
        "<VirtualCard>false</VirtualCard>" +
        "<CTR>========== TRANSACTION RECORD ==========Fake Merchant 54321 Main StAnytown, MI 55555United States54321 Main StTYPE: PurchaseACCT: Visa                   $ 27.00 USDCARDHOLDER NAME : testCARD NUMBER     : ############1111DATE/TIME       : 18 Apr 20 14:42:41REFERENCE #     : 03 000003 TAUTHOR. #       : ET156303TRANS. REF.     : 30cef8ec-915c-41f9-a Approved - Thank You 100Please retain this copy for your records.Cardholder will pay above amount tocard issuer pursuant to cardholderagreement. ========================================</CTR>" +
        "<FraudSuspected/></TransactionResult>";

    public static string AuthorizationCaptureSuccess =>
      "<?xml version =\"1.0\" encoding=\"UTF-8\"?><TransactionResult>" +
        "<ExactID>LD6411-67</ExactID>" +
        "<Password />" +
        "<Transaction_Type>00</Transaction_Type>" +
        "<DollarAmount>27.0</DollarAmount>" +
        "<SurchargeAmount/>" +
        "<Card_Number/>" +
        "<Transaction_Tag>4492062413</Transaction_Tag>" +
        "<Track1/>" +
        "<Track2/>" +
        "<PAN/>" +
        "<Authorization_Num>ET156303</Authorization_Num>" +
        "<Expiry_Date>0121</Expiry_Date>" +
        "<CardHoldersName>test</CardHoldersName>" +
        "<VerificationStr1>21 West 52nd Street|10021|New York|USA</VerificationStr1>" +
        "<VerificationStr2>123</VerificationStr2>" +
        "<CVD_Presence_Ind>1</CVD_Presence_Ind>" +
        "<ZipCode/>" +
        "<Tax1Amount/>" +
        "<Tax1Number/>" +
        "<Tax2Amount/>" +
        "<Tax2Number/>" +
        "<Secure_AuthRequired/>" +
        "<Secure_AuthResult/>" +
        "<Ecommerce_Flag/>" +
        "<XID/>" +
        "<CAVV/>" +
        "<CAVV_Algorithm/>" +
        "<Reference_No>30cef8ec-915c-41f9-a</Reference_No>" +
        "<Customer_Ref>9cdc42a1-763b-4d39-9</Customer_Ref>" +
        "<Reference_3/>" +
        "<Language/>" +
        "<Client_IP>127.0.0.1</Client_IP>" +
        "<Client_Email>admin @yourStore.com</Client_Email>" +
        "<User_Name/>" +
        "<Transaction_Error>false</Transaction_Error>" +
        "<Transaction_Approved>true</Transaction_Approved>" +
        "<EXact_Resp_Code>00</EXact_Resp_Code>" +
        "<EXact_Message>Transaction Normal</EXact_Message>" +
        "<Bank_Resp_Code>100</Bank_Resp_Code>" +
        "<Bank_Message>Approved</Bank_Message>" +
        "<Bank_Resp_Code_2/>" +
        "<SequenceNo>000003</SequenceNo>" +
        "<AVS>2</AVS>" +
        "<CVV2>M</CVV2>" +
        "<Retrieval_Ref_No>200418</Retrieval_Ref_No>" +
        "<CAVV_Response/>" +
        "<Currency>USD</Currency>" +
        "<AmountRequested/>" +
        "<PartialRedemption>false</PartialRedemption>" +
        "<MerchantName>Fake Merchant</MerchantName>" +
        "<MerchantAddress>54321 Main St</MerchantAddress>" +
        "<MerchantCity>Anytown</MerchantCity>" +
        "<MerchantProvince>MI</MerchantProvince>" +
        "<MerchantCountry>United States</MerchantCountry>" +
        "<MerchantPostal>55555</MerchantPostal>" +
        "<MerchantURL>http://www.github.com</MerchantURL>" +
        "<TransarmorToken>9050655610261111</TransarmorToken>" +
        "<CardType>Visa</CardType>" +
        "<CurrentBalance/>" +
        "<PreviousBalance/>" +
        "<EAN/>" +
        "<CardCost/>" +
        "<VirtualCard>false</VirtualCard>" +
        "<CTR>========== TRANSACTION RECORD ==========Fake Merchant 54321 Main StAnytown, MI 55555United States54321 Main StTYPE: PurchaseACCT: Visa                   $ 27.00 USDCARDHOLDER NAME : testCARD NUMBER     : ############1111DATE/TIME       : 18 Apr 20 14:42:41REFERENCE #     : 03 000003 TAUTHOR. #       : ET156303TRANS. REF.     : 30cef8ec-915c-41f9-a Approved - Thank You 100Please retain this copy for your records.Cardholder will pay above amount tocard issuer pursuant to cardholderagreement. ========================================</CTR>" +
        "<FraudSuspected/></TransactionResult>";

    public static string CaptureSuccess =>
      "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
      "<TransactionResult>" +
      "<ExactID>LD6411-67</ExactID>" +
      "<Password/>" +
      "<Transaction_Type>32</Transaction_Type>" +
      "<DollarAmount>27.0</DollarAmount>" +
      "<SurchargeAmount/>" +
      "<Card_Number/>" +
      "<Transaction_Tag>4492207508</Transaction_Tag>" +
      "<Track1/>" +
      "<Track2/>" +
      "<PAN/>" +
      "<Authorization_Num>ET189500</Authorization_Num>" +
      "<Expiry_Date>0121</Expiry_Date>" +
      "<CardHoldersName>test</CardHoldersName>" +
      "<VerificationStr1/>" +
      "<VerificationStr2/>" +
      "<CVD_Presence_Ind>0</CVD_Presence_Ind>" +
      "<ZipCode/>" +
      "<Tax1Amount/>" +
      "<Tax1Number/>" +
      "<Tax2Amount/>" +
      "<Tax2Number/>" +
      "<Secure_AuthRequired/>" +
      "<Secure_AuthResult/>" +
      "<Ecommerce_Flag/>" +
      "<XID/>" +
      "<CAVV/>" +
      "<CAVV_Algorithm/>" +
      "<Reference_No>9</Reference_No>" +
      "<Customer_Ref/>" +
      "<Reference_3/>" +
      "<Language/>" +
      "<Client_IP/>" +
      "<Client_Email/>" +
      "<User_Name/>" +
      "<Transaction_Error>false</Transaction_Error>" +
      "<Transaction_Approved>true</Transaction_Approved>" +
      "<EXact_Resp_Code>00</EXact_Resp_Code>" +
      "<EXact_Message>Transaction Normal</EXact_Message>" +
      "<Bank_Resp_Code>100</Bank_Resp_Code>" +
      "<Bank_Message>Approved</Bank_Message>" +
      "<Bank_Resp_Code_2/>" +
      "<SequenceNo>000006</SequenceNo>" +
      "<AVS/>" +
      "<CVV2/>" +
      "<Retrieval_Ref_No>200418</Retrieval_Ref_No>" +
      "<CAVV_Response/>" +
      "<Currency>USD</Currency>" +
      "<AmountRequested/>" +
      "<PartialRedemption>false</PartialRedemption>" +
      "<MerchantName>Fake Merchant</MerchantName>" +
      "<MerchantAddress>123 Any St.</MerchantAddress>" +
      "<MerchantCity>Anytown</MerchantCity>" +
      "<MerchantProvince>Michigan</MerchantProvince>" +
      "<MerchantCountry>United States</MerchantCountry>" +
      "<MerchantPostal>55555</MerchantPostal>" +
      "<MerchantURL>123 Any St.</MerchantURL>" +
      "<TransarmorToken>8066864841181111</TransarmorToken>" +
      "<CardType>Visa</CardType>" +
      "<CurrentBalance/>" +
      "<PreviousBalance/>" +
      "<EAN/>" +
      "<CardCost/>" +
      "<VirtualCard>false</VirtualCard>" +
      "<CTR>========== TRANSACTION RECORD ==========Fake Merchant 123 Any St.+Anytown, MI 55555+United States+123 Any St.++TYPE: Completion++ACCT: Visa                   $ 27.00 USDCARDHOLDER NAME : testCARD NUMBER     : ############1111DATE/TIME       : 18 Apr 20 22:17:26REFERENCE #     : 03 000006 TAUTHOR. #       : ET189500TRANS. REF.     : 9    Approved - Thank You 100Please retain this copy for your records.Cardholder will pay above amount tocard issuer pursuant to cardholderagreement.========================================</CTR>  <FraudSuspected/></TransactionResult>"
;

    public static string RefundSuccess =>
      "<?xml version =\"1.0\" encoding=\"UTF-8\"?>" +
      "<TransactionResult>" +
      "<ExactID>LD6411-67</ExactID>" +
      "<Password/>" +
      "<Transaction_Type>34</Transaction_Type>" +
      "<DollarAmount>27.0</DollarAmount>" +
      "<SurchargeAmount/>" +
      "<Card_Number/>" +
      "<Transaction_Tag>4492206773</Transaction_Tag>" +
      "<Track1/>" +
      "<Track2/>" +
      "<PAN/>" +
      "<Authorization_Num>RETURN</Authorization_Num>" +
      "<Expiry_Date>0121</Expiry_Date>" +
      "<CardHoldersName>test</CardHoldersName>" +
      "<VerificationStr1/>" +
      "<VerificationStr2/>" +
      "<CVD_Presence_Ind>0</CVD_Presence_Ind>" +
      "<ZipCode/>" +
      "<Tax1Amount/>" +
      "<Tax1Number/>" +
      "<Tax2Amount/>" +
      "<Tax2Number/>" +
      "<Secure_AuthRequired/>" +
      "<Secure_AuthResult/>" +
      "<Ecommerce_Flag/>" +
      "<XID/>" +
      "<CAVV/>" +
      "<CAVV_Algorithm/>" +
      "<Reference_No/>" +
      "<Customer_Ref/>" +
      "<Reference_3/>" +
      "<Language/>" +
      "<Client_IP/>" +
      "<Client_Email/>" +
      "<User_Name/>" +
      "<Transaction_Error>false</Transaction_Error>" +
      "<Transaction_Approved>true</Transaction_Approved>" +
      "<EXact_Resp_Code>00</EXact_Resp_Code>" +
      "<EXact_Message>Transaction Normal</EXact_Message>" +
      "<Bank_Resp_Code>100</Bank_Resp_Code>" +
      "<Bank_Message>Approved</Bank_Message>" +
      "<Bank_Resp_Code_2/>" +
      "<SequenceNo>000004</SequenceNo>" +
      "<AVS/>" +
      "<CVV2/>" +
      "<Retrieval_Ref_No>200418</Retrieval_Ref_No>" +
      "<CAVV_Response/>" +
      "<Currency>USD</Currency>" +
      "<AmountRequested/>" +
      "<PartialRedemption>false</PartialRedemption>" +
      "<MerchantName>Fake Merchant</MerchantName>" +
      "<MerchantAddress>123 Any St.</MerchantAddress>" +
      "<MerchantCity>Anytown</MerchantCity>" +
      "<MerchantProvince>Michigan</MerchantProvince>" +
      "<MerchantCountry>United States</MerchantCountry>" +
      "<MerchantPostal>55555</MerchantPostal>" +
      "<MerchantURL>123 Any St.</MerchantURL>" +
      "<TransarmorToken>7291718023521111</TransarmorToken>" +
      "<CardType>Visa</CardType>" +
      "<CurrentBalance/>" +
      "<PreviousBalance/>" +
      "<EAN/>" +
      "<CardCost/>" +
      "<VirtualCard>false</VirtualCard>" +
      "<CTR>========== TRANSACTION RECORD ==========Fake Merchant123 Any St.Anytown, MI 55555United States123 Any St.TYPE: RefundACCT: Visa                   $ 27.00 USDCARDHOLDER NAME : testCARD NUMBER     : ############1111DATE/TIME       : 18 Apr 20 22:12:02REFERENCE #     : 03 000004 TAUTHOR. #       : RETURNTRANS. REF.     :     Approved - Thank You 100Please retain this copy for your records.========================================</CTR>" +
      "<FraudSuspected/>" +
      "</TransactionResult>";

    public static string RefundFailure =>
      "<?xml version =\"1.0\" encoding=\"UTF-8\"?>" +
      "<TransactionResult>" +
      "<ExactID>LD6411-67</ExactID>" +
      "<Password/>" +
      "<Transaction_Type>34</Transaction_Type>" +
      "<DollarAmount>27.0</DollarAmount>" +
      "<SurchargeAmount/>" +
      "<Card_Number/>" +
      "<Transaction_Tag>4492206773</Transaction_Tag>" +
      "<Track1/>" +
      "<Track2/>" +
      "<PAN/>" +
      "<Authorization_Num>RETURN</Authorization_Num>" +
      "<Expiry_Date>0121</Expiry_Date>" +
      "<CardHoldersName>test</CardHoldersName>" +
      "<VerificationStr1/>" +
      "<VerificationStr2/>" +
      "<CVD_Presence_Ind>0</CVD_Presence_Ind>" +
      "<ZipCode/>" +
      "<Tax1Amount/>" +
      "<Tax1Number/>" +
      "<Tax2Amount/>" +
      "<Tax2Number/>" +
      "<Secure_AuthRequired/>" +
      "<Secure_AuthResult/>" +
      "<Ecommerce_Flag/>" +
      "<XID/>" +
      "<CAVV/>" +
      "<CAVV_Algorithm/>" +
      "<Reference_No/>" +
      "<Customer_Ref/>" +
      "<Reference_3/>" +
      "<Language/>" +
      "<Client_IP/>" +
      "<Client_Email/>" +
      "<User_Name/>" +
      "<Transaction_Error>true</Transaction_Error>" +
      "<Transaction_Approved>false</Transaction_Approved>" +
      "<EXact_Resp_Code>01</EXact_Resp_Code>" +
      "<EXact_Message>Transaction Failed</EXact_Message>" +
      "<Bank_Resp_Code>100</Bank_Resp_Code>" +
      "<Bank_Message>Approved</Bank_Message>" +
      "<Bank_Resp_Code_2/>" +
      "<SequenceNo>000004</SequenceNo>" +
      "<AVS/>" +
      "<CVV2/>" +
      "<Retrieval_Ref_No>200418</Retrieval_Ref_No>" +
      "<CAVV_Response/>" +
      "<Currency>USD</Currency>" +
      "<AmountRequested/>" +
      "<PartialRedemption>false</PartialRedemption>" +
      "<MerchantName>Fake Merchant</MerchantName>" +
      "<MerchantAddress>123 Any St.</MerchantAddress>" +
      "<MerchantCity>Anytown</MerchantCity>" +
      "<MerchantProvince>Michigan</MerchantProvince>" +
      "<MerchantCountry>United States</MerchantCountry>" +
      "<MerchantPostal>55555</MerchantPostal>" +
      "<MerchantURL>123 Any St.</MerchantURL>" +
      "<TransarmorToken>7291718023521111</TransarmorToken>" +
      "<CardType>Visa</CardType>" +
      "<CurrentBalance/>" +
      "<PreviousBalance/>" +
      "<EAN/>" +
      "<CardCost/>" +
      "<VirtualCard>false</VirtualCard>" +
      "<CTR>========== TRANSACTION RECORD ==========Fake Merchant123 Any St.Anytown, MI 55555United States123 Any St.TYPE: RefundACCT: Visa                   $ 27.00 USDCARDHOLDER NAME : testCARD NUMBER     : ############1111DATE/TIME       : 18 Apr 20 22:12:02REFERENCE #     : 03 000004 TAUTHOR. #       : RETURNTRANS. REF.     :     Approved - Thank You 100Please retain this copy for your records.========================================</CTR>" +
      "<FraudSuspected/>" +
      "</TransactionResult>";

    public static string VoidSuccess =>
      "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
      "<TransactionResult>" +
      "<ExactID>LD6411-67</ExactID>" +
      "<Password/>" +
      "<Transaction_Type>33</Transaction_Type>" +
      "<DollarAmount>27.0</DollarAmount>" +
      "<SurchargeAmount/>" +
      "<Card_Number/>" +
      "<Transaction_Tag>4492207352</Transaction_Tag>" +
      "<Track1/>" +
      "<Track2/>" +
      "<PAN/>" +
      "<Authorization_Num>ET193148</Authorization_Num>" +
      "<Expiry_Date>0121</Expiry_Date>" +
      "<CardHoldersName>test</CardHoldersName>" +
      "<VerificationStr1/>" +
      "<VerificationStr2/>" +
      "<CVD_Presence_Ind>0</CVD_Presence_Ind>" +
      "<ZipCode/>" +
      "<Tax1Amount/>" +
      "<Tax1Number/>" +
      "<Tax2Amount/>" +
      "<Tax2Number/>" +
      "<Secure_AuthRequired/>" +
      "<Secure_AuthResult/>" +
      "<Ecommerce_Flag/>" +
      "<XID/>" +
      "<CAVV/>" +
      "<CAVV_Algorithm/>" +
      "<Reference_No/>" +
      "<Customer_Ref/>" +
      "<Reference_3/>" +
      "<Language/>" +
      "<Client_IP/>" +
      "<Client_Email/>" +
      "<User_Name/>" +
      "<Transaction_Error>false</Transaction_Error>" +
      "<Transaction_Approved>true</Transaction_Approved>" +
      "<EXact_Resp_Code>00</EXact_Resp_Code>" +
      "<EXact_Message>Transaction Normal</EXact_Message>" +
      "<Bank_Resp_Code>100</Bank_Resp_Code>" +
      "<Bank_Message>Approved</Bank_Message>" +
      "<Bank_Resp_Code_2/>" +
      "<SequenceNo>000005</SequenceNo>" +
      "<AVS/>" +
      "<CVV2/>" +
      "<Retrieval_Ref_No>200418</Retrieval_Ref_No>" +
      "<CAVV_Response/>" +
      "<Currency>USD</Currency>" +
      "<AmountRequested/>" +
      "<PartialRedemption>false</PartialRedemption>" +
      "<MerchantName>Fake Merchant</MerchantName>" +
      "<MerchantAddress>123 Any St.</MerchantAddress>" +
      "<MerchantCity>Anytown</MerchantCity>" +
      "<MerchantProvince>Michigan</MerchantProvince>" +
      "<MerchantCountry>United States</MerchantCountry>" +
      "<MerchantPostal>55555</MerchantPostal>" +
      "<MerchantURL>123 Any St.</MerchantURL>" +
      "<TransarmorToken>8137113416051111</TransarmorToken>" +
      "<CardType>Visa</CardType>" +
      "<CurrentBalance/>" +
      "<PreviousBalance/>" +
      "<EAN/>" +
      "<CardCost/>" +
      "<VirtualCard>false</VirtualCard>" +
      "<CTR>========== TRANSACTION RECORD ==========Fake Merchant123 Any St.Anytown, MI 55555United States123 Any St.TYPE: VoidACCT: Visa                   $ 27.00 USDCARDHOLDER NAME : testCARD NUMBER     : ############1111DATE/TIME       : 18 Apr 20 22:15:43REFERENCE #     : 03 000005 TAUTHOR. #       : ET193148TRANS. REF.     :     Approved - Thank You 100Please retain this copy for your records.Cardholder will pay above amount tocard issuer pursuant to cardholderagreement.========================================</CTR>" +
      "<FraudSuspected/>" +
      "</TransactionResult>";
  }
}
