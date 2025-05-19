using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;
using VMManager.Common.Logging;

namespace VMManager.Infrastructure.Scheduling
{
    public class QuartzSchedulerService
    {
        private readonly IScheduler _scheduler;
        private readonly ILoggingService _logger;

        public QuartzSchedulerService(ILoggingService logger)
        {
            _logger = logger;
            var factory = new StdSchedulerFactory();
            _scheduler = factory.GetScheduler().Result;
            _scheduler.Start().Wait();
        }

        public async Task ScheduleOnceAsync(string jobId, DateTimeOffset runAt, Action action)
        {
            var job = JobBuilder.Create<GenericQuartzJob>()
                .WithIdentity(jobId)
                .Build();
            job.JobDataMap["action"] = action;

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobId}.trigger")
                .StartAt(runAt)
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }
    }

    public class GenericQuartzJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var action = (Action)context.JobDetail.JobDataMap["action"];
            action();
            return Task.CompletedTask;
        }
    }
}