using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Services.SboServiceLayer;

/// <summary>
/// Represents a single request to be sent in a batch to the Service Layer.
/// </summary>
public interface ISboServiceLayerBatchRequest
{
    /// <summary>
    /// Gets or sets the HTTP method to be used for this request.
    /// </summary>
    HttpMethod HttpMethod { get; set; }
    /// <summary>
    /// Gets or sets the Service Layer resource to be requested.
    /// </summary>
    string Resource { get; set; }
    /// <summary>
    /// Gets or sets the JSON body to be sent. It can be either an object to be serialized as JSON or a JSON string.
    /// </summary>
    object? Data { get; set; }
    /// <summary>
    /// Gets or sets the Content-ID for this request, an entity reference that can be used by subsequent requests to refer to a new entity created within the same change set.
    /// This is optional for OData Version 3 (b1s/v1) but mandatory for OData Version 4 (b1s/v2).
    /// </summary>
    int? ContentID { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> to be used for this request. UTF8 will be used by default.
    /// </summary>
    Encoding Encoding { get; set; }
    /// <summary>
    /// Gets or sets the HTTP message version to be used for this request. Version 1.1 will be used by default.
    /// </summary>
    Version HttpVersion { get; set; }
    /// <summary>
    /// Enables a case-insensitive query.
    /// </summary>
    /// <remarks>
    /// This is only applicable to SAP HANA databases, where every query is case-sensitive by default.
    /// </remarks>
    ISboServiceLayerBatchRequest WithCaseInsensitive();
    /// <summary>
    /// Allows a PATCH request to remove items in a collection.
    /// </summary>
    ISboServiceLayerBatchRequest WithReplaceCollectionsOnPatch();
    /// <summary>
    /// Configures a POST request to not return the created entity.
    /// This is suitable for better performance in demanding scenarios where the return content is not needed.
    /// </summary>
    /// <remarks>
    /// On success, <see cref="HttpStatusCode.NoContent"/> is returned, instead of <see cref="HttpStatusCode.Created"/>.
    /// </remarks>
    ISboServiceLayerBatchRequest WithReturnNoContent();
    /// <summary>
    /// Adds a custom request header to be sent.
    /// </summary>
    /// <param name="name">
    /// The name of the header.
    /// </param>
    /// <param name="value">
    /// The value of the header.
    /// </param>
    ISboServiceLayerBatchRequest WithHeader(string name, string value);
}
