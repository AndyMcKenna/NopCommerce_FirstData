using BitShift.Plugin.Payments.FirstData.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace BitShift.Plugin.Payments.FirstData.Data
{
  public class FirstDataNameCompatibility : INameCompatibility
  {
    public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
    {
      { typeof(SavedCard), "BitShift_FirstData_SavedCard" },
      { typeof(FirstDataStoreSetting), "BitShift_FirstData_StoreSetting" }
    };

    public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>();
  }
}
