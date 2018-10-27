namespace MarkThree.Forms
{

	using System;
	using System.Drawing;
	using System.Data;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;

	/// <summary>
	/// Form to allow for the selection and ordering of the columns in the document viewer.
	/// </summary>
	public partial class ColumnSelector : Form
	{

		// Private Members
		private MarkThree.Forms.DataTransform.ColumnsNode availableList;
		private MarkThree.Forms.DataTransform.ViewNode displayedList;

		// Public Members
		public MarkThree.Forms.DataTransform DataTransform;

		/// <summary>
		/// Create a form for selecting and ordering the columns in a document viewer.
		/// </summary>
		/// <param name="dataTransform">The description of how the document is constructed.</param>
		public ColumnSelector(DataTransform dataTransform)
		{

			// IDE Maintained Components.
			InitializeComponent();

			// Initialize the object.
			this.DataTransform = dataTransform;

			// These lists contain the available (currently invisible) columns and the displayed columns.
			this.availableList = new DataTransform.ColumnsNode();
			this.displayedList = new DataTransform.ViewNode();

			// The available columns are the ones that are not currently part of the view.  This basically selects all the columns
			// that are not in the view.
			foreach (DataTransform.ColumnNode column in this.DataTransform.Columns)
			{
				bool isFound = false;
				foreach (DataTransform.ColumnReferenceNode columnReferenceNode in this.DataTransform.View)
					if (column.ColumnId == columnReferenceNode.ColumnId)
					{
						isFound = true;
						break;
					}
				if (!isFound)
					availableList.Add(column);
			}

			// The items in the 'Display' list are the ones already in the view.
			foreach (DataTransform.ColumnReferenceNode columnReferenceNode in this.DataTransform.View)
				displayedList.Add(columnReferenceNode.Column);
			
			// This will synchronize the form with the two lists.
			DrawListBoxes();
		
		}

		/// <summary>
		/// Populate the two list boxes with the hidden and displayed columns.
		/// </summary>
		private void DrawListBoxes()
		{

			DataTransform.ColumnNode[] availableColumns = new DataTransform.ColumnNode[availableList.Count];
			availableList.CopyTo(availableColumns, 0);

			DataTransform.ColumnNode[] displayedColumns = new DataTransform.ColumnNode[displayedList.Count];
			displayedList.CopyTo(displayedColumns, 0);
			
			// The idea here is to preserve the selected items in the 'available' list box when it is redrawn with the new 
			// elements.
			ListBox.SelectedObjectCollection selectedAvailableObjects = this.listBoxAvailableFields.SelectedItems;
			DataTransform.ColumnNode[] selectedAvailableColumns = new DataTransform.ColumnNode[selectedAvailableObjects.Count];
			selectedAvailableObjects.CopyTo(selectedAvailableColumns, 0);

			// The idea here is to preserve the selected items in the 'displayed' list box when it is redrawn with the new 
			// elements.
			ListBox.SelectedObjectCollection selectedDisplayedObjects = this.listBoxDisplayedFields.SelectedItems;
			DataTransform.ColumnNode[] selectedDisplayedColumns = new DataTransform.ColumnNode[selectedDisplayedObjects.Count];
			selectedDisplayedObjects.CopyTo(selectedDisplayedColumns, 0);

			// Clear out the previous entries and repopulate the list of available (hidden) fields.  Note that the updating is
			// suspended so the user doesn't see the items cleared out.  If the updating were not inhibited, there would be a
			// noticable 'blink' as the items were cleared and repopulated.  Note that the selected state of the items is preserved
			// when the list is redrawn.
			this.listBoxAvailableFields.BeginUpdate();
			this.listBoxAvailableFields.Items.Clear();
			this.listBoxAvailableFields.Items.AddRange(availableColumns);
			foreach (DataTransform.ColumnNode column in selectedAvailableColumns)
			{
				int index = this.listBoxAvailableFields.Items.IndexOf(column);
				if (index != -1)
					this.listBoxAvailableFields.SetSelected(index, true);
			}
			foreach (DataTransform.ColumnNode column in selectedDisplayedColumns)
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
			foreach (DataTransform.ColumnNode column in selectedAvailableColumns)
			{
				int index = this.listBoxDisplayedFields.Items.IndexOf(column);
				if (index != -1)
					this.listBoxDisplayedFields.SetSelected(index, true);
			}
			foreach (DataTransform.ColumnNode column in selectedDisplayedColumns)
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

			// Make each of the selected columns visible and move it from the 'Available' list to the 'Displayed' list.  Note that
			// the selection is changed during the loop, so a copy of the selected items is needed for the loop.
			DataTransform.ColumnNode[] selectedItems =
				new DataTransform.ColumnNode[this.listBoxAvailableFields.SelectedItems.Count];
			this.listBoxAvailableFields.SelectedItems.CopyTo(selectedItems, 0);

			// Move the selected column from the available to the displayed list.
			foreach (DataTransform.ColumnNode columnNode in selectedItems)
			{
				this.availableList.Remove(columnNode);
				this.displayedList.Add(columnNode);
			}
			    
			// Refresh the data in the list boxes.
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
			DataTransform.ColumnNode[] selectedItems = new DataTransform.ColumnNode[this.listBoxDisplayedFields.SelectedItems.Count];
			this.listBoxDisplayedFields.SelectedItems.CopyTo(selectedItems, 0);

			// Move the selected column from the available to the displayed list.
			foreach (DataTransform.ColumnNode columnNode in selectedItems)
			{
				this.displayedList.Remove(columnNode);
				this.availableList.Add(columnNode);
			}

			// Refresh the data in the list boxes.
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
				DataTransform.ColumnNode sourceColumn = (DataTransform.ColumnNode)this.listBoxDisplayedFields.Items[selectedIndex];
				DataTransform.ColumnNode destinationColumn = (DataTransform.ColumnNode)this.listBoxDisplayedFields.Items[selectedIndex - 1];
				int sourceIndex = this.displayedList.IndexOf(sourceColumn);
				int destinationIndex = this.displayedList.IndexOf(destinationColumn);
				this.displayedList.Move(sourceIndex, destinationIndex);

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
				DataTransform.ColumnNode sourceColumn = (DataTransform.ColumnNode)this.listBoxDisplayedFields.Items[selectedIndex];
				DataTransform.ColumnNode destinationColumn = (DataTransform.ColumnNode)this.listBoxDisplayedFields.Items[selectedIndex + 1];
				int sourceIndex = this.displayedList.IndexOf(sourceColumn);
				int destinationIndex = this.displayedList.IndexOf(destinationColumn);
				this.displayedList.Move(sourceIndex, destinationIndex);

				// Move the element down the list box also.
				this.listBoxDisplayedFields.Items.RemoveAt(selectedIndex);
				this.listBoxDisplayedFields.Items.Insert(selectedIndex + 1, sourceColumn);
				this.listBoxDisplayedFields.SetSelected(selectedIndex + 1, true);

			}

			// This will re-enable the events
			this.listBoxDisplayedFields.EndUpdate();

		}

		/// <summary>
		/// Accepts the contents of the form applies it to the document form.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonOK_Click(object sender, EventArgs e)
		{

			// Clear the current contents of the view and reload it with the user's selection from the list boxes.  When this 
			// DataTransform is compiled, the new order of the columns will be reflected in the document in the viewer.
			this.DataTransform.View.Clear();
			foreach (DataTransform.ColumnNode columnNode in this.displayedList)
			{
				DataTransform.ColumnReferenceNode columnReferenceNode = new DataTransform.ColumnReferenceNode();
				columnReferenceNode.ColumnId = columnNode.ColumnId;
				this.DataTransform.View.Add(columnReferenceNode);
			}

		}

	}

}