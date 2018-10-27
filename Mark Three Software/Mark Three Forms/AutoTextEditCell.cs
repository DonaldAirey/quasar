namespace MarkThree.Forms
{

	using MarkThree;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Threading;

	/// <summary>
	/// A text edit control to overlay a cell in a spreadsheet.
	/// </summary>
	public class AutoTextEditTile : MarkThree.Forms.TextEditCell
	{

		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.TextBox textBox;
		private delegate void SetTextDelegate(string text, int startPosition, int length);
		private delegate void EndEditDelegate(TileAddress cellAddress, object code);
		private SetTextDelegate setTextDelegate;
		private EndEditDelegate endEditDelegate;
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Constructor for the AutoTextEditTile.
		/// </summary>
		public AutoTextEditTile()
		{

			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// IMPORTANT CONCEPT: Working with a data model is at odds with a user interface.  The data model can't be
			// accessed without locks or corruption will result.  The message loop can't wait for locks because the
			// application will appear to lock up regularly for a moment or two.  Also, there are deadlocking issues
			// involved when we try to use an 'Invoke' in the background when the message loop is waiting for lock.  So
			// data is accessed in a background thread and the user controls are handled in the message loop thread.
			// These delegates are used to access the control from the background.
			this.setTextDelegate = new SetTextDelegate(this.SetTextForeground);
			this.endEditDelegate = new EndEditDelegate(this.EndEditForeground);

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
			this.panel.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.panel.BackColor = System.Drawing.SystemColors.Window;
			this.panel.Controls.AddRange(new System.Windows.Forms.Control[] {
																				this.textBox});
			this.panel.Location = new System.Drawing.Point(1, 1);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(100, 20);
			this.panel.TabIndex = 0;
			// 
			// textBox
			// 
			this.textBox.AcceptsReturn = true;
			this.textBox.AcceptsTab = true;
			this.textBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.textBox.AutoSize = false;
			this.textBox.BackColor = System.Drawing.SystemColors.Window;
			this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox.Location = new System.Drawing.Point(1, 1);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(99, 19);
			this.textBox.TabIndex = 0;
			this.textBox.Text = "";
			this.textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// TextEditTile
			// 
			this.BackColor = System.Drawing.SystemColors.Highlight;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.panel});
			this.Name = "TextEditTile";
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		/// <summary>
		/// SetS the text of the control.
		/// </summary>
		[Browsable(false)]
		public override string Text
		{
			
			// Gets the text of the edit control.
			get {return base.Text;}
			
			// SetS the text, with auto-complete, of the edit control.
			set {ThreadPool.QueueUserWorkItem(new WaitCallback(SetTextCommand), value);}
	
		}

		/// <summary>
		/// Handles a key being pressed.
		/// </summary>
		/// <param name="e">Event parameters.</param>
		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{

			// The auto-completion must be handled in the background to prevent deadlocks.
			ThreadPool.QueueUserWorkItem(new WaitCallback(KeyPressedCommand),
				new object[] { e.KeyChar, base.Text, this.SelectionStart, this.SelectionLength });

			// This lets the calling method know that we handled the character.
			e.Handled = true;

		}

		/// <summary>
		/// SetS the text of the control.
		/// </summary>
		/// <param name="text">The text of the control.</param>
		/// <param name="startPosition">Starting point of the selection.</param>
		/// <param name="length">Length of the selection.</param>
		private void SetTextForeground(string text, int startPosition, int length)
		{

			// Display the complete text and select the portion that hasn't been typed in yet.
			base.Text = text;
			this.Select(startPosition, length);

		}
		
		/// <summary>
		/// Broadcasts the end of editing event.
		/// </summary>
		/// <param name="key">The key used to finish the edit cell.</param>
		/// <param name="code">The code associated with the user's input.</param>
		private void EndEditForeground(TileAddress cellAddress, object code)
		{

			// Call the base class to invoke an event with the key that finished the control and the internal control
			// value for Time In Force, or null if there's no match.
			base.OnEndEdit(new EndEditEventArgs(code));

		}
		
		/// <summary>
		/// Called to validate the field and broadcast the end of editing event.
		/// </summary>
		/// <param name="endEditEventArgs">Event argument.</param>
		protected override void OnEndEdit(EndEditEventArgs endEditEventArgs)
		{

			// Call the background to find the code of the field based on the text entered and return that code to the
			// caller.
			ThreadPool.QueueUserWorkItem(new WaitCallback(EndEditCommand), endEditEventArgs);

		}

		/// <summary>
		/// Calls the foreground to set the text and the selected text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="startPosition">Starting point of the selection.</param>
		/// <param name="length">Length of selected text.</param>
		protected void SetTextBackground(string text, int startPosition, int length)
		{

			// The control can't be modified in the background, so call the message loop thread to set the
			// text and the selected part of the field.
			Invoke(this.setTextDelegate, new object[] {text, startPosition, length});

		}

		/// <summary>
		/// Calls the foreground thread to finish the editing operation.
		/// </summary>
		/// <param name="key">The key that completed the edit operation.</param>
		/// <param name="code">The code associated with the user input.</param>
		protected void EndEditBackground(TileAddress cellAddress, object code)
		{

			// The control can't be modified in the background, so call the message loop thread to send the key and
			// the code back to the subscribing objects.
			Invoke(this.endEditDelegate, new object[] {cellAddress, code});

		}

		/// <summary>
		/// SetS the text of the cell using the table to auto-complete the field.
		/// </summary>
		/// <param name="text">The text of the cell.</param>
		protected virtual void SetTextCommand(object parameter) {}
		
		/// <summary>
		/// Handles the auto completion when a key is pressed.
		/// </summary>
		/// <param name="keyChar"></param>
		/// <param name="text"></param>
		/// <param name="selectionStart"></param>
		/// <param name="selectionLength"></param>
		protected virtual void KeyPressedCommand(object parameter) {}
			
		/// <summary>
		/// Takes the user entry and finds the code associated with it.
		/// </summary>
		/// <param name="text">The final text of the control.</param>
		/// <param name="key">The key used to terminate the control.</param>
		protected virtual void EndEditCommand(object parameter) {}

	}

}

