using System;
using System.Collections;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Security;
using Microsoft.Win32;

namespace Idera.SQLcompliance.Core.Event
{

   // Class for keeping the trace information
   public class TraceInfo
   {
      #region Properties

      private int traceNumber;
      /// <summary>
      /// 
      /// </summary>
      public  int TraceNumber
      {
         get { return traceNumber; }
         set { traceNumber = value; }
      }

      private int traceId;
      /// <summary>
      /// 
      /// </summary>
      public  int TraceId
      {
         get { return traceId; }
         set { traceId = value; }
      }

      private TraceStatus status;
      /// <summary>
      /// 
      /// </summary>
      public TraceStatus    Status
      {
         get { return status; }
         set { status = value; }
      }

      private string        fileName;
      /// <summary>
      /// 
      /// </summary>
      public string         FileName
      {
         get { return fileName; }
         set { fileName = value; }
      }

      private long          maxSize;
      /// <summary>
      /// 
      /// </summary>
      public long           MaxSize
      {
         get { return maxSize; }
         set { maxSize = value; }
      }

      private DateTime      startTime;
      /// <summary>
      /// 
      /// </summary>
      public DateTime       StartTime
      {
         get { return startTime; }
         set { startTime = value; }
      }

      private DateTime      stopTime;
      /// <summary>
      /// 
      /// </summary>
      public DateTime       StopTime
      {
         get { return stopTime; }
         set { stopTime = value; }
      }

      private TraceOption options;
      /// <summary>
      /// 
      /// </summary>
      public TraceOption    Options
      {
         get { return options; }
         set { options = value; }
      }

      private int version;
      /// <summary>
      /// 
      /// </summary>
      public int Version
      {
         get { return version; }
         set { version = value; }
      }

      private bool rollover = false;

      public bool Rollover
      {
         get { return rollover; }
         set { rollover = value; }
      }


      #endregion

      #region Constructors

      public TraceInfo ()
      {
         status = TraceStatus.Unknown;
         fileName = null;
         maxSize = -1;
         options = TraceOption.Unknown;
         traceNumber = -1;
      }

      #endregion

      #region Static Methods

      #region Public Static Methods


      public static TraceInfo []
         GetStartupSPStartedTraces(
         string instanceAlias
         )
      {
         return RetrieveTraceInfo( instanceAlias, CoreConstants.Agent_StartupSPTraces );
      }


      public static void
         DeleteStartupSPStartedTraceInfo (
         string instanceAlias
         )
      {
         DeleteTraceInfo( instanceAlias, CoreConstants.Agent_StartupSPTraces );
      }

      public static TraceInfo []
         GetAgentStartedTraceInfo (
         string instanceAlias
         )
      {
         return RetrieveTraceInfo( instanceAlias, CoreConstants.Agent_AgentStartedTraces );
      }
      // 5.4_4.1.1_Extended Events
      public static TraceInfo[]
          GetAgentStartedTraceInfoXE(
          string instanceAlias
          )
      {
          return RetrieveTraceInfoXE(instanceAlias, CoreConstants.Agent_AgentStartedTraces);
      }

      public static void
         SaveAgentStartedTraceInfo (
         string instanceAlias,
         string agentVersion,
         TraceInfo [] traces
         )
      {
         PersistTraceInfo( instanceAlias, CoreConstants.Agent_AgentStartedTraces, agentVersion, traces );
      }

      // 5.4_4.1.1_Extended Events
      public static void
          SaveAgentStartedTraceInfoXE(
          string instanceAlias,
          string agentVersion,
          TraceInfo[] traces
          )
      {
          if(traces != null && traces.Length>0)
            PersistTraceInfoXE(instanceAlias, CoreConstants.Agent_AgentStartedTraces, agentVersion, traces);
          else
              DeleteTraceInfoFromRegistryXE(instanceAlias, CoreConstants.Agent_AgentStartedTraces);
      }

      public static void
         DeleteAgentStartedTraceInfo (
         string instanceAlias
         )
      {
         DeleteTraceInfo( instanceAlias, CoreConstants.Agent_AgentStartedTraces );
      }

