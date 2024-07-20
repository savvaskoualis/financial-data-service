using FinancialDataService.Application.Jobs;
using Quartz;

namespace FinancialDataService.Infrastructure.Scheduling
{
    public class QuartzInstrumentsJob : IJob
    {
        private readonly FetchInstrumentsJob _fetchInstrumentsJob;

        public QuartzInstrumentsJob(FetchInstrumentsJob fetchInstrumentsJob)
        {
            _fetchInstrumentsJob = fetchInstrumentsJob;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return _fetchInstrumentsJob.Execute();
        }
    }
}