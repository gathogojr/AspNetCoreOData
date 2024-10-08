//-----------------------------------------------------------------------------
// <copyright file="DefaultODataETagHandler.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.OData.Abstracts;
using Microsoft.Net.Http.Headers;
using Microsoft.OData;

namespace Microsoft.AspNetCore.OData.Formatter;

internal class DefaultODataETagHandler : IETagHandler
{
    /// <summary>null literal that needs to be return in ETag value when the value is null</summary>
    private const string NullLiteralInETag = "null";

    private const char Separator = ',';

    public EntityTagHeaderValue CreateETag(IDictionary<string, object> properties, TimeZoneInfo timeZoneInfo = null)
    {
        if (properties == null)
        {
            throw Error.ArgumentNull(nameof(properties));
        }

        if (properties.Count == 0)
        {
            return null;
        }

        StringBuilder builder = new StringBuilder();
        builder.Append('\"');
        bool firstProperty = true;

        foreach (object propertyValue in properties.Values)
        {
            if (firstProperty)
            {
                firstProperty = false;
            }
            else
            {
                builder.Append(Separator);
            }

            string str = propertyValue == null
                ? NullLiteralInETag
                : ConventionsHelpers.GetUriRepresentationForValue(propertyValue, timeZoneInfo);

            // base64 encode
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            string etagValueText = Convert.ToBase64String(bytes);
            builder.Append(etagValueText);
        }

        builder.Append('\"');
        string tag = builder.ToString();
        return new EntityTagHeaderValue(tag, isWeak: true);
    }

    public IDictionary<string, object> ParseETag(EntityTagHeaderValue etagHeaderValue)
    {
        if (etagHeaderValue == null)
        {
            throw Error.ArgumentNull(nameof(etagHeaderValue));
        }

        string tag = etagHeaderValue.Tag.ToString().Trim('\"');

        // split etag
        string[] rawValues = tag.Split(Separator);
        IDictionary<string, object> properties = new Dictionary<string, object>();
        for (int index = 0; index < rawValues.Length; index++)
        {
            string rawValue = rawValues[index];

            // base64 decode
            byte[] bytes = Convert.FromBase64String(rawValue);
            string valueString = Encoding.UTF8.GetString(bytes);
            object obj = ODataUriUtils.ConvertFromUriLiteral(valueString, ODataVersion.V4);
            if (obj is ODataNullValue)
            {
                obj = null;
            }
            properties.Add(index.ToString(CultureInfo.InvariantCulture), obj);
        }

        return properties;
    }
}
