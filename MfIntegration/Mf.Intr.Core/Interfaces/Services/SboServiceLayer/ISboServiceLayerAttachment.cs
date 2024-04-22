using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Services.SboServiceLayer;

public interface ISboServiceLayerAttachment
{
    string? ODataMetadata { get; set; }
    int AbsoluteEntry { get; set; }
    ISboServiceLayerAttachmentLine[] Attachments2Lines { get; set; }
}
