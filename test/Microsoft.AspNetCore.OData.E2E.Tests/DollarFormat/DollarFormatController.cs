//-----------------------------------------------------------------------------
// <copyright file="DollarFormatController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.AspNetCore.OData.E2E.Tests.DollarFormat;

public class DollarFormatCustomersController : ODataController
{
    private IList<DollarFormatCustomer> customers = Enumerable.Range(0, 10).Select(i =>
            new DollarFormatCustomer
            {
                Id = i,
                Name = "Customer Name " + i,
                Orders = Enumerable.Range(0, i).Select(j =>
                    new DollarFormatOrder
                    {
                        Id = i * 10 + j,
                        PurchaseDate = DateTime.Today.Subtract(TimeSpan.FromDays(i * 10 + j)),
                        Detail = "This is Order " + i * 10 + j
                    }).ToList(),
                SpecialOrder = new DollarFormatOrder
                {
                    Id = i * 10,
                    PurchaseDate = DateTime.Today.Subtract(TimeSpan.FromDays(i * 10)),
                    Detail = "This is Order " + i * 10
                }
            }).ToList();

    [EnableQuery(PageSize = 10, MaxExpansionDepth = 5)]
    public IActionResult Get()
    {
        return Ok(customers);
    }

    [EnableQuery(PageSize = 10, MaxExpansionDepth = 5)]
    public IActionResult Get(int key)
    {
        var customer = customers.FirstOrDefault(c => c.Id == key);

        if (customer == null)
        {
            throw new ArgumentOutOfRangeException("key");
        }

        return Ok(customer);
    }
}
