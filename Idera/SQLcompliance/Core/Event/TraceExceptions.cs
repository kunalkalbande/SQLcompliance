using System;
using System.Runtime.Serialization;

using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Core.Event
{
	/// <summary>
	/// Summary exception classes for trace related error.
	/// </summary>
   [Serializable]
   public class TraceException : CoreException
	{
      public TraceException(string message)
         : base(message) {}

      public TraceException(string message, Exception innerException)
         :base(message, innerException) {}

      public TraceException(SerializationInfo info, StreamingContext context)
         :base(info, context) {}
   }

   #region Trace Configuraion related exception classes
   //--------------------------------------------------------------------------
   //  
   //--------------------------------------------------------------------------
   [Serializable]
   public class InvalidColumnIdException : CoreException
   {
      public InvalidColumnIdException ( string message )
         :base( message ){}
   }

   [Serializable]
   public class InvalidEventIdException : CoreException
   {
      public InvalidEventIdException ( string message )
         : base( message ){}
   }

   [Serializable]
   public class InvalidLogicalOpException : CoreException
   {
      public InvalidLogicalOpException ( string message )
         : base( message ){}
   }

   [Serializable]
   public class InvalidColumnTypeException : CoreException
   {
      public InvalidColumnTypeException ( string message )
         : base( message ){}
   }

   [Serializable]
   public class InvalidFilterTypeException : CoreException
   {
      public InvalidFilterTypeException ( string message )
         : base( message ){}
   }

   [Serializable]
   public class IncompetibleComparisonOpException : CoreException
   {
      public IncompetibleComparisonOpException ( string message )
         : base( message ){}
   }

   [Serializable]
   public class InvalidComparisonOpException : CoreException
   {
      public InvalidComparisonOpException ( string message )
         : base( message ){}
   }
   #endregion

   #region Trace processing related exception classes
   //-------------------------------------------------------------------------------------
   // TraceFileExistsException
   //-------------------------------------------------------------------------------------
   /// <summary>
   /// Thrown by the collector server when a trace file already exists on the server.
   /// </summary>
   [Serializable]
   public class TraceFileExistsException: TraceException
   {
      public TraceFileExistsException ( string fileName )
         : base( CreateMessage( fileName ) ) {}

      public TraceFileExistsException(string fileName, Exception innerException)
         :base(CreateMessage( fileName ), innerException) {}

      public TraceFileExistsException(SerializationInfo info, StreamingContext context)
         :base(info, context) {}

      private static string CreateMessage ( string fileName )
      {
         string message;
         if( fileName != null )
            message = String.Format( CoreConstants.Exception_FileAlreadyExistsOnTheServer, fileName );
         else
         {
            message = String.Format( CoreConstants.Exception_FileAlreadyExistsOnTheServer, "<Unknown>" );
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,  
                                     "The filename is a NULL handle" );
         }
         return message;
      }

   }

   #endregion

   #region Trace management related exception classes

   #endregion
}
