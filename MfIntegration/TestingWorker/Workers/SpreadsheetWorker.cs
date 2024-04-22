using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Interfaces.Services;
using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;
using Mf.Intr.Core.Workers;
using Mf.Intr.Core.Workers.Implemented;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingWorker.Workers;

public class SpreadsheetWorker : FileWorker<string>
{
    public SpreadsheetWorker(IWorkerServiceBox box) : base(box)
    {
        //OnTaskSuccess += WhenTaskSuccess;
        //OnTaskFail += WhenTaskFail;
    }

    public string? ParamX { get; set; }

    public override IWorkableResult<string> Run()
    {
        _logger.LogInformation("Hi from Spreadsheet worker :)");

        WorkerResult<string> workerResult = new WorkerResult<string>();
        return workerResult;
    }
}
