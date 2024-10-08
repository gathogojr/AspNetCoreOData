//-----------------------------------------------------------------------------
// <copyright file="ODataPrimitiveDeserializerTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Formatter.Deserialization;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.AspNetCore.OData.TestCommon;
using Microsoft.AspNetCore.OData.Tests.Commons;
using Microsoft.AspNetCore.OData.Tests.Extensions;
using Microsoft.AspNetCore.OData.Tests.Models;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.OData.Tests.Formatter.Deserialization;

public class ODataPrimitiveDeserializerTests
{
    private IEdmPrimitiveTypeReference _edmIntType = EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int32, isNullable: false);
    public static TheoryDataSet<object, object> NonEdmPrimitiveData
    {
        get
        {
            return new TheoryDataSet<object, object>
            {
                { (char)'1', "1" },
                { (char[]) new char[] {'1'}, "1" },
                { (UInt16)1, (int)1 },
                { (UInt32)1, (long)1 },
                { (UInt64)1, (long)1 },
                //(Stream) new MemoryStream(new byte[] { 1 }), // TODO: Enable once we have support for streams
            };
        }
    }

    public static TheoryDataSet<object, string, string> EdmPrimitiveData
    {
        get
        {
            return new TheoryDataSet<object, string, string>
            {
                { "1", "Edm.String", "\"1\"" },
                { true, "Edm.Boolean", "true" },
                { (Byte)1, "Edm.Byte", "1" },
                { (Decimal)1, "Edm.Decimal", "1" },
                { (Double)1, "Edm.Double", "1.0" },
                { (Guid)Guid.Empty, "Edm.Guid", "\"00000000-0000-0000-0000-000000000000\"" },
                { (Int16)1, "Edm.Int16", "1" },
                { (Int32)1, "Edm.Int32", "1" },
                { (Int64)1, "Edm.Int64", "1" },
                { (SByte)1, "Edm.SByte", "1" },
                { (Single)1, "Edm.Single", "1" },
                { new byte[] { 1 }, "Edm.Binary", "\"AQ==\"" },
                { new TimeSpan(), "Edm.Duration", "\"PT0S\"" },
                { new DateTimeOffset(), "Edm.DateTimeOffset", "\"0001-01-01T00:00:00Z\"" },
                { new Date(2014, 10, 13), "Edm.Date", "\"2014-10-13\"" },
                { new TimeOfDay(15, 38, 25, 109), "Edm.TimeOfDay", "\"15:38:25.1090000\"" },
            };
        }
    }

    public static TheoryDataSet<DateTimeOffset, DateTime, TimeZoneInfo> DateTimePrimitiveData
    {
        get
        {
            DateTime dtUtc = new DateTime(2014, 10, 16, 1, 2, 3, DateTimeKind.Utc);
            DateTime dtLocal = new DateTime(2014, 10, 16, 1, 2, 3, DateTimeKind.Local);
            DateTime dtUnspecified = new DateTime(2014, 10, 16, 1, 2, 3, DateTimeKind.Unspecified);
            TimeZoneInfo pacificStandard = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            TimeZoneInfo chinaStandard = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            return new TheoryDataSet<DateTimeOffset, DateTime, TimeZoneInfo>
            {
                { DateTimeOffset.Parse("2014-10-16T01:02:03Z"), dtUtc, null },
                { new DateTimeOffset(dtLocal), dtLocal, null },
                { new DateTimeOffset(new DateTime(2014, 10, 16, 1, 2, 3, DateTimeKind.Unspecified)), dtUnspecified, null },
                { DateTimeOffset.Parse("2014-10-16T09:02:03+8:00"), dtUtc, chinaStandard },
                { new DateTimeOffset(dtLocal).ToOffset(new TimeSpan(+8,0,0)), dtLocal, chinaStandard },
                { DateTimeOffset.Parse("2014-10-16T01:02:03-7:00"), dtUnspecified, pacificStandard },
            };
        }
    }

    [Fact]
    public void ReadInline_ReturnsNull_IfItemIsNull()
    {
        IEdmPrimitiveTypeReference primitiveType = EdmCoreModel.Instance.GetInt32(isNullable: true);
        var deserializer = new ODataPrimitiveDeserializer();

        Assert.Null(deserializer.ReadInline(item: null, edmType: _edmIntType, readContext: new ODataDeserializerContext()));
    }

    [Fact]
    public void ReadInline_Throws_ArgumentMustBeOfType()
    {
        IEdmPrimitiveTypeReference primitiveType = EdmCoreModel.Instance.GetInt32(isNullable: true);
        var deserializer = new ODataPrimitiveDeserializer();

        ExceptionAssert.ThrowsArgument(
            () => deserializer.ReadInline(42, _edmIntType, new ODataDeserializerContext()),
            "item",
            "The argument must be of type 'ODataProperty'");
    }

    [Fact]
    public void ReadInline_Calls_ReadPrimitive()
    {
        // Arrange
        IEdmPrimitiveTypeReference primitiveType = EdmCoreModel.Instance.GetInt32(isNullable: true);
        Mock<ODataPrimitiveDeserializer> deserializer = new Mock<ODataPrimitiveDeserializer>();
        ODataProperty property = new ODataProperty();
        ODataDeserializerContext readContext = new ODataDeserializerContext();

        deserializer.Setup(d => d.ReadPrimitive(property, readContext)).Returns(42).Verifiable();

        // Act
        var result = deserializer.Object.ReadInline(property, primitiveType, readContext);

        // Assert
        deserializer.Verify();
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task ReadAsync_ThrowsArgumentNull_MessageReader()
    {
        var deserializer = new ODataPrimitiveDeserializer();

        await ExceptionAssert.ThrowsArgumentNullAsync(
            () => deserializer.ReadAsync(messageReader: null, type: typeof(int), readContext: new ODataDeserializerContext()),
            "messageReader");
    }

    [Fact]
    public void ReadPrimitive_ThrowsArgumentNull_PrimitiveProperty()
    {
        var deserializer = new ODataPrimitiveDeserializer();

        ExceptionAssert.ThrowsArgumentNull(
            () => deserializer.ReadPrimitive(primitiveProperty: null, readContext: new ODataDeserializerContext()),
            "primitiveProperty");
    }

    [Fact]
    public void ReadPrimitive_ThrowsArgumentNull_ReadContext()
    {
        var deserializer = new ODataPrimitiveDeserializer();
        ODataProperty property = new ODataProperty();

        ExceptionAssert.ThrowsArgumentNull(() => deserializer.ReadPrimitive(property, null), "readContext");
    }

    [Fact]
    public async Task ReadAsync_ThrowsArgumentNull_ReadContext()
    {
        var deserializer = new ODataPrimitiveDeserializer();
        await ExceptionAssert.ThrowsArgumentNullAsync(() => deserializer.ReadAsync(null, typeof(object), null), "messageReader");

        ODataMessageReader messageReader = ODataFormatterHelpers.GetMockODataMessageReader();
        await ExceptionAssert.ThrowsArgumentNullAsync(() => deserializer.ReadAsync(messageReader, typeof(object), null), "readContext");
    }

    [Theory]
    [MemberData(nameof(EdmPrimitiveData))]
    public async Task ReadAsync_PrimitiveWithTypeInContext(object obj, string edmType, string value)
    {
        // Arrange
        IEdmModel model = CreateModel();
        ODataPrimitiveSerializer serializer = new ODataPrimitiveSerializer();
        ODataPrimitiveDeserializer deserializer = new ODataPrimitiveDeserializer();

        MemoryStream stream = new MemoryStream();
        ODataMessageWrapper message = new ODataMessageWrapper(stream);

        ODataMessageWriterSettings settings = new ODataMessageWriterSettings
        {
            ODataUri = new ODataUri { ServiceRoot = new Uri("http://any/"), }
        };
        settings.SetContentType(ODataFormat.Json);

        Type type = obj == null ? typeof(int) : obj.GetType();

        ODataMessageWriter messageWriter = new ODataMessageWriter(message as IODataResponseMessage, settings, model);
        ODataMessageReader messageReader = new ODataMessageReader(message as IODataResponseMessage, new ODataMessageReaderSettings(), model);
        ODataSerializerContext writeContext = new ODataSerializerContext { RootElementName = "Property", Model = model };
        ODataDeserializerContext readContext = new ODataDeserializerContext { Model = model, ResourceType = type };

        await serializer.WriteObjectAsync(obj, type, messageWriter, writeContext);
        stream.Seek(0, SeekOrigin.Begin);

        // Act & Assert
        Assert.NotNull(edmType);
        Assert.NotNull(value);
        Assert.Equal(obj, await deserializer.ReadAsync(messageReader, type, readContext));
    }

    [Theory]
    [MemberData(nameof(EdmPrimitiveData))]
    public async Task ReadAsync_Primitive(object obj, string edmType, string value)
    {
        // Arrange
        IEdmModel model = CreateModel();
        ODataPrimitiveSerializer serializer = new ODataPrimitiveSerializer();
        ODataPrimitiveDeserializer deserializer = new ODataPrimitiveDeserializer();

        MemoryStream stream = new MemoryStream();
        ODataMessageWrapper message = new ODataMessageWrapper(stream);

        ODataMessageWriterSettings settings = new ODataMessageWriterSettings
        {
            ODataUri = new ODataUri { ServiceRoot = new Uri("http://any/"), }
        };
        settings.SetContentType(ODataFormat.Json);

        ODataMessageWriter messageWriter = new ODataMessageWriter(message as IODataResponseMessage, settings, model);
        ODataMessageReader messageReader = new ODataMessageReader(message as IODataResponseMessage, new ODataMessageReaderSettings(), model);
        ODataSerializerContext writeContext = new ODataSerializerContext { RootElementName = "Property", Model = model };
        ODataDeserializerContext readContext = new ODataDeserializerContext { Model = model };

        Type type = obj == null ? typeof(int) : obj.GetType();

        await serializer.WriteObjectAsync(obj, type, messageWriter, writeContext);
        stream.Seek(0, SeekOrigin.Begin);

        // Act & Assert
        Assert.NotNull(edmType);
        Assert.NotNull(value);
        Assert.Equal(obj, await deserializer.ReadAsync(messageReader, type, readContext));
    }

    [Theory]
    [InlineData("{\"value\":\"FgQF\"}", typeof(byte[]), new byte[] { 22, 4, 5 })]
    [InlineData("{\"value\":1}", typeof(Int16), (Int16)1)]
    [InlineData("{\"value\":1}", typeof(Int32), 1)]
    [InlineData("{\"value\":1}", typeof(Int64), (Int64)1)]
    [InlineData("{\"value\":\"true\"}", typeof(Boolean), true)]
    [InlineData("{\"value\":5}", typeof(SByte), (SByte)5)]
    [InlineData("{\"value\":201}", typeof(Byte), (Byte)201)]
    [InlineData("{\"value\":1.1}", typeof(Double), 1.1)]
    [InlineData("{\"value\":1.1}", typeof(Single), (Single)1.1)]
    public async Task ReadFromStreamAsync_RawPrimitive(string content, Type type, object expected)
    {
        // Arrange
        IEdmModel model = CreateModel();

        HttpRequest request = RequestFactory.Create("Patch", "http://localhost/OData/Suppliers(1)/Address", opt => opt.AddRouteComponents("odata", model));

        ODataPrimitiveDeserializer deserializer = new ODataPrimitiveDeserializer();
        ODataDeserializerContext readContext = new ODataDeserializerContext
        {
            Model = model,
            ResourceType = type
        };

        // Act
        object value = await deserializer.ReadAsync(ODataTestUtil.GetODataMessageReader(request.GetODataMessage(content), model), type, readContext);

        // Assert
        Assert.Equal(expected, value);
    }

    [Fact]
    public async Task ReadFromStreamAsync_RawGuid()
    {
        // Arrange
        string content = "{\"value\":\"f4b787c7-920d-4993-a584-ceb68968058c\"}";
        Type type = typeof(Guid);
        object expected = new Guid("f4b787c7-920d-4993-a584-ceb68968058c");

        IEdmModel model = CreateModel();
        HttpRequest request = RequestFactory.Create("Patch", "http://localhost/OData/Suppliers(1)/Address", opt => opt.AddRouteComponents("odata", model));

        ODataPrimitiveDeserializer deserializer = new ODataPrimitiveDeserializer();
        ODataDeserializerContext readContext = new ODataDeserializerContext
        {
            Model = model,
            ResourceType = type
        };

        // Act
        object value = await deserializer.ReadAsync(ODataTestUtil.GetODataMessageReader(request.GetODataMessage(content), model), type, readContext);

        // Assert
        Assert.Equal(expected, value);
    }

    [Theory]
    [MemberData(nameof(NonEdmPrimitiveData))]
    public async Task ReadAsync_MappedPrimitiveWithTypeinContext(object obj, object expected)
    {
        // Arrange
        IEdmModel model = CreateModel();
        ODataPrimitiveSerializer serializer = new ODataPrimitiveSerializer();
        ODataPrimitiveDeserializer deserializer = new ODataPrimitiveDeserializer();

        MemoryStream stream = new MemoryStream();
        ODataMessageWrapper message = new ODataMessageWrapper(stream);

        ODataMessageWriterSettings settings = new ODataMessageWriterSettings
        {
            ODataUri = new ODataUri { ServiceRoot = new Uri("http://any/"), }
        };
        settings.SetContentType(ODataFormat.Json);

        Type type = obj == null ? typeof(int) : expected.GetType();

        ODataMessageWriter messageWriter = new ODataMessageWriter(message as IODataResponseMessage, settings, model);
        ODataMessageReader messageReader = new ODataMessageReader(message as IODataResponseMessage, new ODataMessageReaderSettings(), model);
        ODataSerializerContext writeContext = new ODataSerializerContext { RootElementName = "Property", Model = model };
        ODataDeserializerContext readContext = new ODataDeserializerContext { Model = model, ResourceType = type };

        await serializer.WriteObjectAsync(obj, type, messageWriter, writeContext);
        stream.Seek(0, SeekOrigin.Begin);

        // Act && Assert
        Assert.Equal(expected, await deserializer.ReadAsync(messageReader, type, readContext));
    }

    [Theory]
    [MemberData(nameof(NonEdmPrimitiveData))]
    public async Task ReadAsync_MappedPrimitive(object obj, object expected)
    {
        // Arrange
        IEdmModel model = CreateModel();
        ODataPrimitiveSerializer serializer = new ODataPrimitiveSerializer();
        ODataPrimitiveDeserializer deserializer = new ODataPrimitiveDeserializer();

        MemoryStream stream = new MemoryStream();
        ODataMessageWrapper message = new ODataMessageWrapper(stream);

        ODataMessageWriterSettings settings = new ODataMessageWriterSettings
        {
            ODataUri = new ODataUri { ServiceRoot = new Uri("http://any/"), }
        };
        settings.SetContentType(ODataFormat.Json);

        ODataMessageWriter messageWriter = new ODataMessageWriter(message as IODataResponseMessage, settings, model);
        ODataMessageReader messageReader = new ODataMessageReader(message as IODataResponseMessage, new ODataMessageReaderSettings(), model);
        ODataSerializerContext writeContext = new ODataSerializerContext { RootElementName = "Property", Model = model };
        ODataDeserializerContext readContext = new ODataDeserializerContext { Model = model };

        Type type = obj == null ? typeof(int) : expected.GetType();

        await serializer.WriteObjectAsync(obj, type, messageWriter, writeContext);
        stream.Seek(0, SeekOrigin.Begin);

        // Act && Assert
        Assert.Equal(expected, await deserializer.ReadAsync(messageReader, type, readContext));
    }

    /*
    [Theory]
    [MemberData(nameof(DateTimePrimitiveData))]
    public void Read_DateTimePrimitive(DateTimeOffset expected, DateTime value, TimeZoneInfo timeZoneInfo)
    {
        // Arrange
        IEdmModel model = CreateModel();

        var config = RoutingConfigurationFactory.CreateWithRootContainer("OData");
        var request = RequestFactory.Create(config, "OData");
        if (timeZoneInfo != null)
        {
            config.SetTimeZoneInfo(timeZoneInfo);
        }
        else
        {
            config.SetTimeZoneInfo(TimeZoneInfo.Local);
        }

        ODataPrimitiveSerializer serializer = new ODataPrimitiveSerializer();
        ODataPrimitiveDeserializer deserializer = new ODataPrimitiveDeserializer();

        MemoryStream stream = new MemoryStream();
        ODataMessageWrapper message = new ODataMessageWrapper(stream);

        ODataMessageWriterSettings settings = new ODataMessageWriterSettings
        {
            ODataUri = new ODataUri { ServiceRoot = new Uri("http://any/"), }
        };
        settings.SetContentType(ODataFormat.Json);

        ODataMessageWriter messageWriter = new ODataMessageWriter(message as IODataResponseMessage, settings, model);
        ODataMessageReader messageReader = new ODataMessageReader(message as IODataResponseMessage, new ODataMessageReaderSettings(), model);
        ODataSerializerContext writeContext = new ODataSerializerContext { RootElementName = "Property", Model = model, Request = request };
        ODataDeserializerContext readContext = new ODataDeserializerContext { Model = model, Request = request };

        serializer.WriteObject(value, typeof(DateTimeOffset), messageWriter, writeContext);
        stream.Seek(0, SeekOrigin.Begin);

        // Act && Assert
        Assert.Equal(expected, deserializer.Read(messageReader, typeof(DateTimeOffset), readContext));
    }
    */
    private static IEdmModel CreateModel()
    {
        ODataModelBuilder builder = new ODataConventionModelBuilder();
        builder.EntitySet<Customer>("Customers");
        return builder.GetEdmModel();
    }
}
