using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Services.SboServiceLayer;

public interface ISboServiceLayerAttachmentLine
{
    string SourcePath { get; set; }
    string FileName { get; set; }
    string FileExtension { get; set; }
    string AttachmentDate { get; set; }
    int UserID { get; set; }
    string Override { get; set; }
}
