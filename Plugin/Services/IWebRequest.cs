using System.Net;

namespace BitShift.Plugin.Payments.FirstData.Services
{
  public interface IWebRequest
  {
    HttpWebRequest Create(string uri);
  }
}
