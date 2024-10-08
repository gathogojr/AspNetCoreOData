//-----------------------------------------------------------------------------
// <copyright file="SkipQueryOption.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.AspNetCore.OData.Query.Validator;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNetCore.OData.Query;

/// <summary>
/// This defines a $skip OData query option for querying.
/// </summary>
public class SkipQueryOption
{
    private int? _value;
    private ODataQueryOptionParser _queryOptionParser;

    /// <summary>
    /// Initialize a new instance of <see cref="SkipQueryOption"/> based on the raw $skip value and
    /// an EdmModel from <see cref="ODataQueryContext"/>.
    /// </summary>
    /// <param name="rawValue">The raw value for $skip query. It can be null or empty.</param>
    /// <param name="context">The <see cref="ODataQueryContext"/> which contains the <see cref="IEdmModel"/> and some type information</param>
    /// <param name="queryOptionParser">The <see cref="ODataQueryOptionParser"/> which is used to parse the query option.</param>
    public SkipQueryOption(string rawValue, ODataQueryContext context, ODataQueryOptionParser queryOptionParser)
    {
        if (context == null)
        {
            throw Error.ArgumentNull(nameof(context));
        }

        if (String.IsNullOrEmpty(rawValue))
        {
            throw Error.ArgumentNullOrEmpty(nameof(rawValue));
        }

        if (queryOptionParser == null)
        {
            throw Error.ArgumentNull(nameof(queryOptionParser));
        }

        Context = context;
        RawValue = rawValue;
        Validator = context.GetSkipQueryValidator();
        _queryOptionParser = queryOptionParser;
    }

    // This constructor is intended for unit testing only.
    internal SkipQueryOption(string rawValue, ODataQueryContext context)
    {
        if (context == null)
        {
            throw Error.ArgumentNull(nameof(context));
        }

        if (string.IsNullOrEmpty(rawValue))
        {
            throw Error.ArgumentNullOrEmpty(nameof(rawValue));
        }

        Context = context;
        RawValue = rawValue;
        Validator = context.GetSkipQueryValidator();
        _queryOptionParser = new ODataQueryOptionParser(
            context.Model,
            context.ElementType,
            context.NavigationSource,
            new Dictionary<string, string> { { "$skip", rawValue } });
    }

    /// <summary>
    /// Gets the given <see cref="ODataQueryContext"/>.
    /// </summary>
    public ODataQueryContext Context { get; private set; }

    /// <summary>
    /// Gets the raw $skip value.
    /// </summary>
    public string RawValue { get; private set; }

    /// <summary>
    /// Gets the value of the $skip as a parsed integer.
    /// </summary>
    public int Value
    {
        get
        {
            if (_value == null)
            {
                long? skipValue = _queryOptionParser.ParseSkip();

                if (skipValue.HasValue && skipValue > Int32.MaxValue)
                {
                    throw new ODataException(Error.Format(
                        SRResources.SkipTopLimitExceeded,
                        Int32.MaxValue,
                        AllowedQueryOptions.Skip,
                        RawValue));
                }

                _value = (int?)skipValue;
            }

            Contract.Assert(_value.HasValue);
            return _value.Value;
        }
    }

    /// <summary>
    /// Gets or sets the Skip Query Validator.
    /// </summary>
    public ISkipQueryValidator Validator { get; set; }

    /// <summary>
    /// Apply the $skip query to the given IQueryable.
    /// </summary>
    /// <param name="query">The original <see cref="IQueryable"/>.</param>
    /// <param name="querySettings">The query settings to use while applying this query option.</param>
    /// <returns>The new <see cref="IQueryable"/> after the skip query has been applied to.</returns>
    public IQueryable<T> ApplyTo<T>(IQueryable<T> query, ODataQuerySettings querySettings)
    {
        return ApplyToCore(query, querySettings) as IOrderedQueryable<T>;
    }

    /// <summary>
    /// Apply the $skip query to the given IQueryable.
    /// </summary>
    /// <param name="query">The original <see cref="IQueryable"/>.</param>
    /// <param name="querySettings">The query settings to use while applying this query option.</param>
    /// <returns>The new <see cref="IQueryable"/> after the skip query has been applied to.</returns>
    public IQueryable ApplyTo(IQueryable query, ODataQuerySettings querySettings)
    {
        return ApplyToCore(query, querySettings);
    }

    /// <summary>
    /// Validate the skip query based on the given <paramref name="validationSettings"/>. It throws an ODataException if validation failed.
    /// </summary>
    /// <param name="validationSettings">The <see cref="ODataValidationSettings"/> instance which contains all the validation settings.</param>
    public void Validate(ODataValidationSettings validationSettings)
    {
        if (validationSettings == null)
        {
            throw Error.ArgumentNull(nameof(validationSettings));
        }

        if (Validator != null)
        {
            Validator.Validate(this, validationSettings);
        }
    }

    private IQueryable ApplyToCore(IQueryable query, ODataQuerySettings querySettings)
    {
        if (query == null)
        {
            throw Error.ArgumentNull(nameof(query));
        }

        if (querySettings == null)
        {
            throw Error.ArgumentNull(nameof(querySettings));
        }

        if (Context.ElementClrType == null)
        {
            throw Error.NotSupported(SRResources.ApplyToOnUntypedQueryOption, "ApplyTo");
        }

        return ExpressionHelpers.Skip(query, Value, query.ElementType, querySettings.EnableConstantParameterization);
    }
}