      public static TraceInfo []
         GetStoppedTraceInfo (
         string instanceAlias
         )
      {
         return RetrieveTraceInfo( instanceAlias, CoreConstants.Agent_StoppedTraces );
      }

      public static void
         SaveStoppedTraceInfo (
         string instanceAlias,
         TraceInfo [] traces
         )
      {
         // Checks if there is any traces to save
         if( traces == null )
            return;

         Hashtable newList = new Hashtable();

         for( int i = 0; i < traces.Length; i++ )
         {
            try
            {
               newList.Add( traces[i].FileName, traces[i] );
            }
            catch( ArgumentException )
            {
#if DEBUG
               // the trace is already in the table.  Ignore it.
               ErrorLog.Instance.Write( String.Format( "{0} is already in the stopped trace list", traces[i].FileName ));
#endif
            }
            catch( Exception e )
            {
               // Something is wrong
               throw new CoreException( CoreConstants.Exception_ErrorCreatingNewStoppedTraceList, e );
            }
         }

         // Get the saved stopped trace info
         TraceInfo [] savedTraces = RetrieveTraceInfo( instanceAlias, CoreConstants.Agent_StoppedTraces );

         if( savedTraces != null )
         {
            // Merge the saved ones with the new ones.  Note that need to keep all the stopped trace info 
            // until trace collector finish sending all these traces to the repository server before deleting
            // them.
            for( int i = 0; i < savedTraces.Length; i++ )
            {
               try
               {
                  if( !newList.Contains( savedTraces[i].FileName ) )
                     newList.Add( savedTraces[i].FileName, savedTraces[i] );
               }
               catch( Exception e )
               {
                  // Something is wrong
                  throw new CoreException( CoreConstants.Exception_ErrorCreatingNewStoppedTraceList, e );
               }
            }
         }
         
         // Save the merged list
         TraceInfo [] list = new TraceInfo[newList.Count];

         IDictionaryEnumerator enumerator = newList.GetEnumerator();
         int idx = 0;
         while( enumerator.MoveNext() )
         {
            list[idx++] = (TraceInfo)enumerator.Value;
         }

         if( list.Length == 0 )
            DeleteTraceInfo( instanceAlias, CoreConstants.Agent_StoppedTraces );  // Delete the value if no stopped traces
         else
            PersistTraceInfo( instanceAlias, 
                              CoreConstants.Agent_StoppedTraces, 
                              CoreConstants.AgentVersion, 
                              list );
         newList.Clear();
      }

      /// <summary>
      /// Deletes all the stopped trace info.
      /// </summary>
      public static void
         DeleteStoppedTraceInfo (
         string instanceAlias
         )
      {
         DeleteTraceInfo( instanceAlias, CoreConstants.Agent_StoppedTraces );
      }

      /// <summary>
      /// Delete the trace info in the list.
      /// </summary>
      /// <param name="traces"></param>
      public static void
         DeleteStoppedTraceInfo (
         string instanceAlias,
         TraceInfo[] traces
         )
      {
         // Just return if there is nothing to delete
         if( traces == null )
            return;

         Hashtable newList = new Hashtable();

         // Get the current stopped trace info
         TraceInfo [] savedTraces = RetrieveTraceInfo( instanceAlias, CoreConstants.Agent_StoppedTraces );

         for( int i = 0; i < savedTraces.Length; i++ )
         {
            try
            {
               newList.Add( savedTraces[i].FileName, savedTraces[i] );
            }
            catch( ArgumentException )
            {
               // Already in the table.  Ignore it.
#if DEBUG
               ErrorLog.Instance.Write( String.Format( "{0} is already in the stopped trace list", savedTraces[i].FileName ));
#endif
            }
            catch( Exception e )
            {
               // Something is wrong
               throw new CoreException( CoreConstants.Exception_ErrorCreatingNewStoppedTraceList, e );
            }
         }

         for( int i = 0; i < traces.Length; i++ )
         {
            // Remove from the saved list if it exists
            newList.Remove( traces[i] );
         }
         
         // Save the merged list
         TraceInfo [] list = new TraceInfo[newList.Count];

         IDictionaryEnumerator enumerator = newList.GetEnumerator();
         int idx = 0;
         while( enumerator.MoveNext() )
         {
            list[idx++] = (TraceInfo)enumerator.Value;
         }

         PersistTraceInfo( instanceAlias, CoreConstants.Agent_StoppedTraces, CoreConstants.AgentVersion, list );
         newList.Clear();
      }


