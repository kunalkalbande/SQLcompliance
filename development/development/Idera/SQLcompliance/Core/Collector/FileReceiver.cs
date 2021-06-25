using System;
using System.IO;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.TraceProcessing;


namespace Idera.SQLcompliance.Core.Collector
{
    public interface IFileReceiver
    {
        void StartReceiving(RemoteFileAttr attr, byte[] content, int length, byte[] binContent, bool morePages);
        void NextPage(byte[] content, int length, bool morePages);
        void Cancel();
    }

    public class FileReceiver : MarshalByRefObject, IFileReceiver
    {
        #region private members

        string filename = null;
        string binFilename = null;
        string instance;
        bool privUserTrace;
        bool SQLcmTrace;

        #endregion

        #region public methods
        public void StartReceiving(RemoteFileAttr attr, byte[] content, int length, byte[] binContent, bool morePages)
        {
            return;
        }

        public void StartReceiving(string fileName, string auditedInstance, string agentServer, bool isPrivUser, bool isSQLcmTrace, byte[] content, int length, byte[] binContent, bool morePages)
        {
            StartReceiving(fileName, auditedInstance, agentServer, isPrivUser, isSQLcmTrace, content, length, binContent, morePages, true);
        }

        public void StartReceiving(string fileName, string auditedInstance, string agentServer, bool isPrivUser, bool isSQLcmTrace, byte[] content, int length, byte[] binContent, bool morePages, bool isCompressedFile)
        {
            bool persisted = false;
            var extension = Path.GetExtension(fileName);
            if (!(string.IsNullOrWhiteSpace(extension)))
                fileName = Path.GetFileNameWithoutExtension(fileName);

            string traceFile = fileName + (string.IsNullOrWhiteSpace(extension) ? ".trz" : ".tr7z");
            string binFile = fileName + (string.IsNullOrWhiteSpace(extension) ? ".biz" : ".bi7z");

            //if (extension == ".trc" || extension == ".xel" || extension == ".sqlaudit" || extension == ".bin")
            if (!isCompressedFile)
            {
                traceFile = Path.ChangeExtension(traceFile, extension);
                binFile = Path.ChangeExtension(binFile, ".bin");
            }
            instance = auditedInstance;
            privUserTrace = isPrivUser;
            SQLcmTrace = isSQLcmTrace;

            // throw exception if server cant receive requests right now (usually due to shutting down)
            CollectionServer.Instance.CheckServerStatus();

            // make sure instance registered with Collector Service - it not an exception is thrown back to agent
            RemoteCollector.CheckForValidInstance(instance);

            ServerRecord.UpdateLastAgentContact(instance);

            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                     String.Format("StartReceiving - Instance: {0} File: {1}", instance, filename));

            try
            {
                // Skip empty files
                if (length > 0)
                {
                    RemoteCollector.CheckFileStatus(instance, traceFile, binFile);

                    // persist the trace and audit files
                    filename = TraceFile.PersistToDisk(CollectionServer.traceDirectory,
                                                                  traceFile,
                                                                  content,
                                                                  length);
                    binFilename = TraceFile.PersistToDisk(CollectionServer.traceDirectory,
                                                                  binFile,
                                                                  binContent);

                    if (!morePages)
                        // Call trace server to start new trace processing job.
                        TraceServer.Instance.GetAgentTrace(instance,
                                                            filename,
                                                            binFilename,
                                                            privUserTrace,
                                                            SQLcmTrace);
                    persisted = true;

                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                             String.Format("StartReceiving (Complete) - Instance: {0} File: {1}", instance, filename));

                }
            }
            catch (IOException ie)
            {
                // some other I/O problem.  Log this and throw the exception back to the agent
                ErrorLog.Instance.Write(ErrorLog.Level.Default, ie, true);
                throw ie;

            }
            catch (Exception e)
            {
                // Need to log this on the collection service side
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                         e.Message,
                                         e,
                                         ErrorLog.Severity.Warning,
                                         true);

                // Throw it back to the agent.
                throw e;
            }
            finally
            {
                // Error occurred.  Rollback.
                if (!persisted)
                {
                    CleanUp();
                }
            }
        }

        public void NextPage(byte[] content, int length, bool morePages)
        {
            bool persisted = false;

            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Append))
                {
                    //stream.Seek( 0, SeekOrigin.End );
                    stream.Write(content, 0, length);

                    // Make sure all of the buffers are flushed to OS
                    NativeMethods.FlushFileBuffers(stream.SafeFileHandle.DangerousGetHandle());
                    stream.Close();

                    // If done, create the trace job
                    if (!morePages)
                        TraceServer.Instance.GetAgentTrace(instance,
                                                           filename,
                                                           binFilename,
                                                           privUserTrace,
                                                           SQLcmTrace);
                }
                persisted = true;
            }
            catch (IOException ie)
            {
                // some other I/O problem.  Log this and throw the exception back to the agent
                ErrorLog.Instance.Write(ErrorLog.Level.Default, ie, true);
                throw ie;

            }
            catch (Exception e)
            {
                // Need to log this on the collection service side
                ErrorLog.Instance.Write(ErrorLog.Level.Default, e.Message, e, ErrorLog.Severity.Warning, true);

                // Throw it back to the agent.
                throw e;
            }
            finally
            {
                // Error occurred.  Rollback.
                if (!persisted)
                {
                    CleanUp();
                }
            }

        }

        public void Cancel()
        {
            CleanUp();
        }

        #endregion

        #region private methods

        private void CleanUp()
        {
            if (filename != null)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   "FileReceiver: Error occurred persisting files.  Deleting " + filename);
                try { File.Delete(filename); }
                catch { }

                // in case we create a job but things went wrong - kill i tnow
                RemoteCollector.DeleteTraceJob(instance, filename);
            }

            if (binFilename != null)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   "FileReceiver: Error occurred persisting files.  Deleting " + binFilename);
                try { File.Delete(binFilename); }
                catch { }
            }
        }

        #endregion
    }
}
