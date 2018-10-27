namespace MarkThree.MiddleTier.Administrator
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	public partial class FormConfirmPassword : Form
	{

		// Private Members
		private string password;

		/// <summary>
		/// Creates a form that prompts for a confirmation of the password.
		/// </summary>
		public FormConfirmPassword()
		{

			// The IDE managed components are initialized here.
			InitializeComponent();

		}

		/// <summary>
		/// Sets the password to be confirmed.
		/// </summary>
		public string Password { set { this.password = value; } }

		/// <summary>
		/// Handles the OK button.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event parameters.</param>
		private void buttonOK_Click(object sender, EventArgs e)
		{

			// If the password entered in this prompt matches the initial password then the dialog
			// can exit successfully.  Otherwise the user is prompted to try again.
			if (this.textBoxPassword.Text == password)
			{
				this.DialogResult = DialogResult.OK;
				Close();
			}
			else
			{
				MessageBox.Show(this, "You did not correctly type the password.", "Certificate Mapping",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

		}

	}

}