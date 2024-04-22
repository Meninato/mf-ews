using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Models;

public class SboServiceLayerCollectionRoot<T>
{
    private readonly Regex _skipRegex = new Regex(@"skip=(\d+)&?");

    [JsonPropertyName("value")]
    public IList<T> Value { get; set; } = null!;

    [JsonPropertyName("odata.nextLink")]
    public string ODataNextLink { get; set; } = null!;

    [JsonIgnore]
    private string ODataNextLinkAlt
    {
        set { ODataNextLink = value; }
    }

    /// <summary>
    /// Gets or sets the string that represents the link to the next page.
    /// </summary>
    [JsonPropertyName("@odata.nextLink")]
    public string ODataNextLinkJson
    {
        get => ODataNextLink;
        set => ODataNextLinkAlt = value;
    }

    /// <summary>
    /// Gets the skip number to obtain the entities of the next page.
    /// </summary>
    public int NextSkip => string.IsNullOrEmpty(ODataNextLink) ? 0 : int.Parse(_skipRegex.Match(ODataNextLink).Groups[1].Value);
}
