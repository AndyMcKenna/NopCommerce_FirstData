using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using System;

namespace BitShift.Plugin.Payments.FirstData.Tests.MockData
{
  public static class MockCustomer
  {
    public static Customer JohnDoe => new Customer
    {
      CustomerGuid = new Guid(),
      BillingAddress = new Address
      {
        Address1 = "123 Fake St.",
        StateProvince = new StateProvince
        {
          Abbreviation = "CA",
          Name = "California"
        },
        Country = new Country
        {
          ThreeLetterIsoCode = "USA"
        },
        ZipPostalCode = "90210"
      }
    };
  }
}
