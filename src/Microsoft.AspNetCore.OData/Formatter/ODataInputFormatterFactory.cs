//-----------------------------------------------------------------------------
// <copyright file="ODataInputFormatterFactory.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.OData.Formatter.MediaType;
using Microsoft.OData;

namespace Microsoft.AspNetCore.OData.Formatter;

/// <summary>
/// Factory for <see cref="ODataInputFormatter"/> classes to handle OData.
/// </summary>
public static class ODataInputFormatterFactory
{
    /// <summary>
    /// Creates a list of media type formatters to handle OData deserialization.
    /// </summary>
    /// <returns>A list of media type formatters to handle OData.</returns>
    public static IList<ODataInputFormatter> Create()
    {
        return new List<ODataInputFormatter>()
        {
            // Place JSON formatter first so it gets used when the request doesn't ask for a specific content type
            CreateApplicationJson(),
            CreateApplicationXml(),
            CreateRawValue()
        };
    }

    private static ODataInputFormatter CreateApplicationJson()
    {
        ODataInputFormatter formatter = CreateFormatterWithoutMediaTypes(
            ODataPayloadKind.ResourceSet,
            ODataPayloadKind.Resource,
            ODataPayloadKind.Property,
            ODataPayloadKind.EntityReferenceLink,
            ODataPayloadKind.EntityReferenceLinks,
            ODataPayloadKind.Collection,
            ODataPayloadKind.ServiceDocument,
            ODataPayloadKind.Error,
            ODataPayloadKind.Parameter,
            ODataPayloadKind.Delta);

        // Add minimal metadata as the first media type so it gets used when the request doesn't
        // ask for a specific content type
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataMinimalMetadataStreamingTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataMinimalMetadataStreamingFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataMinimalMetadata);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataFullMetadataStreamingTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataFullMetadataStreamingFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataFullMetadata);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataNoMetadataStreamingTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataNoMetadataStreamingFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataNoMetadata);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonStreamingTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonStreamingFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJson);
        // NOTE: The order in which the media types are added is relevant due to how ASP.NET Core handles content negotiation
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataMinimalMetadataStreamingTrueIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataMinimalMetadataStreamingTrueIeee754CompatibleTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataMinimalMetadataStreamingFalseIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataMinimalMetadataStreamingFalseIeee754CompatibleTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataMinimalMetadataIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataMinimalMetadataIeee754CompatibleTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataFullMetadataStreamingTrueIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataFullMetadataStreamingTrueIeee754CompatibleTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataFullMetadataStreamingFalseIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataFullMetadataStreamingFalseIeee754CompatibleTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataFullMetadataIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataFullMetadataIeee754CompatibleTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataNoMetadataStreamingTrueIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataNoMetadataStreamingTrueIeee754CompatibleTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataNoMetadataStreamingFalseIeee754CompatibleTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataNoMetadataStreamingFalseIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataNoMetadataIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonODataNoMetadataIeee754CompatibleTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonStreamingTrueIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonStreamingTrueIeee754CompatibleTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonStreamingFalseIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonStreamingFalseIeee754CompatibleTrue);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonIeee754CompatibleFalse);
        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationJsonIeee754CompatibleTrue);

        return formatter;
    }

    private static ODataInputFormatter CreateApplicationXml()
    {
        ODataInputFormatter formatter = CreateFormatterWithoutMediaTypes(
            ODataPayloadKind.MetadataDocument);

        formatter.SupportedMediaTypes.Add(ODataMediaTypes.ApplicationXml);

        return formatter;
    }

    private static ODataInputFormatter CreateRawValue()
    {
        ODataInputFormatter formatter = CreateFormatterWithoutMediaTypes(ODataPayloadKind.Value);

        formatter.SupportedMediaTypes.Add("text/plain");
        return formatter;
    }

    private static ODataInputFormatter CreateFormatterWithoutMediaTypes(params ODataPayloadKind[] payloadKinds)
    {
        ODataInputFormatter formatter = new ODataInputFormatter(payloadKinds);
        AddSupportedEncodings(formatter);
        return formatter;
    }

    private static void AddSupportedEncodings(ODataInputFormatter formatter)
    {
        formatter.SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: true));

        formatter.SupportedEncodings.Add(new UnicodeEncoding(bigEndian: false, byteOrderMark: true,
            throwOnInvalidBytes: true));
    }
}
