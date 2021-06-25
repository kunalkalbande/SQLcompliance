using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace Idera.SQLcompliance.Core.Collector
{
	/// <summary>
	/// Summary description for RemoteFileStream.
	/// </summary>
	[Serializable]
   public class RemoteFile : ISerializable
	{
      // 1.1 and 1.2 fields
      private string instance;
      private string serverName;
      private string filename;
      private string auditFilename;
      private bool   isPrivilegedUserTrace;
      private bool   isSqlSecureTrace;

      //private MemoryStream traceStream;
      //private MemoryStream auditStream;

      private byte [] content;
      private byte [] auditContent;

      // 2.0 fields
      internal int    classVersion = CoreConstants.SerializationVersion;

      #region Properties
      public string Instance
      {
         get { return instance; }
         set { instance = value; }
      }

      public string Server
      {
         get { return serverName; }
         set { serverName = value; }
      }

      public string TraceFilename
      {
         get { return filename; }
         set { filename = value; }
      }

      public string AuditFileName
      {
         get { return auditFilename; }
         set { auditFilename = value; }
      }

      public bool IsPrivilegedUserTrace
      {
         get { return isPrivilegedUserTrace; }
         set { isPrivilegedUserTrace = value; }
      }

      public bool IsSqlSecureTrace
      {
         get { return isSqlSecureTrace; }
         set { isSqlSecureTrace = value; }
      }

      public MemoryStream TraceStream
      {
         //get { return traceStream; }
         set 
         { 
            if( value != null )
               content = value.ToArray(); 
            else
               content = null;
         }
      }

      public MemoryStream AuditStream
      {
         //get { return auditStream; }
         set 
         { 
            if( value != null )
               auditContent = value.ToArray(); 
            else
               content = null;
         }
      }

      public byte [] Content
      {
         get { return content; }
         set { content = value; }
      }

      public byte [] AuditContent
      {
         get { return auditContent; }
         set { auditContent = value; }
      }
      #endregion

      #region Constructors

      public RemoteFile( string traceFileName, 
                         byte [] content, 
                         string auditFileName, 
                         byte [] auditContent )
		{
         filename = traceFileName;
         this.content = content;
         isSqlSecureTrace = false;
         isPrivilegedUserTrace = false;
         auditFilename = auditFileName;
         this.auditContent = auditContent;
		}

      public RemoteFile( string traceFileName,
                           MemoryStream traceStream,
                           string auditFileName,
                           MemoryStream auditStream
         )
      {
         filename = traceFileName;
         if( traceStream != null )
            content = traceStream.ToArray();
         else
            Content = null;

         auditFilename = auditFileName;
         if( auditStream != null )
            auditContent = auditStream.ToArray();
         else
            AuditContent = null;

         isSqlSecureTrace = false;
         isPrivilegedUserTrace = false;

      }

      public RemoteFile( )
         : this( null, (MemoryStream)null, null, (MemoryStream)null ) {}

      // Deserialization constructor for ISerializable
      public RemoteFile(
         SerializationInfo    info,
         StreamingContext     context )
      {
         try
         {
            // 1.1 and 1.2 fields
            instance= info.GetString("instance");
            serverName= info.GetString("serverName");
            filename= info.GetString("filename");
            auditFilename= info.GetString("auditFilename");
            isPrivilegedUserTrace= info.GetBoolean("isPrivilegedUserTrace");
            isSqlSecureTrace= info.GetBoolean("isSqlSecureTrace");

            content= info.GetValue("content", typeof(byte [])) as byte [];
            auditContent= info.GetValue("auditContent",typeof(byte [])) as byte [];

            // 2.0 fields
            // classVersion is added since 2.0
            try
            {
               classVersion = info.GetInt32( "classVersion" );
            }
            catch( Exception ex )
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Debug, ex, ErrorLog.Severity.Warning );
               classVersion = 0;
            }

            if( classVersion >= CoreConstants.SerializationVersion_20 )
            {
            }
            else
            {
            }

            Debug.WriteLine( String.Format("{0} deserializaed", GetType() ));
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowDeserializationException( e, GetType());
         }
      }

      #endregion

      #region ISerializable member implementation
	   public void GetObjectData(SerializationInfo info, StreamingContext context)
	   {
         try
         {
            info.AddValue("instance", instance) ;
            info.AddValue("serverName", serverName);
            info.AddValue("filename", filename);
            info.AddValue("auditFilename", auditFilename);
            info.AddValue("isPrivilegedUserTrace", isPrivilegedUserTrace);
            info.AddValue("isSqlSecureTrace", isSqlSecureTrace);

            info.AddValue("content", content);
            info.AddValue("auditContent", auditContent );

            if( classVersion >= CoreConstants.SerializationVersion_20 )
            {
               info.AddValue( "classVersion", classVersion );
            }
            Debug.WriteLine( String.Format("{0} serialized.", GetType()));
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowSerializationException( e, GetType());
         }

	   }
      #endregion
	}

   [Serializable]
   public class RemoteFileAttr
	{
	   #region private members
	   
      private string instance;
      private string serverName;
      private string filename;
      private string binFilename;
      private bool isPrivUserTrace;
      private bool isSQLcmTrace;
      
      #endregion
      
      #region properties
      
      public string Instance
      {
         get { return instance; }
         set { instance = value; }
      }
      
      public string ServerName
      {
         get { return serverName; }
         set { serverName = value; }
      }
      
      public string Filename
      {
         get { return filename; }
         set { filename = value; }
      }
      
      public string BinFilename
      {
         get { return binFilename; }
         set { binFilename = value; }
      }
      
      public bool IsPrivUserTrace
      {
         get { return isPrivUserTrace; }
         set { isPrivUserTrace = value; }
      }
      
      public bool IsSQLcmTrace
      {
         get { return isSQLcmTrace; }
         set { isSQLcmTrace = value; }
      }
      
      #endregion

   }
}
