using System;
using System.Collections.Generic;
using System.Text;

namespace BitShift.Plugin.Payments.FirstData.Tests.MockData
{
  public static class CardNumbers
  {
    public static Dictionary<string, List<string>> Real = new Dictionary<string, List<string>>
      {
        { "Amex", new List<string>
          {
            "378282246310005",
            "371449635398431",
            "378734493671000"
          }
        },
        { "Discover", new List<string>
          {
            "6011000990139424",
            "6011111111111117"
          }
        },
        { "Mastercard", new List<string>
          {
            "5555555555554444",
            "5105105105105100"
          }
        },
        { "Visa", new List<string>
          {
            "4111111111111111",
            "4012888888881881",
            "4222222222222"
          }
        }
      };
  }
}
