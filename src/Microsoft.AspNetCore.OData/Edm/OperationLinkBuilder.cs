//-----------------------------------------------------------------------------
// <copyright file="OperationLinkBuilder.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.OData.Formatter;

namespace Microsoft.AspNetCore.OData.Edm;

/// <summary>
/// <see cref="OperationLinkBuilder"/> can be used to annotate an action or a function.
/// This is how formatters create links to invoke bound actions or functions.
/// </summary>
public class OperationLinkBuilder
{
    /// <summary>
    /// Create a new <see cref="OperationLinkBuilder"/> based on an entity link factory.
    /// </summary>
    /// <param name="linkFactory">The link factory this <see cref="OperationLinkBuilder"/> should use when building links.</param>
    /// <param name="followsConventions">
    /// A value indicating whether the link factory generates links that follow OData conventions.
    /// </param>
    public OperationLinkBuilder(Func<ResourceContext, Uri> linkFactory, bool followsConventions)
    {
        if (linkFactory == null)
        {
            throw Error.ArgumentNull(nameof(linkFactory));
        }

        LinkFactory = linkFactory;
        FollowsConventions = followsConventions;
    }

    /// <summary>
    /// Create a new <see cref="OperationLinkBuilder"/> based on a feed link factory.
    /// </summary>
    /// <param name="linkFactory">The link factory this <see cref="OperationLinkBuilder"/> should use when building links.</param>
    /// <param name="followsConventions">
    /// A value indicating whether the action link factory generates links that follow OData conventions.
    /// </param>
    public OperationLinkBuilder(Func<ResourceSetContext, Uri> linkFactory, bool followsConventions)
    {
        if (linkFactory == null)
        {
            throw Error.ArgumentNull(nameof(linkFactory));
        }

        FeedLinkFactory = linkFactory;
        FollowsConventions = followsConventions;
    }

    /// <summary>
    /// Gets the resource link factory.
    /// </summary>
    internal Func<ResourceContext, Uri> LinkFactory { get; }

    /// <summary>
    /// Gets the feed link factory.
    /// </summary>
    internal Func<ResourceSetContext, Uri> FeedLinkFactory { get; }

    /// <summary>
    /// Gets a Boolean indicating whether the link factory follows OData conventions or not.
    /// </summary>
    public bool FollowsConventions { get; }

    /// <summary>
    /// Builds the operation link for the given resource.
    /// </summary>
    /// <param name="context">An instance context wrapping the resource instance.</param>
    /// <returns>The generated link.</returns>
    public virtual Uri BuildLink(ResourceContext context)
    {
        if (LinkFactory == null)
        {
            return null;
        }

        return LinkFactory(context);
    }

    /// <summary>
    /// Builds the operation link for the given feed.
    /// </summary>
    /// <param name="context">An feed context wrapping the feed instance.</param>
    /// <returns>The generated link.</returns>
    public virtual Uri BuildLink(ResourceSetContext context)
    {
        if (FeedLinkFactory == null)
        {
            return null;
        }

        return FeedLinkFactory(context);
    }
}
