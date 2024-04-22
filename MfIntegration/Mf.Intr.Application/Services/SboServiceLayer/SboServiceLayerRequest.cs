using Flurl.Http;
using Flurl.Http.Configuration;
using Mf.Intr.Application.Services.SboServiceLayer.Extensions;
using Mf.Intr.Application.Services.SboServiceLayer.Models;
using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer;

/// <summary>
/// Represents a request to the Service Layer.
/// </summary>
public class SboServiceLayerRequest : ISboServiceLayerRequest
{
    private readonly SboServiceLayerConnection _slConnection;
    internal IFlurlRequest FlurlRequest { get; }

    internal SboServiceLayerRequest(SboServiceLayerConnection connection, IFlurlRequest flurlRequest)
    {
        this._slConnection = connection;
        this.FlurlRequest = flurlRequest;
    }

    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in a new instance of the specified type.
    /// </summary>
    /// <typeparam name="T">
    /// The object type for the result to be deserialized into.
    /// </typeparam>
    /// <param name="unwrapCollection">
    /// Whether the result should be unwrapped from the 'value' JSON array in case it is a collection.
    /// </param>
    public async Task<T> GetAsync<T>(bool unwrapCollection = true)
    {
        return await _slConnection.ExecuteRequest(async () =>
        {
            string stringResult = await FlurlRequest.WithCookies(_slConnection.Cookies).GetStringAsync();
            var jsonDoc = JsonDocument.Parse(stringResult);

            string jsonToDeserialize = jsonDoc.RootElement.GetRawText();

            bool valuePropExists = jsonDoc.RootElement.TryGetProperty("value", out JsonElement valueJsonElement);
            if (unwrapCollection && valuePropExists)
            {
                jsonToDeserialize = valueJsonElement.GetRawText();
            }

            return JsonSerializer.Deserialize<T>(jsonToDeserialize, AppDefaults.DefaultJsonSerializerOptions)!;
        });
    }

    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in a value tuple containing the deserialized result and the count of matching resources.
    /// </summary>
    /// <typeparam name="T">
    /// The object type for the result to be deserialized into.
    /// </typeparam>
    /// <param name="unwrapCollection">
    /// Whether the result should be unwrapped from the 'value' JSON array in case it is a collection.
    /// </param>
    public async Task<(T Result, int Count)> GetWithInlineCountAsync<T>(bool unwrapCollection = true)
    {
        return await _slConnection.ExecuteRequest(async () =>
        {
            string stringResult = await FlurlRequest.SetQueryParam("$inlinecount", "allpages").WithCookies(_slConnection.Cookies).GetStringAsync();
            var jsonDoc = JsonDocument.Parse(stringResult);

            bool odataExists = jsonDoc.RootElement.TryGetProperty("odata.count", out JsonElement odataCount1);
            bool atSignOdataExists = jsonDoc.RootElement.TryGetProperty("@odata.count", out JsonElement oDataCount2);
            int inlineCount = odataExists ? odataCount1.GetInt32() : atSignOdataExists ? oDataCount2.GetInt32() : 0;

            string jsonToDeserialize = jsonDoc.RootElement.GetRawText();

            bool valuePropExists = jsonDoc.RootElement.TryGetProperty("value", out JsonElement valueJsonElement);
            if (unwrapCollection && valuePropExists)
            {
                // Checks if the result is a collection by selecting the "value" token
                jsonToDeserialize = valueJsonElement.GetRawText();
            }

            T result = JsonSerializer.Deserialize<T>(jsonToDeserialize, AppDefaults.DefaultJsonSerializerOptions)!;
            return (result, inlineCount);
        });
    }

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
    public async Task<IList<T>> GetAllAsync<T>()
    {
        var allResultsList = new List<T>();
        int skip = 0;

        do
        {
            await _slConnection.ExecuteRequest(async () =>
            {
                var currentResult = await FlurlRequest
                    .WithCookies(_slConnection.Cookies)
                    .SetQueryParam("$skip", skip)
                    .GetJsonAsync<SboServiceLayerCollectionRoot<T>>();

                allResultsList.AddRange(currentResult.Value);
                skip = currentResult.NextSkip;
                return 0;
            });
        }
        while (skip > 0);

        return allResultsList;
    }

    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in a <see cref="string"/>.
    /// </summary>
    public async Task<string> GetStringAsync()
    {
        return await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).GetStringAsync();
        });
    }

    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in an instance of the given anonymous type.
    /// </summary>
    /// <param name="anonymousTypeObject">
    /// The anonymous type object.
    /// </param>
    /// <param name="JsonSerializerOptions">
    /// The <see cref="JsonSerializerOptions"/> used to deserialize the object. If this is null, 
    /// default serialization settings will be used.
    /// </param>
    public async Task<T> GetAnonymousTypeAsync<T>(T anonymousTypeObject, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        return await _slConnection.ExecuteRequest(async () =>
        {
            string stringResult = await FlurlRequest.WithCookies(_slConnection.Cookies).GetStringAsync();
            return JsonSerializer.Deserialize<T>(stringResult, jsonSerializerOptions ?? AppDefaults.DefaultJsonSerializerOptions)!;
        });
    }

    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in a <see cref="byte"/> array.
    /// </summary>
    public async Task<byte[]> GetBytesAsync()
    {
        return await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).GetBytesAsync();
        });
    }

    /// <summary>
    /// Performs a GET request with the provided parameters and returns the result in a <see cref="Stream"/>.
    /// </summary>
    public async Task<Stream> GetStreamAsync()
    {
        return await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).GetStreamAsync();
        });
    }

    /// <summary>
    /// Performs a GET request that returns the count of an entity collection.
    /// </summary>
    public async Task<long> GetCountAsync()
    {
        return await _slConnection.ExecuteRequest(async () =>
        {
            string result = await FlurlRequest.WithCookies(_slConnection.Cookies).AppendPathSegment("$count").GetStringAsync();
            long.TryParse(result, out long quantity);
            return quantity;
        });
    }

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
    public async Task<T> PostAsync<T>(object data, bool unwrapCollection = true)
    {
        return await _slConnection.ExecuteRequest(async () =>
        {
            string stringResult = await FlurlRequest.WithCookies(_slConnection.Cookies).PostJsonAsync(data).ReceiveString();
            var jsonDoc = JsonDocument.Parse(stringResult);

            string jsonToDeserialize = jsonDoc.RootElement.GetRawText();

            bool valuePropsExists = jsonDoc.RootElement.TryGetProperty("value", out JsonElement valueJsonElement);
            if (unwrapCollection && valuePropsExists)
            {
                // Checks if the result is a collection by selecting the "value" token
                jsonToDeserialize = valueJsonElement.GetRawText();
            }

            return JsonSerializer.Deserialize<T>(jsonToDeserialize)!;
        });
    }

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
    public async Task<T> PostStringAsync<T>(string data, bool unwrapCollection = true)
    {
        return await _slConnection.ExecuteRequest(async () =>
        {
            string stringResult = await FlurlRequest.WithCookies(_slConnection.Cookies).PostStringAsync(data).ReceiveString();
            var jsonDoc = JsonDocument.Parse(stringResult);

            string jsonToDeserialize = jsonDoc.RootElement.GetRawText();

            bool hasValueProp = jsonDoc.RootElement.TryGetProperty("value", out JsonElement valueJsonElement);
            if (unwrapCollection && hasValueProp)
            {
                // Checks if the result is a collection by selecting the "value" token
                jsonToDeserialize = valueJsonElement.GetRawText();
            }

            return JsonSerializer.Deserialize<T>(jsonToDeserialize, AppDefaults.DefaultJsonSerializerOptions)!;
        });
    }

    /// <summary>
    /// Performs a POST request with the provided parameters and returns the result in the specified <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The object type for the result to be deserialized into.
    /// </typeparam>
    /// <param name="unwrapCollection">
    /// Whether the result should be unwrapped from the 'value' JSON array in case it is a collection.
    /// </param>
    public async Task<T> PostAsync<T>(bool unwrapCollection = true)
    {
        return await _slConnection.ExecuteRequest(async () =>
        {
            string stringResult = await FlurlRequest.WithCookies(_slConnection.Cookies).PostAsync().ReceiveString();
            var jsonDoc = JsonDocument.Parse(stringResult);

            string jsonToDeserialize = jsonDoc.RootElement.GetRawText();

            bool hasValueProp = jsonDoc.RootElement.TryGetProperty("value", out JsonElement valueJsonElement);
            if (unwrapCollection && hasValueProp)
            {
                // Checks if the result is a collection by selecting the "value" token
                jsonToDeserialize = valueJsonElement.GetRawText();
            }

            return JsonSerializer.Deserialize<T>(jsonToDeserialize, AppDefaults.DefaultJsonSerializerOptions)!;
        });
    }

    /// <summary>
    /// Performs a POST request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The object to be sent as the JSON body.
    /// </param>
    public async Task PostAsync(object data)
    {
        await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).PostJsonAsync(data);
        });
    }

    /// <summary>
    /// Performs a POST request without parameters and returns the result in a <see cref="string"/>.
    /// </summary>
    public async Task<string> PostReceiveStringAsync()
    {
        return await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).PostAsync().ReceiveString();
        });
    }

    /// <summary>
    /// Performs a POST request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The JSON string to be sent as the request body.
    /// </param>
    public async Task PostStringAsync(string data)
    {
        await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).PostStringAsync(data);
        });
    }

    /// <summary>
    /// Performs a POST request with the provided parameters.
    /// </summary>
    public async Task PostAsync()
    {
        await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).PostAsync();
        });
    }

    /// <summary>
    /// Performs a PATCH request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The object to be sent as the JSON body.
    /// </param>
    public async Task PatchAsync(object data)
    {
        await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).PatchJsonAsync(data);
        });
    }

    /// <summary>
    /// Performs a PATCH request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The JSON string to be sent as the request body.
    /// </param>
    public async Task PatchStringAsync(string data)
    {
        await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).PatchStringAsync(data);
        });
    }

    /// <summary>
    /// Performs a PATCH request with the provided file.
    /// </summary>
    /// <param name="path">
    /// The path to the file to be sent.
    /// </param>
    public async Task PatchWithFileAsync(string path) =>
        await PatchWithFileAsync(Path.GetFileName(path), File.ReadAllBytes(path));

    /// <summary>
    /// Performs a PATCH request with the provided file.
    /// </summary>
    /// <param name="fileName">
    /// The file name of the file including the file extension.
    /// </param>
    /// <param name="file">
    /// The file to be sent.
    /// </param>
    public async Task PatchWithFileAsync(string fileName, byte[] file) =>
        await PatchWithFileAsync(fileName, new MemoryStream(file));

    /// <summary>
    /// Performs a PATCH request with the provided file.
    /// </summary>
    /// <param name="fileName">
    /// The file name of the file including the file extension.
    /// </param>
    /// <param name="file">
    /// The file to be sent.
    /// </param>
    public async Task PatchWithFileAsync(string fileName, Stream file)
    {
        await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).PatchMultipartAsync(mp =>
            {
                // Removes double quotes from boundary, otherwise the request fails with error 405 Method Not Allowed
                var boundary = mp.Headers.ContentType!.Parameters.First(o => o.Name.Equals("boundary", StringComparison.OrdinalIgnoreCase));
                boundary.Value = boundary.Value!.Replace("\"", string.Empty);

                var content = new StreamContent(file);
                content.Headers.Add("Content-Disposition", $"form-data; name=\"files\"; filename=\"{fileName}\"");
                content.Headers.Add("Content-Type", "application/octet-stream");
                mp.Add(content);
            });
        });
    }

    /// <summary>
    /// Performs a PUT request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The object to be sent as the JSON body.
    /// </param>
    public async Task PutAsync(object data)
    {
        await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).PutJsonAsync(data);
        });
    }

    /// <summary>
    /// Performs a PUT request with the provided parameters.
    /// </summary>
    /// <param name="data">
    /// The JSON string to be sent as the request body.
    /// </param>
    public async Task PutStringAsync(string data)
    {
        await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).PutStringAsync(data);
        });
    }

    /// <summary>
    /// Performs a DELETE request with the provided parameters.
    /// </summary>
    public async Task DeleteAsync()
    {
        await _slConnection.ExecuteRequest(async () =>
        {
            return await FlurlRequest.WithCookies(_slConnection.Cookies).DeleteAsync();
        });
    }

    /// <summary>
    /// Sets the clause to be used to filter records.
    /// </summary>
    public ISboServiceLayerRequest Filter(string filter)
    {
        this.FlurlRequest.SetQueryParam("$filter", filter);
        return this;
    }

    /// <summary>
    /// Sets the explicit properties that should be returned.
    /// </summary>
    public ISboServiceLayerRequest Select(string select)
    {
        this.FlurlRequest.SetQueryParam("$select", select);
        return this;
    }

    /// <summary>
    /// Sets the order in which entities should be returned.
    /// </summary>
    public ISboServiceLayerRequest OrderBy(string orderBy)
    {
        this.FlurlRequest.SetQueryParam("$orderby", orderBy);
        return this;
    }

    /// <summary>
    /// Sets the maximum number of first records to be included in the result.
    /// </summary>
    public ISboServiceLayerRequest Top(int top)
    {
        this.FlurlRequest.SetQueryParam("$top", top);
        return this;
    }

    /// <summary>
    /// Sets the number of first results to be excluded from the result.
    /// </summary>
    /// <remarks>
    /// Where $top and $skip are used together, the $skip is applied before 
    /// the $top, regardless of the order of appearance in the request.
    /// This can be used when implementing a pagination mechanism.
    /// </remarks>
    public ISboServiceLayerRequest Skip(int skip)
    {
        this.FlurlRequest.SetQueryParam("$skip", skip);
        return this;
    }

    /// <summary>
    /// Sets the aggregation expression.
    /// </summary>
    public ISboServiceLayerRequest Apply(string apply)
    {
        this.FlurlRequest.SetQueryParam("$apply", apply);
        return this;
    }

    /// <summary>
    /// Sets the navigation properties to be retrieved.
    /// </summary>
    public ISboServiceLayerRequest Expand(string expand)
    {
        this.FlurlRequest.SetQueryParam("$expand", expand);
        return this;
    }

    /// <summary>
    /// Sets a custom query parameter to be sent.
    /// </summary>
    public ISboServiceLayerRequest SetQueryParam(string name, string value)
    {
        this.FlurlRequest.SetQueryParam(name, value);
        return this;
    }

    /// <summary>
    /// Sets the page size when paging is applied for a query. The default value is 20.
    /// </summary>
    /// <param name="pageSize">
    /// The page size to be defined for this request.
    /// </param>
    public ISboServiceLayerRequest WithPageSize(int pageSize)
    {
        this.FlurlRequest.WithHeader("B1S-PageSize", pageSize);
        return this;
    }

    /// <summary>
    /// Enables a case-insensitive query.
    /// </summary>
    /// <remarks>
    /// This is only applicable to SAP HANA databases, where every query is case-sensitive by default.
    /// </remarks>
    public ISboServiceLayerRequest WithCaseInsensitive()
    {
        this.FlurlRequest.WithHeader("B1S-CaseInsensitive", "true");
        return this;
    }

    /// <summary>
    /// Allows a PATCH request to remove items in a collection.
    /// </summary>
    public ISboServiceLayerRequest WithReplaceCollectionsOnPatch()
    {
        this.FlurlRequest.WithHeader("B1S-ReplaceCollectionsOnPatch", "true");
        return this;
    }

    /// <summary>
    /// Configures a POST request to not return the created entity.
    /// This is suitable for better performance in demanding scenarios where the return content is not needed.
    /// </summary>
    /// <remarks>
    /// On success, <see cref="HttpStatusCode.NoContent"/> is returned, instead of <see cref="HttpStatusCode.Created"/>.
    /// </remarks>
    public ISboServiceLayerRequest WithReturnNoContent()
    {
        this.FlurlRequest.WithHeader("Prefer", "return-no-content");
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
    public ISboServiceLayerRequest WithHeader(string name, object value)
    {
        this.FlurlRequest.WithHeader(name, value);
        return this;
    }

    /// <summary>
    /// Configures the request to not throw an exception when the response has any of the provided <see cref="HttpStatusCode"/>.
    /// </summary>
    /// <remarks>
    /// By default, every reponse with an unsuccessful <see cref="HttpStatusCode"/> (non-2XX) will result in a throw.
    /// </remarks>
    /// <param name="statusCodes">
    /// The <see cref="HttpStatusCode"/> to be allowed.
    /// </param>
    public ISboServiceLayerRequest AllowHttpStatus(params HttpStatusCode[] statusCodes)
    {
        var statusCodeAsInt = statusCodes.Select(status => (int)status).ToArray();
        this.FlurlRequest.AllowHttpStatus(statusCodeAsInt);
        return this;
    }

    /// <summary>
    /// Configures the request to allow a response with any <see cref="HttpStatusCode"/> without resulting in a throw.
    /// </summary>
    /// <remarks>
    /// By default, every reponse with an unsuccessful <see cref="HttpStatusCode"/> (non-2XX) will result in a throw.
    /// </remarks>
    public ISboServiceLayerRequest AllowAnyHttpStatus()
    {
        this.FlurlRequest.AllowAnyHttpStatus();
        return this;
    }

    /// <summary>
    /// Configures the JSON serializer to include null values (<see cref="JsonIgnoreCondition.Never"/>) for this request.
    /// The default value is <see cref="JsonIgnoreCondition.WhenWritingNull"/>.
    /// </summary>
    public ISboServiceLayerRequest IncludeNullValues()
    {
        JsonSerializerOptions options = AppDefaults.DefaultJsonSerializerOptions;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.Never;

        this.FlurlRequest.Settings.JsonSerializer = new DefaultJsonSerializer(options);

        return this;
    }

    /// <summary>
    /// Sets a custom <see cref="JsonSerializerOptions"/> to be used for this request.
    /// </summary>
    public ISboServiceLayerRequest WithJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions)
    {
        this.FlurlRequest.Settings.JsonSerializer = new DefaultJsonSerializer(jsonSerializerOptions);

        return this;
    }

    /// <summary>
    /// Configures a custom timeout value for this request. The default timeout is 100 seconds.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="TimeSpan"/> representing the timeout value to be configured.
    /// </param>
    public ISboServiceLayerRequest WithTimeout(TimeSpan timeout)
    {
        this.FlurlRequest.WithTimeout(timeout);
        return this;
    }

    /// <summary>
    /// Configures a custom timeout value for this request. The default timeout is 100 seconds.
    /// </summary>
    /// <param name="timeout">
    /// An <see cref="int"/> representing the timeout in seconds to be configured.
    /// </param>
    public ISboServiceLayerRequest WithTimeout(int timeout)
    {
        this.FlurlRequest.WithTimeout(timeout);
        return this;
    }
}
