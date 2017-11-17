using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace api.core.Utils
{
    public class Logging
    {
        public static void InitLog4Net()
        {
            var log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));

            var repo = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }

        public static ILog GetLogger(string name)
        {
            return LogManager.GetLogger(Assembly.GetEntryAssembly(), name);
        }
    }
}
