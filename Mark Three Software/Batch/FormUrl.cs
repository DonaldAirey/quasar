namespace MarkThree
{

	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Web;

	/// <summary>Dialog Box used to select a Url.</summary>
	public class FormUrl : System.Windows.Forms.Form
	{

		private const int mruListLength = 4;
		private const string mruKeyName = "webServiceMru";
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.ComboBox comboBoxUrl;
		private UserPreferences userPreferences;
		private IContainer components;
	
		private delegate DialogResult ShowDialogDelegate(IWin32Window owner);

		/// <summary>Gets the Url from the dialog.</summary>
		public string Url {get {return this.comboBoxUrl.Text;}}

		/// <summary>
		/// Initializes a new instance of the MarkThree.Client.FormUrl class.
		/// </summary>
		public FormUrl()
		{

			// Required for the Designer.
			InitializeComponent();

		}

		/// <summary>Releases all the resources used by MarkThree.Client.FormUrl</summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only
		/// unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{

			// Clean up the resources used by installed components.
			if (disposing)
				if (components != null)
					components.Dispose();

			// Call the base class to finish up the cleanup.
			base.Dispose(disposing);

		}

		#region Windows Form Designer generatedCode
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with theCode editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.comboBoxUrl = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.userPreferences = new MarkThree.UserPreferences(this.components);
			this.SuspendLayout();
			// 
			// comboBoxUrl
			// 
			this.comboBoxUrl.Location = new System.Drawing.Point(8, 32);
			this.comboBoxUrl.Name = "comboBoxUrl";
			this.comboBoxUrl.Size = new System.Drawing.Size(488, 21);
			this.comboBoxUrl.TabIndex = 0;
			this.comboBoxUrl.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxUrl_Validating);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(184, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Please select a Web Server:";
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(248, 64);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(336, 64);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonHelp
			// 
			this.buttonHelp.Location = new System.Drawing.Point(424, 64);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(75, 23);
			this.buttonHelp.TabIndex = 4;
			this.buttonHelp.Text = "Help";
			// 
			// FormUrl
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(506, 96);
			this.ControlBox = false;
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxUrl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormUrl";
			this.ShowInTaskbar = false;
			this.Text = "Client Connect";
			this.ResumeLayout(false);

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
			
			// Load up the combo box with the most recently used URLs.
			for (int counter = 0; counter < FormUrl.mruListLength; counter++)
			{

				// There is no way to save or load an ordered list in the application settings, so a group of strings
				// with the index embedded in the key name are used to keep track of the last couple of URLs used.
				// This will 'guess' at the next element in the most recently used list.  If it doesn't exist, then
				// all the URLs have been read.
				object urlOption = UserPreferences.LocalSettings[string.Format("{0}{1}", mruKeyName, counter)];
				if (urlOption == null)
					break;

				// Add the URL item to the combobox.
                this.comboBoxUrl.Items.Add(urlOption);

			}

			// Select the most recently used URL for the combo box.
			if (this.comboBoxUrl.Items.Count != 0)
				this.comboBoxUrl.SelectedIndex = 0;

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

				// When writing out the most recently used list, don't write too many URLs.  The number of elements is limited by
				// the 'mruListLength' parameter.  The first item in the list will be the one selected in the text field.
				int minCount = FormUrl.mruListLength < this.comboBoxUrl.Items.Count ? FormUrl.mruListLength :
					this.comboBoxUrl.Items.Count;
				for (int counter = 0; counter < minCount; counter++)
					UserPreferences.LocalSettings[string.Format("{0}{1}", mruKeyName, counter)] = this.comboBoxUrl.Items[counter];

			}

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
		/// Validates the data in the URL dialog.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The event arguments.</param>
		private void comboBoxUrl_Validating(object sender, CancelEventArgs e)
		{

			try
			{

				// This will check that the data in the URL edit box is a properly formed URL.  If it isn't, an exception will
				// be generated.
				Uri uri = new Uri(this.comboBoxUrl.Text);

				// Add the most recently added item into the combo as the first item in the list.
				this.comboBoxUrl.Items.Insert(0, this.comboBoxUrl.Text);
				this.comboBoxUrl.SelectedIndex = 0;

				// Remove any redundant entries.
				for (int index = 1; index < this.comboBoxUrl.Items.Count; index++)
					if ((string)this.comboBoxUrl.Items[index] == (string)this.comboBoxUrl.SelectedItem)
						this.comboBoxUrl.Items.RemoveAt(index);

			}
			catch (Exception exception)
			{

				// Display any problems attempting to parse the URL.
				MessageBox.Show(exception.Message, "URL Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

				// Any action -- such as dismissing the dialog box -- will be cancelled if the URL isn't properly formed.
				e.Cancel = true;

			}

		}

	}

}
