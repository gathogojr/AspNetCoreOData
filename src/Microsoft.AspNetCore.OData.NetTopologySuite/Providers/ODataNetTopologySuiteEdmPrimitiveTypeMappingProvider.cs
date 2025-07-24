using System.Diagnostics;
using System.Reflection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder.Providers;
using LineString = NetTopologySuite.Geometries.LineString;
using Point = NetTopologySuite.Geometries.Point;

namespace Microsoft.AspNetCore.OData.NetTopologySuite.Providers
{
    public class ODataNetTopologySuiteEdmPrimitiveTypeMappingProvider : IEdmPrimitiveTypeMappingProvider
    {
        private static readonly EdmCoreModel _coreModel = EdmCoreModel.Instance;

        private static readonly Dictionary<Type, (IEdmPrimitiveType Geometry, IEdmPrimitiveType Geography)> _spatialTypesMapping =
            new[]
            {
                // NTS
                new KeyValuePair<Type, (IEdmPrimitiveType, IEdmPrimitiveType)>(
                    typeof(Point),
                    (GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint), GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint))),
                new KeyValuePair<Type, (IEdmPrimitiveType, IEdmPrimitiveType)>(
                    typeof(LineString),
                    (GetPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString), GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString))),
            }
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public bool TryGetEdmPrimitiveType(Type clrType, out IEdmPrimitiveType primitiveType)
        {
            (IEdmPrimitiveType Geometry, IEdmPrimitiveType Geography) primitiveTypePair;
            if (_spatialTypesMapping.TryGetValue(clrType, out primitiveTypePair))
            {
                primitiveType = primitiveTypePair.Geometry;
                return true;
            }

            primitiveType = null;
            return false;
        }

        public bool TryGetEdmPrimitiveType(TypeMappingContext typeMappingContext, out IEdmPrimitiveType primitiveType)
        {
            if (typeMappingContext.Member == null)
            {
                return TryGetEdmPrimitiveType(typeMappingContext.ClrType, out primitiveType);
            }

            (IEdmPrimitiveType Geometry, IEdmPrimitiveType Geography) primitiveTypePair;
            if (!_spatialTypesMapping.TryGetValue(typeMappingContext.ClrType, out primitiveTypePair))
            {
                primitiveType = null;
                return false;
            }

            switch (typeMappingContext.Member.MemberType)
            {
                case MemberTypes.Property:
                    {
                        PropertyInfo propertyInfo = (PropertyInfo)typeMappingContext.Member;
                        if (propertyInfo.GetCustomAttribute<GeographyAttribute>(inherit: true) != null
                            || propertyInfo.DeclaringType.GetCustomAttribute<GeographyAttribute>(inherit: true) != null)
                        {
                            primitiveType = primitiveTypePair.Geography;
                        }
                        else
                        {
                            primitiveType = primitiveTypePair.Geometry;
                        }

                        return true;
                    }
                default:
                    // TODO: Use resource manager for resources
                    throw new InvalidOperationException("Unexpected member info");
            }
        }

        public bool TryGetClrType(IEdmTypeReference edmTypeReference, out Type clrType)
        {
            clrType = null;

            if (edmTypeReference == null || !edmTypeReference.IsPrimitive())
            {
                return false;
            }

            foreach (KeyValuePair<Type, (IEdmPrimitiveType Geometry, IEdmPrimitiveType Geography)> kvp in _spatialTypesMapping)
            {
                if ((edmTypeReference.Definition.IsEquivalentTo(kvp.Value.Geometry) || edmTypeReference.Definition.IsEquivalentTo(kvp.Value.Geography))
                    && (!edmTypeReference.IsNullable || IsNullable(kvp.Key)))
                {
                    clrType = kvp.Key;
                    return true;
                }
            }

            return false;
        }

        private static IEdmPrimitiveType GetPrimitiveType(EdmPrimitiveTypeKind primitiveKind)
        {
            return _coreModel.GetPrimitiveType(primitiveKind);
        }

        private static bool IsNullable(Type type)
        {
            Debug.Assert(type != null, "Type should not be null.");

            if (!type.IsValueType)
            {
                return true; // Reference types are nullable
            }

            return Nullable.GetUnderlyingType(type) != null; // Nullable value types
        }
    }
}
