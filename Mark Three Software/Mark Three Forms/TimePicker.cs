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
	public partial class TimePicker : DomainUpDown
	{

		// Private members
		private const string initialFormat = "h:mm tt";
		private string customFormat;
		private DateTime startDateTime;
		private DateTime stopDateTime;
		private TimeSpan interval;

		/// <summary>
		/// Used to provide a customFormatted DateTime in the pick list.
		/// </summary>
		internal class FormattedDateTime
		{

			// Private Members
			private DateTime dateTime;
			private string customFormatString;

			/// <summary>
			/// A DateTime that can be entered into a selection list.
			/// </summary>
			/// <param name="dateTime">The DateTime value.</param>
			/// <param name="customFormatString">A customFormat string used to present the data.</param>
			public FormattedDateTime(DateTime dateTime, string customFormatString)
			{

				// Initialize the object
				this.dateTime = dateTime;
				this.customFormatString = customFormatString;

			}

			/// <summary>
			/// Cast the object back to a DateTime.
			/// </summary>
			/// <param name="time">A value to be converted.</param>
			/// <returns>The original DateTime.</returns>
			public static explicit operator DateTime(FormattedDateTime time)
			{
				return time.dateTime;
			}

			/// <summary>
			/// Presents the DateTime in a unified customFormat for the control.
			/// </summary>
			/// <returns>A string representation of the DateTime in a unified customFormat for this control.</returns>
			public override string ToString()
			{
				return string.Format(string.Format("{{0:{0}}}", this.customFormatString), this.dateTime);
			}

		}

		/// <summary>
		/// A Domain Up/Down control used to select a time from a range.
		/// </summary>
		public TimePicker()
		{

			// Initialize the designer maintained resources.
			InitializeComponent();

			// Initialize the object.
			this.startDateTime = DateTime.Parse("00:00:00");
			this.stopDateTime = DateTime.Parse("00:00:00");
			this.interval = TimeSpan.Parse("00:00:00");
			this.customFormat = TimePicker.initialFormat;

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
				if (this.interval != TimeSpan.Zero)
					for (DateTime time = this.stopDateTime; time >= this.startDateTime; time -= this.interval)
						base.Items.Add(new FormattedDateTime(time, this.customFormat));

            }

		}

		/// <summary>
		/// The start of the range of times displayed in the domain control.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[Description("The beginning of the range of times displayed in this control.")]
		public DateTime StartTime
		{

			get { return this.startDateTime; }
			set { this.startDateTime = value; InitializeDomain(); }

		}

		/// <summary>
		/// The end of the range of times displayed in the domain control.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[Description("The end of the range of times displayed in this control.")]
		public DateTime StopFormattedDateTime
		{

			get { return this.stopDateTime; }
			set { this.stopDateTime = value; InitializeDomain(); }

		}

		/// <summary>
		/// The interval between each of the times displayed in the domain of times.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[Description("The increment between each item in the range of times.")]
		public TimeSpan Interval
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
					(DateTime)(FormattedDateTime)this.SelectedItem;

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
					if (value is DateTime)
					{
                        int index = Search((DateTime)value);
						if (index < 0)
						{
							index = ~index;
							this.Items.Insert(index, new FormattedDateTime((DateTime)value, this.customFormat));
						}
						this.SelectedIndex = index;
					}

				}

			}

		}

		/// <summary>
		/// Searches the domain for a DateTime value.
		/// </summary>
		/// <param name="dateTime">The DateTime to be found.</param>
		/// <returns>The index of the item or the one's compliment of the index if the item doesn't exist.</returns>
		private int Search(DateTime dateTime)
		{

			// Comparing DateTimes is tricky because they are an unique, absolute moment in time.  This method needs to find out 
			// where the value belongs relative to the start of day.
			TimeSpan timeSpanValue = dateTime.TimeOfDay;

            // This will examine every item and return the positive index of the item if the DateTime value already exists in the
			// domain of items.  It will return the one's compliment of the index to indicate where the item would be if it was
			// part of this domain.
			for (int index = 0; index < this.Items.Count; index++)
			{
                if (timeSpanValue == ((DateTime)((FormattedDateTime)this.Items[index])).TimeOfDay)
                    return index;

                if (!this.ReadOnly)
                {
                    if (timeSpanValue > ((DateTime)((FormattedDateTime)this.Items[index])).TimeOfDay)
                        return ~index;
                }
			}

			// At this point, the DateTime value doesn't belong to the domain, but this is where it would go if it was added to the
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

				// This will insure that the abbreviation for the AM or PM is expanded because the parsing doesn't seem to be able
				// to take the single letter abbreviation.
				if (base.Text.EndsWith("P", StringComparison.OrdinalIgnoreCase) ||
					base.Text.EndsWith("A", StringComparison.OrdinalIgnoreCase))
					base.Text += "M";

				// Set the value.  Any parsing errors will be caught below.  Otherwise the user supplied time is added to the
				// domain and selected.
				this.Value = Convert.ToDateTime(base.Text);

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
