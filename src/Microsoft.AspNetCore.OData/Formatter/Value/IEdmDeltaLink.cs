//-----------------------------------------------------------------------------
// <copyright file="IEdmDeltaLink.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.AspNetCore.OData.Formatter.Value;

/// <summary>
/// Represents an instance of an <see cref="IEdmChangedObject"/>.
/// Holds the properties necessary to create the ODataDeltaLink.
/// </summary>
public interface IEdmDeltaLink : IEdmDeltaLinkBase, IEdmChangedObject
{
}
