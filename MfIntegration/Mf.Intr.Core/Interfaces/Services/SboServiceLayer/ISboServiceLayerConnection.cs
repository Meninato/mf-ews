using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Services.SboServiceLayer;

public interface ISboServiceLayerConnection
{
    #region Properties
    /// <summary>
    /// Gets the Service Layer root URI.
    /// </summary>
    Uri ServiceLayerRoot { get; }
    /// <summary>
    /// Gets the Company database (schema) to connect to.
    /// </summary>
    string CompanyDB { get; }
    /// <summary>
    /// Gets the username to be used for the Service Layer authentication.
    /// </summary>
    string UserName { get; }
    /// <summary>
    /// Gets the password for the provided username.
    /// </summary>
    string Password { get; }
    /// <summary>
    /// Gets the Service Layer language code provided.
    /// </summary>
    int? Language { get; }
    /// <summary>
    /// Gets or sets the number of attempts for each unsuccessfull request in case of an HTTP response code of 401, 500, 502, 503 or 504.
    /// </summary>
    int NumberOfAttempts { get; set; }
    /// <summary>
    /// Gets or sets the timespan to wait before a batch request times out. The default value is 5 minutes (300 seconds).
    /// </summary>
    TimeSpan BatchRequestTimeout { get; set; }
    /// <summary>
    /// Gets whether this instance is using Single Sign-On (SSO) authentication.
    /// </summary>
    bool IsUsingSingleSignOn { get; }
    /// <summary>
    /// Gets information about the latest Login request.
    /// </summary>
    ISboServiceLayerLoginResponse? LoginResponse { get; }
    #endregion

    #region Authentication methods
    /// <summary>
    /// If the current session is expired or non-existent, performs a POST Login request with the provided information.
    /// Manually performing the Login is often unnecessary because it will be performed automatically anyway whenever needed.
    /// </summary>
    /// <param name="forceLogin">
    /// Whether the login request should be forced even if the current session has not expired.
    /// </param>
    Task<ISboServiceLayerLoginResponse?> LoginAsync(bool forceLogin = false);
    /// <summary>
    /// Performs a POST Logout request, ending the current session.
    /// </summary>
    Task LogoutAsync();
    #endregion

    #region Request methods
    /// <summary>
    /// Initializes a new instance of the <see cref="ISboServiceLayerRequest"/> class that represents a request to the associated <see cref="ISboServiceLayerConnection"/>. 
    /// </summary>
    /// <param name="resource">
    /// The resource name to be requested.
    /// </param>
    ISboServiceLayerRequest Request(string resource);
    /// <summary>
    /// Initializes a new instance of the <see cref="ISboServiceLayerRequest"/> class that represents a request to the associated <see cref="ISboServiceLayerConnection"/>. 
    /// </summary>
    /// <param name="resource">
    /// The resource name to be requested.
    /// </param>
    /// <param name="id">
    /// The entity ID to be requested.
    /// </param>
    ISboServiceLayerRequest Request(string resource, object id)
    /// <summary>
    /// Provides a direct response from the Apache server that can be used for network testing and component monitoring.
    /// In response to a PING request, the Apache server (load balancer or node) will respond directly with a simple PONG response.
    /// </summary>
    /// <remarks>
    /// This feature is only available on version 9.3 PL10 or above. See SAP Note <see href="https://launchpad.support.sap.com/#/notes/2796799">2796799</see> for more details. 
    /// </remarks>
    /// <returns>
    /// A <see cref="ISboServiceLayerPingResponse"/> object containing the response details.
    /// </returns>
    Task<ISboServiceLayerPingResponse> PingAsync();
    /// <summary>
    /// Provides a direct response from the Apache server that can be used for network testing and component monitoring.
    /// In response to a PING request, the Apache server (load balancer or node) will respond directly with a simple PONG response.
    /// </summary>
    /// <remarks>
    /// This feature is only available on version 9.3 PL10 or above. See SAP Note <see href="https://launchpad.support.sap.com/#/notes/2796799">2796799</see> for more details. 
    /// </remarks>
    /// <param name="node">
    /// The specific node to be monitored. If not specified, the request will be directed to the load balancer.
    /// </param>
    /// <returns>
    /// A <see cref="ISboServiceLayerPingResponse"/> object containing the response details.
    /// </returns>
    Task<ISboServiceLayerPingResponse> PingNodeAsync(int? node = null);
    #endregion

