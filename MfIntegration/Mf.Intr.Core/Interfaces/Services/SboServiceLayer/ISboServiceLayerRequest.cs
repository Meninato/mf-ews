using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Services.SboServiceLayer;

/// <summary>
/// Represents a request to the Service Layer.
/// </summary>
public interface ISboServiceLayerRequest
{
    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in a new instance of the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The object type for the result to be deserialized into.
    /// </typeparam>
    /// <param name="unwrapCollection">
    /// Whether the result should be unwrapped from the 'value' JSON array in case it is a collection.
    /// </param>
    Task<T> GetAsync<T>(bool unwrapCollection = true);
    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in a value tuple containing the deserialized result and the count of matching resources.
    /// </summary>
    /// <typeparam name="T">
    /// The object type for the result to be deserialized into.
    /// </typeparam>
    /// <param name="unwrapCollection">
    /// Whether the result should be unwrapped from the 'value' JSON array in case it is a collection.
    /// </param>
    Task<(T Result, int Count)> GetWithInlineCountAsync<T>(bool unwrapCollection = true);
    /// <summary>
    /// Performs multiple GET requests until all entities in a collection are obtained. The result will always be unwrapped from the 'value' array.
    /// </summary>
    /// <remarks>
    /// This can be very slow depending on the total amount of entities in the company database.
    /// </remarks>
    /// <typeparam name="T">
    /// The object type for the result to be deserialized into.
    /// </typeparam>
    /// <returns>
    /// An <see cref="IList{T}"/> containing all the entities in the given collection.
    /// </returns>
    Task<IList<T>> GetAllAsync<T>();
    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in a <see cref="string"/>.
    /// </summary>
    Task<string> GetStringAsync();
    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in an instance of the given anonymous type.
    /// </summary>
    /// <param name="anonymousTypeObject">
    /// The anonymous type object.
    /// </param>
    /// <param name="jsonSerializerOptions">
    /// The <see cref="JsonSerializerOptions"/> used to deserialize the object. If this is null, 
    /// default serialization settings will be used.
    /// </param>
    Task<T> GetAnonymousTypeAsync<T>(T anonymousTypeObject, JsonSerializerOptions? jsonSerializerOptions = null);
    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in a <see cref="byte"/> array.
    /// </summary>
    Task<byte[]> GetBytesAsync();
    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in a <see cref="Stream"/>.
    /// </summary>
    Task<Stream> GetStreamAsync();
    /// <summary>
    /// Performs a GET request that returns the count of an entity collection.
    /// </summary>
    Task<long> GetCountAsync();
    /// <summary>
    /// Performs a POST request with the provided parameters and returns the result in the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="data">
    /// The object to be sent as the JSON body.
    /// </param>
    /// <typeparam name="T">
    /// The object type for the result to be deserialized into.
    /// </typeparam>
    /// <param name="unwrapCollection">
    /// Whether the result should be unwrapped from the 'value' JSON array in case it is a collection.
    /// </param>
    Task<T> PostAsync<T>(object data, bool unwrapCollection = true);
    /// <summary>
    /// Performs a POST request with the provided parameters and returns the result in the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="data">
    /// The JSON string to be sent as the request body.
    /// </param>
    /// <typeparam name="T">
    /// The object type for the result to be deserialized into.
    /// </typeparam>
    /// <param name="unwrapCollection">
    /// Whether the result should be unwrapped from the 'value' JSON array in case it is a collection.
    /// </param>
    Task<T> PostStringAsync<T>(string data, bool unwrapCollection = true);
    /// <summary>
    /// Performs a POST request with the provided parameters and returns the result in the specified <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The object type for the result to be deserialized into.
    /// </typeparam>
    /// <param name="unwrapCollection">
    /// Whether the result should be unwrapped from the 'value' JSON array in case it is a collection.
    /// </param>
    Task<T> PostAsync<T>(bool unwrapCollection = true);
    /// <summary>
    /// Performs a POST request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The object to be sent as the JSON body.
    /// </param>
    Task PostAsync(object data);
    /// <summary>
    /// Performs a POST request without parameters and returns the result in a <see cref="string"/>.
    /// </summary>
    Task<string> PostReceiveStringAsync();
    /// <summary>
    /// Performs a POST request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The JSON string to be sent as the request body.
    /// </param>
    Task PostStringAsync(string data);
    /// <summary>
    /// Performs a POST request with the provided parameters.
    /// </summary>
    Task PostAsync();
    /// <summary>
    /// Performs a PATCH request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The object to be sent as the JSON body.
    /// </param>
    Task PatchAsync(object data);
    /// <summary>
    /// Performs a PATCH request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The JSON string to be sent as the request body.
    /// </param>
    Task PatchStringAsync(string data);
    /// <summary>
    /// Performs a PATCH request with the provided file.
    /// </summary>
    /// <param name="path">
    /// The path to the file to be sent.
    /// </param>
    Task PatchWithFileAsync(string path);
    /// <summary>
    /// Performs a PATCH request with the provided file.
    /// </summary>
    /// <param name="fileName">
    /// The file name of the file including the file extension.
    /// </param>
    /// <param name="file">
    /// The file to be sent.
    /// </param>
    Task PatchWithFileAsync(string fileName, byte[] file);
    /// <summary>
    /// Performs a PATCH request with the provided file.
    /// </summary>
    /// <param name="fileName">
    /// The file name of the file including the file extension.
    /// </param>
    /// <param name="file">
    /// The file to be sent.
    /// </param>
    Task PatchWithFileAsync(string fileName, Stream file);
    /// <summary>
    /// Performs a PUT request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The object to be sent as the JSON body.
    /// </param>
    Task PutAsync(object data);
    /// <summary>
    /// Performs a PUT request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The JSON string to be sent as the request body.
    /// </param>
    Task PutStringAsync(string data);
    /// <summary>
    /// Performs a DELETE request with the provided parameters.
    /// </summary>
    Task DeleteAsync();
    /// <summary>
    /// Sets the clause to be used to filter records.
    /// </summary>
    ISboServiceLayerRequest Filter(string filter);
    /// <summary>
    /// Sets the explicit properties that should be returned.
    /// </summary>
    ISboServiceLayerRequest Select(string select);
    /// <summary>
    /// Sets the order in which entities should be returned.
    /// </summary>
    ISboServiceLayerRequest OrderBy(string orderBy);
    /// <summary>
    /// Sets the maximum number of first records to be included in the result.
    /// </summary>
    ISboServiceLayerRequest Top(int top);
    /// <summary>
    /// Sets the number of first results to be excluded from the result.
    /// </summary>
    /// <remarks>
    /// Where $top and $skip are used together, the $skip is applied before 
    /// the $top, regardless of the order of appearance in the request.
    /// This can be used when implementing a pagination mechanism.
    /// </remarks>
    ISboServiceLayerRequest Skip(int skip);
    /// <summary>
    /// Sets the aggregation expression.
    /// </summary>
    ISboServiceLayerRequest Apply(string apply);
    /// <summary>
    /// Sets the navigation properties to be retrieved.
    /// </summary>
    ISboServiceLayerRequest Expand(string expand);
    /// <summary>
    /// Sets a custom query parameter to be sent.
    /// </summary>
    ISboServiceLayerRequest SetQueryParam(string name, string value);
    /// <summary>
    /// Sets the page size when paging is applied for a query. The default value is 20.
    /// </summary>
    /// <param name="pageSize">
    /// The page size to be defined for this request.
    /// </param>
    ISboServiceLayerRequest WithPageSize(int pageSize);
    /// <summary>
    /// Enables a case-insensitive query.
    /// </summary>
    /// <remarks>
    /// This is only applicable to SAP HANA databases, where every query is case-sensitive by default.
    /// </remarks>
    ISboServiceLayerRequest WithCaseInsensitive();
    /// <summary>
    /// Allows a PATCH request to remove items in a collection.
    /// </summary>
    ISboServiceLayerRequest WithReplaceCollectionsOnPatch();
    /// <summary>
    /// Configures a POST request to not return the created entity.
    /// This is suitable for better performance in demanding scenarios where the return content is not needed.
    /// </summary>
    /// <remarks>
    /// On success, <see cref="HttpStatusCode.NoContent"/> is returned, instead of <see cref="HttpStatusCode.Created"/>.
    /// </remarks>
    ISboServiceLayerRequest WithReturnNoContent();
    /// <summary>
    /// Adds a custom request header to be sent.
    /// </summary>
    /// <param name="name">
    /// The name of the header.
    /// </param>
    /// <param name="value">
    /// The value of the header.
    /// </param>
    ISboServiceLayerRequest WithHeader(string name, object value);
    /// <summary>
    /// Configures the request to not throw an exception when the response has any of the provided <see cref="HttpStatusCode"/>.
    /// </summary>
    /// <remarks>
    /// By default, every reponse with an unsuccessful <see cref="HttpStatusCode"/> (non-2XX) will result in a throw.
    /// </remarks>
    /// <param name="statusCodes">
    /// The <see cref="HttpStatusCode"/> to be allowed.
    /// </param>
    ISboServiceLayerRequest AllowHttpStatus(params HttpStatusCode[] statusCodes);
    /// <summary>
    /// Configures the request to allow a response with any <see cref="HttpStatusCode"/> without resulting in a throw.
    /// </summary>
    /// <remarks>
    /// By default, every reponse with an unsuccessful <see cref="HttpStatusCode"/> (non-2XX) will result in a throw.
    /// </remarks>
    ISboServiceLayerRequest AllowAnyHttpStatus();
    /// <summary>
    /// Configures the JSON serializer to include null values (<see cref="JsonIgnoreCondition.Never"/>) for this request.
    /// The default value is <see cref="JsonIgnoreCondition.WhenWritingNull"/>.
    /// </summary>
    ISboServiceLayerRequest IncludeNullValues();
    /// <summary>
    /// Sets a custom <see cref="JsonSerializerOptions"/> to be used for this request.
    /// </summary>
    ISboServiceLayerRequest WithJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions);
    /// <summary>
    /// Configures a custom timeout value for this request. The default timeout is 100 seconds.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> representing the timeout value to be configured.
    /// </param>
    ISboServiceLayerRequest WithTimeout(TimeSpan timeout);
    /// <summary>
    /// Configures a custom timeout value for this request. The default timeout is 100 seconds.
    /// </summary>
    /// <param name="timeout">
    /// An <see cref="int"/> representing the timeout in seconds to be configured.
    /// </param>
    ISboServiceLayerRequest WithTimeout(int timeout);
}
