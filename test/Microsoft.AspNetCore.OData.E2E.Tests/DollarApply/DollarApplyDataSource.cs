//-----------------------------------------------------------------------------
// <copyright file="DollarApplyDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.AspNetCore.OData.E2E.Tests.DollarApply
{
    public class DollarApplyDbContext : DbContext
    {
        public DollarApplyDbContext(DbContextOptions<DollarApplyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Sale> Sales { get; set; }
    }

    public class DataSource
    {
        private static readonly Company company;
        private static readonly List<Employee> employees;

        static DataSource()
        {
            company = new Company
            {
                Name = "Northwind Traders"
            };

            employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "Nancy Davolio",
                    BaseSalary = 1300,
                    Address = new Address
                    {
                        City = "Seattle",
                        State = "WA"
                    },
                    Company = company,
                    DynamicProperties = new Dictionary<string, object>
                    {
                        { "Commission", 250 },
                        { "Gender", "Female" }
                    }
                },
                new Employee
                {
                    Id = 2,
                    Name = "Andrew Fuller",
                    BaseSalary = 1500,
                    Address = new Address
                    {
                        City = "Tacoma",
                        State = "WA"
                    },
                    Company = company,
                    DynamicProperties = new Dictionary<string, object>
                    {
                        { "Commission", 190 },
                        { "Gender", "Male" }
                    }
                },
                new Employee
                {
                    Id = 3,
                    Name = "Janet Leverling",
                    BaseSalary = 1100,
                    Address = new Address
                    {
                        City = "Kirkland",
                        State = "WA"
                    },
                    Company = company,
                    DynamicProperties = new Dictionary<string, object>
                    {
                        { "Commission", 370 },
                        { "Gender", "Female" }
                    }
                },
                new Employee
                {
                    Id = 9,
                    Name = "Anne Dodsworth",
                    BaseSalary = 1000,
                    Address = new Address
                    {
                        City = "London",
                        State = "UK"
                    },
                    Company = company,
                    DynamicProperties = new Dictionary<string, object>
                    {
                        { "Commission", 310 },
                        { "Gender", "Female" }
                    }
                },
            };

            company.VP = employees.First(e => e.Id == 2);
        }

        public static List<Employee> Employees => employees;
    }
}
