//-----------------------------------------------------------------------------
// <copyright file="DateTimeOffsetDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.AspNetCore.OData.E2E.Tests.DateTimeOffsetSupport;

public class FilesContext : DbContext
{
    public FilesContext(DbContextOptions<FilesContext> options)
        : base(options)
    { }

    public DbSet<File> Files { get; set; }
}

public class File
{
    [Key]
    public int FileId { get; set; }

    public string Name { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public override bool Equals(Object o)
    {
        var f = o as File;
        if (f == null)
        {
            return false;
        }

        var v1 = FileId.Equals(f.FileId);
        var v2 = Name.Equals(f.Name);
        var v3 = CreatedDate.Equals(f.CreatedDate);
        var v4 = Object.Equals(DeleteDate, f.DeleteDate);
        return v1 && v2 && v3 && v4;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
