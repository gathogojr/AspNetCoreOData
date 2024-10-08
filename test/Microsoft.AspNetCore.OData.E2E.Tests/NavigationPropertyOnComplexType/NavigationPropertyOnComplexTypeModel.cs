//-----------------------------------------------------------------------------
// <copyright file="NavigationPropertyOnComplexTypeModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.AspNetCore.OData.E2E.Tests.NavigationPropertyOnComplexType;

public class Person
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }

    public IList<int> Taxes { get; set; }

    public Address HomeLocation { get; set; }

    public IList<Address> RepoLocations { get; set; }

    public GeoLocation PreciseLocation { get; set; }

    public OrderInfo OrderInfo { get; set; }
}

public class VipPerson : Person
{
    public int Bonus { get; set; }
}

public class Address
{
    public string Street { get; set; }

    public int TaxNo { get; set; }

    public IList<string> Emails { get; set; }

    public AddressInfo RelatedInfo { get; set; }

    public IList<AddressInfo> AdditionInfos { get; set; }

    public ZipCode ZipCode { get; set; }

    public IList<ZipCode> DetailCodes { get; set; }
}

public class AddressInfo
{
    public int AreaSize { get; set; }

    public string CountyName { get; set; }
}

public class OrderInfo
{
    public Address BillLocation { get; set; }

    public OrderInfo SubInfo { get; set; }

    public IDictionary<string, object> propertybag { get; set; }
}

public class ZipCode
{
    [Key]
    public int Zip { get; set; }

    public string City { get; set; }

    public string State { get; set; }
}

public class GeoLocation : Address
{
    public string Latitude { get; set; }

    public string Longitude { get; set; }

    public ZipCode Area { get; set; }
}

// with the same propert name in different derived type.
public class GeometryLocation : Address
{
    public string Latitude { get; set; }

    public string Longitude { get; set; }
}

public class ModelGenerator
{
    // Builds the EDM model for the OData service.
    public static IEdmModel GetConventionalEdmModel()
    {
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<Person>("People");
        modelBuilder.EntitySet<ZipCode>("ZipCodes");

        modelBuilder.Namespace = typeof(Person).Namespace;
        return modelBuilder.GetEdmModel();
    }
}
