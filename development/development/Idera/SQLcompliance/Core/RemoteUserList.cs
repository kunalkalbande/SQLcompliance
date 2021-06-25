using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Idera.SQLcompliance.Core
{
	/// <summary>
	/// This struct is used to serailize user lists to the repository table.  Serialization
	/// doesn't need to be backward compatible but deserialization needs to be to read old
	/// versions of the struct to support upgrade scenerios.
	/// </summary>
	[Serializable]
	public struct RemoteUserList : ISerializable
	{
      internal int         targetVersion;
      public ServerRole [] ServerRoles;
      public Login []      Logins;

		public RemoteUserList( 
         ServerRole [] roles,
         Login []      logins )
		{
         targetVersion = CoreConstants.SerializationVersion;
         ServerRoles = roles;
         Logins = logins;
		}

      // Deserialization constructor
      public RemoteUserList(
         SerializationInfo    info,
         StreamingContext     context )
      {
         targetVersion = CoreConstants.SerializationVersion;
         ServerRoles = null;
         Logins = null;

         try
         {
            try
            {
               // Prior to V 2.0 there is no version number for the struct
               targetVersion = info.GetInt32( "targetVersion" );
            }
            catch
            {
               targetVersion = 0;
            }
            ServerRoles = (ServerRole [])info.GetValue( "ServerRoles", typeof( ServerRole []) );
            Logins      = (Login [])info.GetValue( "Logins", typeof(Login []) );
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowDeserializationException( e, this.GetType());
         }

      }

      // Required ISerializable member
	   public void GetObjectData(SerializationInfo info, StreamingContext context)
	   {
         try
         {
            info.AddValue( "targetVersion", targetVersion );
            info.AddValue( "ServerRoles", ServerRoles );
            info.AddValue( "Logins", Logins );
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowSerializationException( e, this.GetType());
         }
	   }
	}

}
