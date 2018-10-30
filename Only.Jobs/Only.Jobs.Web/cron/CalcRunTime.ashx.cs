using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using Quartz;

namespace Only.Jobs.Web.cron
{
    /// <summary>
    /// CalcRunTime 的摘要说明
    /// </summary>
    public class CalcRunTime : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain; charset=utf-8";

            var cron = context.Request.QueryString["CronExpression"];

            //var list = GetCronSchdule("0 */1 * * * ?", 10, DateTimeOffset.Now);
            //context.Response.Write("[\"2018 / 10 / 29 15:21:01\",\"2018 / 10 / 29 15:21:02\",\"2018 / 10 / 29 15:22:01\",\"2018 / 10 / 29 15:22:02\",\"2018 / 10 / 29 15:23:01\"]");
            var list = GetCronSchdule(cron, 5, DateTimeOffset.Now);
            var list2 = list.Select(a => a.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(list2));
        }


        //GetCronSchdule("0 */1 * * * ?", 10,DateTimeOffset.Now);

        /// <summary>
        /// Corn表达式的运行时间
        /// </summary>
        /// <param name="cron">表达式</param>
        /// <param name="times">计算次数</param>
        /// <param name="startTime">开始时间</param>
        /// <returns></returns>
        public static IList<DateTimeOffset?> GetCronSchdule(String cron, int times, DateTimeOffset startTime)
        {
            var list = new List<DateTimeOffset?>();
            if (!CronExpression.IsValidExpression(cron))
            {
                return list;
            }

            try
            {

                ITrigger trigger1 = TriggerBuilder.Create()
                                            .WithCronSchedule(cron)
                                            .StartAt(DateTime.Now)
                                            .Build();

                DateTimeOffset? lastTime = startTime;
                //Console.WriteLine(LastTime.ToString());
                //list.Add(lastTime);
                for (int i = 0; i < times; i++)
                {
                    DateTimeOffset? s = trigger1.GetFireTimeAfter(lastTime);
                    lastTime = s;
                    //Console.WriteLine(((DateTimeOffset)s).AddHours(8).ToString());
                    list.Add(s.Value.AddHours(8));
                }
            }
            catch { }

            return list;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}