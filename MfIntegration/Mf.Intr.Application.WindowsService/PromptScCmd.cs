using Mf.Intr.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.WindowsService;
public class PromptScCmd
{
    private const string CREATE_PATTERN = "sc create {0} BinPath=\"{1}\"";
    private const string START_PATTERN = "sc start {0}";
    private const string STOP_PATTERN = "sc stop {0}";
    private const string DELETE_PATTERN = "sc delete {0}";
    private const string DESCRIPTION_PATTERN = "sc description {0} \"{1}\"";

    public void Create()
    {
        string exeFile = Path.Combine(GetCurrentDirectory(), WindowsServiceDefaults.EXE_FILENAME);
        string argument = string.Format(CREATE_PATTERN, WindowsServiceDefaults.SERVICE_NAME, exeFile);
        var p = CreateProcess(argument);
        p.Start();
    }

    public void Start()
    {
        string argument = string.Format(START_PATTERN, WindowsServiceDefaults.SERVICE_NAME);
        var p = CreateProcess(argument);
        p.Start();
    }

    public void Stop()
    {
        string argument = string.Format(STOP_PATTERN, WindowsServiceDefaults.SERVICE_NAME);
        var p = CreateProcess(argument);
        p.Start();
    }

    public void Delete()
    {
        string argument = string.Format(DELETE_PATTERN, WindowsServiceDefaults.SERVICE_NAME);
        var p = CreateProcess(argument);
        p.Start();
    }

    public void AddDescription()
    {
        string argument = string.Format(DESCRIPTION_PATTERN, WindowsServiceDefaults.SERVICE_NAME, WindowsServiceDefaults.SERVICE_DESCRIPTION);
        var p = CreateProcess(argument);
        p.Start();
    }

    private Process CreateProcess(string argument)
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = "cmd.exe",
            Arguments = @$"/C {argument}",
            WindowStyle = ProcessWindowStyle.Hidden
        };
        return new Process() { StartInfo = startInfo };
    }

    private string GetCurrentDirectory()
    {
        string? dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (string.IsNullOrEmpty(dir))
        {
            dir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName);
        }

        if (dir == null)
        {
            throw new IntegrationException("Not possible to get the path of the executing assembly");
        }

        return dir;
    }
}
