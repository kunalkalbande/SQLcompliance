using System;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Security.AccessControl;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.TraceProcessing;
using Idera.SQLcompliance.Core.Collector;
using Microsoft.Win32;

namespace Idera.SQLcompliance.Core.Agent
{
    /// <summary>
    /// Summary description for FileShipper.
    /// </summary>
    public class FileShipper
    {
        #region Private Fields

        //ServerRecord    server;
        int serverPort;
        string server;
        string instanceName;
        string agentServer;
        string traceDirectory;
        string remotingURL;
        bool isSqlSecureDb;

        #endregion

        #region Properties

        public string TraceDirectory
        {
            get { return traceDirectory; }
            set { traceDirectory = value; }
        }

        #endregion
        
        #region Constructors

        public FileShipper()
        {
            serverPort = SQLcomplianceAgent.Instance.ServerPort;
            server = SQLcomplianceAgent.Instance.Server;
            agentServer = SQLcomplianceAgent.Instance.AgentServer;
            remotingURL = String.Format("tcp://{0}:{1}/{2}",
                                          server,
                                          serverPort,
                                          typeof(RemoteCollector).Name);
        }


        public FileShipper(
              string inInstanceName,
              string inTraceDirectory,
              bool inIsSqlSecureDb
           ) : this()
        {
            instanceName = inInstanceName;
            traceDirectory = inTraceDirectory;
            isSqlSecureDb = inIsSqlSecureDb;
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        public string[]
           ShipFiles(
              bool isCompressedFileFlag,
              ArrayList files
           )
        {
            if (files == null || files.Count == 0)
                return null;

            ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, "Sending " + files.Count.ToString() + " files to the server");
            ArrayList shippedFiles = new ArrayList();
            foreach (TraceFileNameAttributes file in files)
            {
                try
                {
                    // Service is stopping stop shipping files
                    if (SQLcomplianceAgent.Instance.Stopping)
                        break;
                    SendFileToServer(isCompressedFileFlag,file);
                    shippedFiles.Add(file.FullName);
                }
                catch (TraceFileExistsException te)
                {
                    shippedFiles.Add(file.FullName);
                    File.Delete(file.FullName);
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, te, true);
                }
                catch (CoreException ce)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                               ce.Message,
                                               ce,
                                               ErrorLog.Severity.Error,
                                               true);
                    // Stop shipping the file
                    throw ce;
                }
                catch (IOException)
                {
                    // Caused by SQL Server still writing to the file, ignore.
                }
                catch (SocketException se)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                             String.Format("Network error.  Stop sending file {0} to the server.",
                                                           file.FullName),
                                             se,
                                             ErrorLog.Severity.Error,
                                             true);
                    // Throw an exception so the whole trace shipping process is stopped.
                    throw new CoreException(String.Format(CoreConstants.Exception_Format_NetworkErrorSendingFileToServer, se.Message), se);
                }
                catch (Exception e)
                {
                    // Log the error
                    // TODO: Raise an alert to let either the agent or the server handles it
                    CoreException ce = new CoreException(String.Format(CoreConstants.Exception_Format_ErrorSendingFileToServer, file.FullName), e);
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                             e.Message,
                                             ce,
                                             ErrorLog.Severity.Error,
                                             true);

                    // Stop shipping the file
                    throw e;

                }
            }

            ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, shippedFiles.Count.ToString() + " files sent to the server");

            return (string[])shippedFiles.ToArray(typeof(string));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool
           SendFileToServer(
              bool isCompressedFileFlag,
              TraceFileNameAttributes file
           )
        {
            bool sent;
            FileInfo info = new FileInfo(file.FullName);
            if (!info.Exists)
                return true;
            if (CoreConstants.UseClientActivatedFileTransfer)
                sent = SendSmallFile(isCompressedFileFlag, file, info);
            else
            {
                string fileInUseExceptionMessage = string.Format(@"The process cannot access the file '{0}' because it is being used by another process.", file.FullName);

                // Due to .NET remoting defect, in some cluster machines with multiple nodes SendLargeFile method fails.
                // So we force shipping trace files to collection services by SendSmallFile method.
                try
                {
                    sent = SendLargeFile(isCompressedFileFlag, file, info);
                }
                catch (Exception exception)
                {
                    sent = false;

                    // ignore logging of file in use exception
                    if (!(exception is IOException && exception.Message.Equals(fileInUseExceptionMessage, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                string.Format("SendLargeFile method failed to ship file {1}. Trying alternate SendSmallFile method. {0} Exception Details: {0}{2}", Environment.NewLine, file, exception.Message),
                                                exception,
                                                ErrorLog.Severity.Error,
                                                true);
                    }

                }

                // SendLargeFile failed, use alternate SendSmallFile metod
                if (!sent)
                {
                    try
                    {
                        sent = SendSmallFile(isCompressedFileFlag, file, info);
                    }
                    catch (Exception exception)
                    {
                        // ignore logging of file in use exception
                        if (!(exception is IOException && exception.Message.Equals(fileInUseExceptionMessage, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                    string.Format("Alternate SendSmallFile method also failed to ship file {1}.{0} Exception Details: {0}{2}", Environment.NewLine, file, exception.Message),
                                                    exception,
                                                    ErrorLog.Severity.Error,
                                                    true);
                        }
                        throw; // throw back exception to calling methd
                    }

                    // if SendSmallFile method succeds, save theis configuration for further use
                    if (!CoreConstants.UseClientActivatedFileTransfer && sent)
                    {
                        CoreConstants.UseClientActivatedFileTransfer = true;
                        SQLcomplianceAgent.Instance.SetClientActivatedFileTransfer(true);
                    }

                }
            }

            if (sent)
                File.Delete(file.FullName);

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     info.Name + " is sent to collection server for processing.");

            return sent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RemoteCollector
           GetRemoteCollector()
        {
            ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug,
                                     String.Format("remotingURL: {0}", remotingURL),
                                     ErrorLog.Severity.Informational);


            // Get the remote proxy for the collector
            return CoreRemoteObjectsProvider.RemoteCollector(server, serverPort);

        }

        #endregion

        #region Private Methods

        //------------------------------------------------------------------------------
        // SendSmallFile - Send small trace file using the faster in-memory compression
        //------------------------------------------------------------------------------
        private bool SendSmallFile(bool isCompressedFileFlag, TraceFileNameAttributes file, FileInfo info)
        {
            string compressedTraceFileName = info.Name;
            string compressedAuditFileName = Path.GetFileNameWithoutExtension(info.Name) + ".bin";
            if (isCompressedFileFlag)
            {
                compressedTraceFileName = Path.ChangeExtension(info.Name, ".tr7z");
                compressedAuditFileName = Path.GetFileNameWithoutExtension(info.Name) + ".bi7z";
            }          
            string tempFileName = file.InstanceAlias;
            if (tempFileName.StartsWith("XE") || tempFileName.StartsWith("AL"))
            {
                tempFileName = tempFileName.Substring(2);
            }


            string tempAuditFile = Path.Combine(traceDirectory, tempFileName + ".bin");

            if (!File.Exists(tempAuditFile))
            {
                ErrorLog.Instance.Write(CoreConstants.Exception_MissingAuditConfigurationFile + tempAuditFile,
                                         ErrorLog.Severity.Error);
                return false;
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                              "Sending " + compressedTraceFileName + " for processing (client activated).");


            RemoteFile remoteFile = new RemoteFile();
            remoteFile.Server = agentServer;
            remoteFile.Instance = instanceName;
            remoteFile.TraceFilename = compressedTraceFileName;
            remoteFile.AuditFileName = compressedAuditFileName;
            if (isCompressedFileFlag)
            {
                remoteFile.TraceStream = TraceFile.CompressToMemoryStream(info.FullName);
                remoteFile.AuditStream = TraceFile.CompressToMemoryStream(tempAuditFile);
            }
            else
            {
                remoteFile.TraceStream = TraceFile.ConvertToMemoryStream(info.FullName);
                remoteFile.AuditStream = TraceFile.ConvertToMemoryStream(tempAuditFile);
            }            
            if (file.AuditLevel == (int)TraceLevel.User)
                remoteFile.IsPrivilegedUserTrace = true;
            remoteFile.IsSqlSecureTrace = isSqlSecureDb;

            GetRemoteCollector().SendFile(remoteFile);

            return true;
        }

        //------------------------------------------------------------------------------
        // SendLargeFile - Send large trace file using the client activated remote object
        //                 in chunks
        //------------------------------------------------------------------------------
        private bool SendLargeFile(bool isCompressedFileFlag, TraceFileNameAttributes file, FileInfo info)
        {
            string compressedTraceFileName = info.Name;
            string compressedAuditFileName = Path.GetFileNameWithoutExtension(info.Name) + ".bin";
            if (isCompressedFileFlag)
            {
                compressedTraceFileName = Path.ChangeExtension(info.Name, ".tr7z");
                compressedAuditFileName = Path.GetFileNameWithoutExtension(info.Name) + ".bi7z";
            } 
            string tmpFilename = String.Empty;

            string tempFileName = file.InstanceAlias;
            if (tempFileName.StartsWith("XE") || tempFileName.StartsWith("AL"))
            {
                tempFileName = tempFileName.Substring(2);
            }


            string tempAuditFile = Path.Combine(traceDirectory, tempFileName + ".bin");

            if (!File.Exists(tempAuditFile))
            {
                string msg = CoreConstants.Exception_MissingAuditConfigurationFile + tempAuditFile;
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         msg,
                                         ErrorLog.Severity.Error);
                throw new Exception(msg);
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Sending " + compressedTraceFileName + " for processing (server activated).");
            RemoteFileAttr attr = new RemoteFileAttr();
            attr.BinFilename = compressedAuditFileName;
            attr.Filename = compressedTraceFileName;
            attr.Instance = instanceName;
            attr.ServerName = agentServer;
            if (file.AuditLevel == (int)TraceLevel.User)
                attr.IsPrivUserTrace = true;
            attr.IsSQLcmTrace = isSqlSecureDb;


            // Activate an instance of the remote object
            FileReceiver receiver;

            try
            {
                string typeName = typeof(FileReceiver).Name;
                string url = String.Format("tcp://{0}:{1}/{2}", server, serverPort, "SQLcompliance");
                object[] urlAttrs = { new UrlAttribute(url) };
                ObjectHandle handle = Activator.CreateInstance("SQLcomplianceCore", "Idera.SQLcompliance.Core.Collector.FileReceiver", urlAttrs);
                receiver = (FileReceiver)handle.Unwrap();
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Remote object " + typeName + " activated");
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "FileShipper: error creating remote FileReceiver.", e.Message, ErrorLog.Severity.Error);
                throw e;
            }

            int byteCount;
            int total = 0;

            try
            {
                FileStream source;
                if (isCompressedFileFlag)
                {
                    tmpFilename = TraceFile.CompressToDisk(info.FullName);
                }
                else
                {
                    tmpFilename = info.FullName;
                }
                source = new FileStream(tmpFilename, FileMode.Open, FileAccess.Read);
                // Send the trace file in chunks
                using (source)
                {
                    byte[] buffer = new byte[32767];
                    bool finished = true;
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Sending traces files in chunks.");

                    // Send the first chunk
                    if ((byteCount = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        byte[] binContent;
                        if (isCompressedFileFlag)
                        {
                            binContent = TraceFile.CompressToMemoryStream(tempAuditFile).ToArray();
                        }
                        else
                        {
                            binContent = TraceFile.ConvertToMemoryStream(tempAuditFile).ToArray();
                        }                        
                        finished = (source.Position == source.Length);
                        receiver.StartReceiving(Path.GetFileName(tmpFilename),
                                                 instanceName,
                                                 agentServer,
                                                 file.AuditLevel == (int)TraceLevel.User,
                                                 isSqlSecureDb,
                                                 buffer,
                                                 byteCount,
                                                 binContent,
                                                 !finished,
                                                 isCompressedFileFlag);

                        total += byteCount;
                        ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, String.Format("Start: {0} bytes sent to the server.", byteCount));
                    }

                    // Send the remaining chunks
                    while (!finished)
                    {
                        byteCount = source.Read(buffer, 0, buffer.Length);
                        if (byteCount > 0)
                        {
                            finished = (source.Position == source.Length);
                            receiver.NextPage(buffer, byteCount, !finished);
                            total += byteCount;
                            ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, String.Format("Next: {0} bytes sent to the server.", byteCount));
                        }
                        else
                        {
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                    "Premature file end reached while sending files in chunks.",
                                                    ErrorLog.Severity.Warning);
                            finished = true;
                        }
                    }
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Done sending traces files in chunks.");
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, "An error occured when sending files.", e, true);
                // Clean up whatever is left on the server.  Retry later.
                if (total > 0)
                    receiver.Cancel();
                throw e;
            }
            finally
            {
                try
                {
                    if (tmpFilename != String.Empty)
                        File.Delete(tmpFilename);
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                             String.Format("Error deleting the temp file: {0}", tmpFilename),
                                             e,
                                             ErrorLog.Severity.Warning);
                }
            }

            return true;
        }

        #endregion
    }
}
