//-----------------------------------------------------------------------------
// <copyright file="HttpClientExtensions.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.OData.TestCommon;

public static class HttpClientExtensions
{
    /// <summary>
    /// Sends a Patch request as an asynchronous operation to the specified Uri with the given <paramref name="content"/> serialized
    /// as JSON.
    /// </summary>
    /// <remarks>
    /// This method uses a default instance of <see cref="JsonMediaTypeFormatter"/>.
    /// </remarks>
    /// <param name="client">The client used to make the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The value that will be placed in the request's entity body.</param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri, string content)
    {
        return client.PatchAsync(requestUri, content, CancellationToken.None);
    }

    /// <summary>
    /// Sends a Patch request as an asynchronous operation to the specified Uri with the given <paramref name="content"/> serialized
    /// as JSON.
    /// </summary>
    /// <remarks>
    /// This method uses a default instance of <see cref="JsonMediaTypeFormatter"/>.
    /// </remarks>
    /// <param name="client">The client used to make the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The value that will be placed in the request's entity body.</param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, string content)
    {
        return client.PatchAsync(new Uri(requestUri, UriKind.RelativeOrAbsolute), content, CancellationToken.None);
    }

    /// <summary>
    /// Sends a Patch request as an asynchronous operation to the specified Uri with the given <paramref name="value"/> serialized
    /// as JSON.
    /// </summary>
    /// <remarks>
    /// This method uses a default instance of <see cref="JsonMediaTypeFormatter"/>.
    /// </remarks>
    /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
    /// <param name="client">The client used to make the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value that will be placed in the request's entity body.</param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "We want to support URIs as strings")]
    public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
    {
        return client.PatchAsJsonAsync(new Uri(requestUri, UriKind.RelativeOrAbsolute), value, CancellationToken.None);
    }

    /// <summary>
    /// Sends a Patch request as an asynchronous operation to the specified Uri with the given <paramref name="value"/> serialized
    /// as JSON.
    /// </summary>
    /// <remarks>
    /// This method uses a default instance of <see cref="JsonMediaTypeFormatter"/>.
    /// </remarks>
    /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
    /// <param name="client">The client used to make the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value that will be placed in the request's entity body.</param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, Uri requestUri, T value)
    {
        return client.PatchAsJsonAsync(requestUri, value, CancellationToken.None);
    }

    /// <summary>
    /// Sends a Patch request as an asynchronous operation to the specified Uri with the given <paramref name="value"/> serialized
    /// as JSON.
    /// </summary>
    /// <remarks>
    /// This method uses a default instance of <see cref="JsonMediaTypeFormatter"/>.
    /// </remarks>
    /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
    /// <param name="client">The client used to make the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value that will be placed in the request's entity body.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, Uri requestUri, T value, CancellationToken cancellationToken)
    {
        string content = JsonConvert.SerializeObject(value);
        return client.PatchAsync(requestUri, content, cancellationToken);
    }

    /// <summary>
    /// Sends a Patch request as an asynchronous operation to the specified Uri with the given <paramref name="content"/> serialized
    /// as JSON.
    /// </summary>
    /// <remarks>
    /// This method uses a default instance of <see cref="JsonMediaTypeFormatter"/>.
    /// </remarks>
    /// <typeparam name="T">The type of <paramref name="content"/>.</typeparam>
    /// <param name="client">The client used to make the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The value that will be placed in the request's entity body.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri, string content, CancellationToken cancellationToken)
    {
        HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri);
        request.Content = new StringContent(content);
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        return client.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request as an asynchronous operation to the specified Uri with the given <paramref name="value"/> serialized
    /// as JSON.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
    /// <param name="client">The client used to make the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value that will be placed in the request's entity body.</param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "We want to support URIs as strings")]
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
    {
        return client.PostAsJsonAsync(new Uri(requestUri, UriKind.RelativeOrAbsolute), value);
    }

    /// <summary>
    /// Sends a POST request as an asynchronous operation to the specified Uri with the given <paramref name="value"/> serialized
    /// as JSON.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
    /// <param name="client">The client used to make the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value that will be placed in the request's entity body.</param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, Uri requestUri, T value)
    {
        string content = JsonConvert.SerializeObject(value);
        HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), requestUri);
        request.Content = new StringContent(content);
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        request.Content.Headers.ContentLength = content.Length;
        return client.SendAsync(request, CancellationToken.None);
    }

    /// <summary>
    /// Sends a PUT request as an asynchronous operation to the specified Uri with the given <paramref name="value"/> serialized
    /// as JSON.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
    /// <param name="client">The client used to make the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value that will be placed in the request's entity body.</param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "We want to support URIs as strings")]
    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
    {
        return client.PutAsJsonAsync(new Uri(requestUri, UriKind.RelativeOrAbsolute), value);
    }

    /// <summary>
    /// Sends a PUT request as an asynchronous operation to the specified Uri with the given <paramref name="value"/> serialized
    /// as JSON.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
    /// <param name="client">The client used to make the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value that will be placed in the request's entity body.</param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, Uri requestUri, T value)
    {
        string content = JsonConvert.SerializeObject(value);
        HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PUT"), requestUri);
        request.Content = new StringContent(content);
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        request.Content.Headers.ContentLength = content.Length;
        return client.SendAsync(request, CancellationToken.None);
    }

    public static async Task<HttpResponseMessage> GetWithAcceptAsync(this HttpClient client, string uri, string acceptHeader)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Clear();
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(acceptHeader));

        return await client.SendAsync(request);
    }

    public static async Task<HttpResponseMessage> GetWithAcceptAsync(this HttpClient client, Uri uri, string acceptHeader)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Clear();
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(acceptHeader));

        return await client.SendAsync(request);
    }
}
