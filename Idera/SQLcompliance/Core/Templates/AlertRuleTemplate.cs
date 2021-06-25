using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Templates.IOHandlers;

namespace Idera.SQLcompliance.Core.Templates
{
   public class AlertRuleTemplate : Template
   {

      #region Data members

      List<AlertRule> _rules = new List<AlertRule>();
      List<StatusAlertRule> _statusRules = new List<StatusAlertRule>();
      List<DataAlertRule> _dataRules = new List<DataAlertRule>();
      #endregion

      #region Properties
      
      [XmlElement( "AlertRule" )]      
      public AlertRule[] AlertRules
      {
         get
         {
            List<AlertRule> list = new List<AlertRule>( _rules.Count );
            list.AddRange( _rules );
            return list.ToArray();
         }
         set
         {
            if (value != null)
            {
               _rules.Clear();
               _rules.AddRange(value);
            }
         }
      }

      [XmlElement("StatusAlertRule")]
      public StatusAlertRule[] StatusAlertRules
      {
         get
         {
            List<StatusAlertRule> list = new List<StatusAlertRule>(_statusRules.Count);
            list.AddRange(_statusRules);
            return list.ToArray();
         }
         set
         {
            if (value != null)
            {
               _statusRules.Clear();
               _statusRules.AddRange(value);
            }
         }
      }

      [XmlElement("DataAlertRule")]
      public DataAlertRule[] DataAlertRules
      {
         get
         {
            List<DataAlertRule> list = new List<DataAlertRule>(_dataRules.Count);
            list.AddRange(_dataRules);
            return list.ToArray();
         }
         set
         {
            if (value != null)
            {
               _dataRules.Clear();
               _dataRules.AddRange(value);
            }
         }
      }
      #endregion

      #region Constructors


      #endregion

      #region Public Methods



      public override bool Import(string instance)
      {
          try
          {
              //Event AlertRules
              List<AlertRule> ruleList = AlertingDal.SelectAlertRules(_connectionString);
              if (ruleList != null && ruleList.Count > 0)
              {
                  foreach (AlertRule rule in ruleList)
                  {
                      _rules.Add(rule);
                  }
              }

              //Status Alert Rules
              List<StatusAlertRule> statusRules = AlertingDal.SelectStatusAlertRules(_connectionString);
              if (statusRules != null && statusRules.Count > 0)
              {
                  foreach (StatusAlertRule statusRule in statusRules)
                  {
                      _statusRules.Add(statusRule);
                  }
              }

              //Data Alert Rules
              List<DataAlertRule> dataRules = AlertingDal.SelectDataAlertRules(_connectionString);
              if (dataRules != null && dataRules.Count > 0)
              {
                  foreach (DataAlertRule dataRule in dataRules)
                  {
                      _dataRules.Add(dataRule);
                  }
              }

              return true;
          }
          catch (Exception e)
          {
              ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                       String.Format("An error occurred importing {0} to the Alert Rules view.", GetType()),
                                       e,
                                       true);
              return false;
          }

      }


      public bool ImportCWF(string _connectionString, string request)
      {
          try
          {
              //Event AlertRules
              List<AlertRule> ruleList = AlertingDal.SelectAlertRules(_connectionString, request);
              if (ruleList != null && ruleList.Count > 0)
              {
                  foreach (AlertRule rule in ruleList)
                  {
                      _rules.Add(rule);
                  }
              }

              //Status Alert Rules
              List<StatusAlertRule> statusRules = AlertingDal.SelectStatusAlertRules(_connectionString, request);
              if (statusRules != null && statusRules.Count > 0)
              {
                  foreach (StatusAlertRule statusRule in statusRules)
                  {
                      _statusRules.Add(statusRule);
                  }
              }

              //Data Alert Rules
              List<DataAlertRule> dataRules = AlertingDal.SelectDataAlertRules(_connectionString, request);
              if (dataRules != null && dataRules.Count > 0)
              {
                  foreach (DataAlertRule dataRule in dataRules)
                  {
                      _dataRules.Add(dataRule);
                  }
              }

              return true;
          }
          catch (Exception e)
          {
              ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                       String.Format("An error occurred importing {0} to the Alert Rules view.", GetType()),
                                       e,
                                       true);
              return false;
          }

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
            AlertRuleTemplate tmp;
            XMLHandler<AlertRuleTemplate> reader = new XMLHandler<AlertRuleTemplate>();
            tmp = reader.Read(filename);

            _rules.Clear();
            foreach (AlertRule rule in tmp.AlertRules)
            {
               _rules.Add(rule);
            }

            _statusRules.Clear();
            foreach (StatusAlertRule statusRule in tmp.StatusAlertRules)
            {
               _statusRules.Add(statusRule);
            }

            _dataRules.Clear();
            foreach (DataAlertRule dataRule in tmp.DataAlertRules)
            {
               _dataRules.Add(dataRule);
            }
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

      public bool LoadCWF(string xmlContent)
      {
          try
          {
              AlertRuleTemplate tmp;
              XMLHandler<AlertRuleTemplate> reader = new XMLHandler<AlertRuleTemplate>();
              tmp = reader.ReadCWF(xmlContent);

              _rules.Clear();
              foreach (AlertRule rule in tmp.AlertRules)
              {
                  _rules.Add(rule);
              }

              _statusRules.Clear();
              foreach (StatusAlertRule statusRule in tmp.StatusAlertRules)
              {
                  _statusRules.Add(statusRule);
              }

              _dataRules.Clear();
              foreach (DataAlertRule dataRule in tmp.DataAlertRules)
              {
                  _dataRules.Add(dataRule);
              }
              return true;
          }
          catch (Exception e)
          {
              ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "An error occurred reading XML file.",
                                       e,
                                       true);
              return false;
          }
      }

      public override bool Save(string filename)
      {
         return SaveThis(filename, true);
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
            XMLHandler<AlertRuleTemplate> writer = new XMLHandler<AlertRuleTemplate>();
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
   }
}
