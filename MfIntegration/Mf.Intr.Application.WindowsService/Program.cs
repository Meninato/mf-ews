using Mf.Intr.Application.WindowsService;
using Mf.Intr.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
public class Program
{
    private static PromptScCmd promptScCmd = new PromptScCmd();

    public static async Task Main(string[] args)
    {
        if(args.Length == 1)
        {
            CallWindowsServiceControl(args[0]);
        }
        else
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(config =>
                {
                    config.ServiceName = WindowsServiceDefaults.SERVICE_NAME;
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<IntegrationWindowsService>();
                })
                .Build();

            await host.RunAsync();
        }
    }

    private static void CallWindowsServiceControl(string arg)
    {
        Dictionary<string, Action> commands = new Dictionary<string, Action>()
        {
            { "install",  CallInstall },
            { "uninstall" , CallUninstall }
        };

        if(commands.ContainsKey(arg))
        {
            commands[arg].Invoke();
        }
    }

    private static void CallInstall()
    {
        promptScCmd.Create();
        promptScCmd.AddDescription();
    }

    private static void CallUninstall()
    {
        promptScCmd.Stop();
        promptScCmd.Delete();
    }
}
