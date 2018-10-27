namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.IO;
	using System.Threading;
	using System.Xml;

	/// <summary>
	/// A column that defines how the data is sorted.
	/// </summary>
	internal class ViewColumn
	{
		public string ColumnId;
		public SortDirection Direction;
	}

	/// <summary>A structure used to manage the presentation of data in a viewer.</summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Spreadsheet : System.Data.DataTable
	{

		// This is the amount of time the animation thread is given to exit after being given the signal to terminated.  If it
		// fails to end gracefully, the background thread is aborted.
		private const int animationThreadTimeout = 100;

		// This is the amount of time between color changes in the animation sequences.  An animation with 5 steps will take five
		// of these time periods to complete.
		private const int animationPeriod = 1000;

		// Private Members
		internal MarkThree.Forms.AnimatedList animatedList;
		internal MarkThree.Forms.SpreadsheetRowView spreadsheetRowView;
		internal MarkThree.Forms.SpreadsheetColumnView spreadsheetColumnView;
		internal System.Boolean isAnimationRunning;
		internal System.Collections.Generic.Dictionary<string, Style> styleTable;
		internal System.Collections.Generic.List<ViewColumn> defaultView = null;
		internal System.Drawing.Rectangle displayRectangle;
		internal System.Drawing.Rectangle headerRectangle;
		internal System.Drawing.Size broadcastDataSize;
		internal System.Drawing.Size broadcastHeaderSize;
		internal System.Int32 rowIndex;
		internal System.Threading.ReaderWriterLock readerWriterLock;
		internal System.Threading.Thread animationThread;

		// Public Properties
		public MarkThree.Forms.SelectionMode SelectionMode;

		/// <summary>
		/// Initialize the spreadsheet.
		/// </summary>
		public Spreadsheet(IContainer iContainer)
		{

			// Add this spreadsheet to the list of objects in the container.  When the container is destroyed, all the components,
			// including this one, will be disposed of as well.
			iContainer.Add(this);

			// These objects provide sorting and filtering for the rows and columns.
			this.spreadsheetRowView = new SpreadsheetRowView(this);
			this.spreadsheetColumnView = new SpreadsheetColumnView(this);

			// This table contains the styles.
			this.styleTable = new Dictionary<string, Style>();

			// This list is used for all the cells that have been animated.  At regular intervals, the style on the cells in this
			// list is changed to make it appear to be changing.  See the 'AnimationThread' for the usage.
			this.animatedList = new AnimatedList();

			// These are the initial dimensions of the spreadsheet and the header.
			this.displayRectangle = Rectangle.Empty;
			this.headerRectangle = Rectangle.Empty;
			this.broadcastDataSize = Size.Empty;

			// This lock is used to coordinate input and output from the this data structure.  The document can be read from any
			// number of threads, but when the data model is changed (written to), other threads must be prevented from reading
			// intermediate results.
			this.readerWriterLock = new ReaderWriterLock();

			// This flag is used to terminate the background thread used to animate cells that have changed.  When set to false the
			// background thread will exit.
			this.isAnimationRunning = true;

#if DEBUG
			// Disable the background threads when in the design mode.  They will kill the designer.
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
#endif

				// This thread will refresh the document from the background.  That is, all the hard work is done in the background.
				// Delegates will be called when the data is actually ready to be put in the control.
				this.animationThread = new Thread(new ThreadStart(AnimationThread));
				this.animationThread.Name = "AnimationThread";
				this.animationThread.IsBackground = true;
				this.animationThread.Start();

#if DEBUG
			}
#endif

		}

		public SpreadsheetRowView ViewRows { get { return this.spreadsheetRowView; } }

		public SpreadsheetColumnView ViewColumns { get { return this.spreadsheetColumnView; } }

		internal DataColumnCollection BaseColumns { get { return base.Columns; } }

		/// <summary>
		/// Builds a new SpreadsheetRow.
		/// </summary>
		/// <returns>A new SpreadsheetRow.</returns>
		internal SpreadsheetRow NewSpreadsheetRow() { return (SpreadsheetRow)this.NewRow(); }

		/// <summary>
		/// Builds a new row.
		/// </summary>
		/// <param name="builder">An object used to create new rows.</param>
		/// <returns>A new row.</returns>
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder) { return new SpreadsheetRow(builder); }

		public System.Drawing.Rectangle DisplayRectangle {get {return this.displayRectangle;}}
		public System.Drawing.Rectangle HeaderRectangle {get {return this.headerRectangle;}}
		public System.Threading.ReaderWriterLock Lock {get {return this.readerWriterLock;}}

		// Public Events
		public event InvalidRegionEventHandler DataInvalidated;
		public event InvalidRegionEventHandler HeaderInvalidated;
		public event SizeEventHandler DataSizeChanged;
		public event SizeEventHandler HeaderSizeChanged;

		/// <summary>
		/// Indicates whether the background thread is still running.
		/// </summary>
		private bool IsAnimationRunning
		{
			get {lock (this) return this.isAnimationRunning;}
			set {lock (this) this.isAnimationRunning = value;}
		}

		/// <summary>
		/// Release managed resources when the control is finished.
		/// </summary>
		/// <param name="disposing">true indicates that managed resources should be released.</param>
		protected override void Dispose(bool disposing)
		{

			// Abort the background animation thread when closing down this data structure.
			if (disposing)
			{

				// Force the animation thread to exit.  The thread is given a reasonable amount of time to terminate on its own
				// before it is forced to shut down.
				this.IsAnimationRunning = false;
				if (!this.animationThread.Join(Spreadsheet.animationThreadTimeout))
					this.animationThread.Abort();

			}

			// Allow the base class to finish cleaning up the resources.
			base.Dispose (disposing);

		}

		/// <summary>
		/// Broadcast a notification that a section of the spreadsheet data has changed.
		/// </summary>
		/// <param name="region">The region of the spreadsheet that has changed.</param>
		internal void OnDataInvalidated(Region region)
		{

			// Broadcast the modified region to anyone listening.
			if (region != null && this.DataInvalidated != null)
				this.DataInvalidated(this, new InvalidRegionEventArgs(region));

		}

		/// <summary>
		/// Broadcasts a notification that the size of the spreadsheet has changed.
		/// </summary>
		/// <param name="size">The new size of the spreadsheet.</param>
		internal void OnDataSizeChanged(Size size)
		{

			// Broadcast the new size to anyone listening.  Note that the event is only changed when it is different from the last
			// size broadcast.
			if (size != this.broadcastDataSize)
			{
				if (this.DataSizeChanged != null)
					this.DataSizeChanged(this, new SizeEventArgs(size));
				this.broadcastDataSize = size;
			}

		}

		/// <summary>
		/// Broadcast a notification that a section of the spreadsheet header has changed.
		/// </summary>
		/// <param name="region">The region of the spreadsheet header that has changed.</param>
		internal void OnHeaderInvalidated(Region region)
		{

			// Broadcast the modified region to anyone listening.
			if (region != null && this.HeaderInvalidated != null)
				this.HeaderInvalidated(this, new InvalidRegionEventArgs(region));

		}

		/// <summary>
		/// Broadcasts a notification that the size of the header has changed.
		/// </summary>
		/// <param name="size">The new size of the header.</param>
		internal void OnHeaderSizeChanged(Size size)
		{

			// Broadcast the new size to anyone listening.  Note that the message is only sent when the size has changed.
			if (size != this.broadcastHeaderSize)
			{
				if (this.HeaderSizeChanged != null)
					this.HeaderSizeChanged(this, new SizeEventArgs(size));
				this.broadcastHeaderSize = size;
			}

		}

		internal void ApplyView()
		{

			// This will construct the spreadsheetRowView sort string from the ViewColumns.
			int columnIndex = 0;
			string sort = string.Empty;
			foreach (ViewColumn viewColumn in this.defaultView)
				sort += string.Format(columnIndex++ < this.defaultView.Count - 1 ? "{0} {1}," : "{0} {1}",
					viewColumn.ColumnId, viewColumn.Direction == SortDirection.Descending ? "DESC" : "ASC");

			// Apply the sort to the view.
			this.spreadsheetRowView.Sort = sort;

		}

		public void AppendView(string columnId, SortDirection sortDirection)
		{

			ViewColumn viewColumn = new ViewColumn();
			viewColumn.ColumnId = columnId;
			viewColumn.Direction = sortDirection;
			this.defaultView.Add(viewColumn);

			ApplyView();

		}

		public override void Reset()
		{

			// This variable is used to give each row a unique index as they are added.  It will count monotonically until the
			// table is reset.  Rows and columns are ordered to give each cell a cartesean [RowIndex, ColumnIndex] type of 
			// address.
			this.rowIndex = 0;

			// This is the default action for selecting objects in the view.
			this.SelectionMode = DefaultSpreadsheet.SelectionMode;

			// The data in the table is ordered according to the primary key.  However, the displayed data is ordered by a 
			// user-selectable order and can filter out data based on rules.  This filter and sort fields control the data that
			// is displayed in the viewer.
			this.spreadsheetRowView.RowFilter = string.Empty;
			this.spreadsheetRowView.Sort = string.Empty;

			// Release any of the graphical objects in the current spreadsheet.
			foreach (SpreadsheetRow spreadsheetRow in this.Rows)
				foreach (SpreadsheetColumn spreadsheetColumn in this.Columns)
				{
					SpreadsheetCell spreadsheetCell = spreadsheetRow[spreadsheetColumn];
					if (spreadsheetCell.Value is Bitmap)
						((Bitmap)spreadsheetCell.Value).Dispose();
				}

			this.ViewColumns.Clear();

			this.styleTable.Clear();

			// A default style is always available.
			Style defaultStyle = new Style();
			defaultStyle.Id = DefaultSpreadsheet.StyleId;
			this.styleTable.Add(defaultStyle.Id, defaultStyle);

			// The old animation list is no longer valid after reading a new document.  Note that there is an implicit Mutex in the
			// animated list because a there is a background thread that also uses this list.
			this.animatedList.Clear();

			this.headerRectangle = Rectangle.Empty;
			this.displayRectangle = Rectangle.Empty;

			base.Reset();

		}

		public void SetView(string columnId, SortDirection sortDirection)
		{

			this.defaultView.Clear();

			ViewColumn viewColumn = new ViewColumn();
			viewColumn.ColumnId = columnId;
			viewColumn.Direction = sortDirection;
			this.defaultView.Add(viewColumn);

			ApplyView();

		}

		/// <summary>
		/// Measures the header area of the viewer and the width of the screen and printed documents.
		/// </summary>
		/// <returns>The region of the header that needs to be repainted.</returns>
		internal Region MeasureHeader()
		{

			// Initialze the width of the display and printed documents as we're about to remeasure them.
			this.headerRectangle.Width = 0;
			this.displayRectangle.Width = 0;

			// This keeps track of the location of the column.
			Point location = Point.Empty;

			// Measure each column in turn.
			foreach (SpreadsheetColumn spreadsheetColumn in this.ViewColumns)
			{

				// The location of the column is used when drawing and doing hit testing.  Note that the width was set when the 
				// column was parsed in from the XML or by a user's explicit action to widen or shrink the column.
				Rectangle displayRectangle = spreadsheetColumn.Rectangle;
				displayRectangle.Location = location;
				displayRectangle.Height = this.headerRectangle.Height;
				spreadsheetColumn.Rectangle = displayRectangle;

				// The width of visible columns is also added to the horizontal location of the next column to be measured and the
				// width is added to the aggregate width of the display.
				location.X += spreadsheetColumn.Rectangle.Width;
				this.headerRectangle.Width += spreadsheetColumn.Rectangle.Width;
				this.displayRectangle.Width += spreadsheetColumn.Rectangle.Width;

			}

			// This region contains the areas that need to be redrawn to reflect the new 
			return new Region(this.headerRectangle);

		}

		public void Refresh()
		{

			// These will be used to broadcast the change in size and the invalid regions after the document has been measured
			// with the new rowfilter.
			System.Drawing.Size headerSize;
			System.Drawing.Size dataSize;
			System.Drawing.Region headerRegion;
			System.Drawing.Region dataRegion;

			try
			{

				// The document must be locked because changing the row filter changes everything.
				this.readerWriterLock.AcquireWriterLock(Timeout.Infinite);

				// Install the new row filter and remeasure the document.
				headerRegion = MeasureHeader();
				dataRegion = MeasureData();
				headerSize = this.headerRectangle.Size;
				dataSize = this.displayRectangle.Size;

			}

			finally
			{

				// All the work has been completed.
				this.readerWriterLock.ReleaseWriterLock();

			}

			// Broadcast the change in size and the invalid regions so the viewer can draw only the parts that have changed.
			OnHeaderSizeChanged(headerSize);
			OnDataSizeChanged(dataSize);
			OnHeaderInvalidated(headerRegion);
			OnDataInvalidated(dataRegion);

		}

		/// <summary>
		/// Measures the document, row and cell dimensions.
		/// </summary>
		/// <returns>A region of the document that has changed.</returns>
		internal Region MeasureData()
		{

			// This area will collect all the rectangles that need to be updated in the viewer.
			Region region = new Region();
			region.MakeEmpty();

			// This is used to collect smaller row and cell rectangles into the largest possible area.  The idea is to minimzed the
			// number of rectangles that the GDI has to handle when the document is redrawn.  Of course, this could be done with an
			// infinite region, but the idea is also to minimize the amount of drawing that needs to be done.
			Rectangle aggregateRectangle = Rectangle.Empty;

			// This is used as a relatively static area where the dimensions of a row in the current view are calcualted.
			Rectangle rowRectangle = Rectangle.Empty;

			// These provide cursors for keeping track of the location as the document is measured.
			Point displayCursor = Point.Empty;
			Point printerCursor = Point.Empty;

			// Initialize the total area of the screen and printed document.  The width of the documents has been calculated by
			// this point (it is calculated in 'MeasureHeader', but the length has not yet been calculated.
			this.displayRectangle = new Rectangle(0, 0, this.headerRectangle.Width, 0);

			// The default view defines which rows are visible and in what order the appear.  It defines the 'visible' document.
			// This code will run through the visible document and assign coordinates to the rows and cells according to their
			// place in the view.
			int rowIndex = 0;
			foreach (DataRowView dataRowView in this.spreadsheetRowView)
			{

				// The underlying row will be updated with coordinates that represent its location in the view.
				SpreadsheetRow spreadsheetRow = (SpreadsheetRow)dataRowView.Row;

				// This index gives the row position within the sorted and filtered view.
				spreadsheetRow.RowViewIndex = rowIndex++;

				// This define the coordinates of this row in the view.
				rowRectangle.X = displayCursor.X;
				rowRectangle.Y = displayCursor.Y;
				rowRectangle.Width = this.displayRectangle.Width;
				rowRectangle.Height = spreadsheetRow.rectangle.Height;

				// The code below will check the coordinates of the rows and cells with the goal of preserving the data that hasn't
				// changed.  However, if the row is invalid, there's no need to add the cells to the update region.  If every cell
				// of every row were entered into the region, it would choke the GDI.  If the row is out of place, this flag will
				// skip the cell checking logic.
				bool isRowValid = true;

				// If the position and height of the row don't agree with where it should be in the DataView, then the coordinates
				// will have to be updated and the row will be added to the list of things that need redrawing.
				if (spreadsheetRow.rectangle.Location != rowRectangle.Location ||
					spreadsheetRow.rectangle.Height != rowRectangle.Height || !spreadsheetRow.IsVisible)
				{

					// The entire row will be added to the update region, there's no need to add the cell rectangles as well.  
					// This is done as an optimization; if ever cell in a large document were modified at once, it would blow out
					// the storage are for a region.
					isRowValid = false;

					// Update the coordinates of the row with the calculated rectangle from the view.
					spreadsheetRow.rectangle = rowRectangle;

					// This will combine the rectangles when the top and bottom borders coincide.  The aggregate rectangle collects
					// individual rows into one large rectangle and places the largest contiguous rectangle possible into the
					// region.  This simplifies the work the GDI has to do to process the invalid areas of the document.
					if (aggregateRectangle.Left == rowRectangle.Left && aggregateRectangle.Width == rowRectangle.Width &&
						aggregateRectangle.Bottom == rowRectangle.Top)
						aggregateRectangle = Rectangle.Union(aggregateRectangle, rowRectangle);
					else
					{
						if (!aggregateRectangle.IsEmpty)
							region.Union(aggregateRectangle);
						aggregateRectangle = rowRectangle;
					}

				}

				// This will calculate the location of each of the cells in document coordinates.  If the coordinates are 
				// different from the current coordinates that cell will be added to the update region, assuming the row hasn't
				// already been added.
				foreach (SpreadsheetColumn spreadsheetColumn in this.ViewColumns)
				{

					SpreadsheetCell spreadsheetCell = spreadsheetRow[spreadsheetColumn];

					// This is the rectangle that this cell should occupy.
					Rectangle cellRectangle = new Rectangle(displayCursor, new Size(spreadsheetColumn.Rectangle.Width,
						spreadsheetRow.rectangle.Height));

					// This is the test to see if this cell needs to be redrawn.  If the row isn't valid, then we don't need 
					// to redraw the cell because it will be redrawn when the row is painted.  If the dimensions of the cell
					// have changed, then it needs to be repainted.  Finally, if there was an original version of this cell and
					// it isn't the same as the suggested cell, it needs to be repainted.
					if (isRowValid && (spreadsheetCell.DisplayRectangle != cellRectangle || spreadsheetCell.IsModified))
					{

						// This will aggregate the cells into larger rectangles if their edges cooincide.  The large contiguous
						// rectangles are added to the area that needs to be redrawn.
						if (aggregateRectangle.Top == cellRectangle.Top && aggregateRectangle.Height == cellRectangle.Height &&
							aggregateRectangle.Right == cellRectangle.Left)
							aggregateRectangle = Rectangle.Union(aggregateRectangle, cellRectangle);
						else
						{
							if (!aggregateRectangle.IsEmpty)
								region.Union(aggregateRectangle);
							aggregateRectangle = cellRectangle;
						}

					}

					spreadsheetCell.DisplayRectangle = cellRectangle;

					// This flag is set after the cell is measured.  The cell won't be updated again until this flag is set
					// again.
					spreadsheetCell.IsModified = false;

					// Move the cursor up to the next column and test the next cell.
					displayCursor.X += spreadsheetColumn.Rectangle.Width;

				}

				// This will reset the cursor for the next row.
				displayCursor.X = 0;
				displayCursor.Y += spreadsheetRow.rectangle.Height;

			}

			// If all the rows and all the cells have been measured and checked for changes, then check one last time to see if 
			// there is anything in the aggregate rectangel that needs to be redrawn.
			if (!aggregateRectangle.IsEmpty)
				region.Union(aggregateRectangle);

			// This is the height of the displayed document.
			this.displayRectangle.Height = displayCursor.Y;

			// This contains all the areas that need to be redrawn.
			return region;

		}

		/// <summary>
		/// Provides highlighting of modified cells that fade over time.
		/// </summary>
		private void AnimationThread()
		{

			// This loop will use a timer to cycle through all the styles associated with an animated cell.  When an animation
			// attribute is declared for a Font style, an array of styles with colors blending from the start color to the actual
			// cell color is created.  As the timer ticks off, those styles are selected for the cell in sequence, giving the 
			// effect of a burst to the 'StartColor' and a gradual fade to the original color of the style.
			while (this.IsAnimationRunning)
			{

				// This region will be filled in with the rectangle of any cells that have changed color.
				bool hasUpdatedCells = false;
				Region region = new Region();
				region.MakeEmpty();

				try
				{

					// IMPORTANT CONCEPT:  Make sure that the data model of the viewer doesn't change while modifying the styles on
					// the cells that are being animated.  The list of animated cells must also be locked because the XML Reader
					// can add items to the animated list.  BE CAREFUL not to change the order of the locks.  If the locks are not
					// always aquired in the same order, the potential for a deadlock exists.
					this.readerWriterLock.AcquireWriterLock(Timeout.Infinite);
					this.animatedList.Mutex.WaitOne();

					// Cycle through all the cells that have been animated.  The index is used instead of the iterator so the 
					// animation can be removed from the list when it has cycled through all its colors.
					for (int animatedCellIndex = 0; animatedCellIndex < this.animatedList.Count; animatedCellIndex++)
					{

						// The 'animatedList' is a ordered list of row and column addresses, along with members that track the 
						// series of animated styles.  The ordering of the list of animated cells is important because there can be
						// many changes on a document and it's important to quickly access them when purging them.
						SpreadsheetCell spreadsheetCell = this.animatedList[animatedCellIndex];

						// The spreadsheet cell addressed by the animatedCell will have its style changed to the next color in the
						// sequence.  When all the styles (that contain different colors) have been used, the color of the cell
						// will return to the normal color of the cell.  When this happens, the cell is removed from the list of
						// animated cells.  Keeping the list small is important for performance.
						spreadsheetCell.Style = spreadsheetCell.AnimationArray[spreadsheetCell.AnimationIndex++];
						if (spreadsheetCell.AnimationIndex == spreadsheetCell.AnimationArray.Length)
							this.animatedList.Remove(spreadsheetCell);

						// This region will update all the modified cells on the screen when the loop is finished.
						if (spreadsheetCell.ColumnViewIndex != int.MinValue)
						{
							hasUpdatedCells = true;
							region.Union(spreadsheetCell.DisplayRectangle);
						}

					}

				}
				finally
				{

					// The viewer's data and the animated list can be use by other threads now.
					this.readerWriterLock.ReleaseWriterLock();
					this.animatedList.Mutex.ReleaseMutex();

				}

				// Broadcast a message to update the screen if there were any cells updated with new styles.
				if (hasUpdatedCells)
					OnDataInvalidated(region);

				// This provides the time dimension for the animation.
				Thread.Sleep(Spreadsheet.animationPeriod);

			}

		}

		public string ColumnFilter
		{

			set
			{

				// These will be used to broadcast the change in size and the invalid regions after the document has been measured
				// with the new rowfilter.
				System.Drawing.Size size;
				System.Drawing.Region region;

				try
				{

					// The document must be locked because changing the row filter changes everything.
					this.readerWriterLock.AcquireWriterLock(Timeout.Infinite);

					// Install the new row filter and remeasure the document.
					this.spreadsheetColumnView.ColumnFilter = value;
					region = this.MeasureData();
					size = this.displayRectangle.Size;

				}

				finally
				{

					// All the work has been completed.
					this.readerWriterLock.ReleaseWriterLock();

				}

				// Broadcast the change in size and the invalid regions so the viewer can draw only the parts that have changed.
				OnDataSizeChanged(size);
				OnDataInvalidated(region);

			}


		}

		/// <summary>
		/// Sets an expression used to filter which rows are viewed in the spreadsheet table.
		/// </summary>
		public string RowFilter
		{

			set
			{

				// These will be used to broadcast the change in size and the invalid regions after the document has been measured
				// with the new rowfilter.
				System.Drawing.Size size;
				System.Drawing.Region region;

				try
				{

					// The document must be locked because changing the row filter changes everything.
					this.readerWriterLock.AcquireWriterLock(Timeout.Infinite);

					// Install the new row filter and remeasure the document.
					this.spreadsheetRowView.RowFilter = value;
					region = this.MeasureData();
					size = this.displayRectangle.Size;

					foreach (SpreadsheetRow spreadsheetRow in this.Rows)
						spreadsheetRow.IsVisible = false;
					foreach (DataRowView dataRowView in this.ViewRows)
						((SpreadsheetRow)dataRowView.Row).IsVisible = true;

				}

				finally
				{

					// All the work has been completed.
					this.readerWriterLock.ReleaseWriterLock();

				}

				// Broadcast the change in size and the invalid regions so the viewer can draw only the parts that have changed.
				OnDataSizeChanged(size);
				OnDataInvalidated(region);

			}

		}

		/// <summary>
		/// Converts a text string into a System.Drawing.Color value.
		/// </summary>
		/// <param name="colorText">A formatted string containg RGB colors.</param>
		/// <returns>A Color made from the values found in the argument string.</returns>
		private static Color ConvertToColor(string colorText)
		{

			// Extract the RGB values from the string and reconstitute them into a System.Drawing.Color value.
			int rgb = Convert.ToInt32(colorText.Substring(1, colorText.Length - 1), 16);
			return Color.FromArgb(rgb >> 16 & 0xFF, rgb >> 8 & 0xFF, rgb & 0xFF);

		}

	}

}
