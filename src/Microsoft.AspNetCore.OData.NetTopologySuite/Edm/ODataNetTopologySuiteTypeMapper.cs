using Microsoft.AspNetCore.OData.Edm;
using Microsoft.OData.Edm;
using LineString = NetTopologySuite.Geometries.LineString;
using Point = NetTopologySuite.Geometries.Point;

namespace Microsoft.AspNetCore.OData.NetTopologySuite.Edm
{
    public class ODataNetTopologySuiteTypeMapper : DefaultODataTypeMapper
    {
        internal static readonly ODataNetTopologySuiteTypeMapper Instance;

        static ODataNetTopologySuiteTypeMapper()
        {
            Instance = new ODataNetTopologySuiteTypeMapper();
        }

        protected override void RegisterSpatialMappings()
        {
            BuildReferenceTypeMapping<Point>(EdmPrimitiveTypeKind.GeometryPoint, isStandard: false);
            BuildReferenceTypeMapping<LineString>(EdmPrimitiveTypeKind.GeometryLineString, isStandard: false);
        }
    }
}
