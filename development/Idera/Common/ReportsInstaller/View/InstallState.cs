using System;

namespace Idera.Common.ReportsInstaller.View
{
	public delegate void InstallationEventHandler();

	/// <summary>
	/// Summary description for InstallState.
	/// </summary>
	public class InstallState
	{
		public InstallState()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Defines what happens when the user wishes to go to the previous panel.
		/// </summary>
		private event InstallationEventHandler installationEvent;
		public InstallationEventHandler InstallationEvent
		{
			get
			{
				return installationEvent;
			}
			set
			{
				installationEvent += value;
			}
		}

		/// <summary>
		/// Performs the actions required to go to the next panel.
		/// </summary>
		public void Install()
		{
			installationEvent();
		}
	}
}
