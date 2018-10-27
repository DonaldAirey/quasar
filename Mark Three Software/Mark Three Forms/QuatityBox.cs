namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Data;
	using System.Text;
	using System.Windows.Forms;

	/// <summary>
	/// Edit box for entering a quantity of some units.
	/// </summary>
	public partial class QuantityBox : UserControl
	{

		// Private members
		private const string initialFormat = "#,##0";
		private string customFormat;

		public QuantityBox()
		{

			// This is where the designer controls are initialized.
			InitializeComponent();

			// Initialize the object
			this.customFormat = QuantityBox.initialFormat;

		}

		/// <summary>
		/// The quantity entered in the edit box.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public decimal Value
		{

			get
			{

				// This is the parsed, converted quantity.
				return this.textBox.Text == string.Empty ? 0.0m : Convert.ToDecimal(this.textBox.Text);

			}

			set
			{

				this.textBox.Text = string.Format(string.Format("{{0:{0}}}", this.customFormat), value);

			}

		}

		/// <summary>
		/// The format used to display the times.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[Description("The format used to display the control.")]
		public string CustomFormat
		{

			get { return this.customFormat; }
			set { this.customFormat = value; this.Value = this.Value; }

		}

		/// <summary>
		/// Validates the value in the control.
		/// </summary>
		/// <param name="e">Allows navigation out of the control to be cancelled.</param>
		protected override void OnValidating(CancelEventArgs e)
		{

			try
			{

				decimal quantity = 0.0m;

				// Attempt to parse the user entered value (assuming that they entered something).
				string quantityText = this.textBox.Text;
				if (quantityText != string.Empty)
				{

					// Remove any leading or trailing white space.
					quantityText = quantityText.Trim();

					// The characer 'm' or 'M' can be entered as a multipler.
					decimal multiplier = 1.0m;
					while (quantityText.EndsWith("m", StringComparison.OrdinalIgnoreCase))
					{
						multiplier *= 1000.0m;
						quantityText = quantityText.Substring(0, quantityText.Length - 1);
					}

					// This will parse the text into a value and add in the multipler.
					quantity = Convert.ToDecimal(quantityText) * multiplier;

				}

				// Set the value.  Any parsing errors will be caught below.  Otherwise the user supplied time is added to the
				// domain and selected.
				this.Value = quantity;

			}
			catch (FormatException)
			{

				// This is the most common error when formatting.
				MessageBox.Show(Resource.StringFormatTimeError, Resource.StringGuardianError);

				// Cancel the navigation out of the control when a parsing error needs to be corrected.
				e.Cancel = true;

			}
			catch (Exception exception)
			{

				// This is a catch-all that will describe the errors.
				MessageBox.Show(exception.Message, Resource.StringGuardianError);

				// Cancel the navigation out of the control when a parsing error needs to be corrected.
				e.Cancel = true;

			}

		}

	}

}
