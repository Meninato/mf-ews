using Mf.Intr.Application.Services.SboServiceLayer.Converters;
using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Models;

public class SboServiceLayerAttachment : ISboServiceLayerAttachment
{
    [JsonPropertyName("odata.metadata")]
    public string? ODataMetadata { get; set; }
    public int AbsoluteEntry { get; set; }

    [JsonConverter(typeof(InterfaceConverter<SboServiceLayerAttachmentLine, ISboServiceLayerAttachmentLine>))]
    [JsonPropertyName("Attachments2_Lines")]
    public ISboServiceLayerAttachmentLine[] Attachments2Lines { get; set; } = null!;
}

public class SboServiceLayerAttachmentLine : ISboServiceLayerAttachmentLine
{
    public string SourcePath { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FileExtension { get; set; } = null!; 
    public string AttachmentDate { get; set; } = null!;
    public int UserID { get; set; }
    public string Override { get; set; } = null!;
}
