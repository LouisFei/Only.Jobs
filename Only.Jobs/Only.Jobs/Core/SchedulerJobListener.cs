using System;
using System.Text;
using Quartz;
using Only.Jobs.Core.Services;

namespace Only.Jobs.Core
{
    /// <summary>
    /// 计划任务监听器
    /// 监听任务执行,更新每个任务的信息，记录每个任务的运行日志。
    /// </summary>
    public class SchedulerJobListener : IJobListener
    {
        public void JobExecutionVetoed(IJobExecutionContext context) { }

        public void JobToBeExecuted(IJobExecutionContext context) { }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            //获得任务Key
            Guid backgroundJobId = Guid.Empty;
            Guid.TryParse(context.JobDetail.Key.Name, out backgroundJobId);
            //下次运行时间
            DateTime nextFireTimeUtc = TimeZoneInfo.ConvertTimeFromUtc(context.NextFireTimeUtc.Value.DateTime, TimeZoneInfo.Local);
            //最后运行时间
            DateTime fireTimeUtc = TimeZoneInfo.ConvertTimeFromUtc(context.FireTimeUtc.Value.DateTime, TimeZoneInfo.Local);

            //任务运行时长
            double totalSeconds = context.JobRunTime.TotalSeconds;
            string jobName = string.Empty;
            string logContent = string.Empty;

            if (context.MergedJobDataMap != null)
            {
                jobName = context.MergedJobDataMap.GetString("JobName");
                StringBuilder log = new StringBuilder();
                int i = 0;
                foreach (var item in context.MergedJobDataMap)
                {
                    string key = item.Key;
                    if (key.StartsWith("extend_"))
                    {
                        if (i > 0)
                        {
                            log.Append(",");
                        }
                        log.AppendFormat("{0}:{1}", item.Key, item.Value);
                        i++;
                    }
                }
                if (i > 0)
                {
                    logContent = string.Concat("[", log.ToString(), "]");
                }
            }

            if (jobException != null)
            {
                logContent = logContent + " EX:" + jobException.ToString();
            }

            //更新Job运行信息 
            new BackgroundJobService().UpdateBackgroundJobStatus(backgroundJobId,
                jobName,
                fireTimeUtc,
                nextFireTimeUtc,
                totalSeconds,
                logContent);
        }

        /// <summary>
        /// 监听器名称
        /// </summary>
        public string Name
        {
            get { return "SchedulerJobListener"; }
        }
    }
}
