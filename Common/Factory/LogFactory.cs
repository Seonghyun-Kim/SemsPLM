using Common.Constant;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Factory
{
    public class LogFactory
    {
        private static object syncLock = new object();
        private static ILog iLog = null;

        private static ILog LOG
        {
            get
            {
                try
                {
                    if (iLog == null)
                    {
                        lock (syncLock)
                        {
                            if (iLog == null)
                            {
                                string assemblyFile = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
                                FileInfo file = new FileInfo(assemblyFile);

                                string Directory = file.Directory.FullName;

                                FileInfo fInfo = new FileInfo(Directory + @"\Config\SysLog.config");

                                log4net.Config.XmlConfigurator.Configure(fInfo);

                                iLog = log4net.LogManager.GetLogger("SysLog");
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }

                return iLog;
            }
        }

        public static void WriteLog(eLogSpec logType, string sMsg)
        {
            switch (logType)
            {
                case eLogSpec.debug:
                    #if DEBUG
                    LOG.Debug(sMsg);
                    #endif
                    break;
                case eLogSpec.error:
                    LOG.Error(sMsg);
                    break;
                case eLogSpec.info:
                    LOG.Info(sMsg);
                    break;
            }
        }
        public static void WriteLog(Exception ex)
        {
            string logMsg = string.Empty;
            logMsg = ex.ToString() + "\r\n" + ex.StackTrace;

            LOG.Error(logMsg);
        }
    }
}
