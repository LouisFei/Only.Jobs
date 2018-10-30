using System;
using log4net;
using Quartz;
using Only.Jobs.Core;

namespace Only.Jobs.JobItems
{
    /// <summary>
    /// 负责Job的动态调度，此任务为系统核心底层任务，其它业务任务都依赖于它。
    /// </summary>
    [DisallowConcurrentExecution]
    public class ManagerJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ManagerJob));

        public void Execute(IJobExecutionContext context)
        {
            Version Ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _logger.InfoFormat("ManagerJob Execute begin Ver." + Ver.ToString());
            try
            {
                //Job状态管控
                new QuartzManager().JobScheduler(context.Scheduler);
                _logger.InfoFormat("ManagerJob Executing ...");
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                e2.RefireImmediately = true;
            }
            finally
            {
                _logger.InfoFormat("ManagerJob Execute end ");
            }
        }
    }
}
