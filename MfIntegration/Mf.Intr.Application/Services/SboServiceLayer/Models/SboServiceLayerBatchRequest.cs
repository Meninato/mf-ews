using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Models;

/// <summary>
/// Represents a single request to be sent in a batch to the Service Layer.
/// </summary>
public class SboServiceLayerBatchRequest : ISboServiceLayerBatchRequest
{
    /// <summary>
    /// Gets or sets the HTTP method to be used for this request.
    /// </summary>
    public HttpMethod HttpMethod { get; set; }
    /// <summary>
    /// Gets or sets the Service Layer resource to be requested.
    /// </summary>
    public string Resource { get; set; }
    /// <summary>
    /// Gets or sets the JSON body to be sent. It can be either an object to be serialized as JSON or a JSON string.
    /// </summary>
    public object? Data { get; set; }
    /// <summary>
    /// Gets or sets the Content-ID for this request, an entity reference that can be used by subsequent requests to refer to a new entity created within the same change set.
    /// This is optional for OData Version 3 (b1s/v1) but mandatory for OData Version 4 (b1s/v2).
    /// </summary>
    public int? ContentID { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> to be used for this request. UTF8 will be used by default.
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;
    /// <summary>
    /// Gets or sets the <see cref="JsonSerializerOptions"/> to be used for this request.
    /// By default it is configured so the <see cref="JsonSerializerOptions.DefaultIgnoreCondition"/> is set to <see cref="JsonIgnoreCondition.WhenWritingNull"/>.
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = AppDefaults.DefaultJsonSerializerOptions;
    /// <summary>
    /// Gets or sets the HTTP message version to be used for this request. Version 1.1 will be used by default.
    /// </summary>
    public Version HttpVersion { get; set; } = new Version(1, 1);
    /// <summary>
    /// The HTTP headers to be sent in this request.
    /// </summary>
    internal HttpRequestHeaders Headers { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SboServiceLayerBatchRequest"/> class, which represents the details of a request to be sent in a batch.
    /// </summary>
    /// <param name="httpMethod">
    /// The HTTP method to be used for this request.
    /// </param>
    /// <param name="resource">
    /// The Service Layer resource to be requested.
    /// </param>
    /// <param name="data">
    /// The JSON body to be sent. It can be either an object to be serialized as JSON or a JSON string.
    /// </param>
    /// <param name="contentID">
    /// Entity reference that can be used by subsequent requests to refer to a new entity created within the same change set.
    /// This is optional for OData Version 3 (b1s/v1) but mandatory for OData Version 4 (b1s/v2).
    /// </param>
    public SboServiceLayerBatchRequest(HttpMethod httpMethod, string resource, object? data = null, int? contentID = null)
    {
        HttpMethod = httpMethod;
        Resource = resource;
        Data = data;
        ContentID = contentID;

        using (var message = new HttpRequestMessage())
        {
            Headers = message.Headers;
        }
    }

    /// <summary>
    /// Enables a case-insensitive query.
    /// </summary>
    /// <remarks>
    /// This is only applicable to SAP HANA databases, where every query is case-sensitive by default.
    /// </remarks>
    public ISboServiceLayerBatchRequest WithCaseInsensitive()
    {
        this.Headers.Add("B1S-CaseInsensitive", "true");
        return this;
    }

    /// <summary>
    /// Allows a PATCH request to remove items in a collection.
    /// </summary>
    public ISboServiceLayerBatchRequest WithReplaceCollectionsOnPatch()
    {
        this.Headers.Add("B1S-ReplaceCollectionsOnPatch", "true");
        return this;
    }

    /// <summary>
    /// Configures a POST request to not return the created entity.
    /// This is suitable for better performance in demanding scenarios where the return content is not needed.
    /// </summary>
    /// <remarks>
    /// On success, <see cref="HttpStatusCode.NoContent"/> is returned, instead of <see cref="HttpStatusCode.Created"/>.
    /// </remarks>
    public ISboServiceLayerBatchRequest WithReturnNoContent()
    {
        this.Headers.Add("Prefer", "return-no-content");
        return this;
    }

    /// <summary>
    /// Adds a custom request header to be sent.
    /// </summary>
    /// <param name="name">
    /// The name of the header.
    /// </param>
    /// <param name="value">
    /// The value of the header.
    /// </param>
    public ISboServiceLayerBatchRequest WithHeader(string name, string value)
    {
        this.Headers.Add(name, value);
        return this;
    }
}