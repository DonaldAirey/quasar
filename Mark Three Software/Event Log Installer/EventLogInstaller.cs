namespace MarkThree.InstallEventLog
{

	using System;
	using System.Diagnostics;
	using System.Collections;
	using System.ComponentModel;
	using System.Configuration.Install;
	using System.Windows.Forms;
	
	[RunInstaller(true)]
	public class WebTransactionEventLogInstaller : Installer
	{

		private EventLogInstaller eventLogInstaller;

		/// <summary>
		/// Install an Event Log for Web Service events.
		/// </summary>
		public WebTransactionEventLogInstaller()
		{

			// Note that the log and source aren't specified until the actual installation events.  The 'log' and 'source' values
			// for the event log are passed into the installer as parameters.  Those parameters aren't available to this
			// constructor, so they can't be filled in until the installation event.  Also note that the log is to be removed from
			// the registry when the application is removed from the system.
			this.eventLogInstaller = new EventLogInstaller();
			this.eventLogInstaller.UninstallAction = UninstallAction.Remove;

			// Add eventLogInstaller to the Installers Collection.
			Installers.Add(eventLogInstaller);

		}

		/// <summary>
		/// Install the event logs.
		/// </summary>
		/// <param name="stateSaver">Information about the state of the installation.</param>
		public override void Install(System.Collections.IDictionary stateSaver)
		{

			// Get the name of the log and the source from the parameters.  These can be found in the 'CustomActionData' property
			// of the 'Custom Actions' for this setup project.
			eventLogInstaller.Log = this.Context.Parameters["log"];
			eventLogInstaller.Source = this.Context.Parameters["source"];

			// Call the base class to complete the installation/uninstallation of the event logs.
			base.Install(stateSaver);

		}

		/// <summary>
		/// Uninstall the event logs.
		/// </summary>
		/// <param name="stateSaver">Information about the state of the installation.</param>
		public override void Uninstall(System.Collections.IDictionary stateSaver)
		{

			// Get the name of the log and the source from the parameters.  These can be found in the 'CustomActionData' property
			// of the 'Custom Actions' for this setup project.
			eventLogInstaller.Log = this.Context.Parameters["log"];
			eventLogInstaller.Source = this.Context.Parameters["source"];

			// Call the base class to complete the installation/uninstallation of the event logs.
			base.Uninstall(stateSaver);

		}

	}

}