      /// <summary>
      /// Persist the traces information in the registry or a file.
      /// </summary>
      /// <param name="type"></param>
      /// <param name="agentVersion"></param>
      /// <param name="traces"></param>

      public static void
         PersistTraceInfo (
         string instanceAlias,
         string type,
         string agentVersion,
         TraceInfo [] traces
         )
      {

         // Storing the info in the registry.  Add other ways to persist
         // it if necessary.
         StoreTraceInfoInRegistry( instanceAlias, type, agentVersion, traces );
      }

      // 5.4_4.1.1_Extended Events
      public static void
        PersistTraceInfoXE(
        string instanceAlias,
        string type,
        string agentVersion,
        TraceInfo[] traces
        )
      {

          // Storing the info in the registry.  Add other ways to persist
          // it if necessary.
          StoreTraceInfoInRegistryXE(instanceAlias, type, agentVersion, traces);
      }

      /// <summary>
      /// Get the persisted trace info
      /// </summary>
      /// <param name="type">type can be AgentStartedTraces, StartupSPStartedTraces and Stopped traces.  There
      /// can be more types added in the future.</param>
      /// <returns></returns>
      public static TraceInfo []
         RetrieveTraceInfo (
         string instanceAlias,
         string type
         )
      {
         // Retrieve the info from the registry.  Add other ways to retrieve
         // it if needed.  Note that this method should be updated when
         // PersistTraceInfo() is updated.
         return GetTraceInfoFromRegistry( instanceAlias, type );
      }

      // 5.4_4.1.1_Extended Events
      /// <summary>
      /// Get the persisted trace info
      /// </summary>
      /// <param name="type">type can be AgentStartedTraces, StartupSPStartedTraces and Stopped traces.  There
      /// can be more types added in the future.</param>
      /// <returns></returns>
      public static TraceInfo[]
         RetrieveTraceInfoXE(
         string instanceAlias,
         string type
         )
      {
          // Retrieve the info from the registry.  Add other ways to retrieve
          // it if needed.  Note that this method should be updated when
          // PersistTraceInfo() is updated.
          return GetTraceInfoFromRegistryXE(instanceAlias, type);
      }


      public static void
         DeleteTraceInfo (
         string instanceAlias,
         string type
         )
      {
         DeleteTraceInfoFromRegistry( instanceAlias, type );
         DeleteTraceInfoFromRegistryXE(instanceAlias, type);    //5.4_4.1.1_Extended Events
      }

      #endregion

      #region Private

      /// <summary>
      /// Saves the traces' information for the server in the registry.
      /// </summary>
      /// <param name="valueName"></param>
      /// <param name="agentVersion"></param>
      /// <param name="traces"></param>
      private static void
         StoreTraceInfoInRegistry (
         string       instanceAlias,
         string       valueName,
         string       agentVersion,
         TraceInfo [] traces
         )
      {
         if( traces == null )
            return;

         RegistryKey   regKey        = null;
         RegistryKey   regSubkey     = null;
         RegistryKey   serverSubkey     = null;

         ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                  String.Format( "Saving agent trace information in registry entry {0}",
                                                 valueName ));

