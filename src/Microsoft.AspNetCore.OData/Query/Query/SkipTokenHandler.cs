//-----------------------------------------------------------------------------
// <copyright file="SkipTokenHandler.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.AspNetCore.OData.Formatter.Serialization;

namespace Microsoft.AspNetCore.OData.Query;

/// <summary>
/// Represents how NextLink for paging is generated.
/// </summary>
public abstract class SkipTokenHandler
{
    /// <summary>
    /// Apply the $skiptoken query to the given IQueryable.
    /// </summary>
    /// <param name="query">The original <see cref="IQueryable"/>.</param>
    /// <param name="skipTokenQueryOption">The query option that contains all the relevant information for applying skiptoken.</param>
    /// <param name="querySettings">The query settings to use while applying this query option.</param>
    /// <param name="queryOptions">Information about the other query options.</param>
    /// <returns>The new <see cref="IQueryable"/> after the skiptoken query has been applied to.</returns>
    public abstract IQueryable<T> ApplyTo<T>(IQueryable<T> query, SkipTokenQueryOption skipTokenQueryOption,
        ODataQuerySettings querySettings, ODataQueryOptions queryOptions);

    /// <summary>
    /// Apply the $skiptoken query to the given IQueryable.
    /// </summary>
    /// <param name="query">The original <see cref="IQueryable"/>.</param>
    /// <param name="skipTokenQueryOption">The query option that contains all the relevant information for applying skiptoken.</param>
    /// <param name="querySettings">The query settings to use while applying this query option.</param>
    /// <param name="queryOptions">Information about the other query options.</param>
    /// <returns>The new <see cref="IQueryable"/> after the skiptoken query has been applied to.</returns>
    public abstract IQueryable ApplyTo(IQueryable query, SkipTokenQueryOption skipTokenQueryOption,
        ODataQuerySettings querySettings, ODataQueryOptions queryOptions);

    /// <summary>
    /// Returns the URI for NextPageLink
    /// </summary>
    /// <param name="baseUri">BaseUri for nextlink.</param>
    /// <param name="pageSize">Maximum number of records in the set of partial results for a resource.</param>
    /// <param name="instance">Instance based on which SkipToken value will be generated.</param>
    /// <param name="context">Serializer context</param>
    /// <returns>URI for the NextPageLink.</returns>
    public abstract Uri GenerateNextPageLink(Uri baseUri, int pageSize, object instance, ODataSerializerContext context);
}
