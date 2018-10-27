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
	/// Edit box for entering a integer of some units.
	/// </summary>
	public partial class IntegerBox : UserControl
	{

		public IntegerBox()
		{

			// This is where the designer controls are initialized.
			InitializeComponent();

		}

		/// <summary>
		/// The integer entered in the edit box.
		/// </summary>
		public int Integer
		{

			get
			{

				// This will be returned if no useful information can be extracted from the value entered by the user.
				int integer = 0;

				// Attempt to parse the user entered value (assuming that they entered something).
				string integerText = this.textBox.Text;
				if (integerText != string.Empty)
				{

					// This will parse the text into a value and add in the multipler.
					integer = Convert.ToInt32(integerText);

				}

				// This is the parsed, converted integer.
				return integer;

			}

			set
			{

				// The text of the integer is displayed in the box.
				this.textBox.Text = Convert.ToString(value);

			}

		}

	}

}
