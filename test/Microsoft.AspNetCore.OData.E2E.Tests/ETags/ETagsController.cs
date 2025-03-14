//-----------------------------------------------------------------------------
// <copyright file="ETagsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.AspNetCore.OData.E2E.Tests.ETags;

// Be noted, some test cases updates the static "customers".
// So, there's some test cases using certain "id" to make sure all test cases can run in parallel.
public class ETagsDerivedCustomersController : ODataController
{
    internal static IList<ETagsCustomer> customers = Enumerable.Range(0, 10).Select(i => Helpers.CreateCustomer(i)).ToList();

    [EnableQuery(PageSize = 10, MaxExpansionDepth = 5)]
    public IActionResult Get()
    {
        return Ok(customers.Select(c => Helpers.CreateDerivedCustomer(c)));
    }
}

public class ETagsDerivedCustomersSingletonController : ODataController
{
    internal static IList<ETagsCustomer> customers = Enumerable.Range(0, 10).Select(i => Helpers.CreateCustomer(i)).ToList();

    [EnableQuery(PageSize = 10, MaxExpansionDepth = 5)]
    public IActionResult Get()
    {
        return Ok(customers.Select(c => Helpers.CreateDerivedCustomer(c)).FirstOrDefault());
    }
}

public class ETagsCustomersController : ODataController
{
    internal static IList<ETagsCustomer> customers = Enumerable.Range(0, 10).Select(i => Helpers.CreateCustomer(i)).ToList();

    [HttpGet]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(customers);
    }

    [HttpGet]
    [EnableQuery(PageSize = 10, MaxExpansionDepth = 5)]
    public IActionResult Get(int key, ODataQueryOptions<ETagsCustomer> queryOptions)
    {
        IEnumerable<ETagsCustomer> appliedCustomers = customers.Where(c => c.Id == key);

        if (appliedCustomers.Count() == 0)
        {
            return BadRequest("The key is not valid");
        }

        if (queryOptions.IfNoneMatch != null)
        {
            appliedCustomers = queryOptions.IfNoneMatch.ApplyTo(appliedCustomers.AsQueryable()).Cast<ETagsCustomer>();
        }

        if (appliedCustomers.Count() == 0)
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }
        else
        {
            return Ok(new SingleResult<ETagsCustomer>(appliedCustomers.AsQueryable()));
        }
    }

    [HttpPut]
    public IActionResult Put(int key, [FromBody]ETagsCustomer eTagsCustomer, ODataQueryOptions<ETagsCustomer> queryOptions)
    {
        if (key != eTagsCustomer.Id)
        {
            return BadRequest("The Id of customer is not matched with the key");
        }

        IEnumerable<ETagsCustomer> appliedCustomers = customers.Where(c => c.Id == eTagsCustomer.Id);

        if (appliedCustomers.Count() == 0)
        {
            customers.Add(eTagsCustomer);
            return Ok(eTagsCustomer);
        }

        if (queryOptions.IfMatch != null)
        {
            IQueryable<ETagsCustomer> ifMatchCustomers = queryOptions.IfMatch.ApplyTo(appliedCustomers.AsQueryable()).Cast<ETagsCustomer>();

            if (ifMatchCustomers.Count() == 0)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            }
        }

        ETagsCustomer customer = appliedCustomers.Single();
        customer.Name = eTagsCustomer.Name;
        customer.Notes = eTagsCustomer.Notes;
        customer.BoolProperty = eTagsCustomer.BoolProperty;
        customer.ByteProperty = eTagsCustomer.ByteProperty;
        customer.CharProperty = eTagsCustomer.CharProperty;
        customer.DecimalProperty = eTagsCustomer.DecimalProperty;
        customer.DoubleProperty = eTagsCustomer.DoubleProperty;
        customer.ShortProperty = eTagsCustomer.ShortProperty;
        customer.LongProperty = eTagsCustomer.LongProperty;
        customer.SbyteProperty = eTagsCustomer.SbyteProperty;
        customer.FloatProperty = eTagsCustomer.FloatProperty;
        customer.UshortProperty = eTagsCustomer.UshortProperty;
        customer.UintProperty = eTagsCustomer.UintProperty;
        customer.UlongProperty = eTagsCustomer.UlongProperty;
        customer.GuidProperty = eTagsCustomer.GuidProperty;
        customer.DateTimeOffsetProperty = eTagsCustomer.DateTimeOffsetProperty;

        return Ok(customer);
    }

    [HttpDelete]
    public IActionResult Delete(int key, ODataQueryOptions<ETagsCustomer> queryOptions)
    {
        IEnumerable<ETagsCustomer> appliedCustomers = customers.Where(c => c.Id == key);

        if (appliedCustomers.Count() == 0)
        {
            return BadRequest(string.Format("The entry with Id {0} doesn't exist", key));
        }

        if (queryOptions.IfMatch != null)
        {
            IQueryable<ETagsCustomer> ifMatchCustomers = queryOptions.IfMatch.ApplyTo(appliedCustomers.AsQueryable()).Cast<ETagsCustomer>();

            if (ifMatchCustomers.Count() == 0)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            }
        }

        ETagsCustomer customer = appliedCustomers.Single();
        customers.Remove(customer);
        return Ok(customer);
    }

    [HttpPatch]
    public IActionResult Patch(int key, [FromBody]Delta<ETagsCustomer> patch, ODataQueryOptions<ETagsCustomer> queryOptions)
    {
        IEnumerable<ETagsCustomer> appliedCustomers = customers.Where(c => c.Id == key);

        if (appliedCustomers.Count() == 0)
        {
            return BadRequest(string.Format("The entry with Id {0} doesn't exist", key));
        }

        if (queryOptions.IfMatch != null)
        {
            IQueryable<ETagsCustomer> ifMatchCustomers = queryOptions.IfMatch.ApplyTo(appliedCustomers.AsQueryable()).Cast<ETagsCustomer>();

            if (ifMatchCustomers.Count() == 0)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            }
        }
           
        ETagsCustomer customer = appliedCustomers.Single();
        patch.Patch(customer);

        return Ok(customer);
    }
}

