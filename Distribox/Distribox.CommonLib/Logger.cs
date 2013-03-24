using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;

namespace Distribox.CommonLib
{
    public static class Logger
    {
        private static ILog _logger = null;

        private static void EnsureInitialized()
        {
            if (_logger == null)
            {
                _logger = log4net.LogManager.GetLogger(typeof(Logger));
                log4net.Config.XmlConfigurator.Configure();
            }
        }

        public static void Warn(String format, params object[] args)
        {
            EnsureInitialized();
            _logger.Warn(String.Format(format, args));
        }

        public static void Info(String format, params object[] args)
        {
            EnsureInitialized();
            _logger.Info(String.Format(format, args));
        }

        public static void Debug(String format, params object[] args)
        {
            EnsureInitialized();
            _logger.Debug(String.Format(format, args));
        }

        public static void Error(String format, params object[] args)
        {
            EnsureInitialized();
            _logger.Error(String.Format(format, args));
        }

        public static void Fatal(String format, params object[] args)
        {
            EnsureInitialized();
            _logger.Fatal(String.Format(format, args));
        }
    }
}
