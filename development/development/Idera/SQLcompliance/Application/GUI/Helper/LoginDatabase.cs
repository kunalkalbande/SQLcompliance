using System;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for LoginDatabase.
	/// </summary>
	public class LoginDatabase
	{
	   public string   databaseName;
	   public string   databaseUser;

	   public bool     chked         = false;
	   public bool     canViewEvents = false;
	   public bool     canViewSQL    = false;
	   public bool     cannotView    = false;

      public bool     originalChked         = false;	   
	   public bool     originalCanViewEvents = false;
	   public bool     originalCanViewSQL    = false;
	   public bool     originalCannotView    = false;
	   
      public string           DisplayMember
      {
         get
         {
            string t = databaseName;
            
            if ( databaseUser!=null && databaseUser!= "")
            {
               t += "  User: " + databaseUser;
            }
            
            return t;
         }
      }
      override public string  ToString() { return databaseName; }
	}
}