public class ETagSimpleThingsController : ODataController
{
    private readonly static ETagSimpleThing[] things =
    [
        new ETagSimpleThing { Id = 1, Name = "Thing 1", ComplexThing = new ETagComplexThing { Name = "Complex 1" } },
        new ETagSimpleThing { Id = 2, Name = "Thing 2" }
    ];

    [EnableQuery]
    public ActionResult<IEnumerable<ETagSimpleThing>> Get()
    {
        return Ok(things);
    }

    [EnableQuery]
    public ActionResult<ETagSimpleThing> Get(int key)
    {
        var thing = things.FirstOrDefault(t => t.Id == key);

        if (thing == null)
        {
            return NotFound();
        }

        return thing;
    }
}

internal class Helpers
{
    internal static ETagsCustomer CreateCustomer(int i)
    {
        return new ETagsDerivedCustomer
        {
            Id = i,
            Name = "Customer Name " + i,
            ShortProperty = (short)(Int16.MaxValue - i),
            DoubleProperty = 2.0 * (i + 1),
            Notes = Enumerable.Range(0, i + 1).Select(j => "This is note " + (i * 10 + j)).ToList(),
            RelatedCustomer = new ETagsCustomer
            {
                Id = i + 1,
                Name = "Customer Name " + i + 1,
                ShortProperty = (short)(Int16.MaxValue - (i + 1) * 10),
                DoubleProperty = 2.0 * (i + 1) * 10,
                Notes = Enumerable.Range(0, (i + 1) * 10).Select(j => "This is note " + ((i + 1) * 10 + j)).ToList()
            },
            ContainedCustomer = new ETagsCustomer
            {
                Id = (i + 1) * 100,
                Name = "Customer Name " + i * 10,
                ShortProperty = (short)(Int16.MaxValue - i * 10),
                DoubleProperty = 2.0 * (i * 10 + 1),
                Notes = Enumerable.Range(0, i * 10 + 1).Select(j => "This is note " + (i * 100 + j)).ToList()
            }
        };
    }

    internal static bool ValidateEtag(ETagsCustomer customer, ODataQueryOptions options)
    {
        if (options.IfMatch != null)
        {
            IQueryable<ETagsCustomer> ifMatchCustomers = options.IfMatch.ApplyTo((new ETagsCustomer[] { customer }).AsQueryable()).Cast<ETagsCustomer>();

            if (ifMatchCustomers.Count() == 0)
            {
                return false;
            }
        }
        return true;
    }

    internal static ETagsDerivedCustomer CreateDerivedCustomer(ETagsCustomer customer)
    {
        ETagsDerivedCustomer newCustomer = new ETagsDerivedCustomer();
        newCustomer.Id = customer.Id;
        newCustomer.Role = customer.Name + customer.Id;
        ReplaceCustomer(newCustomer, customer);
        return newCustomer;
    }

    internal static void ReplaceCustomer(ETagsCustomer newCustomer, ETagsCustomer customer)
    {
        newCustomer.Name = customer.Name;
        newCustomer.Notes = customer.Notes;
        newCustomer.BoolProperty = customer.BoolProperty;
        newCustomer.ByteProperty = customer.ByteProperty;
        newCustomer.CharProperty = customer.CharProperty;
        newCustomer.DecimalProperty = customer.DecimalProperty;
        newCustomer.DoubleProperty = customer.DoubleProperty;
        newCustomer.ShortProperty = customer.ShortProperty;
        newCustomer.LongProperty = customer.LongProperty;
        newCustomer.SbyteProperty = customer.SbyteProperty;
        newCustomer.FloatProperty = customer.FloatProperty;
        newCustomer.UshortProperty = customer.UshortProperty;
        newCustomer.UintProperty = customer.UintProperty;
        newCustomer.UlongProperty = customer.UlongProperty;
        newCustomer.GuidProperty = customer.GuidProperty;
        newCustomer.DateTimeOffsetProperty = customer.DateTimeOffsetProperty;
    }
}

