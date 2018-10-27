namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	/// <summary>A text edit control to overlay a cell in a spreadsheet.</summary>
	public class TextEditCell : MarkThree.Forms.CellControl
	{

		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.TextBox textBox;
		private System.ComponentModel.IContainer components = null;

		/// <summary>Sets or gets the horizontal alignment of the text box.</summary>
		[Browsable(false)]
		public override StringAlignment Alignment
		{
			
			get {return this.alignment;}

			set
			{

				switch (this.alignment = value)
				{
				case StringAlignment.Near: textBox.TextAlign = HorizontalAlignment.Left; break;
				case StringAlignment.Center: textBox.TextAlign = HorizontalAlignment.Center; break;
				case StringAlignment.Far: textBox.TextAlign = HorizontalAlignment.Right; break;
				}

			}
		
		}

		/// <summary>Sets or gets the horizontal alignment of the text box.</summary>
		[Browsable(false)]
		public override StringAlignment LineAlignment
		{
			
			get {return this.lineAlignment;}

			set
			{

				switch (this.lineAlignment = value)
				{

				case StringAlignment.Near: this.textBox.Top = 0; break;
				case StringAlignment.Center: this.textBox.Top = (this.panel.Height - this.textBox.Height) / 2 + 1; break;
				case StringAlignment.Far: this.textBox.Top = this.panel.Height - this.textBox.Height + 1; break;

				}

			}
		
		}

		/// <summary>
		/// Gets or sets the starting point for the selected text
		/// </summary>
		[Browsable(false)]
		public virtual int SelectionStart {get {return textBox.SelectionStart;} set {textBox.SelectionStart = value;}}

		/// <summary>
		/// Gets or sets the length of the selected text.
		/// </summary>
		[Browsable(false)]
		public virtual int SelectionLength {get {return textBox.SelectionLength;} set {textBox.SelectionLength = value;}}

		/// <summary>
		/// The text of the control.
		/// </summary>
		[Browsable(false)]
		public override string Text
		{
			get {return this.textBox.Text;}
			set {this.textBox.Text = value;this.textBox.Select(value.Length, 0);}
		}

		/// <summary>
		/// Constructor for the TextEditCell.
		/// </summary>
		public TextEditCell()
		{

			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Make the text box exactly as tall as it needs to be to hold the font.
			this.textBox.Height = this.textBox.Font.Height + 1;

		}

		#region Dispose MethodPlan
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel = new System.Windows.Forms.Panel();
			this.textBox = new System.Windows.Forms.TextBox();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel.BackColor = System.Drawing.SystemColors.Window;
			this.panel.Controls.Add(this.textBox);
			this.panel.Location = new System.Drawing.Point(1, 1);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(100, 20);
			this.panel.TabIndex = 0;
			this.panel.Resize += new System.EventHandler(this.panel_Resize);
			// 
			// textBox
			// 
			this.textBox.AcceptsReturn = true;
			this.textBox.AcceptsTab = true;
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.AutoSize = false;
			this.textBox.BackColor = System.Drawing.SystemColors.Window;
			this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox.Location = new System.Drawing.Point(0, 0);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(99, 19);
			this.textBox.TabIndex = 0;
			this.textBox.Text = "";
			this.textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
			this.textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
			this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
			// 
			// TextEditCell
			// 
			this.BackColor = System.Drawing.SystemColors.Highlight;
			this.Controls.Add(this.panel);
			this.Name = "TextEditCell";
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		/// <summary>
		/// Selects all the text in the text box.
		/// </summary>
		public override void SelectAll() {this.textBox.SelectAll();}

		/// <summary>
		/// Selects text in the text box control.
		/// </summary>
		/// <param name="start">Starting point of the selection.</param>
		/// <param name="length">Length of the selection.</param>
		public void Select(int start, int length)
		{

			// Pass the method on to the text box control.
			this.textBox.Select(start, length);

		}

		/// <summary>
		/// Rebroadcasts a key down event.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event parameters.</param>
		private void textBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{

			// Rebroadcast the text box event.  This is done because the text box is buried inside other controls.  This
			// makes the compound control appear to be a single, simple control to the caller.
			OnKeyDown(e);

		}

		/// <summary>
		/// Handles a key down event.
		/// </summary>
		/// <param name="keyEventArgs">The event parameters</param>
		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs keyEventArgs)
		{

			// Filter out the navigation keys from the stream and pass them to the spreadsheet control.
			switch (keyEventArgs.KeyData)
			{

			case Keys.Enter:

				// Broadcast the navigation instruction.
				this.Visible = false;
				OnNavigation(new NavigationEventArgs(NavigationCommand.Enter));
				keyEventArgs.Handled = true;
				break;

			case Keys.Escape:
				
				this.IsValid = false;
				this.Visible = false;
				keyEventArgs.Handled = true;
				break;

			case Keys.Up:
				
				// When navigation is enabled, the keys change the selected cell.  In the 'Edit' mode, these keys navigate through
				// the text that is being entered.
				if (!this.IsNavigationAllowed)
				{

					// Broadcast the navigation instruction.
					this.Visible = false;
					OnNavigation(new NavigationEventArgs(NavigationCommand.Up));
					keyEventArgs.Handled = true;

				}
				break;

			case Keys.Down:

				// When navigation is enabled, the keys change the selected cell.  In the 'Edit' mode, these keys navigate through
				// the text that is being entered.
				if (!this.IsNavigationAllowed)
				{

					// Broadcast the navigation instruction.
					this.Visible = false;
					OnNavigation(new NavigationEventArgs(NavigationCommand.Down));
					keyEventArgs.Handled = true;

				}
				break;

			case Keys.Right:
			case Keys.Tab:

				// When navigation is enabled, the keys change the selected cell.  In the 'Edit' mode, these keys navigate through
				// the text that is being entered.
				if (!this.IsNavigationAllowed)
				{

					// Broadcast the navigation instruction.
					this.Visible = false;
					OnNavigation(new NavigationEventArgs(NavigationCommand.Right));
					keyEventArgs.Handled = true;

				}
				break;

			case Keys.Left:
			case Keys.Shift | Keys.Tab:

				// When navigation is enabled, the keys change the selected cell.  In the 'Edit' mode, these keys navigate through
				// the text that is being entered.
				if (!this.IsNavigationAllowed)
				{

					// Broadcast the navigation instruction.
					this.Visible = false;
					OnNavigation(new NavigationEventArgs(NavigationCommand.Left));
					keyEventArgs.Handled = true;

				}
				break;

			}
			
		}
			
		/// <summary>
		/// Rebroadcasts a key pressed event.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event parameters.</param>
		private void textBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{

			// Rebroadcast the text box event.  This is done because the text box is buried inside other controls.  This
			// makes the compound control appear to be a single, simple control to the implementer.
			this.OnKeyPress(e);
		
		}

		/// <summary>
		/// Handles the key pressed event.
		/// </summary>
		/// <param name="e">Event parameters.</param>
		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{

			// Just trap the return key so we can control the logic in the 'KeyDown' event handler.
			if (e.KeyChar == 0x0D)
				e.Handled = true;

		}

		/// <summary>
		/// Rebroadcasts the 'TextChanged' event.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event parameters.</param>
		private void textBox_TextChanged(object sender, System.EventArgs e)
		{

			// Rebroadcast the text box event.  This is done because the text box is buried inside other controls.  This
			// makes the compound control appear to be a single, simple control to the implementer.
			this.OnTextChanged(e);

		}

		private void panel_Resize(object sender, System.EventArgs e)
		{
			this.textBox.Width = this.panel.Width;
		}

	}

}

