using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Security;

namespace Idera.SQLcompliance.Core.Agent
{
    /// <summary>
    /// Summary description for TraceCollector.
    /// </summary>
    public class TraceCollector : ServiceTimer
    {
        public class FileInfoComparer : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                int rc = 0;
                try
                {
                    rc = Comparer.DefaultInvariant.Compare(((FileInfo)x).LastWriteTime, ((FileInfo)y).LastWriteTime);
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            "An error occurred comparing two FileInfo.",
                                            e,
                                            true);
                }
                return rc;
            }

            #endregion
        }

        #region Private Data Members

        private FileShipper shipper;
        internal object syncObject = new object();
        private bool collecting = false;
        private DateTime lastCollectionTime;
        private int forceCollectionInterval;
        private SQLInstance sqlInstance;
        private string instanceName;

        #endregion

        #region Properties

        public DateTime LastCollectionTime
        {
            get { return lastCollectionTime; }
            set { lastCollectionTime = value; }
        }

        public string TraceDirectory
        {
            get { return shipper.TraceDirectory; }
            set { shipper.TraceDirectory = value; }
        }

        public int ForceCollectionInterval
        {
            get { return forceCollectionInterval; }
            set { forceCollectionInterval = value; }
        }

        #endregion

        #region Constructors

        public TraceCollector(SQLInstance inSqlInstance) : base(inSqlInstance.CollectionInterval * 60 * 1000)
        {
            sqlInstance = inSqlInstance;
            shipper = new FileShipper(sqlInstance.Name, sqlInstance.TraceDirectory, sqlInstance.IsSqlSecureDb);
            forceCollectionInterval = sqlInstance.ForceCollectionInterval;
            instanceName = sqlInstance.Name;
            TimerCallbackHandler = new TimerCallback(CollectTraces);
        }

        #endregion

        #region Public Methods

        //------------------------------------------------------------------
        // Start
        //------------------------------------------------------------------
        /// <summary>
        /// Start collecting traces.
        /// </summary>
        public override void
           Start()
        {
            base.Start();
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    "Trace collection for  " + instanceName + " started.");
        }

        //------------------------------------------------------------------
        // Stop
        //------------------------------------------------------------------
        /// <summary>
        /// Stop collecting traces
        /// </summary>
        public override void
           Stop()
        {
            base.Stop();
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    "Trace collector for " + instanceName + " stopped.");
        }


        //------------------------------------------------------------------
        // CollectTraces
        //------------------------------------------------------------------
        /// <summary>
        /// Delegate that handles the collect trace event.
        /// </summary>
        public void
           CollectTraces(object obj)
        {
            // Check if service is stopping.  Do nothing if it is.
            if (SQLcomplianceAgent.Instance.Stopping)
                return;

            if (instanceName == null || instanceName == "")
            {
                // Do nothing if server is not specified.
                ErrorLog.Instance.Write("Trace collector: cannot collect traces without specifying the server instance",
                                        ErrorLog.Severity.Error);
                return;
            }

            if (!sqlInstance.IsEnabled)
            {
                // Do nothing if server is not specified.
                ErrorLog.Instance.Write("Trace collector: auditing is disabled so collection will not be processed.", ErrorLog.Severity.Warning);
                return;
            }

            if (sqlInstance.TraceCollectionStopped)
                return;

            lock (syncObject)
            {
                if (!collecting)
                {
                    collecting = true;
                }
                else // Already a collector thread working on it.  Do nothing.
                {
                    try
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                "There is already a collector thread running.  Aborting new collector thread.");
                    }
                    catch
                    {
                    }
                    return;
                }
            }

            try
            {
                lock (sqlInstance.syncObj)
                {
                    if (SQLcomplianceAgent.Instance.Stopping)
                        return;
                    Collect();
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        "Error collecting traces for " + sqlInstance.Name,
                                        e.Message,
                                        ErrorLog.Severity.Warning);
            }
            finally
            {
                lock (syncObject)
                {
                    collecting = false;
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            "Trace collecting flag reset.");
                }
            }
        }

        //------------------------------------------------------------------
        // TimeToForceTraceCollection
        //------------------------------------------------------------------
        public bool
           TimeToForceTraceCollection()
        {
            DateTime next = sqlInstance.LastCollectionTime + new TimeSpan(0,
                                                                          0,
                                                                          forceCollectionInterval,
                                                                          0,
                                                                          0);
            // Time to force collection
            if (DateTime.Compare(DateTime.Now, next) >= 0)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Forcing trace collection...");
                return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        //-------------------------------------------------------------------------------------------
        /// <summary>
        /// Checks if running traces have rolled over.
        /// </summary>
        /// <returns></returns>
        //-------------------------------------------------------------------------------------------
        private ArrayList
           GetRunningTraces()
        {
            ArrayList traceFiles = new ArrayList();

            /*
            // Get the running trace info
            TraceInfo [] startupTraces = TraceInfo.GetStartupSPStartedTraces( sqlInstance.Alias);
            if( startupTraces != null )
            {
               foreach( TraceInfo trace in startupTraces )
                  traceFiles.Add( trace );
            }
            */

            int[] runningTraces;
            using (SqlConnection conn = sqlInstance.GetConnection())
            {
                runningTraces = SPHelper.GetRunningTraces(conn);
            }

            ArrayList ids = new ArrayList(runningTraces);

            TraceInfo[] agentTraces = TraceInfo.GetAgentStartedTraceInfo(sqlInstance.Alias);
            if (agentTraces != null)
            {
                foreach (TraceInfo trace in agentTraces)
                {
                    traceFiles.Add(trace);
                    if (ids.BinarySearch(trace.TraceId) < 0)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                String.Format(CoreConstants.Warning_TraceStopped, trace.ToString()),
                                                ErrorLog.Severity.Informational);
                        trace.Status = TraceStatus.Close;
                    }
                }
            }

            // 5.4_4.1.1_Extended Events
            try
            {
                int[] runningTracesXE;
                using (SqlConnection conn = sqlInstance.GetConnection())
                {
                    runningTracesXE = SPHelper.GetRunningTracesXE(conn);
                }

                ArrayList idsXE = new ArrayList(runningTracesXE);

                TraceInfo[] agentTracesXE = TraceInfo.GetAgentStartedTraceInfoXE(sqlInstance.Alias);

                if (agentTracesXE != null)
                {
                    foreach (TraceInfo trace in agentTracesXE)
                    {
                        traceFiles.Add(trace);
                        if (idsXE.BinarySearch(trace.TraceId) < 0)
                        {
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                    String.Format(CoreConstants.Warning_TraceStopped, trace.ToString()),
                                                    ErrorLog.Severity.Informational);
                            trace.Status = TraceStatus.Close;
                        }
                    }
                }
            }
            catch (Exception ex) { }
            // Need to be implemented

            AddLastDefaultTrace(traceFiles);
            return traceFiles;
        }
        // add last default trace file to currently running traces list
        private void AddLastDefaultTrace(ArrayList traceFiles)
        {
            try
            {
                var lastDefaultTrace = GetLastDefaultTrace();
                if (lastDefaultTrace != null)
                {
                    traceFiles.Add(lastDefaultTrace);
                }

            }
            catch (Exception e)
            {
                string errorMessage = string.Format("{0}: {1}", CoreConstants.Error_CantReceiveDefaultTrace, e.Message);
                ErrorLog.Instance.Write(errorMessage, e, true);
            }
        }

        //-------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceAlias"></param>
        /// <param name="excludedTraces"></param>
        /// <returns></returns>
        //-------------------------------------------------------------------------------------------
        private ArrayList
           GetLeftoverTraces(
           string instanceAlias,
           Hashtable excludedTraces)
        {
            DirectoryInfo traceDirectoryInfo = new DirectoryInfo(sqlInstance.TraceDirectory);
            FileInfo[] fileInfoList = traceDirectoryInfo.GetFiles(instanceAlias + "*.trc");
            FileInfo[] fileInfoListXE = traceDirectoryInfo.GetFiles("XE" + instanceAlias + "*.xel");
            FileInfo[] auditLogsFileInfoList = traceDirectoryInfo.GetFiles("AL" + instanceAlias + "*.sqlaudit");

            if (fileInfoList == null)
                return new ArrayList();

            if (excludedTraces == null)
                excludedTraces = new Hashtable();

            ArrayList tmpFileList = new ArrayList(fileInfoList);

            if (fileInfoListXE != null && fileInfoListXE.Length > 0)
            {
                for (int i = 0; i < fileInfoListXE.Length; i++)
                {
                    tmpFileList.Add(fileInfoListXE[i]);
                }
            }

            if (auditLogsFileInfoList != null && auditLogsFileInfoList.Length > 0)
                tmpFileList.AddRange(auditLogsFileInfoList);

            tmpFileList.Sort(new FileInfoComparer());

            Hashtable leftoverTraces = new Hashtable();
            TraceFileNameAttributes attr;
            ArrayList finalList = new ArrayList();
            StringBuilder msg = new StringBuilder("Sending leftover trace files from the last trace collection.  File List:\n");

            foreach (FileInfo file in tmpFileList)
            {
                if (!excludedTraces.ContainsKey(file.FullName))
                {
                    attr = TraceFileNameAttributes.GetNameAttributes(file);
                    try
                    {
                        leftoverTraces.Add(file.Name, attr);
                        finalList.Add(attr);
                        msg.AppendFormat("{0}\n", file.Name);
                    }
                    catch
                    {
                    }
                }
            }

            if (finalList.Count > 0)
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        msg.ToString());

            return finalList;
        }


        //------------------------------------------------------------------
        // ShipTraceFiles
        //------------------------------------------------------------------
        /// <summary>
        /// Send the trace files starting from the sequence number to the repository server.  Returns
        /// true if all the trace files are shipped.
        /// </summary>
        /// <param name="traceName">Original trace file name when it is created.</param>
        /// <param name="startingSequenceNumber">The sequence number of the first file to send</param>
        /// <returns></returns>
        private bool
           ShipTraceFiles(
           string traceName,
           int startingSequenceNumber
           )
        {
            // Collect the traces
            ArrayList files = GetFileNamesToCollect(traceName, startingSequenceNumber);
            if (files == null || files.Count == 0)
                return false;

            // Ship the traces
            bool isCompressedFileFlag = SQLcomplianceAgent.Instance.IsCompressedFile;
            string[] shippedFiles = shipper.ShipFiles(isCompressedFileFlag,files);

            // Delete the traces sent
            //DeleteShippedFiles( shippedFiles ); /* Files are deleted right after they are shipped */

            // Returns true if at least one file shipped
            return shippedFiles != null && shippedFiles.Length > 0;
        }


        //------------------------------------------------------------------
        // GetFileNamesToCollect
        //------------------------------------------------------------------
        private ArrayList
           GetFileNamesToCollect(
           string traceFileName,
           int startsWith
           )
        {
            if (traceFileName == null)
                return null;

            FileInfo traceFileInfo = new FileInfo(traceFileName);
            string directoryName = traceFileInfo.DirectoryName;
            DirectoryInfo traceDirectory = new DirectoryInfo(directoryName);
            FileInfo[] fileInfoList = traceDirectory.GetFiles(String.Format("{0}*.trc", traceFileInfo.Name));
            FileInfo[] fileInfoListXE = traceDirectory.GetFiles(String.Format("{0}*.xel", traceFileInfo.Name));
            FileInfo[] auditLogsFileInfoList = traceDirectory.GetFiles(String.Format("{0}*.sqlaudit", traceFileInfo.Name));


            ArrayList tmpFiles = new ArrayList(fileInfoList);
            if (fileInfoListXE != null && fileInfoListXE.Length > 0)
            {
                for (int i = 0; i < fileInfoListXE.Length; i++)
                {
                    tmpFiles.Add(fileInfoListXE[i]);
                }
            }

            if (auditLogsFileInfoList != null && auditLogsFileInfoList.Length > 0)
                tmpFiles.AddRange(auditLogsFileInfoList);
            tmpFiles.Sort(new FileInfoComparer());
            ArrayList files = new ArrayList();
            StringBuilder msg = new StringBuilder("Sending current trace files.  File List:\n");

            // Collecting everything
            for (int i = 0; i < tmpFiles.Count; i++)
            {
                TraceFileNameAttributes attrs = TraceFileNameAttributes.GetNameAttributes(fileInfoList[i]);
                if (attrs.Sequence >= startsWith)
                {
                    files.Add(attrs);
                    msg.AppendFormat("{0}\n", fileInfoList[i].Name);
                }
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    msg.ToString());

            return files;
        }

        //-------------------------------------------------------------------------
        // Collect traces
        //-------------------------------------------------------------------------
        private void Collect()
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                    String.Format("Collecting traces for {0}", instanceName));

            ArrayList currentTraces = null;
            ArrayList currentTracesXE = null;
            bool shipperError = false;

            // Sometimes the timer triggers the event too fast.  Make adjustment for it.
            DateTime newCollectionTime = DateTime.Now.AddSeconds(-2);

            string stateMsg = "collecting traces";
            Hashtable excludedList = new Hashtable();
            int startTime = Environment.TickCount;

            try
            {
                sqlInstance.detector.CheckTraceStatus(true, false); //instance lock is already acquired.

                if (sqlInstance.IsEnabled)
                {
                    stateMsg = "collecting running traces information";

                    currentTraces = GetRunningTraces();
                    //currentTracesXE = GetRunningTracesXE();
                    if (currentTraces != null && currentTraces.Count > 0)
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                String.Format("There are {0} currently running traces.",
                                                              currentTraces.Count));
                    else
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                "There are no currently running traces.");
                    // Create a list of traces to be excluded from
                    // being check again in GetLeftoverTraces() 
                    if (currentTraces != null &&
                        currentTraces.Count > 0)
                    {
                        try
                        {
                            DirectoryInfo traceDirectoryInfo = new DirectoryInfo(sqlInstance.TraceDirectory);
                            FileInfo[] fileInfoList;

                            foreach (TraceInfo file in currentTraces)
                            {
                                string tmpFileName = Path.GetFileNameWithoutExtension(file.FileName);
                                fileInfoList = traceDirectoryInfo.GetFiles(tmpFileName + "*.trc");

                                if (fileInfoList != null)
                                {
                                    foreach (FileInfo traceFile in fileInfoList)
                                    {
                                        excludedList.Add(traceFile.FullName, file);
                                    }
                                }

                                fileInfoList = traceDirectoryInfo.GetFiles(tmpFileName + "*.xel");

                                if (fileInfoList != null)
                                {
                                    foreach (FileInfo traceFile in fileInfoList)
                                    {
                                        excludedList.Add(traceFile.FullName, file);
                                    }
                                }

                                //5.5 Audit Logs
                                fileInfoList = traceDirectoryInfo.GetFiles(tmpFileName + "*.sqlaudit");
                                if (fileInfoList != null)
                                {
                                    foreach (FileInfo traceFile in fileInfoList)
                                    {
                                        excludedList.Add(traceFile.FullName, file);
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                stateMsg = "collecting information for left over traces";
                ArrayList leftoverTraces = GetLeftoverTraces(sqlInstance.Alias, excludedList);
                if (leftoverTraces != null && leftoverTraces.Count > 0)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            String.Format("There are {0} leftover traces from the previous trace collection to be sent.",
                                                          leftoverTraces.Count));

                    stateMsg = "sending traces to the collection server";

                    string[] shippedFiles = null;
                    try
                    {
                        bool isCompressedFileFlag = SQLcomplianceAgent.Instance.IsCompressedFile;
                        shippedFiles = shipper.ShipFiles(isCompressedFileFlag,leftoverTraces);
                    }
                    catch
                    {
                        shipperError = true;
                    }

                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            String.Format("{0} leftover traces are sent to the server for processing.",
                                                          shippedFiles == null ? 0 : shippedFiles.Length));

                    // Check if the service is stopping
                    if (SQLcomplianceAgent.Instance.Stopping)
                        return;
                }

                // Look for rollover trace files to collect
                if (!shipperError && currentTraces != null && currentTraces.Count > 0)
                {
                    bool restartAll = false;
                    bool checkForceCollection = false;
                    int idx = 0;
                    stateMsg = "sending rollover traces to the collection server";

                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            "Sending rollover files for current traces."); ;

                    foreach (TraceInfo traceInfo in currentTraces)
                    {
                        try
                        {
                            traceInfo.Rollover = ShipTraceFiles(traceInfo.FileName, 0);
                        }
                        catch (Exception se)
                        {
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                    "Error sending rollover trace files.",
                                                    se.Message,
                                                    ErrorLog.Severity.Warning);

                            break;
                        }

                        // Something is wrong with the traces, restart them all.
                        if (traceInfo.Status == TraceStatus.Close)
                        {
                            restartAll = true; // Trace stopped.  Restart them all.
                        }
                        else
                        {
                            // Check if we need to force the trace collection
                            if (!traceInfo.Rollover)
                            {
                                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                        String.Format("There are no rollover files for trace {0}",
                                                                      traceInfo.TraceId));
                                checkForceCollection = true;
                            }
                            else
                            {
                                try
                                {
                                    if (sqlInstance.rollover != null)
                                        sqlInstance.rollover[idx] = true;
                                }
                                catch (Exception e)
                                {
                                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                            String.Format("Error updating rollover flags for currently running traces.  Index = {0}.", idx),
                                                            e,
                                                            true);
                                }
                            }
                            idx++;
                        }
                    }

                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            String.Format("Rollover files for {0} of the current traces are sent.",
                                                          idx));

                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Rollover traces sent ({0}ms).  Checking whether need to restart traces for " + instanceName + ".",
                                                          Environment.TickCount - startTime));
                    startTime = Environment.TickCount;
                    //----------------
                    // Restart traces
                    //----------------
                    if (sqlInstance.IsEnabled)
                    {
                        stateMsg = "checking running traces status";
                        string[] newUserList = UserList.GetNewUserList(sqlInstance.AuditConfig.Instance,
                                                                       sqlInstance.AuditConfig.AuditedUserGroups,
                                                                       sqlInstance.AuditConfig.AuditedServerRoles,
                                                                       sqlInstance.AuditConfig.AuditedUsers);

                        bool newUsersFound = false;
                        if (newUserList != null)
                        {
                            sqlInstance.AuditConfig.AuditedUsers = newUserList;
                            newUsersFound = true;
                        }

                        if (sqlInstance.AuditConfig.AuditDBList != null)
                        {
                            foreach (
                               DBAuditConfiguration dbConfig in
                                  sqlInstance.AuditConfig.AuditDBList)
                            {
                                string[] newDbUsers =
                                   UserList.GetNewUserList(sqlInstance.AuditConfig.Instance,
                                                           dbConfig.AuditedUserGroups,
                                                           dbConfig.AuditedServerRoles,
                                                           dbConfig.AuditedUsers);
                                if (newDbUsers != null)
                                {
                                    dbConfig.AuditedUsers = newDbUsers;
                                    newUsersFound = true;
                                }
                            }
                        }


                        // Update the last collection time if all the files are shipped.
                        if ((checkForceCollection || newUsersFound) && !restartAll)
                        {
                            if ((TimeToForceTraceCollection() || newUsersFound) &&
                                !SQLcomplianceAgent.Instance.Stopping)
                            {
                                bool recreate = false;

                                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                        String.Format("Forcing collection for {0}", instanceName));

                                if (newUsersFound)
                                {
                                    recreate = true;
                                }
                                else if (sqlInstance.rollover != null &&
                                         sqlInstance.rollover.Length > 0)
                                {
                                    for (int i = 0; i < sqlInstance.rollover.Length; i++)
                                    {
                                        ((TraceInfo)currentTraces[i]).Rollover = sqlInstance.rollover[i];
                                    }
                                }
                                else
                                    recreate = true;
                                stateMsg = "restarting the traces to force trace collection";

                                sqlInstance.RestartTraces(recreate, currentTraces);
                                sqlInstance.LastCollectionTime = newCollectionTime;
                            }
                        }
                        else
                        {
                            sqlInstance.RestartTraces();
                            sqlInstance.LastCollectionTime = newCollectionTime;
                        }
                    }
                }
                else
                {
                    if (!SQLcomplianceAgent.Instance.Stopping && sqlInstance.IsEnabled)
                    {
                        // No trace is running or traces are tampered, restart them.
                        stateMsg = "restarting the traces";
                        sqlInstance.RestartTraces();
                    }
                }

                // Check if the service is stopping
                if (SQLcomplianceAgent.Instance.Stopping)
                    return;

                stateMsg = "sending the trace collected message to the collection server";

                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("Restart trace checked for {0} ({1}ms).",
                                                      instanceName, Environment.TickCount - startTime));
                startTime = Environment.TickCount;

                SQLcomplianceAgent.Instance.SendTraceCollectedMessage(instanceName);

                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("TracesCollected message sent for {0} ({1}ms).",
                                                      instanceName, Environment.TickCount - startTime));
            }
            catch (IOException ie)
            {
                // ignore
                ErrorLog.Instance.Write(ie);
            }
            catch (Exception e)
            {
                //
                ErrorLog.Instance.Write(String.Format("{0} for {1} encounters an error when {2}: {3}",
                                                      GetType(),
                                                      instanceName,
                                                      stateMsg,
                                                      e.Message),
                                        ErrorLog.Severity.Error);
            }
            finally
            {
            }
        }

        private TraceInfo GetLastDefaultTrace()
        {
            object result;
            using (var connection = sqlInstance.GetConnection())
            {
                var query = new StringBuilder("SELECT path FROM sys.traces WHERE is_default = 1");

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = query.ToString();
                    result = command.ExecuteScalar();

                    if (result == null ||
                        result == DBNull.Value)
                    {
                        return null;
                    }
                }
            }

            const string prefix = "log_";
            const string suffix = ".trc";

            // get default trace file info
            var fileInfo = new FileInfo(result.ToString());

            // make last default trace file info
            var lastDefaultTraceNumber = Convert.ToInt32(fileInfo.Name.Replace(prefix, string.Empty).Replace(suffix, string.Empty)) - 1;
            var lastDefaultTraceFile = Path.Combine(fileInfo.DirectoryName, string.Format("{0}{1}{2}", prefix, lastDefaultTraceNumber, suffix));
            var lastDefaultTraceFileInfo = new FileInfo(lastDefaultTraceFile);
            if (!lastDefaultTraceFileInfo.Exists)
                return null;

            // don't send trace if already sent
            var lastTraceNumber = SQLcomplianceAgent.GetLastSentDefaultTraceNumber(sqlInstance.Name);
            if (lastTraceNumber == lastDefaultTraceNumber)
                return null;

            // SQLCM-6059 Format for miliseconds need to have 3 digits. Following tracefiles are getting processed in TraceFileNameAttributes.cs of core\Agent as well as trace register utility.
            var newTraceName = string.Format("{0}{1}{2}",
                                            TraceFileNameAttributes.ComposeTraceFileName(sqlInstance.Alias, (int)TraceLevel.Server, (int)AuditCategory.ServerStartStopEvent, sqlInstance.ConfigVersion),
                                            DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"),
                                            suffix);
            newTraceName = Path.Combine(sqlInstance.TraceDirectory, newTraceName);

            // copy last default trace file to trace directory
            File.Copy(lastDefaultTraceFile, newTraceName, true);

            SQLcomplianceAgent.SetLastSentDefaultTraceNumber(sqlInstance.Name, lastDefaultTraceNumber);

            // make fake last default trace file info
            var traceInfo = new TraceInfo
            {
                FileName = newTraceName.Replace(suffix, string.Empty),
                MaxSize = lastDefaultTraceFileInfo.Length,
                Options = TraceOption.RollOver,
                Rollover = false,
                StartTime = lastDefaultTraceFileInfo.CreationTimeUtc,
                Status = TraceStatus.Close,
                StopTime = lastDefaultTraceFileInfo.LastWriteTimeUtc,
                TraceId = 1,
                TraceNumber = 1,
                Version = sqlInstance.ConfigVersion
            };
            return traceInfo;
        }

        #endregion
    }
}