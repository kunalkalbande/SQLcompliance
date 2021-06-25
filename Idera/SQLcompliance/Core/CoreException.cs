using System;
using System.Runtime.Serialization;

namespace Idera.SQLcompliance.Core
{
	/// <summary>
	/// Core exception class for SQLsecure.  Used to wrap exceptions coming out of the 
	/// SQLsecureCore assembly.  Note that we don't have anything need custom serialization
	/// so GetObjectData() and deserialization constructor from base class are used.
	/// </summary>
	[Serializable]
	public class CoreException : ApplicationException, ISerializable
	{
		public CoreException(string message)
			: base(message) {}

		public CoreException(string message, Exception innerException)
			:base(message, innerException) {}

		public CoreException(SerializationInfo info, StreamingContext context)
         :base(info, context) {}

   }

   //-------------------------------------------------------------------------------------
   // UnregisteredInstanceException
   //-------------------------------------------------------------------------------------
   /// <summary>
   /// Thrown by the server when an agent from an unregistered instance send messages
   /// to the server.
   /// </summary>
   [Serializable]
   public class UnregisteredInstanceException: CoreException , ISerializable
   {
      public UnregisteredInstanceException ( string message )
         : base( message ) {}

      public UnregisteredInstanceException(string message, Exception innerException)
         :base(message, innerException) {}

      public UnregisteredInstanceException(SerializationInfo info, StreamingContext context)
         :base(info, context) {}

   }

   [Serializable]
   public class IncompatibleAgentRepositoryVersionException: CoreException , ISerializable
   {
      public IncompatibleAgentRepositoryVersionException ( )
         : base( CoreConstants.Exception_IncompatibleAgentRepositoryVersion ) {}

		public IncompatibleAgentRepositoryVersionException(SerializationInfo info, StreamingContext context)
			:base(info, context) {}
   }

   [Serializable]
   public class IncompatibleSQLServerVersionException: CoreException , ISerializable
   {
      public IncompatibleSQLServerVersionException ( )
         : base( CoreConstants.Exception_IncompatibleRepositoryAndAgentSQLServerVersion ) {}

      public IncompatibleSQLServerVersionException( Exception innerException )
         : base( CoreConstants.Exception_IncompatibleRepositoryAndAgentSQLServerVersion,
                 innerException ) {}

		public IncompatibleSQLServerVersionException(SerializationInfo info, StreamingContext context)
			:base(info, context) {}
   }

	/// <summary>
	/// Thrown when the backup archive directory is invalid or not accessible (i.e. a mapped drive)
	/// This is not a security exception.
	/// </summary>
	[Serializable]
	public class InvalidBackupArchiveDirectoryException : CoreException , ISerializable
   {

		public InvalidBackupArchiveDirectoryException(string message)
			: base(message) {}

		public InvalidBackupArchiveDirectoryException(string message, Exception innerException)
			:base(message, innerException) {}

		public InvalidBackupArchiveDirectoryException(SerializationInfo info, StreamingContext context)
			:base(info, context) {}

	}

   [Serializable]
   public class ConnectionFailedException : CoreException, ISerializable
   {

      public ConnectionFailedException(string message)
         : base(message) { }

      public ConnectionFailedException(string message, Exception innerException)
         : base(message, innerException) { }

      public ConnectionFailedException(SerializationInfo info, StreamingContext context)
         : base(info, context) { }

   }
}
