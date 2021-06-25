using System;
using System.IO;

using Idera.SQLcompliance.Core.Templates.IOHandlers;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Agent;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Core.Templates
{
   public class InstanceTemplate : Template
   {
      #region Data members
       ICollection _tablelist;
      private readonly ServerRecord _server;
      //Start - Requirement - 4.1.4.1
      public static List<String> dbDetails = new List<String>();
      //End - Requirement - 4.1.4.1
      AuditTemplate _auditTemplate = null;
      AlertRuleTemplate _alertTemplate = null;
      FilterTemplate _filterTemplate = null;

      #endregion

      #region Properties
      
      public AuditTemplate AuditTemplate
      {
         get { return _auditTemplate; }
         set { _auditTemplate = value; }
      }
      
      public AlertRuleTemplate AlertTemplate
      {
         get { return _alertTemplate; }
         set { _alertTemplate = value; }
      }
      
      public FilterTemplate FilterTemplate
      {
         get { return _filterTemplate; }
         set { _filterTemplate = value; }
      }

      #endregion

      #region Constructors


      #endregion

      #region Public Methods


      //-----------------------------------------------------------------
      // Apply: apply the template to the instance
      //-----------------------------------------------------------------
//      public bool Apply(string instance, bool replace)
//      {
//         bool success = false;
//         
//         try
//         {
//            if( _auditTemplate != null )
//               _auditTemplate.Apply( instance, replace );
//            
//            if(  _alertTemplate != null )
//               _alertTemplate.Apply( instance, replace );
//            
//            if( _filterTemplate != null )
//               _filterTemplate.Apply( instance, replace );
//         }
//         catch( Exception e )
//         {
//            success = false;
//            ErrorLog.Instance.Write(ErrorLog.Level.Default,e, true);
//         }
//
//         return success;
//      }

      public override bool Import(string instance)
      {
         // ErrorHandling in sub-methods
         if (!ImportAuditSettings(instance))
            return false;
         if (!ImportAlertRules())
            return false;
         if (!ImportEventFilters())
            return false;
         return true;
      }

      public bool ImportAuditSettings(string instance)
      {
         bool success;

         try
         {
            _auditTemplate = new AuditTemplate();
            _auditTemplate.Init(_repositoryServer);
            success = _auditTemplate.Import(instance);
            if (!success)
               return success;
         }
         catch (Exception e)
         {
            success = false;
            ErrorLog.Instance.Write(ErrorLog.Level.Default, e, true);
         }

         return success;
      }

      public bool ExportDatabaseAuditSettings(DatabaseRecord db)
      {
         try
         {
            var connectionString = db.Connection.ConnectionString;
            var database = DatabaseRecord.GetDatabaseRecord(db.DbId, connectionString);
            _auditTemplate = new AuditTemplate();
            _auditTemplate.Init(_repositoryServer);
            _auditTemplate.ExportDatabase(database, connectionString);
             
            return true;
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Default, e, true);
            return false;
         }
      }

      public bool ImportAlertRules()
      {
         bool success;

         try
         {
            _alertTemplate = new AlertRuleTemplate();
            _alertTemplate.Init(_repositoryServer);
            success = _alertTemplate.Import(null);
            if (!success)
               return success;
         }
         catch (Exception e)
         {
            success = false;
            ErrorLog.Instance.Write(ErrorLog.Level.Default, e, true);
         }

         return success;
      }

      public bool ImportEventFilters()
      {
         bool success;

         try
         {
            _filterTemplate = new FilterTemplate();
            _filterTemplate.Init(_repositoryServer);
            success = _filterTemplate.Import(null);
            if (!success)
               return success;
         }
         catch (Exception e)
         {
            success = false;
            ErrorLog.Instance.Write(ErrorLog.Level.Default, e, true);
         }

         return success;
      }

      public override bool Export(string instance,
                                   string filename,
                                   bool overwrite)
      {
         bool success;
         try
         {
            if (Import(instance))
            {
               success = SaveThis(filename,
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

      public override bool Load(string filename)
      {
         try
         {
            InstanceTemplate tmp;
            XMLHandler<InstanceTemplate> reader = new XMLHandler<InstanceTemplate>();
            tmp = reader.Read(filename);
            
            _auditTemplate = tmp.AuditTemplate;
            if(_auditTemplate != null)
               _auditTemplate.Init( _repositoryServer );
            _alertTemplate = tmp.AlertTemplate;
            if (_alertTemplate != null)
               _alertTemplate.Init(_repositoryServer);
            _filterTemplate = tmp.FilterTemplate;
            if (_filterTemplate != null)
               _filterTemplate.Init(_repositoryServer);
            
            return true;
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                     String.Format("An error occurred reading the XML template file {0}.",
                                                    filename),
                                     e,
                                     true);
            return false;
         }
      }

      public void LoadCWF(string xmlData)
      {
          try
          {
              InstanceTemplate tmp;
              XMLHandler<InstanceTemplate> reader = new XMLHandler<InstanceTemplate>();
              tmp = reader.ReadCWF(xmlData);

              _auditTemplate = tmp.AuditTemplate;
              if (_auditTemplate != null)
                  _auditTemplate.Init(_repositoryServer);
              _alertTemplate = tmp.AlertTemplate;
              if (_alertTemplate != null)
                  _alertTemplate.Init(_repositoryServer);
              _filterTemplate = tmp.FilterTemplate;
              if (_filterTemplate != null)
                  _filterTemplate.Init(_repositoryServer);
          }
          catch (Exception e)
          {
              ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                       String.Format("An error occurred reading the XML template file XML Data."),
                                       e,
                                       true);
          }
      }

      public override bool Save(string filename)
      {
         return SaveThis( filename, true );
      }
      
      #endregion
      
      #region Private Methods
      
      bool SaveThis(string filename,
              bool overwrite)
      {
         FileInfo info = new FileInfo(filename);
         if (info.Exists)
         {
            if (!overwrite)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format(TemplateFileExistsError,
                                                       filename),
                                        ErrorLog.Severity.Warning);
               return false;
            }
            else
            {
               info.Delete();
            }
         }

         try
         {
            XMLHandler<InstanceTemplate> writer = new XMLHandler<InstanceTemplate>();
            writer.Write(filename,
                          this,
                          overwrite);
            return true;
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                     "An error occurred writing the XML template file.",
                                     e,
                                     true);
            return false;
         }
      }
      
      #endregion

 	#region 5.4

      ///<Export Audit Setting Server Details>
      /// Rest Services Method to export the Database Audit Setting xml file.
      ///</Export>
      public bool ImportAuditSettingsCwf(SqlConnection connection, string instance)
      {
          bool success;

          try
          {
              _auditTemplate = new AuditTemplate();
              _auditTemplate.Init(_repositoryServer);
              success = _auditTemplate.ImportCwf(connection, instance);
              if (!success)
                  return success;
          }
          catch (Exception e)
          {
              success = false;
              ErrorLog.Instance.Write(ErrorLog.Level.Default, e, true);
          }

          return success;
      }

      ///<Export Database Audit XML File>
      /// Rest Services Method to export the Database Audit Setting xml file.
      ///</ExportAudit>
      public bool ExportDatabaseAuditSettingsCwf(SqlConnection connection, DatabaseRecord db)
      {
          try
          {
              var connectionString = connection.ConnectionString;
              var database = DatabaseRecord.GetDatabaseRecord(db.DbId, connectionString);
              _auditTemplate = new AuditTemplate();
              _auditTemplate.Init(_repositoryServer);
              _auditTemplate.ExportDatabase(database, connectionString);

              return true;
          }
          catch (Exception e)
          {
              ErrorLog.Instance.Write(ErrorLog.Level.Default, e, true);
              return false;
          }
      }

      #endregion 5.4
      
	
   }
}