    #region Attachments Methods
    /// <summary>
    /// Uploads the provided file as an attachment.
    /// </summary>
    /// <remarks>
    /// An attachment folder must be defined. See section 'Setting up an Attachment Folder' in the Service Layer User Manual for more details.
    /// </remarks>
    /// <param name="path">
    /// The path to the file to be uploaded.
    /// </param>
    /// <returns>
    /// A object with information about the created attachment entry.
    /// </returns>
    Task<ISboServiceLayerAttachment> PostAttachmentAsync(string path);
    /// <summary>
    /// Uploads the provided file as an attachment.
    /// </summary>
    /// <remarks>
    /// An attachment folder must be defined. See section 'Setting up an Attachment Folder' in the Service Layer User Manual for more details.
    /// </remarks>
    /// <param name="fileName">
    /// The file name of the file to be uploaded including the file extension.
    /// </param>
    /// <param name="file">
    /// The file to be uploaded.
    /// </param>
    /// <returns>
    /// A object with information about the created attachment entry.
    /// </returns>
    Task<ISboServiceLayerAttachment> PostAttachmentAsync(string fileName, byte[] file);
    /// <summary>
    /// Uploads the provided file as an attachment.
    /// </summary>
    /// <remarks>
    /// An attachment folder must be defined. See section 'Setting up an Attachment Folder' in the Service Layer User Manual for more details.
    /// </remarks>
    /// <param name="fileName">
    /// The file name of the file to be uploaded including the file extension.
    /// </param>
    /// <param name="file">
    /// The file to be uploaded.
    /// </param>
    /// <returns>
    /// A object with information about the created attachment entry.
    /// </returns>
    Task<ISboServiceLayerAttachment> PostAttachmentAsync(string fileName, Stream file);
    /// <summary>
    /// Uploads the provided files as an attachment. All files will be posted as attachment lines in a single attachment entry.
    /// </summary>
    /// <remarks>
    /// An attachment folder must be defined. See section 'Setting up an Attachment Folder' in the Service Layer User Manual for more details.
    /// </remarks>
    /// <param name="files">
    /// A Dictionary containing the files to be uploaded, where the file name is the Key and the file is the Value.
    /// </param>
    /// <returns>
    /// A object with information about the created attachment entry.
    /// </returns>
    Task<ISboServiceLayerAttachment> PostAttachmentsAsync(IDictionary<string, byte[]> files);
    /// <summary>
    /// Uploads the provided files as an attachment. All files will be posted as attachment lines in a single attachment entry.
    /// </summary>
    /// <remarks>
    /// An attachment folder must be defined. See section 'Setting up an Attachment Folder' in the Service Layer User Manual for more details.
    /// </remarks>
    /// <param name="files">
    /// A Dictionary containing the files to be uploaded, where the file name is the Key and the file is the Value.
    /// </param>
    /// <returns>
    /// A object with information about the created attachment entry.
    /// </returns>
    Task<ISboServiceLayerAttachment> PostAttachmentsAsync(IDictionary<string, Stream> files);
    /// <summary>
    /// Updates an existing attachment entry with the provided file. If the file already exists
    /// in the attachment entry, it will be replaced. Otherwise, a new attachment line is added.
    /// </summary>
    /// <param name="attachmentEntry">
    /// The attachment entry ID to be updated.
    /// </param>
    /// <param name="path">
    /// The file path for the file to be updated including the file extension.
    /// </param>
    Task PatchAttachmentAsync(int attachmentEntry, string path);
    /// <summary>
    /// Updates an existing attachment entry with the provided file. If the file already exists
    /// in the attachment entry, it will be replaced. Otherwise, a new attachment line is added.
    /// </summary>
    /// <param name="attachmentEntry">
    /// The attachment entry ID to be updated.
    /// </param>
    /// <param name="fileName">
    /// The file name of the file to be updated including the file extension.
    /// </param>
    /// <param name="file">
    /// The file to be updated.
    /// </param>
    Task PatchAttachmentAsync(int attachmentEntry, string fileName, byte[] file);
    /// <summary>
    /// Updates an existing attachment entry with the provided file. If the file already exists
    /// in the attachment entry, it will be replaced. Otherwise, a new attachment line is added.
    /// </summary>
    /// <param name="attachmentEntry">
    /// The attachment entry ID to be updated.
    /// </param>
    /// <param name="fileName">
    /// The file name of the file to be updated including the file extension.
    /// </param>
    /// <param name="file">
    /// The file to be updated.
    /// </param>
    Task PatchAttachmentAsync(int attachmentEntry, string fileName, Stream file);
    /// <summary>
    /// Updates an existing attachment entry with the provided files. If the file already exists
    /// in the attachment entry, it will be replaced. Otherwise, a new attachment line is added.
    /// </summary>
    /// <param name="attachmentEntry">
    /// The attachment entry ID to be updated.
    /// </param>
    /// <param name="files">
    /// A Dictionary containing the files to be updated, where the file name is the Key and the file is the Value.
    /// </param>
    Task PatchAttachmentsAsync(int attachmentEntry, IDictionary<string, byte[]> files);
    /// <summary>
    /// Updates an existing attachment entry with the provided files. If the file already exists
    /// in the attachment entry, it will be replaced. Otherwise, a new attachment line is added.
    /// </summary>
    /// <param name="attachmentEntry">
    /// The attachment entry ID to be updated.
    /// </param>
    /// <param name="files">
    /// A Dictionary containing the files to be updated, where the file name is the Key and the file is the Value.
    /// </param>
    Task PatchAttachmentsAsync(int attachmentEntry, IDictionary<string, Stream> files);
    /// <summary>
    /// Downloads the specified attachment file as a <see cref="Stream"/>. By default, the first attachment
    /// line is downloaded if there are multiple attachment lines in one attachment.
    /// </summary>
    /// <param name="attachmentEntry">
    /// The attachment entry ID to be downloaded.
    /// </param>
    /// <param name="fileName">
    /// The file name of the attachment to be downloaded  (including the file extension). Only required if 
    /// you want to download an attachment line other than the first attachment line.
    /// </param>
    /// <returns>
    /// The downloaded attachment file as a <see cref="Stream"/>.
    /// </returns>
    Task<Stream> GetAttachmentAsStreamAsync(int attachmentEntry, string? fileName = null);
    /// <summary>
    /// Downloads the specified attachment file as a <see cref="byte"/> array. By default, the first attachment
    /// line is downloaded if there are multiple attachment lines in one attachment.
    /// </summary>
    /// <param name="attachmentEntry">
    /// The attachment entry ID to be downloaded.
    /// </param>
    /// <param name="fileName">
    /// The file name of the attachment to be downloaded  (including the file extension). Only required if 
    /// you want to download an attachment line other than the first attachment line.
    /// </param>
    /// <returns>
    /// The downloaded attachment file as a <see cref="byte"/> array.
    /// </returns>
    Task<byte[]> GetAttachmentAsBytesAsync(int attachmentEntry, string? fileName = null);
    #endregion

