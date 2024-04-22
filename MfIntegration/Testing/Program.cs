using Mf.Intr.Application;
using Mf.Intr.Application.Jobs;
using Microsoft.Extensions.Logging;

//var integrationApp = new IntegrationApp();
//try
//{
//    //await integrationApp.CompanyScheduler.Start();

//    //Debug setup
//    await integrationApp.CompanyScheduler.Prepare();
//    await integrationApp.CompanyScheduler.StartOnce("479cdf52-1bf6-46ff-8df7-50aec4f60068");
//}
//catch (Exception ex)
//{
//    integrationApp.Logger.LogCritical(ex, "Something bad happened...");
//    await integrationApp.CompanyScheduler.Stop();
//}


//TODO: probably add nuget for application and core


var integrationApp = new IntegrationApp();
try
{
    await integrationApp.FileScheduler.Start();
    //await integrationApp.CompanyScheduler.Start();
    //Debug setup
    //await integrationApp.CompanyScheduler.Prepare();
    //await integrationApp.CompanyScheduler.StartOnce("479cdf52-1bf6-46ff-8df7-50aec4f60068");
}
catch (Exception ex)
{
    integrationApp.Logger.LogCritical(ex, "Something bad happened...");
    await integrationApp.FileScheduler.Stop();
}

Console.ReadKey();