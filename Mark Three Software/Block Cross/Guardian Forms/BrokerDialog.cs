namespace MarkThree.Guardian.Forms
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Data;
	using System.Diagnostics;
	using System.Drawing;
	using System.Threading;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for BrokerDialog.
	/// </summary>
	public class BrokerDialog : System.Windows.Forms.Form
	{

		private int brokerId;
		private System.Windows.Forms.ComboBox comboBoxBrokerSymbol;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.ComponentModel.Container components = null;
		private DataTable brokers;
		private DataView brokersView;
		private ReaderWriterLock brokersLock;

		/// <summary>
		/// The broker id selected by the user.
		/// </summary>
		public int BrokerId {get {return brokerId;}}

		/// <summary>
		/// Initializes the Broker Dialog.
		/// </summary>
		public BrokerDialog()
		{

			// Initialize
			InitializeComponent();

			// Initialize the broker id.
			this.brokerId = 0;

			// IMPORTANT CONCEPT: This lock is used to make sure the control doesn't try to access the temporary brokers 
			// table while it is being updated.  The temporary brokers table is needed because we prohibit waiting for
			// table locks in the foreground: it leads to nasty deadlocking issues.  However, there is no problem in
			// waiting for a local ReaderWriterLock that isn't shared with the background.
			brokersLock = new ReaderWriterLock();

			// A 
			CopyBrokers();

			try
			{
			
				// Acquire Global Locks
				Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Install the event handlers.  The ClientMarketData component will advise us when the data has changed.  This
				// handler is removed in the 'Dispose' method.
				ClientMarketData.Broker.BrokerRowChanged += new ClientMarketData.BrokerRowChangeEventHandler(BrokerChangedEvent);
				ClientMarketData.Broker.BrokerRowDeleted += new ClientMarketData.BrokerRowChangeEventHandler(BrokerChangedEvent);

			}
			finally
			{

				// Release Global Locks
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		#region Dispose MethodPlan
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{

			// This will prevent recursively reclaiming the resources.
			if (disposing)
			{

				// Remove the event handler so it isn't called after the control has disappeared.
				ClientMarketData.Broker.BrokerRowChanged -= new ClientMarketData.BrokerRowChangeEventHandler(BrokerChangedEvent);
				ClientMarketData.Broker.BrokerRowDeleted -= new ClientMarketData.BrokerRowChangeEventHandler(BrokerChangedEvent);

				// Dispose of any components.
				if (components != null) 
					components.Dispose();

			}

			// Propogate the cleanup action through the hierarchy of classes.
			base.Dispose(disposing);

		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.comboBoxBrokerSymbol = new System.Windows.Forms.ComboBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// comboBoxBrokerSymbol
			// 
			this.comboBoxBrokerSymbol.Location = new System.Drawing.Point(8, 8);
			this.comboBoxBrokerSymbol.MaxLength = 4;
			this.comboBoxBrokerSymbol.Name = "comboBoxBrokerSymbol";
			this.comboBoxBrokerSymbol.Size = new System.Drawing.Size(152, 21);
			this.comboBoxBrokerSymbol.TabIndex = 0;
			this.comboBoxBrokerSymbol.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBoxBrokerSymbol_KeyPress);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(8, 40);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(88, 40);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			// 
			// BrokerDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(176, 69);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.comboBoxBrokerSymbol);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BrokerDialog";
			this.ShowInTaskbar = false;
			this.Text = "Select a Broker";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Copies the global brokers table into a table local to this dialog.
		/// </summary>
		private void CopyBrokers()
		{

			try
			{

				// IMPORTANT CONCEPT: Locking the data model tables is prohibited in the foreground because it can lead 
				// to deadlocks if an 'Invoke' is called while the foreground has a lock.  To get at the data we keep a
				// local copy of the brokers around in this control.  Any changes to the central data model will be
				// reflected in a table that is local to this control.  That architecture allows for a local lock on a
				// local table that won't be subject to the same restriction as the global tables.
				this.brokersLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Acquire Global Locks
				Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BrokerLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Create a local copy of the brokers for this control.
				brokers = ClientMarketData.Broker.Copy();

				// Create a view for the brokers table.  This view is used to find the symbols basd on a few characters.
				brokersView = new DataView(brokers);
				brokersView.RowFilter = null;
				brokersView.Sort = "[Symbol]";

				// Place the symbols into the combobox according to the sort order.
				foreach (DataRowView dataRowView in brokersView)
					this.comboBoxBrokerSymbol.Items.Add(dataRowView["Symbol"]);

			}
			finally
			{

				// Release Global Locks
				if (ClientMarketData.BrokerLock.IsWriterLockHeld) ClientMarketData.BrokerLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.IsLocked);

				// Release the Local Locks.
				if (this.brokersLock.IsWriterLockHeld) this.brokersLock.ReleaseWriterLock();

			}

		}

		/// <summary>
		/// Handles a broker record being changed from the server.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="rowChangeEventArgs">The event parameters.</param>
		public void BrokerChangedEvent(object sender, ClientMarketData.BrokerRowChangeEvent rowChangeEventArgs)
		{

			// Whenever any record changes, the entire table will be copied.  This is not a high performance table and
			// doesn't need attension to individual records.  Feel free to re-examine this assumption any time.
			CopyBrokers();

		}

		/// <summary>
		/// Handles a key being pressed.
		/// </summary>
		/// <param name="e">Event parameters.</param>
		private void comboBoxBrokerSymbol_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{

			try
			{

				// This will get the part of the symbol that the user has typed.  There may be text in the cell from the
				// last time an attempt was made to auto-complete the input.  The selected characters are not considered
				// when we search for a complete symbol.
				int selectionStart = this.comboBoxBrokerSymbol.SelectionStart;
				string partialSymbol = this.comboBoxBrokerSymbol.Text.Substring(0, selectionStart);

				// If the character was a backspace (and there are still characters to delete), move the symbol text back
				// by one character.
				if (e.KeyChar == Convert.ToChar(Keys.Back) && partialSymbol.Length > 0)
					partialSymbol = partialSymbol.Substring(0, partialSymbol.Length - 1);
				else
					partialSymbol += e.KeyChar.ToString();

				// Make sure that the local table isn't modified while we try to complete the text.
				this.brokersLock.AcquireReaderLock(CommonTimeout.LockWait);

				// This will find the first symbol in the brokers table that completes the partial text entered by the 
				// user.
				string completeSymbol = partialSymbol;
				if (partialSymbol.Length > 0)
				{

					// If any records are found, choose the first one to complete the symbol text.
					this.brokersView.RowFilter = String.Format("symbol like '{0}*'", partialSymbol);
					if (this.brokersView.Count > 0)
					{

						DataRowView dataRowView = this.brokersView[0];
						if (dataRowView != null)
							completeSymbol = (string)dataRowView["symbol"];

					}

				}

				// Display the complete symbol and select the portion that hasn't been typed in yet.
				this.comboBoxBrokerSymbol.Text = completeSymbol;
				this.comboBoxBrokerSymbol.Select(partialSymbol.Length, completeSymbol.Length - partialSymbol.Length);

				// Let the calling method know that it doesn't need to take any more action on this character.
				e.Handled = true;

			}
			finally
			{

				// Release the lock on the local broker table.
				if (this.brokersLock.IsReaderLockHeld) this.brokersLock.ReleaseReaderLock();

			}

		}

		/// <summary>
		/// Accepts the broker symbol from the user.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The Event Arguments.</param>
		private void buttonOK_Click(object sender, System.EventArgs e)
		{
		
			try
			{

				// Make sure that the local table isn't modified while we try to complete the text.
				this.brokersLock.AcquireReaderLock(CommonTimeout.LockWait);

				// The data view is going to be used to find the record based on it's mnemonic.  There may be some 
				// filtering left on the view from previous attempts at auto completion, so the filter is removed while
				// the mnemonic is looked up.
				brokersView.RowFilter = null;

				// If the symbol entered by the user corresponds to a valid broker, then we can accept the input and
				// dismiss the dialog box.
				int index = this.brokersView.Find(Convert.ToString(this.comboBoxBrokerSymbol.Text));
				if (index != -1)
				{

					// Close out the dialog and pass back the status and the selected broker id to the caller.
					this.brokerId = Convert.ToInt32(this.brokersView[index]["brokerId"]);
					this.DialogResult = DialogResult.OK;
					this.Close();

				}

			}
			finally
			{

				// Release the lock on the local broker table.
				if (this.brokersLock.IsReaderLockHeld) this.brokersLock.ReleaseReaderLock();

			}

		}

	}

}
