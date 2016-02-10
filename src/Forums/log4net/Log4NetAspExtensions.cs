using System.IO;
using log4net;
using log4net.Config;
using Microsoft.AspNet.Hosting;

namespace Forums.log4net
{
    public static class Log4NetAspExtensions
    {
        public static void ConfigureLog4Net(this IHostingEnvironment env, string configFileRelativePath)
        {
            var basePath = env.WebRootPath + "\\..\\log4net";
            GlobalContext.Properties["appRoot"] = basePath;
            var a = new FileInfo(Path.Combine(basePath, configFileRelativePath));
            var i = a.Exists;
            var p = a.FullName;
            XmlConfigurator.Configure(a);
        }
    }
}