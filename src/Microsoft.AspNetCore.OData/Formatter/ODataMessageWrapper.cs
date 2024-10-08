//-----------------------------------------------------------------------------
// <copyright file="ODataMessageWrapper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.OData;

namespace Microsoft.AspNetCore.OData.Formatter;

/// <summary>
/// Wrapper for IODataRequestMessage and IODataResponseMessage.
/// </summary>
internal class ODataMessageWrapper : IODataRequestMessageAsync, IODataResponseMessageAsync, IODataPayloadUriConverter, IServiceCollectionProvider,  IDisposable
{
    private Stream _stream;
    private Dictionary<string, string> _headers;
    private IDictionary<string, string> _contentIdMapping;
    private static readonly Regex ContentIdReferencePattern = new Regex(@"\$\d", RegexOptions.Compiled);

    public ODataMessageWrapper()
        : this(stream: null, headers: null)
    {
    }

    public ODataMessageWrapper(Stream stream)
        : this(stream: stream, headers: null)
    {
    }

    public ODataMessageWrapper(Stream stream, Dictionary<string, string> headers)
        : this(stream: stream, headers: headers, contentIdMapping: null)
    {
    }

    public ODataMessageWrapper(Stream stream, Dictionary<string, string> headers, IDictionary<string, string> contentIdMapping)
    {
        _stream = stream;
        if (headers != null)
        {
            _headers = headers;
        }
        else
        {
            _headers = new Dictionary<string, string>();
        }
        _contentIdMapping = contentIdMapping ?? new Dictionary<string, string>();
    }

    public IEnumerable<KeyValuePair<string, string>> Headers
    {
        get
        {
            return _headers;
        }
    }

    public string Method
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public Uri Url
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public int StatusCode
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public IServiceProvider ServiceProvider { get; set; }

    public string GetHeader(string headerName)
    {
        string value;
        if (_headers.TryGetValue(headerName, out value))
        {
            return value;
        }

        // try case-insensitive
        foreach (string key in _headers.Keys)
        {
            if (key.Equals(headerName, StringComparison.OrdinalIgnoreCase))
            {
                return _headers[key];
            }
        }

        return null;
    }

    public Stream GetStream()
    {
        return _stream;
    }

    public Task<Stream> GetStreamAsync()
    {
        TaskCompletionSource<Stream> taskCompletionSource = new TaskCompletionSource<Stream>();
        taskCompletionSource.SetResult(_stream);
        return taskCompletionSource.Task;
    }

    public void SetHeader(string headerName, string headerValue)
    {
        _headers[headerName] = headerValue;
    }

    public Uri ConvertPayloadUri(Uri baseUri, Uri payloadUri)
    {
        if (payloadUri == null)
        {
            throw new ArgumentNullException(nameof(payloadUri));
        }

        string originalPayloadUri = payloadUri.OriginalString;
        if (ContentIdReferencePattern.IsMatch(originalPayloadUri))
        {
            string resolvedUri = ContentIdHelpers.ResolveContentId(originalPayloadUri, _contentIdMapping);
            return new Uri(resolvedUri, UriKind.RelativeOrAbsolute);
        }

        // Returning null for default resolution.
        return null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <inheritdoc/>
    protected void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_stream != null)
            {
                _stream.Dispose();
            }
        }
    }
}
