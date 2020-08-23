using System.Net;

namespace BitShift.Plugin.Payments.FirstData.Services
{
  public class FDWebRequest : IWebRequest
  {
    public HttpWebRequest Create(string uri)
    {
      return (HttpWebRequest)WebRequest.Create(uri);
    }
  }
}
