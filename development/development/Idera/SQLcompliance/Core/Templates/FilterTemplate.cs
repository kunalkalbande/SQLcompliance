using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using Idera.SQLcompliance.Core.Rules.Filters;
using Idera.SQLcompliance.Core.Templates.IOHandlers;

namespace Idera.SQLcompliance.Core.Templates
{
   public class FilterTemplate : Template
   {
      private List<EventFilter> _filters = new List<EventFilter>();

      #region Properties
      
      [XmlElement("EventFilter")]      
      public EventFilter[] EventFilters
      {
         get
         {
            List<EventFilter> list = new List<EventFilter>(_filters.Count);
            list.AddRange(_filters);
            return list.ToArray();
         }
         set
         {
            _filters.Clear();
            _filters.AddRange(value) ;
         }
      }

      #endregion

      #region Public Methods

      public override bool Import(string instance)
      {
         ArrayList filterList = FiltersDal.SelectEventFilters(_connectionString);
         if (filterList == null || filterList.Count == 0)
            return true;

         foreach (EventFilter filter in filterList)
         {
            _filters.Add(filter) ;
         }

         return true;
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
            FilterTemplate tmp;
            XMLHandler<FilterTemplate> reader = new XMLHandler<FilterTemplate>();
            tmp = reader.Read(filename);

            _filters.Clear();
            foreach ( EventFilter filter in tmp.EventFilters )
            {
               _filters.Add(filter );
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
              FilterTemplate tmp;
              XMLHandler<FilterTemplate> reader = new XMLHandler<FilterTemplate>();
              tmp = reader.ReadCWF(xmlContent);

              _filters.Clear();
              foreach (EventFilter filter in tmp.EventFilters)
              {
                  _filters.Add(filter);
              }
              return true;
          }
          catch (Exception e)
          {
              ErrorLog.Instance.Write(ErrorLog.Level.Verbose,"An error occurred reading the XML template file {0}.",
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
            XMLHandler<FilterTemplate> writer = new XMLHandler<FilterTemplate>();
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

       public string ImportCWF(string _connectionString, string request)
      {
          string result = "failed";
          try
          {
              ArrayList filterList = FiltersDal.SelectEventFilters(_connectionString, request);
              if (filterList == null || filterList.Count == 0)
                  return result;

              foreach (EventFilter filter in filterList)
              {
                  _filters.Add(filter);
              }
          }
          catch (Exception ex)
          {
              return result;
          }

          return result;
      }
      #endregion
   }
}
