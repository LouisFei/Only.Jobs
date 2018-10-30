using System.Configuration;

using Common.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Topshelf;

using Only.Jobs.Core;

namespace Only.Jobs
{
    public sealed class ServiceRunner : ServiceControl, ServiceSuspend
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ServiceRunner));
        private readonly IScheduler scheduler;

        private string ServiceName
        {
            get { return ConfigurationManager.AppSettings.Get("ServiceName"); }
        }

        public ServiceRunner()
        {
            //默认的任务调度器
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
        }

        public bool Start(HostControl hostControl)
        {
            //添加任务监视器，以便更新每个任务的运行信息，记录它们的运行日志。
            scheduler.ListenerManager.AddJobListener(new SchedulerJobListener(), GroupMatcher<JobKey>.AnyGroup());
            //启动任务调度器
            scheduler.Start();

            //Job状态管控
            new QuartzManager().JobScheduler(scheduler);

            _logger.Info(string.Format("{0} Start", ServiceName));
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            scheduler.Shutdown(false);
            _logger.Info(string.Format("{0} Stop", ServiceName));
            return true;
        }

        public bool Continue(HostControl hostControl)
        {
            scheduler.ResumeAll();
            _logger.Info(string.Format("{0} Continue", ServiceName));
            return true;
        }

        public bool Pause(HostControl hostControl)
        {
            scheduler.PauseAll();
            _logger.Info(string.Format("{0} Pause", ServiceName));
            return true;
        }

    }
}
