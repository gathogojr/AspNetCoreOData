//-----------------------------------------------------------------------------
// <copyright file="SingletonDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.AspNetCore.OData.E2E.Tests.Singleton;

/// <summary>
/// Present the EntityType "Partner"
/// </summary>
public class Partner
{
    public int ID { get; set; }
    public string Name { get; set; }

    [Singleton]
    public Company Company { get; set;}
}

/// <summary>
/// Present company category, which is an enum type
/// </summary>
public enum CompanyCategory
{
    IT = 0,
    Communication = 1,
    Electronics = 2,
    Others = 3
}

/// <summary>
/// Present the EntityType "Company"
/// </summary>
public class Company
{
    public int ID { get; set; }

    public string Name { get; set; }

    public long Revenue { get; set; }
    public CompanyCategory Category { get; set; }

    [NotCountable]
    public IList<Partner> Partners { get; set; }
    public IList<Office> Branches { get; set; }

    [Contained]
    [AutoExpand]
    public IList<Project> Projects { get; set; }
}

/// <summary>
/// Present a complex type
/// </summary>
public class Office
{
    public string City { get; set; }
    public string Address { get; set; }
}

/// <summary>
/// Present a contained navigation property
/// </summary>
public class Project
{
    public int Id { get; set; }
    public string Title { get; set; }

    [AutoExpand]
    [Contained]
    public IList<ProjectDetail> ProjectDetails { get; set; }
}

/// <summary>
/// Present a nested contained navigation property
/// </summary>
public class ProjectDetail
{
    public int Id { get; set; }
    public string Comment { get; set; }
}

/// <summary>
/// EntityType derives from "Company"
/// </summary>
public class SubCompany : Company
{
    public string Location { get; set; }
    public string Description { get; set; }
    public Office Office { get; set; }
}
