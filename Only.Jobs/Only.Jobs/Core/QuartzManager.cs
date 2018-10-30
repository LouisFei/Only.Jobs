using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;

using Quartz;
using Only.Jobs.Core.Business.Info;
using Only.Jobs.Core.Services;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace Only.Jobs.Core
{
    public class QuartzManager
    {
        /// <summary>
        /// 从程序集中加载指定类
        /// </summary>
        /// <param name="assemblyName">含后缀的程序集名</param>
        /// <param name="className">含命名空间完整类名</param>
        /// <returns></returns>
        private Type GetClassInfo(string assemblyName, string className)
        {
            Type type = null;
            try
            {
                assemblyName = GetAbsolutePath(assemblyName);
                Assembly assembly = null;
                assembly = Assembly.LoadFrom(assemblyName);
                type = assembly.GetType(className, true, true);
            }
            catch
            {
            }
            return type;
        }

        /// <summary>
        /// 校验字符串是否为正确的Cron表达式
        /// </summary>
        /// <param name="cronExpression">带校验表达式</param>
        /// <returns></returns>
        public bool ValidExpression(string cronExpression)
        {
            return CronExpression.IsValidExpression(cronExpression);
        }

        /// <summary>
        ///  获取文件的绝对路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns></returns>
        public string GetAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                throw new ArgumentNullException("参数relativePath空异常！");
            }
            relativePath = relativePath.Replace("/", "\\");
            if (relativePath[0] == '\\')
            {
                relativePath = relativePath.Remove(0, 1);
            }
            if (HttpContext.Current != null)
            {
                return Path.Combine(HttpRuntime.AppDomainAppPath, relativePath);
            }
            else
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            }
        }


        /// <summary>
        /// Job调度
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="jobInfo"></param>
        public void ScheduleJob(IScheduler scheduler, BackgroundJobInfo jobInfo)
        {
            if(ValidExpression(jobInfo.CronExpression) == false)
            {
                //不正确的Cron表达式，无法启动该任务
                new BackgroundJobService().WriteBackgroundJoLog(jobInfo.BackgroundJobId, jobInfo.Name, DateTime.Now, jobInfo.CronExpression + "不是正确的Cron表达式,无法启动该任务");
                return;
            }

            Type type = GetClassInfo(jobInfo.AssemblyName, jobInfo.ClassName);
            if (type == null)
            {
                //任务类型无效，无法启动该任务
                new BackgroundJobService().WriteBackgroundJoLog(jobInfo.BackgroundJobId, jobInfo.Name, DateTime.Now, jobInfo.AssemblyName + jobInfo.ClassName + "无效，无法启动该任务");
                return;
            }

            IJobDetail job = new JobDetailImpl(jobInfo.BackgroundJobId.ToString(), jobInfo.BackgroundJobId.ToString() + "Group", type);
            job.JobDataMap.Add("Parameters", jobInfo.JobArgs);
            job.JobDataMap.Add("JobName", jobInfo.Name);

            CronTriggerImpl trigger = new CronTriggerImpl();
            trigger.CronExpressionString = jobInfo.CronExpression;
            trigger.Name = jobInfo.BackgroundJobId.ToString();
            trigger.Description = jobInfo.Description;
            trigger.StartTimeUtc = DateTime.UtcNow;
            trigger.Group = jobInfo.BackgroundJobId + "TriggerGroup";

            scheduler.ScheduleJob(job, trigger);
        }


        /// <summary>
        /// Job状态管控
        /// </summary>
        /// <param name="scheduler">调度器</param>
        public void JobScheduler(IScheduler scheduler)
        {
            //获取允许调度的Job集合
            List<BackgroundJobInfo> jobList = new BackgroundJobService().GetAllowScheduleJobInfoList();
            if (jobList == null || jobList.Count <= 0)
            {
                return;
            }

            foreach (var jobInfo in jobList)
            {
                var jobKey = new JobKey(jobInfo.BackgroundJobId.ToString(), jobInfo.BackgroundJobId.ToString() + "Group");

                if (scheduler.CheckExists(jobKey) == false) //判断任务是否已在任务调度器中。
                {
                    //新添加任务
                    if (jobInfo.State == 1 || jobInfo.State == 3)
                    {
                        //把（1启动，3启动中的）任务添加到任务调度器
                        ScheduleJob(scheduler, jobInfo);

                        if (scheduler.CheckExists(jobKey) == false)
                        {
                            new BackgroundJobService().UpdateBackgroundJobState(jobInfo.BackgroundJobId, 0); //停止
                        }
                        else
                        {
                            new BackgroundJobService().UpdateBackgroundJobState(jobInfo.BackgroundJobId, 1); //运行
                        }
                    }
                    else if (jobInfo.State == 5)
                    {
                        new BackgroundJobService().UpdateBackgroundJobState(jobInfo.BackgroundJobId, 0);//停止
                    }
                }
                else
                {
                    //已存在任务
                    if (jobInfo.State == 5) //停止中……
                    {
                        scheduler.DeleteJob(jobKey); //删除任务
                        new BackgroundJobService().UpdateBackgroundJobState(jobInfo.BackgroundJobId, 0);//停止
                    }
                    else if (jobInfo.State == 3) //正在启动中……
                    {
                        new BackgroundJobService().UpdateBackgroundJobState(jobInfo.BackgroundJobId, 1);//运行
                    }
                }
            }
        }
    }
}
