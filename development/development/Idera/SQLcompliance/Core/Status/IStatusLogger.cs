using System;

using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Core.Status
{
	/// <summary>
	/// Interface description for IStatusLogger
	/// </summary>
	public interface IStatusLogger
	{
		/// <summary>
		/// Log a status event
		/// </summary>
		/// <param name="status">The AgentStatus status event</param>
		bool [] SendStatus( AgentStatusMsg status );
		bool GetAuditSettings( AgentStatusMsg status );
	}
}

