namespace MarkThree.Forms
{

	using System;
	using System.Drawing;
	using System.Data;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;

	/// <summary>Dialog box used to select and order the columns in a spreadsheet view.</summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ColumnSelector : System.Windows.Forms.Form
	{

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBoxAvailableFields;
		private System.Windows.Forms.ListBox listBoxDisplayedFields;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.Button buttonMoveUp;
		private System.Windows.Forms.Button buttonMoveDown;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private Stylesheet.ColumnsNode columnList;

		/// <summary>The table containing the columns of the viewer.</summary>
		public Stylesheet.ColumnsNode Columns
		{
			
			// Get the list of columns.
			get {return this.columnList;}
			
			// Initialize the list boxes with a list of columns.
			set {this.columnList = (Stylesheet.ColumnsNode)value.Clone(); this.DrawListBoxes();}
		
		}

		/// <summary>
		/// Creates a dialog box for showing/hiding/sorting the columns in a viewer.
		/// </summary>
		public ColumnSelector()
		{

			// Required for Windows Form Designer support
			InitializeComponent();

		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxAvailableFields = new System.Windows.Forms.ListBox();
			this.listBoxDisplayedFields = new System.Windows.Forms.ListBox();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonMoveUp = new System.Windows.Forms.Button();
			this.buttonMoveDown = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "A&vailable fields:";
			// 
			// listBoxAvailableFields
			// 
			this.listBoxAvailableFields.Location = new System.Drawing.Point(6, 24);
			this.listBoxAvailableFields.Name = "listBoxAvailableFields";
			this.listBoxAvailableFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxAvailableFields.Size = new System.Drawing.Size(185, 186);
			this.listBoxAvailableFields.TabIndex = 1;
			// 
			// listBoxDisplayedFields
			// 
			this.listBoxDisplayedFields.Location = new System.Drawing.Point(284, 24);
			this.listBoxDisplayedFields.Name = "listBoxDisplayedFields";
			this.listBoxDisplayedFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxDisplayedFields.Size = new System.Drawing.Size(185, 186);
			this.listBoxDisplayedFields.TabIndex = 2;
			// 
			// buttonAdd
			// 
			this.buttonAdd.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.buttonAdd.Location = new System.Drawing.Point(199, 24);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(76, 23);
			this.buttonAdd.TabIndex = 3;
			this.buttonAdd.Text = "&Add ->";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.buttonRemove.Location = new System.Drawing.Point(199, 56);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(76, 23);
			this.buttonRemove.TabIndex = 4;
			this.buttonRemove.Text = "<- &Remove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(285, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(179, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Show these fields in this order:";
			// 
			// buttonMoveUp
			// 
			this.buttonMoveUp.Location = new System.Drawing.Point(288, 216);
			this.buttonMoveUp.Name = "buttonMoveUp";
			this.buttonMoveUp.TabIndex = 6;
			this.buttonMoveUp.Text = "Move &Up";
			this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
			// 
			// buttonMoveDown
			// 
			this.buttonMoveDown.Location = new System.Drawing.Point(376, 216);
			this.buttonMoveDown.Name = "buttonMoveDown";
			this.buttonMoveDown.TabIndex = 7;
			this.buttonMoveDown.Text = "Move &Down";
			this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(288, 248);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 8;
			this.buttonOK.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(376, 248);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "Cancel";
			// 
			// ColumnSelector
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(476, 280);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonMoveDown);
			this.Controls.Add(this.buttonMoveUp);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.listBoxDisplayedFields);
			this.Controls.Add(this.listBoxAvailableFields);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ColumnSelector";
			this.ShowInTaskbar = false;
			this.Text = "Show Fields";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>Populate the list boxes with the hidden and displayed columns available.</summary>
		private void DrawListBoxes()
		{

			// Populate the 'Available' list of columns with the hidden columns.  Note that a 'ColumnItem' class is needed to get a
			// readable name for the items displayed in the list box.
			// HACK - This needs to work with the ViewColumns list.
			Stylesheet.ColumnsNode availableList = new Stylesheet.ColumnsNode();
			Stylesheet.ColumnsNode displayedList = new Stylesheet.ColumnsNode();
			//foreach (Stylesheet.ColumnNode column in this.columnList)
			//    (column.IsDisplayed ? displayedList : availableList).Add(column);
			Stylesheet.ColumnNode[] availableColumns = new Stylesheet.ColumnNode[availableList.Count];
			availableList.CopyTo(availableColumns, 0);
			Stylesheet.ColumnNode[] displayedColumns = new Stylesheet.ColumnNode[displayedList.Count];
			displayedList.CopyTo(displayedColumns, 0);

			// This will preserve the selection of the available (hidden) columns when the list is redrawn.
			ListBox.SelectedObjectCollection selectedAvailableObjects = this.listBoxAvailableFields.SelectedItems;
			Stylesheet.ColumnNode[] selectedAvailableColumns = new Stylesheet.ColumnNode[selectedAvailableObjects.Count];
			selectedAvailableObjects.CopyTo(selectedAvailableColumns, 0);
			
			// This will preserve the selection of the displayed (visible) columns when the list is redrawn.
			ListBox.SelectedObjectCollection selectedDisplayedObjects = this.listBoxDisplayedFields.SelectedItems;
			Stylesheet.ColumnNode[] selectedDisplayedColumns = new Stylesheet.ColumnNode[selectedDisplayedObjects.Count];
			selectedDisplayedObjects.CopyTo(selectedDisplayedColumns, 0);

			// Clear out the previous entries and repopulate the list of available (hidden) fields.  Note that the updating is
			// suspended so the user doesn't see the items cleared out.  If the updating were not inhibited, there would be a 
			// noticable 'blink' as the items were cleared and repopulated.  Note that the selected state of the items is preserved when the list is
			// redrawn.
			this.listBoxAvailableFields.BeginUpdate();
			this.listBoxAvailableFields.Items.Clear();
			this.listBoxAvailableFields.Items.AddRange(availableColumns);
			foreach (Stylesheet.ColumnNode column in selectedAvailableColumns)
			{
				int index = this.listBoxAvailableFields.Items.IndexOf(column);
				if (index != -1)
					this.listBoxAvailableFields.SetSelected(index, true);
			}
			foreach (Stylesheet.ColumnNode column in selectedDisplayedColumns)
			{
				int index = this.listBoxAvailableFields.Items.IndexOf(column);
				if (index != -1)
					this.listBoxAvailableFields.SetSelected(index, true);
			}
			this.listBoxAvailableFields.EndUpdate();

			// Do the same for the displayed fields.  Note that the selected state of the items is preserved when the list is
			// redrawn.
			this.listBoxDisplayedFields.BeginUpdate();
			this.listBoxDisplayedFields.Items.Clear();
			this.listBoxDisplayedFields.Items.AddRange(displayedColumns);
			foreach (Stylesheet.ColumnNode column in selectedAvailableColumns)
			{
				int index = this.listBoxDisplayedFields.Items.IndexOf(column);
				if (index != -1)
					this.listBoxDisplayedFields.SetSelected(index, true);
			}
			foreach (Stylesheet.ColumnNode column in selectedDisplayedColumns)
			{
				int index = this.listBoxDisplayedFields.Items.IndexOf(column);
				if (index != -1)
					this.listBoxDisplayedFields.SetSelected(index, true);
			}
			this.listBoxDisplayedFields.EndUpdate();

		}
		
