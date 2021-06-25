using System;
using System.Reflection;
using System.Text;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Idera.SQLcompliance.Core
{

	public static partial class Logger
	{
		private static volatile bool			initialized;
		private static volatile int				logfileRecycleLimit;
		private static bool						uniqueFilename = false;

		private static			string			applicationName;
		private static			FileAppender	fileAppender;
		private static			ILog			log;


		#region Nested Types

		private sealed class CustomPatternLayout : log4net.Layout.PatternLayout
		{
			public CustomPatternLayout(string pattern, string header, string footer) : base(pattern)
			{
				this.Header = header;
				this.Footer = footer;
			}

			public override string Header
			{
				get
				{
					return base.Header.Replace("%datetime", DateTime.Now.ToString());
				}
				set
				{
					base.Header = value;
				}
			}

			public override string Footer
			{
				get
				{
					return base.Footer.Replace("%datetime", DateTime.Now.ToString());
				}
				set
				{
					base.Footer = value;
				}
			}
		}

		#endregion

		#region Properties

		public static bool UniqueFilename
		{
			get
			{
				return uniqueFilename;
			}
			set
			{
				uniqueFilename = value;
			}
		}

		#endregion

		#region Methods

        public static void Initialize(string applicationName, int totalNumLogFiles, long sizeLimit)
		{
			if (!initialized)
			{
				initialized = true;

				Logger.applicationName = applicationName;
				Logger.log = LogManager.GetLogger(applicationName);
                InitializeFileAppender(totalNumLogFiles, sizeLimit);
                
//#if DEBUG
//                log4net.Util.LogLog.QuietMode = false;
//                log4net.Util.LogLog.InternalDebugging = true;
//#endif
			}
		}

        private static void InitializeFileAppender(int totalNumLogFiles, long sizeLimit)
		{
			if (fileAppender != null)
			{
				BasicConfigurator.RemoveAppender(fileAppender);

				fileAppender.Close();
				fileAppender = null;
			}
			
			// If a flag has been set for a bit that we log on, then enable the appender.
			string assemblyName = Assembly.GetEntryAssembly().GetName().Name;
			Version assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;

            string basePath = ".\\Logs";
			StringBuilder logFilename = new StringBuilder(basePath);
            //if (uniqueFilename)
            //    logFilename.AppendFormat(@"\{0}_{1}_{2}.log", assemblyName, DateTime.Now.ToString("yyyyMMddHHmmss"), Process.GetCurrentProcess().Id);
            //else
				logFilename.AppendFormat(@"\{0}.log", assemblyName);

            if (totalNumLogFiles > 0) // Create a rolling file appender
			{
				RollingFileAppender r = new RollingFileAppender();
                r.MaxSizeRollBackups = 10;
                r.MaxFileSize = 0x2000000; // bytes (32MB)
				r.RollingStyle = RollingFileAppender.RollingMode.Size;
				r.AppendToFile = false;
				fileAppender = r;
			}
			else // Create non-rolling appender that will grow indefinately (classical SQLsafe log behavior)
			{
				fileAppender = new FileAppender();
				fileAppender.AppendToFile = true;
			}

			fileAppender.File = logFilename.ToString();
			fileAppender.Threshold = Level.Debug;
            fileAppender.Layout = new CustomPatternLayout("%date{yyyy-MM-dd HH:mm:ss.ff} \t|%-1.1level| %message%newline",
				string.Format("{2}********** Logging of {0} version {1} starting %datetime **********{2}", assemblyName, assemblyVersion, Environment.NewLine),
				string.Format("{2}********** Logging of {0} version {1} stopping %datetime **********{2}", assemblyName, assemblyVersion, Environment.NewLine)
			);				

			fileAppender.ActivateOptions();
			BasicConfigurator.Configure(fileAppender);
		}

		#endregion

		#region Logger Methods

		public static void Info(string message)
		{
				log.Info(message);
		}

		#endregion
	}
}
