using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Serialization;
using Idera.SQLcompliance.Core.Agent;


namespace Idera.SQLcompliance.Core.Status
{
	/// <summary>
	/// Summary description for AgentStatusMsg.
	/// </summary>
   [Serializable]
	public class AgentStatusMsg : ISerializable
	{
		#region Enums

		public enum MsgType : int
		{
			Unknown           =  0 ,
			Heartbeat         =  1,   // Regular heartbeat message
			Startup           =  2,   // Agent has started
			Shutdown          =  3,   // Agent has been requested to shutdown
         Update            =  4,   // Agent audit settings updated
			TraceReceived     =  5,   // Received trace files from agent
			Registered        =  6,   // SQL Server instance registered
			Unregistered      =  7,   // SQL Server instance unregistered
			UnknownInstance   =  8,   // Message from unregistered SQL server instance
			Alert             =  9,   // Agent alert status message
			Error             = 10,   // Agent error status message
			Warning           = 11,   // Agent warning status message
			FirstTimeUpdate   = 12,   // Special case of Update 
         Suspend           = 13,   // Agent has been suspended
         Resume            = 14,   // Agent has resumed services
         Undeployed        = 15,   // Agent undeployed
         ManuallyDeployed  = 16,   // Agent is manully deployed
         UnsupportedSQLVerion = 17, // SQL Server instance version is higher than the repository server's
		   TraceStopped      = 18,   
		   TraceClosed       = 19,
		   TraceAltered      = 20

         // Note: A string needs to be added the AgentEventTypes table for
         //       each type here so that something shows up in lists
      }
		
		#endregion

      #region Fields

		private DateTime statusTime;            // Not used
		private AgentConfiguration config;
      private InstanceStatus instanceStatus;
		private MsgType type;

      // V 2.0 fields
      internal int           classVersion = CoreConstants.SerializationVersion;
	   
	   // V 2.1 fields
	   internal bool          cached = false;

      #endregion

		#region Properties

		public  DateTime StatusTime
		{
			get { return statusTime;  }
			set {	statusTime = value; }
		}	
		
		public  AgentConfiguration Config
		{
		   get { return config;  }
		   set { config = value; }
		}

      public InstanceStatus Status
      {
         get { return instanceStatus; }
         set { instanceStatus = value; }
      }
		
		public MsgType Type
		{
		   get { return type;  }
		   set { type = value; }
		}

		#endregion
	
		#region Constructors
		
		public AgentStatusMsg()
		{
		   config = new AgentConfiguration();
			statusTime            = DateTime.UtcNow;
		   type                  = MsgType.Unknown;
         instanceStatus = null;
		}
		
		public AgentStatusMsg( AgentConfiguration     inConfig,
		                       AgentStatusMsg.MsgType inType )
		{
			statusTime            = DateTime.UtcNow;
		   config = inConfig;
		   type   = inType;
         instanceStatus = null;

		}

      public AgentStatusMsg( InstanceStatus  status,
                             MsgType         inType,
                             string          server,
                             int             port )
      {
         config = new AgentConfiguration();
         config.Server = server;
         config.ServerPort = port;
         config.AgentServer = status.AgentServer;
         statusTime = DateTime.UtcNow;
         instanceStatus = status;
         type = inType;
      }
      
      public AgentStatusMsg(
         SerializationInfo    info,
         StreamingContext     context )
      {
         try
         {
            statusTime = info.GetDateTime( "statusTime");  // retrieve field value
            config = (AgentConfiguration) info.GetValue( "config", typeof(AgentConfiguration));  // retrieve field value
            instanceStatus = info.GetValue( "instanceStatus", typeof(InstanceStatus)) as InstanceStatus;  // retrieve field value
            type = (MsgType) info.GetValue( "type", typeof(MsgType));  // retrieve field value

            // V 2.0 fields
            try
            {
               classVersion = info.GetInt32( "classVersion" );
            }
            catch( Exception e )
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Error Deserializing AgentMsg.", e);
               classVersion = 0;
               return;
            }
            
            // V2.1 field
            try
            {
               cached = info.GetBoolean("cached");
            }
            catch
            {
               cached = false;
            }
            Debug.WriteLine( String.Format("Deserializing {0}",this.GetType()));
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowDeserializationException( e, this.GetType());
         }
     }

      #endregion

      #region Public Methods
      
      public void SetType( System.Int32 inType )
      {
         switch ( inType )
         {
            case (int)MsgType.Heartbeat:
              type = MsgType.Heartbeat;
              break;
            case (int)MsgType.Startup:
              type = MsgType.Startup;
              break;
            case (int)MsgType.Shutdown:
              type = MsgType.Shutdown;
              break;
            case (int)MsgType.Error:
              type = MsgType.Error;
              break;
            default:
              type = MsgType.Unknown;
              break;
         }
      }
      
