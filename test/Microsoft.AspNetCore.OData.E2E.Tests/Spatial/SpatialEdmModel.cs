//-----------------------------------------------------------------------------
// <copyright file="SpatialEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.AspNetCore.OData.E2E.Tests.Spatial;

public static class IsofEdmModel
{
    private static IEdmModel _model;

    public static IEdmModel GetEdmModel()
    {
        if (_model != null)
        {
            return _model;
        }

        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<SpatialCustomer>("SpatialCustomers");
        return _model = builder.GetEdmModel();
    }
}
