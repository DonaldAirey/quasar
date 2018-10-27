namespace MarkThree.Guardian.Forms
{

	using MarkThree;
	using MarkThree.Guardian.Client;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// An Auto Complete Edit box for Security Symbols.
	/// </summary>
	public partial class SymbolBox : System.Windows.Forms.UserControl
	{

		// Private Methods
		private string symbolText;
		private int selectedSecurityId;
		private int selectedSettlementId;
		private delegate void ValidateSymbolCallback(string symbol, int selectedSecurityId, int settlementId);

		/// <summary>
		/// An EditBox for accepting security symbols.
		/// </summary>
		public SymbolBox()
		{

			// IDE Generated Controls.
			InitializeComponent();

			// Initialize the object.
			this.symbolText = string.Empty;

		}

		/// <summary>
		/// The internal identifier of the selected security, -1 for no valid security selected.
		/// </summary>
		public string SelectedSecurityId
		{

			get { return this.textBox.Text; }
			set	{this.textBox.Text = value;	}

		}

		/// <summary>
		/// The internal identifier of the settlement currency of the selected security, -1 for no valid security selected.
		/// </summary>
		public string SelectedSettlementId { get { return "USD"; } }

		/// <summary>
		/// Handles the completion of the autoselection.
		/// </summary>
		/// <param name="symbol">The completed symbol text, or String.Empty if there was no match.</param>
		/// <param name="securityId">The selected security identifier, -1 if there is no valid selection.</param>
		/// <param name="settlementId">The selected security identifier, -1 if there is no valid selection.</param>
		private void ValidateSymbol(string symbol, int securityId, int settlementId)
		{

			// These values can be accessed by other foreground threads from here.
			this.selectedSecurityId = securityId;
			this.selectedSettlementId = settlementId;
			this.textBox.Text = symbol;

		}

		private void textBox_Validating(object sender, CancelEventArgs e)
		{

		}

	}

}
