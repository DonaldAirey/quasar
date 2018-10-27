/*************************************************************************************************************************
*
*	File:			CellEditControl.cs
*	Description:	An edit control element used to overlay a cell in a spreadsheet
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace MarkThree.Forms
{

	using MarkThree;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Data;
	using System.Windows.Forms;

	/// <summary>
	/// A text edit control to overlay a cell in a spreadsheet.
	/// </summary>
	public class CellControl : System.Windows.Forms.UserControl
	{

		private bool isNavigationAllowed;
		private bool isValid;
		protected StringAlignment alignment;
		protected StringAlignment lineAlignment;
		private System.ComponentModel.Container components = null;

		/// <summary>The destination cell for the data in this control.</summary>
		[Browsable(false)]
		public virtual StringAlignment Alignment {get {return this.alignment;} set {this.alignment = value;}}

		/// <summary>The destination cell for the data in this control.</summary>
		[Browsable(false)]
		public virtual StringAlignment LineAlignment {get {return this.lineAlignment;} set {this.lineAlignment = value;}}

		/// <summary>
		/// Constructor for the CellEditControl.
		/// </summary>
		public CellControl()
		{

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// This is the default alignment for a cell
			this.alignment = StringAlignment.Near;
			this.lineAlignment = StringAlignment.Near;

			// This control will generate an 'OnEndEdit' event when the focus is lost, unless this flag is cleared.
			this.isValid = true;

			// This control is initially invisible.
			this.Visible = false;

			// This controls whether the navigation (arrow) keys are passed on to the parent or handled by the edit
			// control.  Some modes of editing allow the user to use the arrow keys inside the cell for editing, other 
			// modes interpret the arrow keys as navigation in the spreadsheet.
			this.isNavigationAllowed = false;

		}

		#region Dispose MethodPlan
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// CellControl
			// 
			this.Name = "CellControl";
			this.Size = new System.Drawing.Size(102, 22);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Gets or sets whether the cell has valid information in it.
		/// </summary>
		[Browsable(false)]
		public bool IsValid {get {return this.isValid;} set {this.isValid = value;}}

		/// <summary>
		/// Gets or sets the ability to use navigation keys for editing.
		/// </summary>
		[Browsable(false)]
		public bool IsNavigationAllowed {get {return this.isNavigationAllowed;} set {this.isNavigationAllowed = value;}}

		/// <summary>
		/// SetS or gets the text of the cell.
		/// </summary>
		[Browsable(false)]
		public virtual new string Text {get {return string.Empty;} set {}}

		/// <summary>
		/// Notification of the termination of the editing by the user.
		/// </summary>
		[Browsable(true)]
		public event EndEditEventHandler EndEdit;

		/// <summary>
		/// Notification of the termination of the editing by the user.
		/// </summary>
		[Browsable(true)]
		public event NavigationEventHandler Navigation;

		/// <summary>
		/// Selects the contents of the entire control.
		/// </summary>
		public virtual void SelectAll() {}

		/// <summary>
		/// Handles the loss of focus to the edit control.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event parameters.</param>
		protected override void OnLeave(System.EventArgs eventArgs)
		{

			// If the control is invisible, then we've already left it through one of the keystrokes. If, however, the
			// control is still visible when it looses the focus, then it must be through some mouse action, such as
			// selecting another cell.  Make the input cell invisible and accept whatever input we have.
			if (this.Visible)
			{

				// Hide the cell.
				this.Visible = false;
		
				// Broadcast event to anyone listening.  Typically we'll at least have the spreadsheet control interested
				// in selecting the spreadsheet again after the edit control has been hidden.  Also, there will usually be
				// one top-level window that's actually interested in the content.
				if (this.isValid)
					OnEndEdit(new EndEditEventArgs(this.Text));

			}

		}
		
		/// <summary>
		/// Called to broadcast the end of editing event.
		/// </summary>
		/// <param name="endEditEventArgs">Event argument.</param>
		protected virtual void OnEndEdit(EndEditEventArgs endEditEventArgs)
		{

			// If the escape key was used to end the control's focus, we inhibit the 'EndEdit' event from fireing.  All
			// other methods of exiting the control will multicast this event to anyone listening.  This is usually a
			// signal that the user has accepted the input and the program should process it now.
			if (this.EndEdit != null)
				this.EndEdit(this, endEditEventArgs);

		}

		/// <summary>
		/// Called to broadcast the end of editing event.
		/// </summary>
		/// <param name="endEditEventArgs">Event argument.</param>
		protected virtual void OnNavigation(NavigationEventArgs navigationEventArgs)
		{

			// If the escape key was used to end the control's focus, we inhibit the 'Navigation' event from fireing.  All
			// other methods of exiting the control will multicast this event to anyone listening.  This is usually a
			// signal that the user has accepted the input and the program should process it now.
			if (this.Navigation != null)
				this.Navigation(this, navigationEventArgs);

		}

	}

}
