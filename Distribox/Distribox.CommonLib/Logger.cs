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

		/// <summary>
		/// Ensures the instance initialized.
		/// </summary>
        private static void EnsureInitialized()
        {
            if (_logger == null)
            {
                _logger = log4net.LogManager.GetLogger(typeof(Logger));
                log4net.Config.XmlConfigurator.Configure();
            }
        }

		/// <summary>
		/// Log warning message.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
        public static void Warn(String format, params object[] args)
        {
            EnsureInitialized();
            _logger.Warn(String.Format(format, args));
        }

		/// <summary>
		/// Log info message.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
        public static void Info(String format, params object[] args)
        {
            EnsureInitialized();
            _logger.Info(String.Format(format, args));
        }

		/// <summary>
		/// Log debug message.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
        public static void Debug(String format, params object[] args)
        {
            EnsureInitialized();
            _logger.Debug(String.Format(format, args));
        }

		/// <summary>
		/// Log error message.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
        public static void Error(String format, params object[] args)
        {
            EnsureInitialized();
            _logger.Error(String.Format(format, args));
        }

		/// <summary>
		/// Log fatal message.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
        public static void Fatal(String format, params object[] args)
        {
            EnsureInitialized();
            _logger.Fatal(String.Format(format, args));
        }
    }
}
