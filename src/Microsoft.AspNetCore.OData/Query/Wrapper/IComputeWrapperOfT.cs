//-----------------------------------------------------------------------------
// <copyright file="IComputeWrapperOfT.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;

namespace Microsoft.AspNetCore.OData.Query.Wrapper
{
    /// <summary>
    /// Represents a wrapper for an entity with computed values in an OData query.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped entity.</typeparam>
    public interface IComputeWrapper<T>
    {
        /// <summary>
        /// Gets or sets the original instance of entity.
        /// </summary>
        public T Instance { get; set; }

        /// <summary>
        /// Gets or sets the Edm model associated with the wrapper.
        /// </summary>
        public IEdmModel Model { get; set; }
    }
}
