using System;
using System.Reflection;
using System.IO;
using System.Configuration;

using Topshelf;
using log4net.Config;

namespace Only.Jobs
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));
            HostFactory.Run(x =>
            {
                x.UseLog4Net(); //内部使用log4net日志组件
                x.RunAsLocalSystem(); //服务使用NETWORK_SERVICE内置帐户运行
                x.Service<ServiceRunner>(); //服务
                x.SetDescription($"{ConfigurationManager.AppSettings.Get("ServiceName")} Ver:{Assembly.GetExecutingAssembly().GetName().Version.ToString()}");
                x.SetDisplayName(ConfigurationManager.AppSettings.Get("ServiceDisplayName"));
                x.SetServiceName(ConfigurationManager.AppSettings.Get("ServiceName"));
                x.EnablePauseAndContinue();
            });
        }
    }
}