		/// <summary>
		/// Add an available column to the list of displayed columns.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonAdd_Click(object sender, System.EventArgs e)
		{

			// Make each of the selected columns visible and move it from the 'Available' list to the 'Displayed list.  Note that
			// the selection is changed during the loop, so a copy of the selected items is needed for the loop.
			Stylesheet.ColumnNode[] selectedItems = new Stylesheet.ColumnNode[this.listBoxAvailableFields.SelectedItems.Count];
			this.listBoxAvailableFields.SelectedItems.CopyTo(selectedItems, 0);
			// HACK - Make this work with the new COlumnView.
			//foreach (Stylesheet.ColumnNode column in selectedItems)
			//    column.IsDisplayed = true;
			DrawListBoxes();

		}

		/// <summary>
		/// Removes a column from the list of displayed columns.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonRemove_Click(object sender, System.EventArgs e)
		{

			// Make each of the selected columns visible and move it from the 'Displayed' list to the 'Displayed list.  Note that
			// the selection is changed during the loop, so a copy of the selected items is needed for the loop.
			Stylesheet.ColumnNode[] selectedItems = new Stylesheet.ColumnNode[this.listBoxDisplayedFields.SelectedItems.Count];
			this.listBoxDisplayedFields.SelectedItems.CopyTo(selectedItems, 0);
			// HACK - make this work with the new ColumnView
			//foreach (Stylesheet.ColumnNode column in selectedItems)
			//    column.IsDisplayed = false;
			DrawListBoxes();

		}

