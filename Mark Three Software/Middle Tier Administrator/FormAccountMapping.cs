namespace MarkThree.MiddleTier.Administrator
{

	using MarkThree;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Security.Cryptography.X509Certificates;
	using System.Text;
	using System.Windows.Forms;

	/// <summary>
	/// Prompts the user for the mappings of X509 Certificates to Windows User accounts.
	/// </summary>
	public partial class AccountMappings : Form
	{

		// Private Members
		private CertificateMappingStore certificateMappingStore;

		/// <summary>
		/// Construct a dialog to prompt the user for mappings between X509 Certificates and Windows User Accounts.
		/// </summary>
		public AccountMappings()
		{

			// Initialize the object.
			this.certificateMappingStore = new CertificateMappingStore();

			// The IDE created components are initialized here.
			InitializeComponent();

			// This will populate the ListView with each of the mappings stored.  The name of the mapping is used as the primary
			// key for finding items again in the list.
			foreach (CertificateMap certificateMap in this.certificateMappingStore.CertificateMaps)
			{
				ListViewItem listViewItem = new ListViewItem();
				listViewItem.Name = certificateMap.MappingName;
				this.listViewOneToOne.Items.Add(listViewItem);
				listViewItem.Checked = certificateMap.IsEnabled;
				listViewItem.SubItems.Add(certificateMap.MappingName);
				listViewItem.SubItems.Add(certificateMap.Account);
			}

		}

		/// <summary>
		/// Adds a mapping between a client certificate and a Windows User Account.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonAdd_Click(object sender, EventArgs e)
		{

			try
			{

				// This will prompt the user to select a file-based certificate.  The addition of a mapping can't go forward
				// without a valid certificate file.
				if (this.openFileDialogCertificate.ShowDialog() == DialogResult.OK)
				{

					// Open up the selected file and read the data into an X509 Certificate.  This certificate will be used to map
					// an incoming certificate in an SSL stream to a Windows User Account.
					Stream stream = this.openFileDialogCertificate.OpenFile();
					byte[] rawData = new byte[stream.Length];
					stream.Read(rawData, 0, (int)stream.Length);
					X509Certificate2 x509Certificate2 = new X509Certificate2(rawData);

					// This dialog will prompt the user for the account information.
					using (FormMapToAccount formMapToAccount = new FormMapToAccount())
					{

						// The mapping is enabled by default for a new item.
						formMapToAccount.IsEnabled = true;

						// Prompt the user for the Windows User Account information that is used to impersonate a user when the
						// incoming certificate in an SSL stream matches the certificate selected above.
						if (formMapToAccount.ShowDialog() == DialogResult.OK)
						{

							// When the user has successfully entered a mapping to an account it is added to the store.
							CertificateMap certificateMap = new CertificateMap(formMapToAccount.MappingName, x509Certificate2,
								formMapToAccount.Account, formMapToAccount.Password);
							this.certificateMappingStore.Add(certificateMap);

							// This will update the list view with a new entry for the mapping.
							ListViewItem listViewItem = new ListViewItem();
							listViewItem.Name = certificateMap.MappingName;
							this.listViewOneToOne.Items.Add(listViewItem);
							listViewItem.Checked = certificateMap.IsEnabled;
							listViewItem.SubItems.Add(certificateMap.MappingName);
							listViewItem.SubItems.Add(certificateMap.Account);

							// The 'Apply' button is enabled to show that there are changes to be committed.
							this.buttonApply.Enabled = true;

						}

					}

				}

			}
			catch (Exception exception)
			{

				// The most likely exception is an inability to load the certificate from a file.
				MessageBox.Show(this, exception.Message, "Account Mapping");

			}

		}

		/// <summary>
		/// Delete a mapping between a client certificate and a Windows User Account.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonDelete_Click(object sender, EventArgs e)
		{

			// Delete each of the items selected in the ListView.
			foreach (ListViewItem listViewItem in this.listViewOneToOne.SelectedItems)
			{
				string mappingName = listViewItem.Name;
				this.certificateMappingStore.Delete(mappingName);
				this.listViewOneToOne.Items.Remove(listViewItem);
			}

			// The 'Apply' button is enabled to show that there are changes to be committed.
			this.buttonApply.Enabled = true;

		}

		/// <summary>
		/// Edits a mapping between a client certificate and a Windows User Account.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonEditMap_Click(object sender, EventArgs e)
		{

			// Mapping between a single account and several certificates is made when more than one mapping is selected for 
			// editing.
			if (this.listViewOneToOne.SelectedItems.Count > 1)
			{

				// This dialog will prompt the user for a single account.
				using (FormMapToSingleAccount formMapToSingleAccount = new FormMapToSingleAccount())
				{

					// The edited items will be enabled by default.
					formMapToSingleAccount.IsEnabled = true;

					// This will prompt the user for a single account to which the accounts should be mapped and then run through
					// each of the selected items and modify them.
					if (formMapToSingleAccount.ShowDialog() == DialogResult.OK)
						foreach (ListViewItem listViewItem in this.listViewOneToOne.SelectedItems)
						{

							// Find the certificate in the store based on the text stored with the list item when it was created.
							// The text is used as a key for the list.
							CertificateMap certificateMap = this.certificateMappingStore.Find(listViewItem.Name);

							// Copy the info from the dialog into each of the selected mappings.  This will change the account to 
							// which each of the selected certificates is mapped.
							certificateMap.Account = formMapToSingleAccount.Account;
							certificateMap.IsEnabled = formMapToSingleAccount.IsEnabled;
							certificateMap.Password = formMapToSingleAccount.Password;

							// This will update each of the items in the ListView with the account entered into the dialog.
							listViewItem.Tag = certificateMap.MappingName;
							listViewItem.SubItems[2].Text = certificateMap.Account;
							listViewItem.Checked = certificateMap.IsEnabled;

							// The 'Apply' button is enabled to show that there are changes to be committed.
							this.buttonApply.Enabled = true;

						}

				}

			}

			// When only a single account is selected all the members of the mapping are displayed in the prompt.
			if (this.listViewOneToOne.SelectedItems.Count == 1)
			{

				// This dialog is used to prompt the user for the Mapping Name as well as the other components of the
				// Windows User Account.
				using (FormMapToAccount formMapToAccount = new FormMapToAccount())
				{

					// Populate the dialog box items with the mapping selected in the ListView.
					ListViewItem listViewItem = this.listViewOneToOne.SelectedItems[0];
					CertificateMap certificateMap = this.certificateMappingStore.Find(listViewItem.Name);
					formMapToAccount.Account = certificateMap.Account;
					formMapToAccount.IsEnabled = certificateMap.IsEnabled;
					formMapToAccount.MappingName = certificateMap.MappingName;
					formMapToAccount.Password = certificateMap.Password;

					// This will prompt the user for all the elements of the Windows User Account mapping.
					if (formMapToAccount.ShowDialog() == DialogResult.OK)
					{

						// If the elements are entered properly then copy the data out of the dialog and into the
						// local mapping storage.
						certificateMap.Account = formMapToAccount.Account;
						certificateMap.IsEnabled = formMapToAccount.IsEnabled;
						certificateMap.MappingName = formMapToAccount.MappingName;
						certificateMap.Password = formMapToAccount.Password;

						// This will copy the updated fields (the mapping name and the account into the ListView.
						listViewItem.Name = certificateMap.MappingName;
						listViewItem.SubItems[1].Text = certificateMap.MappingName;
						listViewItem.SubItems[2].Text = certificateMap.Account;
						listViewItem.Checked = certificateMap.IsEnabled;

						// The 'Apply' button is enabled to show that there are changes to be committed.
						this.buttonApply.Enabled = true;

					}

				}

			}

		}

		/// <summary>
		/// Commits the modifications to the local data store.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonApply_Click(object sender, EventArgs e)
		{

			// Save the local copy of the data store to a persistent store.
			this.certificateMappingStore.Save();

			// Allow the user to continue with the changes.
			this.buttonApply.Enabled = false;

		}

		/// <summary>
		/// Commits the modifications to the local data store and exits the dialog.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonOK_Click(object sender, EventArgs e)
		{

			// Save the local copy of the data store to a persistent store and exit the dialog.
			this.certificateMappingStore.Save();
			this.DialogResult = DialogResult.OK;
			Close();

		}

	}

}