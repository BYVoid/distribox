//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.CommonLib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using log4net;
    using log4net.Config;
    
    /// <summary>
    /// Record Console.Write line style logs. Different log levels are provided, these levels can be turn on / off separately. Record levels are
    /// * Debug
    /// * Warn
    /// * Info
    /// * Error
    /// * Fatal
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The _logger.
        /// </summary>
        private static ILog logger = null;

        /// <summary>
        /// Log warning message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Warn(string format, params object[] args)
        {
            EnsureInitialized();
            logger.Warn(string.Format(format, args));
        }

        /// <summary>
        /// Log info message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Info(string format, params object[] args)
        {
            EnsureInitialized();
            logger.Info(string.Format(format, args));
        }

        /// <summary>
        /// Log debug message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Debug(string format, params object[] args)
        {
            EnsureInitialized();
            logger.Debug(string.Format(format, args));
            Console.WriteLine(format, args);
        }

        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Error(string format, params object[] args)
        {
            EnsureInitialized();
            logger.Error(string.Format(format, args));
        }

        /// <summary>
        /// Log fatal message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Fatal(string format, params object[] args)
        {
            EnsureInitialized();
            logger.Fatal(string.Format(format, args));
        }
        
        /// <summary>
        /// Ensures the instance initialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (logger == null)
            {
                logger = log4net.LogManager.GetLogger(typeof(Logger));
                log4net.Config.XmlConfigurator.Configure();
            }
        }
    }
}