		/// <summary>
		/// Moves the selected elements up in the order of columns.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonMoveUp_Click(object sender, System.EventArgs e)
		{

			// This will suspend the events while the lists are udpated.
			this.listBoxDisplayedFields.BeginUpdate();

			// This will move the selected items upward in the list box.
			foreach (int selectedIndex in this.listBoxDisplayedFields.SelectedIndices)
			{

				// Halt the operation if the columns are already at the start of the report.  With a multiple selection, this will
				// appear to halt the whole train of selected items when it reaches the start of the column list.
				if (selectedIndex == 0)
					break;

				// Move the element up one in the ordering of the columns in the internal data structures.
				Stylesheet.ColumnNode sourceColumn = (Stylesheet.ColumnNode)this.listBoxDisplayedFields.Items[selectedIndex];
				Stylesheet.ColumnNode destinationColumn = (Stylesheet.ColumnNode)this.listBoxDisplayedFields.Items[selectedIndex - 1];
				int sourceIndex = this.columnList.IndexOf(sourceColumn);
				int destinationIndex = this.columnList.IndexOf(destinationColumn);
				this.columnList.Move(sourceIndex, destinationIndex);

				// Move the element up in the list box also.
				this.listBoxDisplayedFields.Items.RemoveAt(selectedIndex);
				this.listBoxDisplayedFields.Items.Insert(selectedIndex - 1, sourceColumn);
				this.listBoxDisplayedFields.SetSelected(selectedIndex - 1, true);

			}

			// This will re-enable the events
			this.listBoxDisplayedFields.EndUpdate();

		}

		/// <summary>
		/// Moves the selected elements down in the order of columns.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonMoveDown_Click(object sender, System.EventArgs e)
		{
		
			// This will suspend the events while the lists are udpated.
			this.listBoxDisplayedFields.BeginUpdate();

			// This willl move each of the items in a downward direction in the list of displayed columns.
			int[] selectedIndices = new int[this.listBoxDisplayedFields.SelectedIndices.Count];
			this.listBoxDisplayedFields.SelectedIndices.CopyTo(selectedIndices, 0);
			Array.Reverse(selectedIndices);
			foreach (int selectedIndex in selectedIndices)
			{

				// This will stop the operation when the selected columns have reached the rightmost side of the document.
				if (selectedIndex == this.listBoxDisplayedFields.Items.Count - 1)
					break;

				// Move the element down in the ordering of the column list data structure.
				Stylesheet.ColumnNode sourceColumn = (Stylesheet.ColumnNode)this.listBoxDisplayedFields.Items[selectedIndex];
				Stylesheet.ColumnNode destinationColumn = (Stylesheet.ColumnNode)this.listBoxDisplayedFields.Items[selectedIndex + 1];
				int sourceIndex = this.columnList.IndexOf(sourceColumn);
				int destinationIndex = this.columnList.IndexOf(destinationColumn);
				this.columnList.Move(sourceIndex, destinationIndex);

				// Move the element down the list box also.
				this.listBoxDisplayedFields.Items.RemoveAt(selectedIndex);
				this.listBoxDisplayedFields.Items.Insert(selectedIndex + 1, sourceColumn);
				this.listBoxDisplayedFields.SetSelected(selectedIndex + 1, true);

			}

			// This will re-enable the events
			this.listBoxDisplayedFields.EndUpdate();

		}

	}

}
