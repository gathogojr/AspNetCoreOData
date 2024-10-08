//-----------------------------------------------------------------------------
// <copyright file="ODataValidationSettingsTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;
using Microsoft.AspNetCore.OData.Tests.Commons;
using Xunit;

namespace Microsoft.AspNetCore.OData.Tests.Query;

public class ODataValidationSettingsTest
{
    [Fact]
    public void Ctor_Initializes_All_Properties()
    {
        // Arrange & Act
        ODataValidationSettings querySettings = new ODataValidationSettings();

        // Assert
        Assert.Equal(AllowedArithmeticOperators.All, querySettings.AllowedArithmeticOperators);
        Assert.Equal(AllowedFunctions.AllFunctions, querySettings.AllowedFunctions);
        Assert.Equal(AllowedLogicalOperators.All, querySettings.AllowedLogicalOperators);
        Assert.Empty(querySettings.AllowedOrderByProperties);
        Assert.Equal(AllowedQueryOptions.Supported, querySettings.AllowedQueryOptions);
        Assert.Equal(1, querySettings.MaxAnyAllExpressionDepth);
        Assert.Equal(100, querySettings.MaxNodeCount);
        Assert.Null(querySettings.MaxSkip);
        Assert.Null(querySettings.MaxTop);
    }

    [Fact]
    public void AllowedFunctions_Property_RoundTrips()
    {
        ReflectionAssert.EnumProperty<ODataValidationSettings, AllowedFunctions>(
            new ODataValidationSettings(),
            o => o.AllowedFunctions,
            expectedDefaultValue: AllowedFunctions.AllFunctions,
            illegalValue: AllowedFunctions.AllFunctions + 1,
            roundTripTestValue: AllowedFunctions.AllMathFunctions);
    }

    [Fact]
    public void AllowedFunctions_SetToAllFunctions_DoesNotThrow()
    {
        ExceptionAssert.DoesNotThrow(() => new ODataValidationSettings().AllowedFunctions = AllowedFunctions.AllFunctions);
    }

    [Fact]
    public void AllowedArithmeticOperators_Property_RoundTrips()
    {
        ReflectionAssert.EnumProperty<ODataValidationSettings, AllowedArithmeticOperators>(
            new ODataValidationSettings(),
            o => o.AllowedArithmeticOperators,
            expectedDefaultValue: AllowedArithmeticOperators.All,
            illegalValue: AllowedArithmeticOperators.All + 1,
            roundTripTestValue: AllowedArithmeticOperators.Multiply);
    }

    [Fact]
    public void AllowedLogicalOperators_Property_RoundTrips()
    {
        ReflectionAssert.EnumProperty<ODataValidationSettings, AllowedLogicalOperators>(
            new ODataValidationSettings(),
            o => o.AllowedLogicalOperators,
            expectedDefaultValue: AllowedLogicalOperators.All,
            illegalValue: AllowedLogicalOperators.All + 1,
            roundTripTestValue: AllowedLogicalOperators.GreaterThanOrEqual | AllowedLogicalOperators.LessThanOrEqual);
    }

    [Fact]
    public void AllowedQueryOptions_Property_RoundTrips()
    {
        ReflectionAssert.EnumProperty<ODataValidationSettings, AllowedQueryOptions>(
            new ODataValidationSettings(),
            o => o.AllowedQueryOptions,
            expectedDefaultValue: AllowedQueryOptions.Supported,
            illegalValue: AllowedQueryOptions.All + 1,
            roundTripTestValue: AllowedQueryOptions.Filter);
    }

    [Fact]
    public void AllowedOrderByProperties_Property_RoundTrips()
    {
        ODataValidationSettings settings = new ODataValidationSettings();
        Assert.NotNull(settings.AllowedOrderByProperties);
        Assert.Empty(settings.AllowedOrderByProperties);

        settings.AllowedOrderByProperties.Add("Id");
        settings.AllowedOrderByProperties.Add("Name");

        Assert.Equal(2, settings.AllowedOrderByProperties.Count);
        Assert.Contains("Id", settings.AllowedOrderByProperties);
        Assert.Contains("Name", settings.AllowedOrderByProperties);
    }

    [Fact]
    public void MaxAnyAllExpressionDepth_Property_RoundTrips()
    {
        ReflectionAssert.IntegerProperty<ODataValidationSettings, int>(
            new ODataValidationSettings(),
            o => o.MaxAnyAllExpressionDepth,
            expectedDefaultValue: 1,
            minLegalValue: 1,
            maxLegalValue: int.MaxValue,
            illegalLowerValue: 0,
            illegalUpperValue: null,
            roundTripTestValue: 2);
    }

    [Fact]
    public void MaxNodeCount_Property_RoundTrips()
    {
        ReflectionAssert.IntegerProperty<ODataValidationSettings, int>(
            new ODataValidationSettings(),
            o => o.MaxNodeCount,
            expectedDefaultValue: 100,
            minLegalValue: 1,
            maxLegalValue: int.MaxValue,
            illegalLowerValue: 0,
            illegalUpperValue: null,
            roundTripTestValue: 2);
    }

    [Fact]
    public void MaxTop_Property_RoundTrips()
    {
        ReflectionAssert.NullableIntegerProperty<ODataValidationSettings, int>(
            new ODataValidationSettings(),
            o => o.MaxTop,
            expectedDefaultValue: null,
            minLegalValue: 0,
            maxLegalValue: int.MaxValue,
            illegalLowerValue: -1,
            illegalUpperValue: null,
            roundTripTestValue: 2);
    }

    [Fact]
    public void MaxSkip_Property_RoundTrips()
    {
        ReflectionAssert.NullableIntegerProperty<ODataValidationSettings, int>(
            new ODataValidationSettings(),
            o => o.MaxSkip,
            expectedDefaultValue: null,
            minLegalValue: 0,
            maxLegalValue: int.MaxValue,
            illegalLowerValue: -1,
            illegalUpperValue: null,
            roundTripTestValue: 2);
    }

    [Fact]
    public void MaxExpansionDepth_Property_RoundTrips()
    {
        ReflectionAssert.IntegerProperty(
         new ODataValidationSettings(),
         o => o.MaxExpansionDepth,
         expectedDefaultValue: 2,
         minLegalValue: 0,
         maxLegalValue: int.MaxValue,
         illegalLowerValue: -1,
         illegalUpperValue: null,
         roundTripTestValue: 100);
    }

    [Fact]
    public void MaxOrderByNodeCount_Property_RoundTrips()
    {
        ReflectionAssert.IntegerProperty(
         new ODataValidationSettings(),
         o => o.MaxOrderByNodeCount,
         expectedDefaultValue: 5,
         minLegalValue: 1,
         maxLegalValue: int.MaxValue,
         illegalLowerValue: -1,
         illegalUpperValue: null,
         roundTripTestValue: 100);
    }
}