      public string ToTypeString()
      {
         switch ( type )
         {
            case MsgType.Heartbeat:
              return CoreConstants.AgentStatus_Heartbeat;
            case MsgType.Startup:
              return CoreConstants.AgentStatus_Startup;
            case MsgType.Shutdown:
              return CoreConstants.AgentStatus_Shutdown;
            case MsgType.Warning:
              return CoreConstants.AgentStatus_Warning;
            case MsgType.Error:
              return CoreConstants.AgentStatus_Error;
            default:
              return CoreConstants.AgentStatus_Unknown;
         }
      }
      
 		public string ToShortDateString()
		{
		   return statusTime.ToShortDateString();
		}
		
		public string ToShortTimeString()
		{
		   return statusTime.ToShortTimeString();
		}
		
		#endregion
		
      #region LogStatus
      
      //------------------------------------------------------------		
      // LogStatus
      //------------------------------------------------------------		
      public static void
         LogStatus(
            AgentStatusMsg   statusMsg,
            SqlConnection    dbConn
      )
      {
         LogStatus( statusMsg.Config.AgentServer, 
                    statusMsg.Config.InstanceStatusList[0].Instance, 
                    statusMsg.Type,
                    dbConn );
      }

      //------------------------------------------------------------		
      // LogStatus
      //------------------------------------------------------------		
      public static void
         LogStatus(
            InstanceStatus   status,
            MsgType          type,
            SqlConnection    dbConn
         )
      {
         LogStatus( status.AgentServer, status.Instance, type, dbConn );
      }

      
      //------------------------------------------------------------		
      // LogStatus
      //------------------------------------------------------------		
      public static void
         LogStatus(
            string                  agentServer,
            string                  instance,
            AgentStatusMsg.MsgType  msgType,
            SqlConnection           dbConn
         )
      {
         LogStatus(agentServer, instance, msgType, dbConn, DateTime.MinValue);
      }
      
     //------------------------------------------------------------		
      // LogStatus
      //------------------------------------------------------------		
      public static void
         LogStatus(
            string                  agentServer,
            string                  instance,
            AgentStatusMsg.MsgType  msgType,
            SqlConnection           dbConn,
            DateTime                eventTime
         )
      {
         string cmdStr = String.Format( "INSERT INTO {0} (agentServer, instance,eventType,eventTime) VALUES ('{1}','{2}',{3},{4})",
                                        CoreConstants.RepositoryAgentEventTable,
                                        agentServer.ToUpper(),
                                        instance,
                                        (int)msgType,
                                        eventTime == DateTime.MinValue ? "GETUTCDATE()" : SQLHelpers.CreateSafeDateTimeString(eventTime) );

         using ( SqlCommand cmd = new SqlCommand( cmdStr, dbConn ) )
         {  
            cmd.ExecuteNonQuery();
         }
      }
	   
		#endregion

      #region ISerializable Members

      public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            info.AddValue( "statusTime", statusTime); 
            info.AddValue( "config", config);  
            info.AddValue( "instanceStatus", instanceStatus);  
            info.AddValue( "type", type); 

            if( classVersion >= CoreConstants.SerializationVersion_20 )
            {
               info.AddValue( "classVersion", classVersion );
            }
            
            if( classVersion >= CoreConstants.SerializationVersion_21 )
            {
               info.AddValue("cached", cached);
            }
            Debug.WriteLine( String.Format("{0} serialized.",this.GetType()));
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowSerializationException( e, this.GetType());
         }
      }

      #endregion
   }
	
#region AgentErrorMsg class
/*
/// <summary>
	/// Error status event
	/// </summary>
   [Serializable]
   public class AgentErrorMsg : AgentStatusMsg, ISerializable
	{
		#region Properties	
		
      private int  errorNumber;
		/// <summary>
		/// errorNumber
		/// </summary>
		public int ErrorNumber
		{
			get { return errorNumber;  }
			set { errorNumber = value; }
		}
		
		#endregion		
		
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public AgentErrorMsg( AgentConfiguration     inConfig,
		                      AgentStatusMsg.MsgType inType,
		                      int                    inErrNum ) : base( inConfig, inType )
		{
			errorNumber = inErrNum;
		}

      public AgentErrorMsg( InstanceStatus  inStatus,
         int             inErrNum,
         string          server,
         int             port ) 
         : base ( inStatus, AgentStatusMsg.MsgType.Error, server, port )
      {
         errorNumber = inErrNum;
      }

      // Custom deserializatioin constructor
      public AgentErrorMsg(
         SerializationInfo    info,
         StreamingContext     context )
      {
         SerializationHelper.DeserializeType( this, info, context );
      }

		#endregion

      #region ISerializable Members

      public override void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         SerializationHelper.SerializeType( this, info, context );
      }

   #endregion
   }	
   */
   #endregion
}
