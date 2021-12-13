using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Quartz
{
    internal sealed class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IOptions<QuartzHostedServiceOptions> options;
        private IScheduler scheduler = null!;

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            IOptions<QuartzHostedServiceOptions> options)
        {
            this.schedulerFactory = schedulerFactory;
            this.options = options;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            scheduler = await schedulerFactory.GetScheduler(cancellationToken);

            if (options.Value.StartDelay.HasValue)
            {
                await scheduler.StartDelayed(options.Value.StartDelay.Value, cancellationToken);
            }
            else
            {
                await scheduler.Start(cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (scheduler != null)
            {
                await scheduler.Shutdown(options.Value.WaitForJobsToComplete, cancellationToken);
            }
        }
    }
}
