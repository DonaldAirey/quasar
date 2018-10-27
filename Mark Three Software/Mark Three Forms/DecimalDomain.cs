namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Data;
	using System.Resources;
	using System.Text;
	using System.Windows.Forms;

	/// <summary>
	/// Selects a time from a range of times.
	/// </summary>
	public partial class DecimalDomain : DomainUpDown
	{

		// Private members
		private const string initialFormat = "#,##0";
		private string customFormat;
		private string customUnit;
		private decimal startDecimal;
		private decimal stopDecimal;
		private decimal interval;

		/// <summary>
		/// Used to provide a customFormatted decimal value in the pick list.
		/// </summary>
		internal class FormattedDecimal
		{

			// Private Members
			private decimal value;
			private string customFormatString;
			private string customUnitString;

			/// <summary>
			/// A decimal value that can be entered into a selection list.
			/// </summary>
			/// <param name="decial">The decimal value.</param>
			/// <param name="customFormatString">A customFormat string used to present the data.</param>
			public FormattedDecimal(decimal value, string customFormatString, string customUnitString)
			{

				// Initialize the object
				this.value = value;
				this.customFormatString = customFormatString;
				this.customUnitString = customUnitString;

			}

			/// <summary>
			/// Cast the object back to a decimal.
			/// </summary>
			/// <param name="time">A value to be converted.</param>
			/// <returns>a decimal value.</returns>
			public static explicit operator decimal(FormattedDecimal formattedDecimal)
			{
				return formattedDecimal.value;
			}

			/// <summary>
			/// Presents the decimal in a unified customFormat for the control.
			/// </summary>
			/// <returns>A string representation of the decimal in a unified customFormat for this control.</returns>
			public override string ToString()
			{
				return this.customUnitString == string.Empty ?
					string.Format(string.Format("{{0:{0}}}", this.customFormatString), this.value) :
					string.Format(string.Format("{{0:{0}}} {1}", this.customFormatString, this.customUnitString), this.value);
			}

		}

		/// <summary>
		/// A Domain Up/Down control used to select a time from a range.
		/// </summary>
		public DecimalDomain()
		{

			// Initialize the designer maintained resources.
			InitializeComponent();

			// Initialize the object.
			this.startDecimal = Decimal.Zero;
			this.stopDecimal = Decimal.Zero;
			this.interval = Decimal.Zero;
			this.customFormat = DecimalDomain.initialFormat;
			this.customUnit = string.Empty;

		}

		/// <summary>
		/// This will initialize the domain with the selected range and customFormat.
		/// </summary>
		private void InitializeDomain()
		{

			// This will prevent the designer from attempting to serialize the control.
			if (!this.DesignMode)
			{

				// Clear the control of any previous ranges and add the new range using the custom format
				// selected for this control.
				base.Items.Clear();
                if (this.interval != Decimal.Zero)
                {
                    //for (decimal iterationValue = this.stopDecimal; iterationValue >= this.startDecimal; iterationValue -= this.interval)
                    //	base.Items.Add(new FormattedDecimal(iterationValue, this.customFormat, this.customUnit));

                    // insert all of the values up right up until the stopDecimal
                    for (decimal iterationValue = this.startDecimal; iterationValue < this.stopDecimal; iterationValue += this.interval)
                    {
                        if (iterationValue > this.stopDecimal)
                            iterationValue = this.stopDecimal;

                        base.Items.Insert(0, new FormattedDecimal(iterationValue, this.customFormat, this.customUnit));
                    }

                    // insert the stopDecimal
                    base.Items.Insert(0, new FormattedDecimal(this.stopDecimal, this.customFormat, this.customUnit));
                }
			}

		}

        /// <summary>
        /// The start of the range of times displayed in the domain control.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The beginning of the range of duration displayed in this control.")]
        public decimal StartDecimal
		{

			get { return this.startDecimal; }
			set { this.startDecimal = value; InitializeDomain(); }

		}

		/// <summary>
		/// The end of the range of times displayed in the domain control.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[Description("The end of the range of times duration in this control.")]
		public decimal StopDecimal
		{

			get { return this.stopDecimal; }
			set { this.stopDecimal = value; InitializeDomain(); }

		}

		/// <summary>
		/// The interval between each of the times displayed in the domain of times.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[Description("The increment between each item in the range of times.")]
		public decimal Interval
		{

			get { return this.interval; }
			set { this.interval = value; InitializeDomain(); }

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
			set { this.customFormat = value; InitializeDomain(); }

		}

		/// <summary>
		/// The format used to display the times.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[Description("An optional text next to the displayed value that describes the units used.")]
		public string CustomUnit
		{

			get { return this.customUnit; }
			set { this.customUnit = value; InitializeDomain(); }

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
					(decimal)(FormattedDecimal)this.SelectedItem;

			}

			set
			{

				// DBNull will clear the control of any selected items.
				if (value is DBNull)
				{
					base.Text = string.Empty;
					this.SelectedIndex = -1;
				}
				else
				{

					// This will select the item from the list that corresponds to the value.  If the value is between ranges, it
					// will add the value to the items in the domain as well.
					if (value is decimal)
					{
						int index = Search((decimal)value);
						if (index < 0)
						{
							index = ~index;
							this.Items.Insert(index, new FormattedDecimal((decimal)value, this.customFormat, this.customUnit));
						}
						this.SelectedIndex = index;
					}

				}

			}

		}

		/// <summary>
		/// Searches the domain for a decimal value.
		/// </summary>
		/// <param name="value">The value to be found.</param>
		/// <returns>The index of the item or the one's compliment of the index if the item doesn't exist.</returns>
		private int Search(decimal value)
		{

			// This will examine every item and return the positive index of the item if the decimal value already exists in the
			// domain of items.  It will return the one's compliment of the index to indicate where the item would be if it was
			// part of this domain.
			for (int index = 0; index < this.Items.Count; index++)
			{
				if (value > ((decimal)((FormattedDecimal)this.Items[index])))
					return ~index;
				if (value == ((decimal)((FormattedDecimal)this.Items[index])))
					return index;
			}

			// At this point, the int value doesn't belong to the domain, but this is where it would go if it was added to the
			// ordered list.
			return ~this.Items.Count;

		}

		/// <summary>
		/// Validates the value in the control.
		/// </summary>
		/// <param name="e">Allows navigation out of the control to be cancelled.</param>
		protected override void OnValidating(CancelEventArgs e)
		{

			// An empty string is a command to unselect the values.
			if (base.Text == string.Empty)
			{
				this.Value = DBNull.Value;
				return;
			}

			try
			{

				decimal quantity = 0.0m;

				// Attempt to parse the user entered value (assuming that they entered something).
				string quantityText = this.Text;
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

	}

}
