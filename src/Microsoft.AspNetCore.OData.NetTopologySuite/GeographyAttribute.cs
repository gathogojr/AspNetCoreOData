using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.OData.NetTopologySuite
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class GeographyAttribute : Attribute
    {
    }
}
