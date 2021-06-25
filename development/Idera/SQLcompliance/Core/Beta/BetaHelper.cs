#define BETA_RELEASE

using System;

namespace Idera.SQLcompliance.Core.Beta
{
	/// <summary>
	/// Summary description for BetaHelper.
	/// </summary>
	public class BetaHelper
	{
		private BetaHelper()
		{
		}
		
		public const string Title_Beta            = "SQL Compliance Manager Beta Release";
		public const string Msg_BetaExpired       = "This beta release of SQL Compliance Manager has expired and cannot be used. Please contact IDERA technical support for assistance.";
		public const string Msg_BetaAboutToExpire = "This beta release of SQL Compliance Manager will expire within the next few days. Please contact IDERA technical support for assistance.";
		
		//-------------------------------------------------------
		// IsBeta - tests the date set via a #define during
		//          the build process - if it has then major
		//          components display or log a message and die
		//-------------------------------------------------------
		public static bool
		   IsBeta
		{
		   get{ return BetaConstants.betaRelease; }
		}
		
		//------------------------------------------------------------------
		// IsBetaAboutToExpire - tests if we are within 7 days of beta
		//                       expiration date so we can warn use
		//------------------------------------------------------------------
		public static bool
		   IsBetaAboutToExpire
		{
		   get
		   {
		      bool betaAboutToExpire = false;
   		   
		      if (BetaConstants.betaRelease )
		      {
		         if ( BetaConstants.betaDropDeadDate != DateTime.MaxValue )
		         {
		            DateTime futureDate = DateTime.Now.AddDays(7);
		            
		            if ( BetaConstants.betaDropDeadDate < futureDate )
		            {
		               betaAboutToExpire = true;
		            }
		         }
		      }
   		   
		      return betaAboutToExpire;
		   }
		}

		//------------------------------------------------------------------
		// IsBetaExpired - tests the date set via a #define during
		//                 the build process - if it has then major
		//                 components display or log a message and die
		//------------------------------------------------------------------
		public static bool
		   IsBetaExpired
		{
		   get
		   {
		      bool betaExpired = false;
   		   
		      if ( BetaConstants.betaRelease )
		      {
		         if ( BetaConstants.betaDropDeadDate != DateTime.MaxValue )
		         {
		            if ( BetaConstants.betaDropDeadDate < DateTime.Now )
		            {
		               betaExpired = true;
		            }
		         }
		      }
   		   
		      return betaExpired;
		   }
		}
	}
}
