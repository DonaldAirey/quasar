namespace MarkThree.MiddleTier.Administrator
{

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	/// <summary>
	/// Administers the Middle Tier runtime.
	/// </summary>
	public partial class FormAdministrator : Form
	{

		/// <summary>
		/// Administers the metadata for the middle tier runtime.
		/// </summary>
		public FormAdministrator()
		{

			// The IDE managed components are initialized here.
			InitializeComponent();

		}

		/// <summary>
		/// Allow the user to manage the mappings between certificates and Windows User Accounts.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event parameters.</param>
		private void accountMappingsToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// This will allow the usre to manage the mappings between certificates and Windows User Accounts.
			AccountMappings accountMappings = new AccountMappings();
			accountMappings.ShowDialog();

		}

	}

}