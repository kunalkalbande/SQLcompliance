using System;
using System.Collections;
using System.Data.SqlClient;

using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for LoginFilter.
	/// </summary>
	public class LoginFilter
	{
	   #region Properties

      private string    m_instance;	   
	   private int       m_loginHash         = 0;
	   
	   private bool      m_enabled           = false;
       private int       m_timeDiffInSeconds = 60;
	   private int       m_maxCacheSize      = 500;
      private int       m_currentCacheSize  = 0;
	   
      private Hashtable m_loginCache        = null;
      private bool      m_dirty             = false;
	   
	   #endregion
	   
	   #region Constructor
	
		public
		   LoginFilter(
		      string   instance,
            bool     enabled,
            int      timeDiffInSeconds,
		      int      maxCacheSize
		   )
		{
		   m_dirty             = false;
		   
		   m_enabled           = enabled;
		   m_instance          = instance;
           m_timeDiffInSeconds = timeDiffInSeconds;
		   m_maxCacheSize      = maxCacheSize;
		   
		   // no reason to read cache if not enabled since we dont look at it
		   if ( enabled ) ReadCache();
		}
		
		#endregion
		
		#region Methods
		
		//---------------------------------------------------------------------
		// IsLoginUnique
      //---------------------------------------------------------------------
      public bool
		   IsLoginUnique(
		      string            newLogin,
		      string            newAppName,
		      string            newHostName,
            DateTime          newLoginTime
            )
		{
         bool     isLoginUnique = true;

		   // if filtering is not enabled then we just return that this is unique
		   if ( ! m_enabled ) return true;
		   
		   m_loginHash = GetLoginHash( newLogin,
		                               newAppName,
		                               newHostName );
         // look for this login in collection
         DateTime lastLoginTime = FindLogin( m_loginHash );
         if ( lastLoginTime == DateTime.MinValue )
         {
            // matching login not found - we have a unique one!
            isLoginUnique = true;
            
            // add to cache
            AddLogin( m_loginHash, newLoginTime);
         }
         else
         {
            // we have a match - but is it close enough in time?
            if ( ! LoginTimesCloseEnough( lastLoginTime, newLoginTime ) )
            {
               // too far apart - keep this one and update the cache timestamp
               isLoginUnique = true;
               
               // update cache time stamp for this login
               UpdateLogin( m_loginHash, newLoginTime);
            }
            else
            {
               isLoginUnique = false;
            }
         }
         return isLoginUnique;
		}
		
      //--------------------------------------------------------------------
      // LoginTimesCloseEnough - Compare two date time values and determine
      //                         if they are within the time period
      //--------------------------------------------------------------------
      public bool
         LoginTimesCloseEnough(
         DateTime    cacheTimestamp,
         DateTime    newTimestamp
         )
      {
         bool loginTimesMatch = false;
         
         if ( cacheTimestamp.CompareTo(newTimestamp) < 0 )
         {
             //DateTime endOfRange = cacheTimestamp.AddMinutes( m_timeDiffInMinutes );
            // Login events are not showing because timespan is 60 minutes in that's why changing time span into 10 secs.
            DateTime endOfRange = cacheTimestamp.AddSeconds(m_timeDiffInSeconds);  
            if ( endOfRange.CompareTo(newTimestamp) > 0 )
            {
               return true;
            }
            // else
            //   cacheTime is outside of timeDiff - so they dont match 
         }
         // else
         //   cacheTime is greater then new time - so they dont match 
         
         return loginTimesMatch;
      }

      //--------------------------------------------------------------------
      // GetLoginHash
      //--------------------------------------------------------------------
      public int
         GetLoginHash(
         string   instance,
         string   user,
         string   host
         )
      {
         string loginString = String.Format( "{0}**{1}**{2}",
            instance,
            user,
            host );
         return loginString.GetHashCode();                                             
      }
      
      #endregion
 		
		#region Cache Access Methods
      
      //------------------------------------------------------------------
      // FindLogin - Look for user,app,host trio in cache - if found
      //             return the timestamp from the cached event
      //-------------------------------------------------------------------
      public DateTime
         FindLogin(
            int         loginHash
         )
      {		
         // load instance cache if it is persisted - otherwise just initalize it
         if ( m_loginCache.Contains( loginHash ) )
         {
            return (DateTime)m_loginCache[loginHash];
         }
         else
         {
            return DateTime.MinValue;
         }
      }
      
      //------------------------------------------------------------------
      // AddLogin - Add an unmatched login to cache
      //-------------------------------------------------------------------
      public void
         AddLogin(
            int         loginHash,
            DateTime    timestamp
         )
      {		
         m_dirty = true;

         // already in cache?
         
         // if too many - delete oldest from cache
         if ( m_currentCacheSize >= m_maxCacheSize )
         {
            DateTime earliestTime = DateTime.MaxValue;
            int      earliestKey  = -1;
            
            IDictionaryEnumerator myEnumerator = m_loginCache.GetEnumerator();
            while ( myEnumerator.MoveNext() )
            {
               DateTime curr = (DateTime)myEnumerator.Value;
               
               if ( ( earliestTime == DateTime.MaxValue ) ||
                    ( curr.CompareTo(earliestTime) < 0) )
               {
                  earliestTime = curr;
                  earliestKey  = (int)myEnumerator.Key;
               }
            }
            
            if ( earliestTime != DateTime.MaxValue )
            {
               m_loginCache.Remove( earliestKey );
            }
         }

         // add to cache
         try
         {
            m_loginCache.Add( loginHash, timestamp );
         }
         catch
         {
            // probably a duplicate - we shouldnt ever be here but we
            // will just try calling update login to see what happens
         }
      }
      
      //------------------------------------------------------------------
      // UpdateLogin - update the timestamp on a existing login in cache
      //-------------------------------------------------------------------
      public void
         UpdateLogin(
            int         loginHash,
            DateTime    timestamp
         )
      {		
         m_dirty = true;

         try
         {
            m_loginCache[ loginHash ] = timestamp;
         }
         catch
         {
            // probably doesnt exists - but just ignore for now
         }
      }
      
     
      #endregion
      
      #region Cache Persistence (SQL)
      
      //------------------------------------------------------------------
      // ReadCache - loads the cache for a particular instance
      //-------------------------------------------------------------------
      private void
         ReadCache()
      {
         Repository     rep       = new Repository();

         string cmdStr = String.Format( "SELECT TOP {0} loginKey,loginValue from {1} where instance = {2}",
            m_maxCacheSize,
            CoreConstants.RepositoryTemp_LoginsTable,
            SQLHelpers.CreateSafeString(m_instance) );

         m_loginCache = new Hashtable();

         try
         {
            rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
               
            using ( SqlCommand cmd = new SqlCommand( cmdStr, rep.connection ) )
            {
               cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                  while ( reader.Read() )
                  {
                     int      loginKey   = SQLHelpers.GetInt32( reader, 0);
                     DateTime loginValue = SQLHelpers.GetDateTime( reader, 1 );                  
                     m_loginCache.Add( loginKey, loginValue );
                  }
               }   
            }
         }
         catch( Exception ex )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
               String.Format( "ReadLoginCache {0} - {1}",
                              m_instance,
                              cmdStr ),
               ex );
         }
         finally
         {
            rep.CloseConnection();
         }
      }
      
      //------------------------------------------------------------------
      // WriteCache - writes the cache to the processing database
      //-------------------------------------------------------------------
      public void
         WriteCache()
      {
         // if not dirty nothing to write so just return      
         if ( ! m_dirty ) return;
     
         bool           bDeleted  = false;
         Repository     rep       = new Repository();
         
         string deleteStr = String.Format( "DELETE FROM {0} WHERE instance = {1}",
            CoreConstants.RepositoryTemp_LoginsTable,
            SQLHelpers.CreateSafeString(m_instance) );

         try
         {
            rep.OpenConnection( CoreConstants.RepositoryTempDatabase );
         
            try
            {
               // delete existing entries
               using ( SqlCommand cmd = new SqlCommand( deleteStr, rep.connection ) )
               {
                  cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                  cmd.ExecuteNonQuery();
               }
               bDeleted = true;
            }
            catch( Exception ex )
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                  String.Format( "WriteLoginCache (Delete) {0} - {1}",
                  m_instance,
                  deleteStr ),
                  ex );
            }
            
            if ( bDeleted )
            {
               string createStr = "INSERT INTO {0} (instance,loginKey,loginValue) VALUES ({1},{2},{3})";
               string createCmd = "";

               try
               {
                  // create new entries
                  IDictionaryEnumerator myEnumerator = m_loginCache.GetEnumerator();
                  while ( myEnumerator.MoveNext() )
                  {
                     createCmd = String.Format( createStr,
                        CoreConstants.RepositoryTemp_LoginsTable,
                        SQLHelpers.CreateSafeString(m_instance),
                        myEnumerator.Key,
                        SQLHelpers.CreateSafeDateTimeString((DateTime)myEnumerator.Value) );
                     using ( SqlCommand cmd = new SqlCommand( createCmd, rep.connection ) )
                     {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        cmd.ExecuteNonQuery();
                     }
                  }
                  
                  // once written, we are no longer dirty
                  m_dirty = false;
               }                                                 
               catch( Exception ex )
               {
                  ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                     String.Format( "WriteLoginCache (create) {0} - {1}",
                     m_instance,
                     createCmd ),
                     ex );
               }
            }
         }
         finally
         {
            rep.CloseConnection();
         }
      }
      
      #endregion
	}
}
