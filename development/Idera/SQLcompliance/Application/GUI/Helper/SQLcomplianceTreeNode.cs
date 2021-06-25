using System;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Controls ;

namespace Idera.SQLcompliance.Application.GUI.Helper
{

   public enum CMNodeType
   {
      Unknown,
      MgmtServer,
      ServerRoot,
      LogRoot,
      RoleRoot,
      AlertRoot,
      AuditByServerRoot,
      AuditServer,
      AuditByGroupRoot,
      AuditGroup,
      Server,
      Database,
      DatabaseGroup,
      ReportCategoryRoot,
      ReportCategory,
      Report,
      AlertRulesRoot,
      PrivilegedUsers,
      ArchivesRoot,
      Archive,
      LoginsRoot,
      EventFilters,
      ActivityLog,
      ChangeLog,
      //start sqlcm 5.6 - 5467
      DefaultSettings
      //end sqlcm 5.6 - 5467
   } ;

	/// <summary>
	/// Summary description for SQLcomplianceTreeNode.
	/// </summary>
   [Serializable()]
	public class SQLcomplianceTreeNode : TreeNode, IMenuFlags
	{
      private bool[] _flags ;
      
      #region Constructors
	   
		public SQLcomplianceTreeNode() 
		{
		   ResetMenuFlags();
		}
		
		public SQLcomplianceTreeNode( string s ) : base(s)
		{
		   ResetMenuFlags();
		}
		
		public SQLcomplianceTreeNode( string s, int i1, int i2 ) : base(s,i1,i2)
		{
		   ResetMenuFlags();
		}

		public SQLcomplianceTreeNode( string s, int i1, int i2, CMNodeType inType ) : base(s,i1,i2)
		{
         _flags = new bool[Enum.GetValues(typeof(CMMenuItem)).Length] ;
		    type = inType;
		    ResetMenuFlags();
		}
		
		#endregion
		
		#region Context Menu Flag Handling
	
      private void ResetMenuFlags()
      {
         for ( int i=0; i<_flags.Length; i++)
         {
            _flags[i] = false;
         }
      }

      public bool GetMenuFlag(CMMenuItem item)
      {
         return _flags[(int)item] ;
      }

      public void SetMenuFlag(CMMenuItem item)
      {
         SetMenuFlag(item, true) ;
      }

      public void SetMenuFlag(CMMenuItem item, bool flag)
      {
         if(_flags[(int)item] != flag)
         {
            _flags[(int)item] = flag ;
         }
      }
		
		#endregion
		
		#region Properties
		
		private CMNodeType type = CMNodeType.Unknown ;
		public CMNodeType Type
		{
		   get { return type;  }
		   set { type = value; }
		}
		
		#endregion
	}
}
