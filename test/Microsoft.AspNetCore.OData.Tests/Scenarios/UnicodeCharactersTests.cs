//-----------------------------------------------------------------------------
// <copyright file="UnicodeCharactersTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.TestCommon;
using Microsoft.OData.ModelBuilder;
using Xunit;

namespace Microsoft.AspNetCore.OData.Tests.Scenarios;

public class UnicodeCharactersTest
{
    [Fact]
    public async Task PostEntity_WithUnicodeCharactersInKey()
    {
        // Arrange
        const string payload = "{" +
            "\"LogonName\":\"Ärne Bjørn\"," +
            "\"Email\":\"ärnebjørn@test.com\"" +
            "}";

        const string uri = "http://localhost/odata/UnicodeCharUsers";
        const string expectedLocation = "http://localhost/odata/UnicodeCharUsers('%C3%84rne%20Bj%C3%B8rn')";

        HttpClient client = GetClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = new StringContentWithLength(payload);
        request.Content.Headers.ContentType = MediaTypeWithQualityHeaderValue.Parse("application/json");

        // Act
        HttpResponseMessage response = await client.SendAsync(request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(expectedLocation, response.Headers.Location.AbsoluteUri);
    }

    [Fact]
    public async Task PostEntity_WithUnicodeCharactersInKey_PreferNoContent()
    {
        // Arrange
        const string payload = "{" +
            "\"LogonName\":\"Ärne Bjørn\"," +
            "\"Email\":\"ärnebjørn@test.com\"" +
            "}";

        const string uri = "http://localhost/odata/UnicodeCharUsers";
        const string preferNoContent = "return=minimal";
        const string expectedLocation = "http://localhost/odata/UnicodeCharUsers('%C3%84rne%20Bj%C3%B8rn')";

        HttpClient client = GetClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.TryAddWithoutValidation("Prefer", preferNoContent);

        request.Content = new StringContentWithLength(payload);
        request.Content.Headers.ContentType = MediaTypeWithQualityHeaderValue.Parse("application/json");

        // Act
        HttpResponseMessage response = await client.SendAsync(request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(expectedLocation, response.Headers.Location.AbsoluteUri);
        IEnumerable<string> values;
        Assert.True(response.Headers.TryGetValues("OData-EntityId", out values));
        Assert.Single(values);
        Assert.Equal(expectedLocation, values.First());
    }

    private static HttpClient GetClient()
    {
        ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<UnicodeCharUser>("UnicodeCharUsers");

        var controllers = new[] { typeof(UnicodeCharUsersController) };
        var server = TestServerUtils.Create(opt =>
        {
            opt.Select().OrderBy().Filter().Expand().Count().SetMaxTop(null);
            opt.AddRouteComponents("odata", modelBuilder.GetEdmModel());
        },
        controllers);

        return server.CreateClient();
    }
}

public class UnicodeCharUsersController : ODataController
{
    public IActionResult Post([FromBody] UnicodeCharUser user)
    {
        return Created(user);
    }
}

public class UnicodeCharUser
{
    [Key]
    public string LogonName { get; set; }
    public string Email { get; set; }
}
