using Ardalis.SmartEnum;
using Mf.Intr.Core.Db;
using Mf.Intr.Core.Exceptions;
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
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestingWorker.Workers;

public class BpWorker : CompanyWorker<List<string>, string>
{
    public string? Param1 { get; set; }

    //declare what services you may need
    private readonly IUnitOfWork _intrDataAccess;
    private readonly IConfigurationService _configuration;
    private readonly ISboServiceLayerPoolService _slPoolService;

    public BpWorker(IWorkerServiceBox box) : base(box)
    {
        //OnTaskSuccess += WhenTaskSuccess;
        //OnTaskFail += WhenTaskFail;

        _intrDataAccess = _box.GetUnitOfWork();
        _configuration = _box.GetConfiguration();
        _slPoolService = _box.GetSboServiceLayerPool();
    }

    public sealed class Food : SmartEnum<Food>
    {
        public static readonly Food One = new Food(nameof(One), 1);
        public static readonly Food Two = new Food(nameof(Two), 2);
        public static readonly Food Three = new Food(nameof(Three), 3);

        private Food(string name, int value) : base(name, value)
        {
        }
    }

    public class BusinessPartnerExample
    {
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
    }

    public override IWorkableResult<List<string>> Run(string a)
    {
        WorkerResult<List<string>> workerResult;

        ISboServiceLayerConnection slConnection = _slPoolService.Get("abc");

        slConnection.Request("BusinessPartners")
            .Filter("startswith(CardCode, 'c')")
            .Select("CardCode, CardName")
            .OrderBy("CardName")
            .WithPageSize(50)
            .WithCaseInsensitive()
            .GetAsync<List<BusinessPartnerExample>>();

        //SqlQueryConverter sqlQueryConverter = new SqlQueryConverter();
        Person person= new Person();
        person.Id = "";
        person.Name = "Joana Dark I";
        //person.Amount = 100.58;
        person.IsGood = true;
        //person.Balance = 1500.12f;
        //person.Price = 27124.9641m;
        //person.Id = 99;
        person.Description = "ACtually I think a shower would be awesome";

        //string insert = sqlQueryConverter.ConvertToInsert(person);
        //string update = sqlQueryConverter.ConvertToUpdate(person);

        IIntrDataAccess con = _intrDataAccess.GetDataAccess(this.Connection);
        //int key = con.Insert<Person, int>(person);
        con.Update(person);

        var persons = con.Query<Person>("SELECT * FROM Person").ToList();

        //con.Open();
        //string query = "SELECT 'OI FROM MSSQL' as db ";
        //DbCommand command = con.CreateCommand();
        //command.CommandText = query;
        //command.ExecuteScalar();

        var options = _configuration.Get<TestingWorkerOptions>("TestingWorker");

        _logger.LogInformation("the options is: {@options}", options);

        //Imagine here that you have a list of values and you need to process
        //them and do something with them in case of failure or success
        //In that case you can call a base method called StartTask
        //from that you may want to register an event to do an action when success or false
        //like logging or sending an email use the RegisterEvents and pass a callback

        throw new Exception("HEEEEEEEEEEEY");

        Food rice = Food.One;

        List<string> processes = new List<string>() { "A", "B", "C", "D" };
        foreach (string process in processes)
        {
            //Using strongly typed reference method which receive a string as parameter
            StartTask<string>(GetStrings, new object[] { process });

            //Using a delegate which receive a string as parameter
            //StartTask<string>((value) =>
            //{
            //    return new TaskResult(false);
            //}, new object[] { process });

            ////Using a delegate which doesn't accept parameter
            //StartTask(() =>
            //{
            //    return new TaskResult(false);
            //});

            ////Now imagine that you need the response of your task to continue your logic
            ////you can receive the TaskResult and do your specifics, also we dont want that this task
            ////to be stacked in ours task control. We are using just to wrapper our code nicely
            //TaskResult myResult = StartTask(() =>
            //{
            //    return new TaskResult(false);
            //}, append: false);

            //if(myResult.HasError == false)
            //{
            //    //continue
            //}
        }



        //Make a conclusion by your choice, all succeed, all failed or partial
        if(GetWorkStatus() == WorkStatus.AllSucceed)
        {
            //advance worker

            workerResult = new WorkerResult<List<string>>();
            workerResult.Success = true;
            workerResult.Data = new List<string>() { "Hello", "World", "From", "Worker" };
            workerResult.Message = "Hello World :)";
            workerResult.Next = Next;
        }
        else
        {
            //stop worker

            workerResult = new WorkerResult<List<string>>();
            workerResult.Success = false;
            workerResult.Message = "Bye with error.";
        }

        return workerResult;
    }

    private TaskResult GetStrings(string process)
    {
        TaskResult taskResult;
        taskResult = new TaskResult(false);
        if (process == "C")
        {
            taskResult = new TaskResult(true);
        }
        return taskResult;
    }

    private void WhenTaskFail(object? sender, TaskEventArgs e)
    {
        _logger.LogError("Something went wrong... never mind, next");
    }

    private void WhenTaskSuccess(object? sender, TaskEventArgs e)
    {
        _logger.LogInformation("Everything is ok... keep moving");
    }
}
