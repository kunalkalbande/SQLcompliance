using System;
using System.Collections.Generic;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Rules.Filters;

namespace Idera.SQLcompliance.Application.GUI
{
   public class ServerEventArgs : EventArgs
   {
      private ServerRecord _server;

      public ServerRecord Server
      {
         get { return _server; }
         set { _server = value; }
      }

      public ServerEventArgs(ServerRecord record)
      {
         _server = record;
      }
   }

   public class MultiServerEventArgs : EventArgs
   {
      private List<ServerRecord> _servers;

      public List<ServerRecord> ServerList
      {
         get { return _servers; }
         set { _servers = value; }
      }

      public MultiServerEventArgs()
      {
         _servers = new List<ServerRecord> () ;
      }

      public void AddServer(ServerRecord record)
      {
         _servers.Add(record) ;
      }
   }

   public class DatabaseEventArgs : EventArgs
   {
      private DatabaseRecord _database;

      public DatabaseRecord Database
      {
         get { return _database; }
         set { _database = value; }
      }

      public DatabaseEventArgs(DatabaseRecord record)
      {
         _database = record;
      }
   }

   public class AlertRuleEventArgs : EventArgs
   {
      private AlertRule _rule;

      public AlertRule Rule
      {
         get { return _rule; }
         set { _rule = value; }
      }

      public AlertRuleEventArgs(AlertRule rule)
      {
         _rule = rule;
      }
   }

   public class EventFilterEventArgs : EventArgs
   {
      private EventFilter _filter;

      public EventFilter Filter
      {
         get { return _filter; }
         set { _filter = value; }
      }

      public EventFilterEventArgs(EventFilter filter)
      {
         _filter = filter;
      }
   }

   public class LoginEventArgs : EventArgs
   {
      private DatabaseRecord _database;

      public DatabaseRecord Database
      {
         get { return _database; }
         set { _database = value; }
      }

      public LoginEventArgs(DatabaseRecord record)
      {
         _database = record;
      }
   }

   public class ArchiveEventArgs : EventArgs
   {
      private ArchiveRecord _archive;

      public ArchiveRecord Archive
      {
         get { return _archive; }
         set { _archive = value; }
      }

      public ArchiveEventArgs(ArchiveRecord record)
      {
         _archive = record;
      }
   }

   public class ConnectionChangedEventArgs : EventArgs
   {
      private SQLRepository _repository;

      public SQLRepository Repository
      {
         get { return _repository; }
         set { _repository = value; }
      }

      public ConnectionChangedEventArgs(SQLRepository repository)
      {
         _repository = repository;
      }
   }

}
