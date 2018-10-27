namespace MarkThree.Guardian.Forms
{

	using MarkThree;
	using MarkThree.Guardian.Client;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Data;
	using System.Resources;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// Selects a time from a range of times.
	/// </summary>
	public partial class OrderTypePicker : DomainUpDown
	{

		private bool isDelete;
		private string mnemonicText;
		private int selectedOrderTypeCode;
		private AutoCompleteCallback autoCompleteCallback;
		private InitializationCallback initializationCallback;
		private delegate void InitializationCallback(ArrayList arrayList);
		private delegate void AutoCompleteCallback(string completedText, int orderTypeCode);

		/// <summary>
		/// Used to provide a customFormatted int in the pick list.
		/// </summary>
		internal class FormattedOrderType
		{

			// Private Members
			private int orderType;
			private string mnemonic;
			private string description;

			/// <summary>
			/// A int that can be entered into a selection list.
			/// </summary>
			/// <param name="orderType">The int value.</param>
			/// <param name="mnemonic">A customFormat string used to present the data.</param>
			public FormattedOrderType(int orderType, string mnemonic, string description)
			{

				// Initialize the object
				this.orderType = orderType;
				this.mnemonic = mnemonic;
				this.description = description;

			}

			/// <summary>
			/// Cast the object back to a int.
			/// </summary>
			/// <param name="time">A value to be converted.</param>
			/// <returns>The original int.</returns>
			public static explicit operator int(FormattedOrderType formattedint)
			{
				return formattedint.orderType;
			}

			public string Description { get { return this.description; } }

			public string Mnemonic { get { return this.mnemonic; } }

			/// <summary>
			/// Presents the int in a unified customFormat for the control.
			/// </summary>
			/// <returns>A string representation of the int in a unified customFormat for this control.</returns>
			public override string ToString() { return this.description; }

		}

		/// <summary>
		/// A Domain Up/Down control used to select a time from a range.
		/// </summary>
		public OrderTypePicker()
		{

			// Initialize the designer maintained resources.
			InitializeComponent();

			// Initialize the object.
			this.mnemonicText = string.Empty;

			// This delegate is used to call the foreground when the attempt to find a matching symbol in the security table has
			// completed.
			this.autoCompleteCallback = new AutoCompleteCallback(OnTextCompleted);
			this.initializationCallback = new InitializationCallback(OnInitializationCompleted);

#if DEBUG
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
#endif

				// A background thread will extract the order type information and pass it to the foreground.
				ThreadPool.QueueUserWorkItem(new WaitCallback(InitializeThread));

#if DEBUG
			}
#endif

		}

		/// <summary>
		/// Initialize the data elements of this control.
		/// </summary>
		/// <param name="parameter">Unused thread parameters.</param>
		private void InitializeThread(object parameter)
		{

			ArrayList itemArray = new ArrayList();

			try
			{

				// Lock the table
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(Timeout.Infinite);

				foreach (ClientMarketData.OrderTypeRow orderTypeRow in ClientMarketData.OrderType.Rows)
					itemArray.Add(new object[] { orderTypeRow.OrderTypeCode, orderTypeRow.Mnemonic, orderTypeRow.Description });

			}
			finally
			{

				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
					ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			this.initializationCallback(itemArray);

		}

		private void OnInitializationCompleted(ArrayList arrayList)
		{

			// Clear the control of any previous ranges and add the new range using the custom format
			// selected for this control.
			base.Items.Clear();
			foreach (object[] item in arrayList)
				base.Items.Add(new FormattedOrderType((int)item[0], (string)item[1], (string)item[2]));

		}

		/// <summary>
		/// Gets the text in the control.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Text
		{

			get { return base.Text; }

		}

		/// <summary>
		/// The value of the selected item or DBNull.Value if nothing is selected.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Value
		{

			get
			{

				// DBNull.Value is returned if nothing is selected.
				return this.SelectedIndex == -1 ? (object)DBNull.Value :
					(int)(FormattedOrderType)this.SelectedItem;

			}

			set
			{

				// DBNull will clear the control of any selected items.
				if (value is DBNull)
				{
					base.Text = string.Empty;
					this.mnemonicText = string.Empty;
					this.SelectedIndex = -1;
				}
				else
				{

					// This will select the item from the list that corresponds to the value.  If the value is between ranges, it
					// will add the value to the items in the domain as well.
					if (value is int)
					{
						this.SelectedIndex = Search((int)value);
						FormattedOrderType formattedOrderType = (FormattedOrderType)this.SelectedItem;
						this.mnemonicText = formattedOrderType.Mnemonic;
						base.Text = formattedOrderType.Description;
					}

				}

			}

		}

		/// <summary>
		/// Searches the domain for a int value.
		/// </summary>
		/// <param name="orderType">The int to be found.</param>
		/// <returns>The index of the item or the one's compliment of the index if the item doesn't exist.</returns>
		private int Search(int orderType)
		{

			// This will examine every item and return the positive index of the item if the int value already exists in the
			// domain of items.  It will return the one's compliment of the index to indicate where the item would be if it was
			// part of this domain.
			for (int index = 0; index < this.Items.Count; index++)
				if (orderType == ((int)((FormattedOrderType)this.Items[index])))
					return index;

			// This should never happen.  It is provided for the compiler.
			throw new Exception("Illegal Side Code");

		}

		/// <summary>
		/// Handles the entry of the focus into the control.
		/// </summary>
		/// <param name="e">Empty event arguments.</param>
		protected override void OnEnter(EventArgs e)
		{

			// When the cursor enters the control, select the entire value in the domain.  This makes it easier to change values as
			// the user will likey want to overwrite anything that was in the control.
			base.Select(0, base.Text.Length);

			// Allow the base to complete the operation.
			base.OnEnter(e);

		}

		/// <summary>
		/// Handles the completion of the autoselection.
		/// </summary>
		/// <param name="completedText">The completed symbol text, or String.Empty if there was no match.</param>
		/// <param name="securityId">The selected security identifier, -1 if there is no valid selection.</param>
		/// <param name="settlementId">The selected security identifier, -1 if there is no valid selection.</param>
		private void OnTextCompleted(string completedText, int orderTypeCode)
		{

			// These values can be accessed by other foreground threads from here.
			this.selectedOrderTypeCode = orderTypeCode;

			// If a match was found, place it in the edit box and select the part that wasn't entered by the user.
			if (completedText != string.Empty)
			{
				string previousText = this.isDelete ? base.Text.Substring(0, base.Text.Length - 1) : base.Text;
				base.Text = completedText;
				this.Select(previousText.Length, completedText.Length - previousText.Length);
			}

		}

		/// <summary>
		/// Searches through the security database for a match of the entered security symbol.
		/// </summary>
		/// <param name="parameter"></param>
		private void AutoCompleteThread(object parameter)
		{

			// Extract the partial symbol from the parameter.
			string text = (string)parameter;

			// These values will collect data from the security if a match to the symbol is found.
			string completedText = string.Empty;
			int selectedOrderTypeCode = -1;
			int selectedSettlementId = -1;

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Search through the databas the hard way looking for a symbol that starts with the partial symbol text.
				foreach (ClientMarketData.SecurityRow securityRow in ClientMarketData.Security.Rows)
					if (securityRow.Symbol.StartsWith(text, StringComparison.OrdinalIgnoreCase))
					{

						// At this point, a match has been found.  Collect the data from the security.
						selectedOrderTypeCode = securityRow.SecurityId;
						completedText = securityRow.Symbol;

						// Select the settlement currency for equities.
						foreach (ClientMarketData.EquityRow equityRow in securityRow.GetEquityRowsBySecurityEquityEquityId())
							selectedSettlementId = equityRow.SettlementId;

						// Select the settlement currency for bonds.
						foreach (ClientMarketData.DebtRow debtRow in securityRow.GetDebtRowsBySecurityDebtDebtId())
							selectedSettlementId = debtRow.SettlementId;

						break;

					}

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.DebtLock.IsReaderLockHeld)
					ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld)
					ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Invoke the foreground with the information of any match to the partial security symbol.
			BeginInvoke(this.autoCompleteCallback, completedText, selectedOrderTypeCode, selectedSettlementId);

		}

		private void textBox_KeyUp(object sender, KeyEventArgs e)
		{

			this.isDelete = e.KeyCode == Keys.Back;

		}

		private void OrderTypePicker_KeyPress(object sender, KeyPressEventArgs e)
		{

			if (e.KeyChar == 0x08)
			{
				if (this.mnemonicText.Length > 0)
					this.mnemonicText = this.mnemonicText.Substring(0, this.mnemonicText.Length - 1);
			}
			else
			{
				this.mnemonicText += e.KeyChar;
			}

			bool found = false;
			foreach (FormattedOrderType formattedOrderType in this.Items)
			{
				if (string.Compare(formattedOrderType.Mnemonic, this.mnemonicText, true) == 0)
				{
					this.SelectedItem = formattedOrderType;
					found = true;
				}

			}

			// If the mnemonic doesn't exist, then clear out the control of any selection.
			if (!found)
			{
				this.mnemonicText = string.Empty;
				base.Text = string.Empty;
				this.SelectedIndex = -1;
			}

			// This indicates that the key was handled and it shouldn't be passed on to the Domain control (which would print the 
			// character in the text space).
			e.Handled = true;

		}

	}

}
