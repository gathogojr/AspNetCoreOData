//-----------------------------------------------------------------------------
// <copyright file="TestAssembly.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Reflection;

namespace Microsoft.AspNetCore.OData.E2E.Tests.Commons;

/// <summary>
/// This class is used in AspNetCore to add controllers as an assembly part for discovery.
/// </summary>
internal sealed class TestAssembly : Assembly
{
    Type[] _types;

    public TestAssembly(params Type[] types)
    {
        _types = types;
    }

    public override Type[] GetTypes()
    {
        return _types;
    }
}
