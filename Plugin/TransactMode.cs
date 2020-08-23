namespace BitShift.Plugin.Payments.FirstData
{
  public enum TransactMode : int
  {
    /// <summary>
    /// Authorize
    /// </summary>
    Authorize = 1,
    /// <summary>
    /// Authorize and capture
    /// </summary>
    AuthorizeAndCapture = 2,
    /// <summary>
    /// Authorize, capture once the order is created
    /// </summary>
    AuthorizeAndCaptureAfterOrder = 3,
    /// <summary>
    /// Hosted payment page: authorize, capture once the order is created
    /// </summary>
    HostedPaymentPagePostCapture = 4,
    /// <summary>
    /// Hosted payment page: authorize only
    /// </summary>
    HostedPaymentPageAuthOnly = 5
  }

  /// <summary>
  /// Represents FirstData payment processor transaction mode
  /// </summary>
  public class TransactionType
  {
    public const string Purchase = "00";
    public const string PreAuth = "01";
    public const string PreAuthCompletion = "02";
    public const string Refund = "04";
    public const string Void = "13";
    public const string TaggedPreAuthCompletion = "32";
    public const string TaggedVoid = "33";
    public const string TaggedRefund = "34";
  }
}
