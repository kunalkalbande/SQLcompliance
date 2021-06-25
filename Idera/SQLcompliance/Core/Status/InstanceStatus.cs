using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace Idera.SQLcompliance.Core.Status
{
	/// <summary>
	/// Agent uses this class to update the current status with the server.
	/// </summary>
	//TODO: make it a struct?
	[Serializable]
	public class InstanceStatus : ISerializable
	{
      // The data members of this class is purposely set to public to
      // reduce remoting overhead and prevent the remote object being
      // too chatty.
      #region Public Data Member

      public int              SqlVersion;    
      public int              ConfigVersion;  
      public string           Instance;      
      public string           AgentServer;
      //public bool             IsClustered;

      public DateTime         LastUpdateTime;      // Not used
      public DateTime         LastCollectionTime;  // Not used
      public DateTime         LastModifiedTime;    // Not used

      // Trace settings
      public long             MaxTraceSize;
      public string           TraceDirectory;
      public DateTime         StopTime;            // Not used

      // Configuration
      public  int             CollectionInterval;
      public  int             ForceCollectionInterval;
      public  int             TraceStartTimeout;

      // V 2.0 fields
      internal int            classVersion = CoreConstants.SerializationVersion;

      #endregion

      #region Constructors

		public InstanceStatus()
		{
		}

      public InstanceStatus ( string instanceName ) : base()
      {
         Instance = instanceName;
      }

      // Custom deserialization constructor
      public InstanceStatus(
         SerializationInfo    info,
         StreamingContext     context )
      {
         try
         {
            SqlVersion = info.GetInt32( "SqlVersion" );    
            ConfigVersion = info.GetInt32("ConfigVersion");  
            Instance = info.GetString( "Instance" );      
            AgentServer = info.GetString("AgentServer");

            LastUpdateTime = info.GetDateTime("LastUpdateTime");
            LastCollectionTime = info.GetDateTime("LastCollectionTime");
            LastModifiedTime = info.GetDateTime("LastModifiedTime");

            // Trace settings
            MaxTraceSize = info.GetInt64("MaxTraceSize");
            TraceDirectory = info.GetString("TraceDirectory");
            StopTime = info.GetDateTime("StopTime");

            // Configuration
            CollectionInterval = info.GetInt32( "CollectionInterval" );
            ForceCollectionInterval = info.GetInt32( "ForceCollectionInterval" );

            // V 2.0 fields
            try
            {
               classVersion = info.GetInt32( "classVersion" );
            }
            catch
            {
               classVersion = 0;
            }

         }
         catch( Exception e )
         {
            SerializationHelper.ThrowDeserializationException( e, this.GetType());
         }

         Debug.WriteLine( String.Format("{0} deserializaed", this.GetType()));
      }
      #endregion

      #region Custom Serialization

      // Required custom serialization method
      public void GetObjectData(
         SerializationInfo    info,
         StreamingContext     context )
      {
         try
         {
            info.AddValue("SqlVersion", SqlVersion);
            info.AddValue("ConfigVersion", ConfigVersion);
            info.AddValue("Instance", Instance);
            info.AddValue("AgentServer", AgentServer);

            info.AddValue("LastUpdateTime", LastUpdateTime);
            info.AddValue("LastCollectionTime", LastCollectionTime);
            info.AddValue("LastModifiedTime", LastModifiedTime);

            // Trace settings
            info.AddValue("MaxTraceSize", MaxTraceSize);
            info.AddValue("TraceDirectory", TraceDirectory);
            info.AddValue("StopTime", StopTime);

            // Configuration
            info.AddValue("CollectionInterval",CollectionInterval);
            info.AddValue("ForceCollectionInterval",ForceCollectionInterval);

            if( classVersion >= CoreConstants.SerializationVersion_20 )
            {
               info.AddValue( "classVersion", classVersion );
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
}