    #region Batch Request Methods
    /// <summary>
    /// Create a SboServiceLayerBatchRequest object to use in a batch request operation.
    /// </summary>
    /// <param name="httpMethod">HTTP method to be used for this request.</param>
    /// <param name="resource">Service Layer resource to be requested.</param>
    /// <param name="data">JSON body to be sent. It can be either an object to be serialized as JSON or a JSON string.</param>
    /// <param name="contentID">Content-ID for this request, an entity reference that can be used by subsequent requests to refer to a new entity created within the same change set.
    /// This is optional for OData Version 3 (b1s/v1) but mandatory for OData Version 4 (b1s/v2).</param>
    /// <returns>Returns an instance of SboServiceLayerBatchRequest.</returns>
    ISboServiceLayerBatchRequest CreateBatchRequest(HttpMethod httpMethod, string resource, object? data = null, int? contentID = null);
    /// <summary>
    /// Sends a batch request (multiple operations sent in a single HTTP request) with the provided <see cref="SLBatchRequest"/> instances.
    /// All requests are sent in a single change set.
    /// </summary>
    /// <remarks>
    /// See section 'Batch Operations' in the Service Layer User Manual for more details.
    /// </remarks>
    /// <param name="requests">
    /// <see cref="ISboServiceLayerBatchRequest"/> instances to be sent in the batch.
    /// </param>
    /// <returns>
    /// An <see cref="HttpResponseMessage"/> array containg the response messages of the batch request. 
    /// </returns>
    Task<HttpResponseMessage[]> PostBatchAsync(params ISboServiceLayerBatchRequest[] requests);
    /// <summary>
    /// Sends a batch request (multiple operations sent in a single HTTP request) with the provided <see cref="SLBatchRequest"/> collection. 
    /// </summary>
    /// <remarks>
    /// See section 'Batch Operations' in the Service Layer User Manual for more details.
    /// </remarks>
    /// <param name="requests">
    /// A collection of <see cref="ISboServiceLayerBatchRequest"/> to be sent in the batch.</param>
    /// <param name="singleChangeSet">
    /// Whether all the requests in this batch should be sent in a single change set. This means that any unsuccessful request will cause the whole batch to be rolled back.
    /// </param>
    /// <returns>
    /// An <see cref="HttpResponseMessage"/> array containg the response messages of the batch request. 
    /// </returns>
    Task<HttpResponseMessage[]> PostBatchAsync(IEnumerable<ISboServiceLayerBatchRequest> requests, bool singleChangeSet = true);
    #endregion
}
