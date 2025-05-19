using System;
using System.Threading.Tasks;
using VMManager.Infrastructure.Scheduling;
using VMManager.Common.Logging;
using Xunit;

namespace VMManager.Tests.Infrastructure
{
    public class QuartzSchedulerTests
    {
        [Fact]
        public async Task ScheduleOnceAsync_ExecutesAction()
        {
            bool executed = false;
            var logger = new VMManager.Common.Logging.SerilogLoggingService();
            var scheduler = new QuartzSchedulerService(logger);

            await scheduler.ScheduleOnceAsync("test", DateTimeOffset.UtcNow.AddSeconds(1), () => executed = true);
            await Task.Delay(1500);

            Assert.True(executed);
        }
    }
}
