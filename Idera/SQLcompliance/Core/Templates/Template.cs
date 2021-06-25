using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;

using Idera.SQLcompliance.Core.Templates.IOHandlers;

namespace Idera.SQLcompliance.Core.Templates
{
   [XmlRoot("Template")]
   public abstract class Template : ITemplate
   {
      #region Data members
      
      protected string _name;
      protected string _desc;
      protected string _filename;
      protected string _repositoryServer;
      protected string _connectionString;
      //protected Dictionary<string, ITElement> _elements;
      
      #endregion
      
      #region Constants
      
      internal const string TemplateFileExistsError = "An error occurred saving template file. " 
                                                    + "A file with the same name already exists."
                                                    + "File name: {0}.";
      
      #endregion
      
      #region Properties
      
      [XmlAttribute("Name")]
      public string Name
      {
         get { return _name; }
         set { _name = value; }
      }

      [XmlAttribute("Description")]
      public string Description
      {
         get { return _desc; }
         set { _desc = value; }
      }

      [XmlAttribute("FullFilename")]
      public string FullFilename
      {
         get { return _filename; }
         set { _filename = value; }
      }
      
      [XmlIgnore]
      public virtual string RepositoryServer
      {
         get
         {
            return _repositoryServer;
         }
         set
         {
            _repositoryServer = value;
            CreateConnectionString( _repositoryServer );
         }
      }

         /*
      public List<ITElement> Elements
      {
         get
         {
            List<ITElement> items = new List<ITElement>(_elements.Count);
            foreach( ITElement e in _elements.Values )
               items.Add( e );
            return items;
         }
         
         set
         {
            foreach( ITElement e in value )
               Add( e ); 
         }
      }
      */
            
      
      #endregion
      
      #region Constructors
      
      public Template()
      {
         _name = "";
         _desc = "";
         _filename = "";
      }

      
      #endregion
      
      #region Public abstract methods
      
      //public abstract bool Apply( string instance, bool replace );
      public abstract bool Import( string instance );
      public abstract bool Save( string filename );  // Save the template to an XML file
      public abstract bool Load( string filename );  // Load a template from an XML file
      
      #endregion
      
      #region Public Methods
      /*
      
      //
      // Add an element to the template
      //
      public virtual bool Add( ITElement e )
      {
         try
         {
            if( e == null )
               return false;
            
            if( _elements.ContainsKey( e.Name ) )
               _elements[e.Name] = e;
            else
               _elements.Add( e.Name, e );
         }
         catch( Exception ex )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "An error occurred when adding an element to the template.",
                                     ex,
                                     true );
            return false;
         }
         
         return true;
      }
      
      //
      // Remove an element from the template
      //
      public virtual bool Remove( ITElement e )
      {
         try
         {
            if( e == null )
               return false;
            
            if( _elements.ContainsKey( e.Name ) )
               _elements.Remove( e.Name );
            else 
               return false;
         }
         catch( Exception ex )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "An error occurred removing an element from the template.",
                                     ex,
                                     true );
            return false;
         }
         
         return true;
      }
       */
      
      public virtual void Init( string server )
      {
         RepositoryServer = server;
      }

      //
      // Loads a template from a file
      //
      public T Load<T>( string filename )
      {
         T obj = default(T);
         try
         {
            XMLHandler<T> reader = new XMLHandler<T>();
            obj = reader.Read( filename );
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     String.Format( "An error occurred reading the XML template file {0}.",
                                                    filename ),
                                     e,
                                     true );
         }
         return obj;
      }
      
      //
      // Save a template to a file
      //
      [XmlInclude(typeof(AuditTemplate))]
      public bool Save<T>(string filename, T obj, bool overwrite)
      {
         FileInfo info = new FileInfo( filename );
         if ( info.Exists )
         {
            if ( !overwrite )
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                        String.Format( TemplateFileExistsError,
                                                       filename ),
                                        ErrorLog.Severity.Warning );
               return false;
            }
            else
            {
               info.Delete();
            }
         }

         try
         {
            XMLHandler<T> writer = new XMLHandler<T>();
            writer.Write( filename,
                          obj,
                          overwrite );
            return true;
         }
         catch ( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Default,
                                     "An error occurred writing the XML template file.",
                                     e,
                                     true );
            return false;
         }
      }

      //
      // Export an instance's settings to an XML template file
      //
      public virtual bool Export( string instance,
                                  string filename,
                                  bool overwrite)
      {
         bool success;
         try
         {
            if (Import(instance))
            {
               success = Save(filename,
                               this,
                               overwrite);
            }
            else
               success = false;
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("An error occurred exporting {0} to an XML template file.", GetType()),
                                     e,
                                     true);
            throw;
         }
         return success;
      }
      
      

      #endregion
      
      #region Private/Protected Methods
      

                     
      void CreateConnectionString( string server )
      {
         _connectionString = String.Format("server={0};" +
                     "database={1};" +
                     "integrated security=SSPI;" +
                     "Connect Timeout=30;" +
                     "Application Name='{2}';",
                     server,
                     CoreConstants.RepositoryDatabase,
                     CoreConstants.ManagementConsoleName);
      }
      
      protected SqlConnection OpenNewConnection( )
      {
         SqlConnection conn = new SqlConnection( _connectionString );
         conn.Open();
         return conn;
      }

      #endregion
   }
}
