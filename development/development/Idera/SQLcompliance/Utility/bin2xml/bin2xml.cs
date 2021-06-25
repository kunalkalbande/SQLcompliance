using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Configuration;
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Utility.bin2xml
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
         string [] binFiles;
         string    xmlFile;
         RemoteAuditConfiguration config = new RemoteAuditConfiguration();
         binFiles = Directory.GetFiles( ".", "*.bin" );

         if( binFiles != null )
         {
            List<string> msgs = new List<string>();
            for( int i = 0; i < binFiles.Length; i++ )
            {
               try
               {
                  config = LoadBinFile( binFiles[i] );
                  xmlFile = Path.GetFileNameWithoutExtension( binFiles[i] ) + ".xml";
                  SaveToXMLFile( config, xmlFile );
                  Dictionary<string, int> dups = GetDuplicateDbIds(config);
                  if (dups.Count > 0)
                  {
                     msgs.Add(String.Format("*** {0} *** Databases with duplicate IDs: ", binFiles[i]));
                     foreach (string dbName in dups.Keys)
                     {
                        msgs.Add(String.Format("Database {0}: ID = {1}", dbName, dups[dbName]));
                     }
                     msgs.Add("");
                  }
               }
               catch( Exception e )
               {
                  Console.WriteLine( "Error creating XML file for {0}.  Error: {2}.",
                                     binFiles[i],
                                     e.Message );
               }
            }
            if (msgs.Count > 0)
               File.WriteAllLines(@".\dupdbids.txt", msgs.ToArray());
         }
         else
         {
            Console.WriteLine( "No .bin file found" );
         }
		}





      //-------------------------------------------------------------------
      //-------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      /// <param name="config"></param>
      /// <param name="remoteConfig"></param>
      static void
         SaveToXMLFile(
            RemoteAuditConfiguration config,
            string filename
         )
      {

         XmlSerializer serializer = null;

         try
         {
            // TODO: XML schema versioning
            serializer = new XmlSerializer( typeof(RemoteAuditConfiguration) );

			 using(Stream fs = new FileStream( filename , FileMode.Create))
			 {
				 XmlWriter writer = new XmlTextWriter( fs, new UnicodeEncoding() );
				 serializer.Serialize( writer, config );
				 writer.Close();
			 }

         }
         catch( Exception e)
         {
            throw e;
         }


      }

      static RemoteAuditConfiguration LoadBinFile( string filename )
      {
         IFormatter formatter = null;
         Stream stream = null;
         RemoteAuditConfiguration remoteConfig;
         try
         {
            formatter = new BinaryFormatter();
			 using(stream = new FileStream( filename, 
				 FileMode.Open, 
				 FileAccess.Read,
				 FileShare.Read ))
			 {
				 remoteConfig = (RemoteAuditConfiguration)formatter.Deserialize(stream);
			 }


         }
         catch ( Exception e )
         {
            throw e;

         }
         finally
         {
            formatter = null;
         }

         return remoteConfig;

      }

      static Dictionary<string, int> GetDuplicateDbIds(RemoteAuditConfiguration config)
      {
         Dictionary<string, int> list = new Dictionary<string, int>(0);
         if (config.DBConfigs == null || config.DBConfigs.Length == 0)
            return list;

         Dictionary<int, string> all = new Dictionary<int, string>();
         foreach (DBRemoteAuditConfiguration dbConfig in config.DBConfigs)
         {
            try
            {
               if (all.ContainsKey(dbConfig.DbId))
               {
                  if (!list.ContainsKey(all[dbConfig.DbId]))
                     list.Add(all[dbConfig.DbId], dbConfig.DbId);
                  list.Add(dbConfig.dbName, dbConfig.DbId);
               }
               else
                  all.Add(dbConfig.DbId, dbConfig.dbName);
            }
            catch (Exception e)
            {
               Console.WriteLine(String.Format("Error checking duplicate ID for database {0}. Error: {1}",
                                                 dbConfig.dbName, e.Message));
            }
         }

         return list;
      }

	}
}
