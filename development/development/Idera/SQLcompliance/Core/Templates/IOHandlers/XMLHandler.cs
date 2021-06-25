using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using Idera.SQLcompliance.Core.Templates.AuditSettings;

namespace Idera.SQLcompliance.Core.Templates.IOHandlers
{
   public class XMLHandler <T>
   {
   
      [XmlInclude(typeof(AuditTemplate))]
      public T Read( string filename )
      {
         try
         {
            XmlSerializer serializer = new XmlSerializer( typeof ( T ) );
            T tObj;

            using ( TextReader reader = new StreamReader( filename ) )
            {
               tObj = (T) serializer.Deserialize( reader );
            }

            return tObj;
         }
         catch( Exception ex )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     String.Format( "An error occurred reading XML file {0}.",
                                                    filename ),
                                     ex,
                                     true );
            throw;
         }
      }

      [XmlInclude(typeof(AuditTemplate))]
      public T ReadCWF(string xmlContent)
      {
          try
          {
              XmlSerializer serializer = new XmlSerializer(typeof(T));
              T tObj;

              using (TextReader reader = new StringReader(xmlContent))
              {
                  tObj = (T)serializer.Deserialize(reader);
              }

              return tObj;
          }
          catch (Exception ex)
          {
              ErrorLog.Instance.Write(ErrorLog.Level.Verbose,"An error occurred reading XML file.",
                                       ex,
                                       true);
              throw;
          }
      }

      public void Write(string filename, T tObj, bool overwrite)
      {
         try
         {
            XmlSerializer serializer = new XmlSerializer( typeof ( T ) );

            if ( overwrite )
            {
               FileInfo fInfo = new FileInfo( filename );
               if ( fInfo.Exists )
                  fInfo.Delete();
            }

            using ( TextWriter writer = new StreamWriter( filename ) )
            {
               serializer.Serialize( writer,
                                     tObj );
            }
         }
         catch( Exception ex )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     String.Format( "An error occurred writing XML file {0}.",
                                                    filename ),
                                     ex,
                                     true );
            throw;
         }
      }
   }
}
