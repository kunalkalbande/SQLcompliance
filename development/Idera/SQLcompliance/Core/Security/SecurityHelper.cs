using System;
using System.Security;
using System.Security.Principal;
using System.Threading;

namespace Idera.SQLcompliance.Core.Security
{
	/// <summary>
	/// Summary description for SecurityHelper.
	/// </summary>
	internal class SecurityHelper
	{
		public SecurityHelper()
		{
			//
			// TODO: Add constructor logic here
			//
		}

      /// <summary>
      /// Impersonate using the identity in this context.
      /// </summary>
      public static WindowsImpersonationContext
         ImpersonateAsIdentity(
         WindowsIdentity             identity
         ) 
      {
         WindowsImpersonationContext context = null;
         if (identity != null) 
         {
            try 
            {
               context = identity.Impersonate();
            } 
            catch (Exception e) 
            {
               throw new CoreException(CoreConstants.Exception_CouldNotImpersonateUser, e);
            }
         }

         return context;
      }

      /// <summary>
      /// Undo the impersonation in the context.
      /// </summary>
      public static void 
         UndoImpersonation(
         WindowsIdentity identity,
         WindowsImpersonationContext context
         ) 
      {
         if (identity != null && context != null) 
         {
            context.Undo();
         }
      }
      
      //-----------------------------------------------------------------------
      // IsLocalAdmin
      //-----------------------------------------------------------------------
      public static bool
         IsLocalAdmin()
      {
         bool isAdmin = false;

         try
         {
		      AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
		      WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
		      WindowsIdentity identity = (WindowsIdentity)principal.Identity;
		      isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
		   }
		   catch {}
		   
		   return isAdmin;
      }
      
   }
}
