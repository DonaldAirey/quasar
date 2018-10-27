namespace MarkThree
{

	using System;
	using System.Configuration;
	using System.Diagnostics;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Net;

	/// <summary>Dialog Box used to select a Url and enter user credentials.</summary>
	public class FormNetworkCredential : System.Windows.Forms.Form
	{

		// Private Members
		private const string usernameKey = "userName";
		private const string domainKey = "domain";
		private System.Windows.Forms.Label labelUserName;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.TextBox textBoxUserName;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private IContainer components;
		private System.Windows.Forms.TextBox textBoxDomain;
		private System.Windows.Forms.Label labelDomain;
		private UserPreferences userPreferences;
	
		private delegate DialogResult ShowDialogDelegate(IWin32Window owner);

		// Public Members
		public NetworkCredential NetworkCredential;

		/// <summary>
		/// Creates a screen that asks for Basic Network Credentials (User Name, Password).
		/// </summary>
		public FormNetworkCredential()
		{

			// Required for the Designer.
			InitializeComponent();

			// Create a new set of credentials.  This can be overridden by the caller to get a predefined set of credentials.
			this.NetworkCredential = new NetworkCredential();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{

			// Clean up the resources used by installed components.
			if (disposing && components != null) 
				components.Dispose();

			// Call the base class to finish up the cleanup.
			base.Dispose(disposing);

		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.labelUserName = new System.Windows.Forms.Label();
			this.labelPassword = new System.Windows.Forms.Label();
			this.textBoxUserName = new System.Windows.Forms.TextBox();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.textBoxDomain = new System.Windows.Forms.TextBox();
			this.labelDomain = new System.Windows.Forms.Label();
			this.userPreferences = new MarkThree.UserPreferences(this.components);
			this.SuspendLayout();
			// 
			// labelUserName
			// 
			this.labelUserName.Location = new System.Drawing.Point(8, 8);
			this.labelUserName.Name = "labelUserName";
			this.labelUserName.Size = new System.Drawing.Size(72, 16);
			this.labelUserName.TabIndex = 0;
			this.labelUserName.Text = "&User Name:";
			this.labelUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(8, 40);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(72, 16);
			this.labelPassword.TabIndex = 4;
			this.labelPassword.Text = "&Password:";
			this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBoxUserName
			// 
			this.textBoxUserName.Location = new System.Drawing.Point(88, 8);
			this.textBoxUserName.Name = "textBoxUserName";
			this.textBoxUserName.Size = new System.Drawing.Size(216, 20);
			this.textBoxUserName.TabIndex = 1;
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(88, 40);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.Size = new System.Drawing.Size(216, 20);
			this.textBoxPassword.TabIndex = 2;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(144, 104);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(232, 104);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			// 
			// textBoxDomain
			// 
			this.textBoxDomain.Location = new System.Drawing.Point(88, 72);
			this.textBoxDomain.Name = "textBoxDomain";
			this.textBoxDomain.Size = new System.Drawing.Size(216, 20);
			this.textBoxDomain.TabIndex = 3;
			// 
			// labelDomain
			// 
			this.labelDomain.Location = new System.Drawing.Point(8, 72);
			this.labelDomain.Name = "labelDomain";
			this.labelDomain.Size = new System.Drawing.Size(72, 16);
			this.labelDomain.TabIndex = 11;
			this.labelDomain.Text = "&Log on to:";
			this.labelDomain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormNetworkCredential
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(314, 136);
			this.ControlBox = false;
			this.Controls.Add(this.labelDomain);
			this.Controls.Add(this.textBoxDomain);
			this.Controls.Add(this.textBoxPassword);
			this.Controls.Add(this.textBoxUserName);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.labelPassword);
			this.Controls.Add(this.labelUserName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormNetworkCredential";
			this.ShowInTaskbar = false;
			this.Text = "Network Credentials";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Populates the dialog with data when it is created.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected override void OnLoad(EventArgs eventArgs)
		{

			// Give the base class a chance to populate any controls.
			base.OnLoad(eventArgs);
			
			// Load the credential info
			this.textBoxUserName.Text = this.NetworkCredential.UserName;
			this.textBoxDomain.Text = this.NetworkCredential.Domain;

		}

		/// <summary>ShowDialog</summary>
		/// <param name="owner">Any object that implements an IWin32Window interface that represents a top-level window
		/// that will own the modal dialog box.</param>
		/// <remarks>Whether by design or flaw, when a dialog box is run from outside the main application window's thread
		/// it will not block the main window.  That is, non-modally.  To force a dialog box created outside the main thread
		/// to block user commands from reaching the main window, we need to ask the main window to run the dialog box.
		/// This function provides the extra steps necessary to make the dialog boxes run properly.</remarks>
		public new DialogResult ShowDialog()
		{

			// Invoking a dialog box from outside the main window's thread requires a little jury-rigging.  We need to ask
			// the main window of the application to run the dialog for us, otherwise it runs modally (if that's an
			// real adverb).  Details are: create a delegate that matches the parameters we want to pass in and out of the
			// dialog box's 'ShowDialog' method.  Then, ask the main window (that we extract from the process information)
			// to run the dialog box on our behalf (we're in a different thread here).  Return the thread-safe results
			// back to the caller.  Note that I did try to use the 'Forms.ActiveForm', but found it to be too unreliable
			// to get the main window for the application when accessed from a DLL.
			ShowDialogDelegate showDialogDelegate = new ShowDialogDelegate(ShowDialogHandler);
			Form mainForm = (Form)Form.FromHandle(Process.GetCurrentProcess().MainWindowHandle);
			DialogResult dialogResult = (mainForm == null) ? base.ShowDialog() :
				(DialogResult)mainForm.Invoke(showDialogDelegate, new object[] {mainForm});

			// If one of the URL strings was selected, then remember it in the user preferences.
			if (dialogResult == DialogResult.OK)
			{

				// update the credential info see if the password changed
				this.NetworkCredential.UserName = this.textBoxUserName.Text;
				this.NetworkCredential.Password = this.textBoxPassword.Text;
				this.NetworkCredential.Domain = this.textBoxDomain.Text;

				// Save the Credential Info (Note that the password is not saved for security reasons).
				UserPreferences.LocalSettings[usernameKey] = this.NetworkCredential.UserName;
				UserPreferences.LocalSettings[domainKey] = this.NetworkCredential.Domain;

			}

			// The caller uses this value to determine how the user exited the dialog.
			return dialogResult;

		}

		/// <summary>showDialogDelegate</summary>
		/// <param name="owner">This method is called from outside the thread that created the dialog box to process the
		/// dialog modally. </param>
		private DialogResult ShowDialogHandler(IWin32Window owner)
		{

			// At this point the code is running in the main Window's thread.  It's safe to call the base class to display and
			// execute the dialog.
			return base.ShowDialog(owner);

		}

	}

}
