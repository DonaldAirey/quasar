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
	/// Prompts the user for the fields used to map a certificate to a user account.
	/// </summary>
	public partial class FormMapToSingleAccount : Form
	{

		// Private Members
		private string originalPassword;

		/// <summary>
		/// Creates the dialog to prompt the user for a certificate/account mapping.
		/// </summary>
		public FormMapToSingleAccount()
		{

			// The IDE supported code is initialized here.
			InitializeComponent();

		}

		/// <summary>
		/// Indicates that the certificate in this mapping should be used to allow a user to connect.
		/// </summary>
		public bool IsEnabled
		{
			get { return this.checkBoxEnableMapping.Checked; }
			set { this.checkBoxEnableMapping.Checked = value; }
		}

		/// <summary>
		/// The Windows user account name to associate with a certificate.
		/// </summary>
		public string Account
		{
			get { return this.textBoxAccount.Text; }
			set { this.textBoxAccount.Text = value; }
		}

		/// <summary>
		/// The password of the user account.
		/// </summary>
		public string Password
		{
			get { return this.textBoxPassword.Text; }
			set { this.originalPassword = this.textBoxPassword.Text = value; }
		}

		/// <summary>
		/// Handles the clicking of the 'OK' button.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event parameters.</param>
		private void buttonOK_Click(object sender, EventArgs e)
		{

			// Since the password is hidden while it is typed in, the user is required to type it in twice before it is accepted.
			// This will test the dialog to see if the password has been modified since it was initialized.  If the password was
			// changed, then the user is prompted to type it again.
			bool isPasswordConfirmed = this.originalPassword == this.textBoxPassword.Text;
			if (!isPasswordConfirmed)
				using (FormConfirmPassword formConfirmPassword = new FormConfirmPassword())
				{
					formConfirmPassword.Password = this.textBoxPassword.Text;
					isPasswordConfirmed = formConfirmPassword.ShowDialog() == DialogResult.OK;
				}

			// The dialog can be dismissed when the same password has been entered twice.
			if (isPasswordConfirmed)
			{
				DialogResult = DialogResult.OK;
				Close();
			}

		}

	}

}