//-----------------------------------------------------------------------------
// <copyright file="UnboundOperationDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.OData.E2E.Tests.UnboundOperation;

public class ConventionCustomer
{
    public int ID { get; set; }

    public string Name { get; set; }

    public ConventionAddress Address { get; set; }

    public List<ConventionOrder> Orders { get; set; }
}

public class ConventionAddress
{
    public string Street { get; set; }

    public string City { get; set; }

    public string ZipCode { get; set; }
}

public class ConventionOrder
{
    public int ID { get; set; }

    public string OrderName { get; set; }

    public decimal Price { get; set; }

    public Guid OrderGuid { get; set; }
}

public enum ConventionGender
{
    Male,

    Female,
}
