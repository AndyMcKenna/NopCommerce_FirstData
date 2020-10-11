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
      BillingAddressId = 1      
    };

    public static Address JohnDoesAddress => new Address
    {
      Address1 = "123 Fake St.",
      StateProvinceId = 1,
      CountryId = 1,
      ZipPostalCode = "90210"
    };

    public static Address SerbianAddress => new Address
    {
      Address1 = "Dragoslava Jovanovića 2",
      City = "Beograd",
      StateProvinceId = null,
      CountryId = 189,
      ZipPostalCode = "11000"
    };

    public static StateProvince StateProvince => new StateProvince
    {
      Abbreviation = "CA",
      Name = "California"
    };

    public static Country USA => new Country
    {
      ThreeLetterIsoCode = "USA"
    };

    public static Country Serbia => new Country
    {
      ThreeLetterIsoCode = "SRB"
    };
  }
}
