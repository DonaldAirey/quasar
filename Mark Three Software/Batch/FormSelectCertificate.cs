namespace MarkThree
{

	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Windows.Forms;
	using System.Security.Cryptography.X509Certificates;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Summary description for FormSelectCertificate.
	/// </summary>
	public class FormSelectCertificate : System.Windows.Forms.Form
	{

		private const string myCertificateStoreName = "My";
		private System.Windows.Forms.ListBox listBoxCertificates;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label2;
		private System.Security.Cryptography.X509Certificates.X509CertificateCollection x509CertificateCollection;
		private UserPreferences userPreferences;
		private IContainer components;
	
		private delegate DialogResult ShowDialogDelegate(IWin32Window owner);

		/// <summary>
		/// The certificate selected by the user.
		/// </summary>
		public X509Certificate SelectedCertificate
		{

			get
			{

				// This is the certificate selected by the user from the list box.  Use the index into the
				// list box (which only contains a descriptive text of the certificate) to choose the binary
				// certificate from the collection.
				int selectedIndex = this.listBoxCertificates.SelectedIndex;
				return selectedIndex == -1 ? null : this.x509CertificateCollection[selectedIndex];

			}

		}
		
		/// <summary>
		/// Initialize the dialog box used to select certificates.
		/// </summary>
		public FormSelectCertificate()
		{

			// This is needed for the designer.
			InitializeComponent();

			// Place an image of the icon in the dialog (I've found no way to do this from the designer).
			this.label2.Image = SystemIcons.Exclamation.ToBitmap();

			// This collection contains the certificates in the 'My' certificate store.  When a selection is made from the
			// list box, the corresponding certificate in this selection is made available to the caller.
			this.x509CertificateCollection = new X509CertificateCollection();

		}

		#region Dispose Batch.Method
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.listBoxCertificates = new System.Windows.Forms.ListBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.userPreferences = new MarkThree.UserPreferences(this.components);
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listBoxCertificates
			// 
			this.listBoxCertificates.HorizontalScrollbar = true;
			this.listBoxCertificates.Location = new System.Drawing.Point(64, 72);
			this.listBoxCertificates.Name = "listBoxCertificates";
			this.listBoxCertificates.Size = new System.Drawing.Size(272, 147);
			this.listBoxCertificates.TabIndex = 0;
			this.listBoxCertificates.DoubleClick += new System.EventHandler(this.listBoxCertificates_DoubleClick);
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(184, 248);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(64, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(272, 32);
			this.label1.TabIndex = 2;
			this.label1.Text = "The Web Site you want to view requires identification.  Select the certificate to" +
				" use when connecting.";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(344, 224);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Identification";
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.SystemColors.Control;
			this.label2.Location = new System.Drawing.Point(16, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 32);
			this.label2.TabIndex = 5;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(272, 248);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			// 
			// FormSelectCertificate
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(362, 286);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.listBoxCertificates);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSelectCertificate";
			this.ShowInTaskbar = false;
			this.Text = "Client Authentication";
			this.TransparencyKey = System.Drawing.Color.Cyan;
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Initializes the dialog box.
		/// </summary>
		/// <param name="eventArgs">Event arguments.</param>
		protected override void OnLoad(EventArgs eventArgs)
		{

			// Call the base class to inherit any initialization.
			base.OnLoad(eventArgs);

			// Load up the certificates from the personal certificate store.
			this.x509CertificateCollection = Crypt.GetCertificateStore(myCertificateStoreName);

			// Parse the friendly name out of the different tags in the certificate name and place them
			// in the list box.
			foreach (X509Certificate x509Certificate in this.x509CertificateCollection)
			{
				Regex regex = new Regex("CN=(?<friendlyName>[^,]+)");

				MatchCollection matches = regex.Matches(x509Certificate.Subject);
				foreach (Match match in matches)
					if (match.Groups["friendlyName"].Value != "Users")
					{
						this.listBoxCertificates.Items.Add(match.Groups["friendlyName"].Value);
						break;
					}

			}

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
				UserPreferences.LocalSettings["certificateName"] = this.SelectedCertificate.Subject;

			// The caller uses this value to determine how the user exited the dialog.
			return dialogResult;

		}

		/// <summary>showDialogDelegate</summary>
		/// <param name="owner">This method is called from outside the thread that created the dialog box to process the
		/// dialog modally. </param>
		private DialogResult ShowDialogHandler(IWin32Window owner)
		{

			// At this point the code is running in the main Window's thread.  It's safe to call the base class to display
			// and execute the dialog.
			return base.ShowDialog(owner);

		}

		/// <summary>
		/// Handles the double clicking of an item in the list box.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void listBoxCertificates_DoubleClick(object sender, EventArgs e)
		{

			// A double click is the same as hitting the 'OK' button so long as an item is selected.
			if (this.listBoxCertificates.SelectedIndex != -1)
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}

		}

	}

}
