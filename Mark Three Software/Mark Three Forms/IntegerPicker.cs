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
	public partial class IntegerPicker : DomainUpDown
	{

		// Private members
		private const string initialFormat = "0";
		private string customFormat;
		private string customUnit;
		private int startInteger;
		private int stopInteger;
		private int interval;

		/// <summary>
		/// Used to provide a customFormatted int in the pick list.
		/// </summary>
		internal class FormattedInteger
		{

			// Private Members
			private int integer;
			private string customFormatString;
			private string customUnitString;

			/// <summary>
			/// A int that can be entered into a selection list.
			/// </summary>
			/// <param name="integer">The int value.</param>
			/// <param name="customFormatString">A customFormat string used to present the data.</param>
			public FormattedInteger(int integer, string customFormatString, string customUnitString)
			{

				// Initialize the object
				this.integer = integer;
				this.customFormatString = customFormatString;
				this.customUnitString = customUnitString;

			}

			/// <summary>
			/// Cast the object back to a int.
			/// </summary>
			/// <param name="time">A value to be converted.</param>
			/// <returns>The original int.</returns>
			public static explicit operator int(FormattedInteger formattedInteger)
			{
				return formattedInteger.integer;
			}

			/// <summary>
			/// Presents the int in a unified customFormat for the control.
			/// </summary>
			/// <returns>A string representation of the int in a unified customFormat for this control.</returns>
			public override string ToString()
			{
				return this.customUnitString == string.Empty ?
					string.Format(string.Format("{{0:{0}}}", this.customFormatString), this.integer) :
					string.Format(string.Format("{{0:{0}}} {1}", this.customFormatString, this.customUnitString), this.integer);
			}

		}

		/// <summary>
		/// A Domain Up/Down control used to select a time from a range.
		/// </summary>
		public IntegerPicker()
		{

			// Initialize the designer maintained resources.
			InitializeComponent();

			// Initialize the object.
			this.startInteger = 0;
			this.stopInteger = 0;
			this.interval = 0;
			this.customFormat = IntegerPicker.initialFormat;
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
				if (this.interval != 0)
					for (int time = this.stopInteger; time >= this.startInteger; time -= this.interval)
						base.Items.Add(new FormattedInteger(time, this.customFormat, this.customUnit));

			}

		}

		/// <summary>
		/// The start of the range of times displayed in the domain control.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[Description("The beginning of the range of duration displayed in this control.")]
		public int StartInteger
		{

			get { return this.startInteger; }
			set { this.startInteger = value; InitializeDomain(); }

		}

		/// <summary>
		/// The end of the range of times displayed in the domain control.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[Description("The end of the range of times duration in this control.")]
		public int StopInteger
		{

			get { return this.stopInteger; }
			set { this.stopInteger = value; InitializeDomain(); }

		}

		/// <summary>
		/// The interval between each of the times displayed in the domain of times.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[Description("The increment between each item in the range of times.")]
		public int Interval
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
		[Description("Displays an optional text next to the integer that describes the units used.")]
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
					(int)(FormattedInteger)this.SelectedItem;

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
					if (value is int)
					{
						int index = Search((int)value);
						if (index < 0)
						{
							index = ~index;
							this.Items.Insert(index, new FormattedInteger((int)value, this.customFormat, this.customUnit));
						}
						this.SelectedIndex = index;
					}

				}

			}

		}

		/// <summary>
		/// Searches the domain for a int value.
		/// </summary>
		/// <param name="integer">The int to be found.</param>
		/// <returns>The index of the item or the one's compliment of the index if the item doesn't exist.</returns>
		private int Search(int integer)
		{

			// This will examine every item and return the positive index of the item if the int value already exists in the
			// domain of items.  It will return the one's compliment of the index to indicate where the item would be if it was
			// part of this domain.
			for (int index = 0; index < this.Items.Count; index++)
			{
				if (integer > ((int)((FormattedInteger)this.Items[index])))
					return ~index;
				if (integer == ((int)((FormattedInteger)this.Items[index])))
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

				// Set the value.  Any parsing errors will be caught below.  Otherwise the user supplied time is added to the
				// domain and selected.
				string integerText = this.customUnit == string.Empty ? base.Text :
					base.Text.Replace(this.customUnit, string.Empty);
				this.Value = int.Parse(integerText);

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
