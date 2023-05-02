using Sgbj.Cron;
using AES;
using Quartz;
using CredentialsHandler;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Cron.Handler;

public class LasmService : IJob
    {
        private readonly IConfiguration _config;
    public LasmService(IConfiguration config)
    {
        this._config = config;
    }
        public async Task Execute(IJobExecutionContext context)
        {
            //Write your custom code here
            var cred = new CredHandler(_config);
            await cred.Renew();
            Console.WriteLine("job ran");
            // return Task.CompletedTask;
        }
    }
