using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.OData.Edm;
using Microsoft.AspNetCore.OData.Formatter.Deserialization;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.AspNetCore.OData.NetTopologySuite.Edm;
using Microsoft.AspNetCore.OData.NetTopologySuite.Formatter.Deserialization;
using Microsoft.AspNetCore.OData.NetTopologySuite.Formatter.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OData.Edm;

namespace Microsoft.AspNetCore.OData.NetTopologySuite.Extensions
{
    // TODO: Add to declared API when finalized
    public static class ODataNetTopologyServiceCollectionExtensions
    {
        // TODO: We pass the model to this method because the type mapper is intended to be
        // associated with the model itself (i.e., different models can have different type mappers).
        // For this reason, the mapper is stored as an annotation on the model.
        // Otherwise, we would simply inject it into the container as a route service.
        // A model is mapped to a route... Does that not mean that we could think of the type mapper as a route service?
        public static IServiceCollection AddODataNetTopologySuite(this IServiceCollection services, IEdmModel model)
        {
            services.RemoveAll<ODataSpatialDeserializer>();
            services.AddSingleton<ODataSpatialDeserializer, ODataSpatialNetTopologySuiteDeserializer>();
            services.RemoveAll<ODataSpatialSerializer>();
            services.AddSingleton<ODataSpatialSerializer, ODataSpatialNetTopologySuiteSerializer>();

            model.SetTypeMapper(ODataNetTopologySuiteTypeMapper.Instance);

            return services;
        }
    }
}