         try
         {
            regKey = Registry.LocalMachine;
            regSubkey = regKey.OpenSubKey( SQLcomplianceAgent.AgentRegistryKey, true );

            serverSubkey = regSubkey.CreateSubKey( instanceAlias );

            if( traces.Length == 0 )  // No traces
            {
               serverSubkey.SetValue( valueName, new string[0]);
               return;
            }

            ArrayList values = new ArrayList();
            values.Add( String.Format( "{0}", traces[0].StartTime.ToUniversalTime().ToString() ) );
            values.Add( traces[0].Version.ToString() );
            values.Add( agentVersion );
            for( int i = 0; i < traces.Length; i++ )
            {
               values.Add( traces[i].TraceNumber.ToString() );
               values.Add( traces[i].TraceId.ToString() );
               values.Add( traces[i].FileName );
            }

            serverSubkey.SetValue( valueName, values.ToArray( typeof(string) ));
            values.Clear();
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "Agent trace information saved.");
         }
         catch ( SqlComplianceSecurityException se )
         {
            string msg = String.Format( "{0} does not have the permission to access the registry: {1}",
               se.Source, se.Message );
            ErrorLog.Instance.Write( msg, ErrorLog.Severity.Error );
         }
         catch ( Exception e )
         {
            ErrorLog.Instance.Write( "An error occurred saving agent trace information into registry.", 
                                     e,
                                     true );
            throw e;
         }
         finally
         {
            if( serverSubkey != null )
               serverSubkey.Close();

            if( regSubkey != null )
               regSubkey.Close();

            if( regKey != null )
               regKey.Close();
         }
      }

      // 5.4_4.1.1_Extended Events
      /// <summary>
      /// Saves the traces' information for the server in the registry.
      /// </summary>
      /// <param name="valueName"></param>
      /// <param name="agentVersion"></param>
      /// <param name="traces"></param>
      private static void
         StoreTraceInfoInRegistryXE(
         string instanceAlias,
         string valueName,
         string agentVersion,
         TraceInfo[] traces
         )
      {
          if (traces == null)
              return;

          RegistryKey regKey = null;
          RegistryKey regSubkey = null;
          RegistryKey serverSubkey = null;

          ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                   String.Format("Saving agent extended event file information in registry entry {0}",
                                                  valueName));

          try
          {
              regKey = Registry.LocalMachine;
              regSubkey = regKey.OpenSubKey(SQLcomplianceAgent.AgentRegistryKey, true);

              serverSubkey = regSubkey.CreateSubKey("XE"+ instanceAlias);

              if (traces.Length == 0)  // No traces
              {
                  serverSubkey.SetValue(valueName, new string[0]);
                  return;
              }

              ArrayList values = new ArrayList();
              values.Add(String.Format("{0}", traces[0].StartTime.ToUniversalTime().ToString()));
              values.Add(traces[0].Version.ToString());
              values.Add(agentVersion);
              for (int i = 0; i < traces.Length; i++)
              {
                  if (traces[i].FileName.Contains("XE"))
                  {
                      values.Add(traces[i].TraceNumber.ToString());
                      values.Add(traces[i].TraceId.ToString());
                      values.Add(traces[i].FileName);
                  }
              }
              if(values != null && values.Count > 3)
                serverSubkey.SetValue(valueName, values.ToArray(typeof(string)));
              values.Clear();
              ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                       "Agent extended event file information saved.");
          }
          catch (SqlComplianceSecurityException se)
          {
              string msg = String.Format("{0} does not have the permission to access the registry: {1}",
                 se.Source, se.Message);
              ErrorLog.Instance.Write(msg, ErrorLog.Severity.Error);
          }
          catch (Exception e)
          {
              ErrorLog.Instance.Write("An error occurred saving agent trace information into registry.",
                                       e,
                                       true);
              throw e;
          }
          finally
          {
              if (serverSubkey != null)
                  serverSubkey.Close();

            if( regSubkey != null )
               regSubkey.Close();

            if( regKey != null )
               regKey.Close();
         }
      }


      /// <summary>
      /// Returns all the information for the server's traces stored in the registry.
      /// </summary>
      /// <param name="valueName">Registry value name to read the trace info</param>
      private static TraceInfo []
         GetTraceInfoFromRegistry(
         string instanceAlias,
         string valueName )
      {
         RegistryKey   regKey        = null;
         RegistryKey   regSubkey     = null;
         RegistryKey   serverSubkey  = null;
         TraceInfo [] traces         = null;

         if( instanceAlias == null )
            return null;

         try
         {
            regKey = Registry.LocalMachine;
            regSubkey = regKey.OpenSubKey(SQLcomplianceAgent.AgentRegistryKey);
            if( regKey == null )
               return null;
            serverSubkey = regSubkey.OpenSubKey( instanceAlias );
            if( serverSubkey == null )
               return null;
            
            // The startup SP writes the tracing information into the registry as
            // a multi-string value.  Parse it here.

            string [] values = null;
            try
            {
               values = (string [])serverSubkey.GetValue( valueName );
            }
            catch{} // ignore if the value is not there.

            if( values != null && values.Length >= 3 )
            {
                    DateTime startTime;
                    // Startup SP creation time
                    try
                    {
                        startTime = DateTime.Parse(values[0]);

                    }
                    catch
                    {
                        startTime = DateTime.ParseExact(values[0], "dd/MM/yyyy hh:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("EN-us"));
                    }
               // Trace configurtion version
               int version = int.Parse( values[1] );
               // SQLsecure Agent's version number
               // Each trace has three entries
               int traceCount = ( values.Length - 3 )/3;
               traces = new TraceInfo[traceCount];
               for( int i = 0, j = 3; i < traceCount; i++ )
               {
                  traces[i] = new TraceInfo();
                  traces[i].TraceNumber = int.Parse( values[j++] );
                  traces[i].TraceId = int.Parse( values[j++] );
                  traces[i].FileName = values[j++];
                  traces[i].StartTime = startTime;
                  traces[i].Version = version;
               }
            }
   
         }
         catch ( SqlComplianceSecurityException se )
         {
            string msg = String.Format( "{0} does not have the permission to access the registry: {1}",
               se.Source, se.Message );
            ErrorLog.Instance.Write( msg, ErrorLog.Severity.Error );
         }
         catch (Exception e)
         {
            // TODO: wrap it with SQLsecure exception and give it detailed error message
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     CoreConstants.Exception_ErrorReadingTraceInfoFromRegistry,
                                     e );
            // propagate the exception to the caller
            throw e;
         }
         finally
         {
            if( serverSubkey != null )
               serverSubkey.Close();

            if( regSubkey != null )
               regSubkey.Close();

            if( regKey != null )
               regKey.Close();
         }

         return traces;
      }


	  // 5.4_4.1.1_Extended Events
      /// <summary>
      /// Returns all the information for the server's traces stored in the registry.
      /// </summary>
      /// <param name="valueName">Registry value name to read the trace info</param>
	  
      private static TraceInfo []
         GetTraceInfoFromRegistryXE(
         string instanceAlias,
         string valueName )
      {
         RegistryKey   regKey        = null;
         RegistryKey   regSubkey     = null;
         RegistryKey   serverSubkey  = null;
         TraceInfo [] traces         = null;

         if( instanceAlias == null )
            return null;
        
         try
         {
            regKey = Registry.LocalMachine;
            regSubkey = regKey.OpenSubKey(SQLcomplianceAgent.AgentRegistryKey);
            if( regKey == null )
               return null;
            serverSubkey = regSubkey.OpenSubKey("XE"+ instanceAlias );
            if( serverSubkey == null )
               return null;
            
            // The startup SP writes the tracing information into the registry as
            // a multi-string value.  Parse it here.

            string [] values = null;
            try
            {
               values = (string [])serverSubkey.GetValue( valueName );
            }
            catch{} // ignore if the value is not there.

            if( values != null && values.Length >= 3 )
            {
               // Startup SP creation time
               DateTime startTime = DateTime.Parse( values[0] );
               // Trace configurtion version
               int version = int.Parse( values[1] );
               // SQLsecure Agent's version number
               // Each trace has three entries
               int traceCount = ( values.Length - 3 )/3;
               traces = new TraceInfo[traceCount];
                
                   for (int i = 0, j = 3; i < traceCount; i++)
                   {
                       traces[i] = new TraceInfo();
                       traces[i].TraceNumber = int.Parse(values[j++]);
                       traces[i].TraceId = int.Parse(values[j++]);
                       traces[i].FileName = values[j++];
                       traces[i].StartTime = startTime;
                       traces[i].Version = version;
                   }


               
            }
   
         }
         catch ( SqlComplianceSecurityException se )
         {
            string msg = String.Format( "{0} does not have the permission to access the registry: {1}",
               se.Source, se.Message );
            ErrorLog.Instance.Write( msg, ErrorLog.Severity.Error );
         }
         catch (Exception e)
         {
            // TODO: wrap it with SQLsecure exception and give it detailed error message
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     CoreConstants.Exception_ErrorReadingTraceInfoFromRegistry,
                                     e );
            // propagate the exception to the caller
            throw e;
         }
         finally
         {
            if( serverSubkey != null )
               serverSubkey.Close();

            if( regSubkey != null )
               regSubkey.Close();

            if( regKey != null )
               regKey.Close();
         }

         return traces;
      }



      /// <summary>
      /// Deletes the server's trace info stored in the registry.
      /// 
      /// </summary>
      /// <param name="valueName"></param>
      private static void
         DeleteTraceInfoFromRegistry(
         string instanceAlias,
         string valueName )
      {
         RegistryKey   regKey        = null;
         RegistryKey   regSubkey     = null;
         RegistryKey   serverSubkey  = null;

         try
         {
            regKey = Registry.LocalMachine;
            regSubkey = regKey.OpenSubKey(SQLcomplianceAgent.AgentRegistryKey);
            serverSubkey = regSubkey.OpenSubKey( instanceAlias, true );
            serverSubkey.DeleteValue( valueName );    
         }
         catch( ArgumentException )
         {
            // Do nothing.  The value is not there.
         }
         catch( UnauthorizedAccessException ue )
         {
            string msg = String.Format( "The registry key {0} is read-only.\n  Exception: {1}",
               serverSubkey.Name, ue.Message );
            ErrorLog.Instance.Write( msg, ErrorLog.Severity.Error );
         }
         catch ( SqlComplianceSecurityException se )
         {
            string msg = String.Format( "{0} does not have the permission to access the registry: {1}",
               se.Source, se.Message );
            ErrorLog.Instance.Write( msg, ErrorLog.Severity.Error );
         }
         catch (Exception e)
         {
            // TODO: wrap it with SQLsecure exception and give it detailed error message
            ErrorLog.Instance.Write( e );
            // propagate the exception to the caller
            throw e;
         }
         finally
         {
            if( serverSubkey != null )
               serverSubkey.Close();

            if( regSubkey != null )
               regSubkey.Close();

            if( regKey != null )
               regKey.Close();

         }
      }

      // 5.4_4.1.1_Extended Events
      /// <summary>
      /// Deletes the server's trace info stored in the registry.
      /// 
      /// </summary>
      /// <param name="valueName"></param>
      private static void
         DeleteTraceInfoFromRegistryXE(
         string instanceAlias,
         string valueName)
      {
          RegistryKey regKey = null;
          RegistryKey regSubkey = null;
          RegistryKey serverSubkey = null;

          try
          {
              regKey = Registry.LocalMachine;
              regSubkey = regKey.OpenSubKey(SQLcomplianceAgent.AgentRegistryKey);
              serverSubkey = regSubkey.OpenSubKey("XE"+instanceAlias, true);
              if(serverSubkey!=null)
                serverSubkey.DeleteValue(valueName);
          }
          catch (ArgumentException)
          {
              // Do nothing.  The value is not there.
          }
          catch (UnauthorizedAccessException ue)
          {
              string msg = String.Format("The registry key {0} is read-only.\n  Exception: {1}",
                 serverSubkey.Name, ue.Message);
              ErrorLog.Instance.Write(msg, ErrorLog.Severity.Error);
          }
          catch (SqlComplianceSecurityException se)
          {
              string msg = String.Format("{0} does not have the permission to access the registry: {1}",
                 se.Source, se.Message);
              ErrorLog.Instance.Write(msg, ErrorLog.Severity.Error);
          }
          catch (Exception e)
          {
              // TODO: wrap it with SQLsecure exception and give it detailed error message
              ErrorLog.Instance.Write(e);
              // propagate the exception to the caller
              throw e;
          }
          finally
          {
              if (serverSubkey != null)
                  serverSubkey.Close();

              if (regSubkey != null)
                  regSubkey.Close();

              if (regKey != null)
                  regKey.Close();

          }
      }

      #endregion

      #endregion

      #region Public Methods

      public override string ToString()
      {
         return String.Format( "File: {0}, Trace ID: {1}", fileName, traceId );
      }

      #endregion

      #region Audit Logs

      // 5.5 Audit Logs
      public static void
          SaveAgentStartedAuditLogsInfo(
          string instanceAlias,
          string agentVersion,
          TraceInfo[] traces
          )
      {
          if (traces != null && traces.Length > 0)
              PersistAuditLogInfo(instanceAlias, CoreConstants.Agent_AgentStartedTraces, agentVersion, traces);
          else
              DeleteAuditLogsInfoFromRegistry(instanceAlias, CoreConstants.Agent_AgentStartedTraces);
      }
      /// <summary>
      /// Deletes the server's audit logs info stored in the registry.      /// 
      /// </summary>
      /// <param name="valueName"></param>
      private static void
         DeleteAuditLogsInfoFromRegistry(
         string instanceAlias,
         string valueName)
      {
          RegistryKey regKey = null;
          RegistryKey regSubkey = null;
          RegistryKey serverSubkey = null;

          try
          {
              regKey = Registry.LocalMachine;
              regSubkey = regKey.OpenSubKey(SQLcomplianceAgent.AgentRegistryKey);
              serverSubkey = regSubkey.OpenSubKey("AL" + instanceAlias, true);
              if (serverSubkey != null)
                  serverSubkey.DeleteValue(valueName);
          }
          catch (ArgumentException)
          {
              // Do nothing.  The value is not there.
          }
          catch (UnauthorizedAccessException ue)
          {
              string msg = String.Format("The registry key {0} is read-only.\n  Exception: {1}",
                 serverSubkey.Name, ue.Message);
              ErrorLog.Instance.Write(msg, ErrorLog.Severity.Error);
          }
          catch (SqlComplianceSecurityException se)
          {
              string msg = String.Format("{0} does not have the permission to access the registry: {1}",
                 se.Source, se.Message);
              ErrorLog.Instance.Write(msg, ErrorLog.Severity.Error);
          }
          catch (Exception e)
          {
              // TODO: wrap it with SQLsecure exception and give it detailed error message
              ErrorLog.Instance.Write(e);
              // propagate the exception to the caller
              throw e;
          }
          finally
          {
              if (serverSubkey != null)
                  serverSubkey.Close();

              if (regSubkey != null)
                  regSubkey.Close();

              if (regKey != null)
                  regKey.Close();

          }
      }

      // 5.5 Audit Logs
      public static void
        PersistAuditLogInfo(
        string instanceAlias,
        string type,
        string agentVersion,
        TraceInfo[] traces
        )
      {

          // Storing the info in the registry.  Add other ways to persist
          // it if necessary.
          StoreAuditLogsInfoInRegistry(instanceAlias, type, agentVersion, traces);
      }

      /// <summary>
      /// Saves the AuditLogs' information for the server in the registry.
      /// </summary>
      /// <param name="valueName"></param>
      /// <param name="agentVersion"></param>
      /// <param name="auditLogs"></param>
      private static void
         StoreAuditLogsInfoInRegistry(
         string instanceAlias,
         string valueName,
         string agentVersion,
         TraceInfo[] auditLogs
         )
      {
          if (auditLogs == null)
              return;
          RegistryKey regKey = null;
          RegistryKey regSubkey = null;
          RegistryKey serverSubkey = null;

          ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                   String.Format("Saving agent extended event file information in registry entry {0}",
                                                  valueName));

          try
          {
              regKey = Registry.LocalMachine;
              regSubkey = regKey.OpenSubKey(SQLcomplianceAgent.AgentRegistryKey, true);

              serverSubkey = regSubkey.CreateSubKey("AL" + instanceAlias);

              if (auditLogs.Length == 0)  // No traces
              {
                  serverSubkey.SetValue(valueName, new string[0]);
                  return;
              }

              ArrayList values = new ArrayList();
              values.Add(String.Format("{0}", auditLogs[0].StartTime.ToUniversalTime().ToString()));
              values.Add(auditLogs[0].Version.ToString());
              values.Add(agentVersion);
              for (int i = 0; i < auditLogs.Length; i++)
              {
                  if (auditLogs[i].FileName.Contains("AL"))
                  {
                      values.Add(auditLogs[i].TraceNumber.ToString());
                      values.Add(auditLogs[i].TraceId.ToString());
                      values.Add(auditLogs[i].FileName);
                  }
              }
              if (values != null && values.Count > 3)
                  serverSubkey.SetValue(valueName, values.ToArray(typeof(string)));
              values.Clear();
              ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                       "Agent extended event file information saved.");
          }
          catch (SqlComplianceSecurityException se)
          {
              string msg = String.Format("{0} does not have the permission to access the registry: {1}",
                 se.Source, se.Message);
              ErrorLog.Instance.Write(msg, ErrorLog.Severity.Error);
          }
          catch (Exception e)
          {
              ErrorLog.Instance.Write("An error occurred saving agent trace information into registry.",
                                       e,
                                       true);
              throw e;
          }
          finally
          {
              if (serverSubkey != null)
                  serverSubkey.Close();

              if (regSubkey != null)
                  regSubkey.Close();

              if (regKey != null)
                  regKey.Close();
          }
      }
      public static TraceInfo[]
         GetAgentStartedAuditLogsInfo(
         string instanceAlias
         )
      {
          return RetrieveAuditLogsInfo(instanceAlias, CoreConstants.Agent_AgentStartedTraces);
      }

      public static TraceInfo[]
        RetrieveAuditLogsInfo(
        string instanceAlias,
        string type
        )
      {
          // Retrieve the info from the registry.  Add other ways to retrieve
          // it if needed.  Note that this method should be updated when
          // PersistAuditLogInfo() is updated.
          return GetAuditLogsInfoFromRegistry(instanceAlias, type);
      }

      private static TraceInfo[]
        GetAuditLogsInfoFromRegistry(
        string instanceAlias,
        string valueName)
      {
          RegistryKey regKey = null;
          RegistryKey regSubkey = null;
          RegistryKey serverSubkey = null;
          TraceInfo[] auditLogs = null;

          if (instanceAlias == null)
              return null;

          try
          {
              regKey = Registry.LocalMachine;
              regSubkey = regKey.OpenSubKey(SQLcomplianceAgent.AgentRegistryKey);
              if (regKey == null)
                  return null;
              serverSubkey = regSubkey.OpenSubKey("AL" + instanceAlias);
              if (serverSubkey == null)
                  return null;

              // The startup SP writes the tracing information into the registry as
              // a multi-string value.  Parse it here.

              string[] values = null;
              try
              {
                  values = (string[])serverSubkey.GetValue(valueName);
              }
              catch { } // ignore if the value is not there.

              if (values != null && values.Length >= 3)
              {
                  // Startup SP creation time
                  DateTime startTime = DateTime.Parse(values[0]);
                  // Trace configurtion version
                  int version = int.Parse(values[1]);
                  // SQLsecure Agent's version number
                  // Each trace has three entries
                  int auditLogsCount = (values.Length - 3) / 3;
                  auditLogs = new TraceInfo[auditLogsCount];

                  for (int i = 0, j = 3; i < auditLogsCount; i++)
                  {
                      auditLogs[i] = new TraceInfo();
                      auditLogs[i].TraceNumber = int.Parse(values[j++]);
                      auditLogs[i].TraceId = int.Parse(values[j++]);
                      auditLogs[i].FileName = values[j++];
                      auditLogs[i].StartTime = startTime;
                      auditLogs[i].Version = version;
                  }



              }

          }
          catch (SqlComplianceSecurityException se)
          {
              string msg = String.Format("{0} does not have the permission to access the registry: {1}",
                 se.Source, se.Message);
              ErrorLog.Instance.Write(msg, ErrorLog.Severity.Error);
          }
          catch (Exception e)
          {
              // TODO: wrap it with SQLsecure exception and give it detailed error message
              ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                       CoreConstants.Exception_ErrorReadingTraceInfoFromRegistry,
                                       e);
              // propagate the exception to the caller
              throw e;
          }
          finally
          {
              if (serverSubkey != null)
                  serverSubkey.Close();

              if (regSubkey != null)
                  regSubkey.Close();

              if (regKey != null)
                  regKey.Close();
          }

          return auditLogs;
      }

       #endregion


   }
   
}
