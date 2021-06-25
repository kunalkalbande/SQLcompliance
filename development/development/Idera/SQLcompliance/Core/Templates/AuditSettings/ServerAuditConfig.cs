using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Core.Templates.AuditSettings
{
   public class ServerAuditConfig : AuditConfig
   {
      protected override bool IsValidCategory(AuditCategory cat)
      {
         switch ( cat )
         {
            case AuditCategory.SELECT:
            case AuditCategory.DML:
               return false;

            default:
               return true;
         }
      }
   }
}
