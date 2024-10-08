//-----------------------------------------------------------------------------
// <copyright file="NavigationLinkBuilder.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.OData.Edm;

namespace Microsoft.AspNetCore.OData.Edm;

/// <summary>
/// Encapsulates a navigation link factory and whether the link factory follows conventions or not.
/// </summary>
public class NavigationLinkBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationLinkBuilder"/> class.
    /// </summary>
    /// <param name="navigationLinkFactory">The navigation link factory for creating navigation links.</param>
    /// <param name="followsConventions">Represents whether this factory follows OData conventions or not.</param>
    public NavigationLinkBuilder(Func<ResourceContext, IEdmNavigationProperty, Uri> navigationLinkFactory, bool followsConventions)
    {
        Factory = navigationLinkFactory ?? throw Error.ArgumentNull(nameof(navigationLinkFactory));
        FollowsConventions = followsConventions;
    }

    /// <summary>
    /// Gets the navigation link factory for creating navigation links.
    /// </summary>
    public Func<ResourceContext, IEdmNavigationProperty, Uri> Factory { get; }

    /// <summary>
    /// Gets a value representing whether this factory follows OData conventions or not.
    /// </summary>
    public bool FollowsConventions { get; }
}
