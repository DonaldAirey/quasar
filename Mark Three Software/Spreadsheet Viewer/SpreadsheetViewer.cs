namespace MarkThree.Forms
{

	using MarkThree;
	using MarkThree.Forms;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Drawing.Drawing2D;
	using System.Drawing.Text;
	using System.IO;
	using System.Reflection;
	using System.Threading;
	using System.Windows.Forms;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Xml.XPath;

	/// <summary>A document viewer based on the Microsoft Office Spreadsheet control.</summary>
	/// <copyright>Copyright (c) 2004 Mark Three Software - All Rights Reserved.</copyright>
	public class SpreadsheetViewer : MarkThree.Forms.Viewer
	{

		// Private Members
		private MarkThree.Forms.ClearControl controlData;
		private MarkThree.Forms.ClearControl controlHeader;
		private MarkThree.Forms.ColumnCursor columnCursor;
		private MarkThree.Forms.DestinationCursor destinationCursor;
		private MarkThree.Forms.Spreadsheet spreadsheet;
		private MarkThree.Forms.SpreadsheetColumn activeColumn;
		private MarkThree.Forms.SpreadsheetColumn anchorColumn;
		private MarkThree.Forms.SpreadsheetColumn destinationColumn;
		private MarkThree.Forms.SpreadsheetColumn resizeColumn;
		private MarkThree.Forms.SpreadsheetColumn selectedColumn;
		private MarkThree.Forms.SpreadsheetRow activeRow;
		private MarkThree.Forms.SpreadsheetRow anchorRow;
		private MarkThree.Forms.SpreadsheetViewer.InvalidRegionHandler invalidDataHandler;
		private MarkThree.Forms.SpreadsheetViewer.InvalidRegionHandler invalidHeaderHandler;
		private MarkThree.Forms.SpreadsheetViewer.SizeHandler sizeDataHandler;
		private MarkThree.Forms.SpreadsheetViewer.SizeHandler sizeHeaderHandler;
		private MarkThree.Forms.XmlSpreadsheetWriter spreadsheetWriter;
		private MarkThree.Forms.XmlStylesheetWriter xmlStylesheetWriter;
		private MarkThree.Forms.XmlStylesheetReader xmlStylesheetReader;
		private MarkThree.Forms.Stylesheet stylesheet;
		private MarkThree.Forms.CellControl cellControl;
		private MarkThree.Forms.CellControl textEditCell;
		private System.Boolean isDestinationSelected;
		private System.Collections.ArrayList selectedRanges;
		private System.ComponentModel.IContainer components = null;
		private System.Drawing.Brush backgroundBrush;
		private System.Drawing.Brush selectedBrush;
		private System.Drawing.Font font;
		private System.Drawing.Imaging.ImageAttributes invertedColorAttributes;
		private System.Drawing.Pen greyPen1;
		private System.Drawing.Pen greyPen2;
		private System.Drawing.Pen greyPen3;
		private System.Drawing.Pen greyPen4;
		private System.Drawing.Pen transparentPen;
		private System.Drawing.Point mouseDownLocation;
		private System.Drawing.Point[] lastLine;
		private System.Threading.ReaderWriterLock readerWriterLock;
		private System.Windows.Forms.Cursor bigEx;
		private System.Windows.Forms.HScrollBar hScrollBar;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.VScrollBar vScrollBar;
		private System.Xml.XmlResolver xmlUrlResolver;
		private System.Xml.Xsl.XslCompiledTransform xslCompiledTransform;
		private System.Drawing.Printing.PrintDocument printDocument;
		private System.Windows.Forms.PrintPreviewDialog printPreviewDialog;
		private System.Windows.Forms.PrintDialog printDialog;
		private System.Windows.Forms.PageSetupDialog pageSetupDialog;
	
		// Private Delegates
		private delegate void InvalidRegionHandler(Region region);
		private delegate void SizeHandler(Size size);

		/// <summary>The title displayed on the printed page.</summary>
		public string Title;

		// Public Events
		public event EndEditEventHandler EndEdit;
		public event EventHandler StylesheetChanged;
		public event EventHandler SelectionChanged;

		/// <summary>
		/// The menu that is displayed when the right mouse button is clicked over the data area of the control.
		/// </summary>
		[Browsable(true)]
		public override ContextMenu ContextMenu
		{
			get {return this.controlData.ContextMenu;}
			set {this.controlData.ContextMenu = this.panel1.ContextMenu = value;}
		}

		/// <summary>
		/// Where the data for the viewer is managed.
		/// </summary>
		[Browsable(false)]
		public Spreadsheet Spreadsheet {get {return this.spreadsheet;}}

		/// <summary>
		/// Describes how to transform the XML data into viewable data.
		/// </summary>
		[Browsable(false)]
		public Stylesheet Stylesheet {get {return this.stylesheet;}}

		/// <summary>
		/// The cell that has the input focus.
		/// </summary>
		[Browsable(false)]
		public SpreadsheetCell ActiveCell {get {return this.activeRow[this.activeColumn];}}

		/// <summary>
		/// The row containing the input focus.
		/// </summary>
		[Browsable(false)]
		public SpreadsheetRow ActiveRow {get {return this.activeRow;}}

		/// <summary>
		/// The column containing the input focus.
		/// </summary>
		[Browsable(false)]
		public SpreadsheetColumn ActiveColumn {get {return this.activeColumn;}}

		/// <summary>
		/// Used to filter out rows that shouldn't be displayed.
		/// </summary>
		[Browsable(false)]
		public string RowFilter {set {this.spreadsheet.RowFilter = value;}}

		[Browsable(false)]
		public string ColumnFilter { set { this.spreadsheet.ColumnFilter = value; } }

		/// <summary>
		/// Constructor for the SpreadsheetViewer object.
		/// </summary>
		public SpreadsheetViewer()
		{

			// Visual Studio maintained initialization.
			InitializeComponent();

			this.spreadsheetWriter = new XmlSpreadsheetWriter(this.spreadsheet);

			this.xmlStylesheetWriter = new XmlStylesheetWriter(this.stylesheet);

			this.xmlStylesheetReader = new XmlStylesheetReader(this.stylesheet);

			this.xslCompiledTransform = new XslCompiledTransform();

			// This is the text that is displayed on a printed page in the footer.
			this.Title = string.Empty;

			// This spreadsheet control has both a generic text window and a broker symbol cell control for getting input
			// from the user.  They are selected based on the column names found in the stylesheet.  Basically, the generic 
			// text cell will be selected unless a special column token is used for the broker symbol.
			this.cellControl = null;
			this.textEditCell = new TextEditCell();
			this.textEditCell.EndEdit += new EndEditEventHandler(cellControl_EndEditControl);
			this.textEditCell.Navigation += new NavigationEventHandler(cellControl_NavigationControl);

			// These values indicate the current anchor for the selection of multiple cells.  That is, when the 'Shift' key is 
			// held down, everything between the first mouse click and the second is selected.  The first cell selected in this
			// sequence is the 'anchor' cell.
			this.anchorRow = null;
			this.anchorColumn = null;

			// This lock coordinates multiple threads when they are trying to read and write from this control.
			this.readerWriterLock = new ReaderWriterLock();

			// This is the big, honkin' X that is used to indicate that if you drop and object during a drag-and-drop operation, 
			// it will disappear.
			this.bigEx = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("MarkThree.Forms.BigEx.cur"));

			// This 'ImageAttributes' object contains a matrix that can be used to invert the colors of a bitmap.  It is used to 
			// highlight important sections of the screen.  Most notably, it is used during the selection process to highlight the
			// area around the first selected range.
			this.invertedColorAttributes = new ImageAttributes();
			this.invertedColorAttributes.SetColorMatrix(new ColorMatrix(new float[][] {
																						  new float[] {-1.0f, 0.0f, 0.0f, 0.0f, 0.0f},
																						  new float[] {0.0f, -1.0f, 0.0f, 0.0f, 0.0f},
																						  new float[] {0.0f, 0.0f, -1.0f, 0.0f, 0.0f},
																						  new float[] {0.0f, 0.0f, 0.0f, 1.0f, 0.0f},
																						  new float[] {1.0f, 1.0f, 1.0f, 0.0f, 1.0f}}));

			// These members are used during the drag-and-drop operations.  These operations are modeled after the Microsoft 
			// OutLook control and allow the user to select the order of the columns, remove (hide) columns and increase or
			// decrease the size of the columns.
			this.selectedColumn = null;
			this.isDestinationSelected = false;
			this.columnCursor = null;
			this.resizeColumn = null;
			
			// The selected area of the screen is a collection of ranges.  The ranges are nothing more than a collection of 
			// cells.  The selected area is maintained using bits on the cells to track the selected/unselected status, and this
			// collection of collections of cells.  This 'selectedRanges' collection is used primarily to quickly erase the
			// selected areas without having to scan the entire document.
			this.selectedRanges = new System.Collections.ArrayList();
			this.activeRow = null;
			this.activeColumn = null;

			// These event handlers allow background threads to invalidate areas of the screen and to change the size of the
			// controls.  These operations are generally frowned upon from a background thread as they can cause all sorts of
			// concurrency problems.
			this.invalidDataHandler = new InvalidRegionHandler(DataInvalidatedHandler);
			this.invalidHeaderHandler = new InvalidRegionHandler(HeaderInvalidatedHandler);
			this.sizeDataHandler = new SizeHandler(DataSizeChangedHandler);
			this.sizeHeaderHandler = new SizeHandler(HeaderSizeChangedHandler);
			
			// These events allow the spreadsheet data structure to communicate a change in size or an invalid area as they occur.
			// The work in conjunction with the above handlers to allow the background threads to asynchrnously advise the
			// foreground that a section of the viewed document needs to be updated.
			this.spreadsheet.DataInvalidated += new MarkThree.Forms.InvalidRegionEventHandler(spreadsheet_DataInvalidated);
			this.spreadsheet.DataSizeChanged += new MarkThree.Forms.SizeEventHandler(spreadsheet_DataSizeChanged);
			this.spreadsheet.HeaderInvalidated += new MarkThree.Forms.InvalidRegionEventHandler(spreadsheet_HeaderInvalidated);
			this.spreadsheet.HeaderSizeChanged += new MarkThree.Forms.SizeEventHandler(spreadsheet_HeaderSizeChanged);

			// This is a default resover for transforming XML documents.
			this.xmlUrlResolver = new XmlUrlResolver();

			// These brushes are used for effects when painting in the data control window.
			this.greyPen1 = new Pen(System.Drawing.Color.FromArgb(255, 207, 206, 210));
			this.greyPen2 = new Pen(System.Drawing.Color.FromArgb(255, 197, 196, 200));
			this.greyPen3 = new Pen(System.Drawing.Color.FromArgb(255, 183, 183, 187));
			this.greyPen4 = new Pen(System.Drawing.Color.FromArgb(255, 157, 157, 161));
			this.transparentPen = new Pen(Color.Transparent, 3);
			this.selectedBrush = new SolidBrush(System.Drawing.Color.FromArgb(0xFF, 0xB6, 0xCA, 0xEA));
			this.backgroundBrush = new SolidBrush(Color.White);

			// This is the default font for drawing titles, footers, etc.
			this.font = new System.Drawing.Font("Tahoma", 8.0f);

			// Default Page Settings for printing.
			this.printDocument.DefaultPageSettings.Landscape = true;
			this.printDocument.DefaultPageSettings.Margins.Left = 50;
			this.printDocument.DefaultPageSettings.Margins.Right = 50;
			this.printDocument.DefaultPageSettings.Margins.Top = 50;
			this.printDocument.DefaultPageSettings.Margins.Bottom = 50;

		}

		/// <summary>
		/// Performs the application defined tasks associates with freeing, releasing or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">true indicates that managed resources should be released.</param>
		protected override void Dispose(bool disposing)
		{

			// Dispose of all the managed resources.
			if (disposing)
				if (this.components != null)
					this.components.Dispose();

			// Allow the base class to continue cleaning up the resources.
			base.Dispose(disposing);

		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpreadsheetViewer));
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.controlHeader = new MarkThree.Forms.ClearControl();
			this.controlData = new MarkThree.Forms.ClearControl();
			this.stylesheet = new MarkThree.Forms.Stylesheet();
			this.spreadsheet = new MarkThree.Forms.Spreadsheet(this.components);
			this.printDocument = new System.Drawing.Printing.PrintDocument();
			this.printPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
			this.printDialog = new System.Windows.Forms.PrintDialog();
			this.pageSetupDialog = new System.Windows.Forms.PageSetupDialog();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spreadsheet)).BeginInit();
			this.SuspendLayout();
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "Web Page|*.HTML|XML Spreadsheet|*.XML|CSV (Comma delimited)|*.CSV";
			this.saveFileDialog.FilterIndex = 2;
			this.saveFileDialog.Title = "Save As";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Controls.Add(this.hScrollBar);
			this.panel1.Controls.Add(this.vScrollBar);
			this.panel1.Controls.Add(this.controlHeader);
			this.panel1.Controls.Add(this.controlData);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(512, 256);
			this.panel1.TabIndex = 0;
			this.panel1.Click += new System.EventHandler(this.panel1_Click);
			this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.BackColor = System.Drawing.SystemColors.Control;
			this.panel2.Location = new System.Drawing.Point(495, 239);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(17, 17);
			this.panel2.TabIndex = 3;
			// 
			// hScrollBar
			// 
			this.hScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.hScrollBar.Location = new System.Drawing.Point(0, 239);
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(495, 17);
			this.hScrollBar.TabIndex = 0;
			this.hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar_Scroll);
			// 
			// vScrollBar
			// 
			this.vScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.vScrollBar.Location = new System.Drawing.Point(495, 0);
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(17, 239);
			this.vScrollBar.TabIndex = 0;
			this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
			// 
			// controlHeader
			// 
			this.controlHeader.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.controlHeader.Location = new System.Drawing.Point(0, 0);
			this.controlHeader.Name = "controlHeader";
			this.controlHeader.Size = new System.Drawing.Size(0, 0);
			this.controlHeader.TabIndex = 0;
			this.controlHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.controlHeader_MouseDown);
			this.controlHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ControlHeader_MouseMove);
			this.controlHeader.Resize += new System.EventHandler(this.panel1_Resize);
			this.controlHeader.Paint += new System.Windows.Forms.PaintEventHandler(this.controlHeader_Paint);
			this.controlHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.controlHeader_MouseUp);
			// 
			// controlData
			// 
			this.controlData.BackColor = System.Drawing.SystemColors.Window;
			this.controlData.Location = new System.Drawing.Point(0, 0);
			this.controlData.Name = "controlData";
			this.controlData.Size = new System.Drawing.Size(0, 0);
			this.controlData.TabIndex = 1;
			this.controlData.DoubleClick += new System.EventHandler(this.childViewer_DoubleClick);
			this.controlData.Click += new System.EventHandler(this.childViewer_Click);
			this.controlData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.controlData_MouseDown);
			this.controlData.Resize += new System.EventHandler(this.panel1_Resize);
			this.controlData.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.controlData_KeyPress);
			this.controlData.Paint += new System.Windows.Forms.PaintEventHandler(this.controlData_Paint);
			// 
			// stylesheet
			// 
			this.stylesheet.Changed += new System.EventHandler(this.Stylesheet_Changed);
			// 
			// spreadsheet
			// 
			this.spreadsheet.Locale = new System.Globalization.CultureInfo("en-US");
			// 
			// printPreviewDialog
			// 
			this.printPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
			this.printPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);
			this.printPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
			this.printPreviewDialog.Document = this.printDocument;
			this.printPreviewDialog.Enabled = true;
			this.printPreviewDialog.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog.Icon")));
			this.printPreviewDialog.Name = "printPreviewDialog";
			this.printPreviewDialog.UseAntiAlias = true;
			this.printPreviewDialog.Visible = false;
			// 
			// printDialog
			// 
			this.printDialog.Document = this.printDocument;
			// 
			// pageSetupDialog
			// 
			this.pageSetupDialog.Document = this.printDocument;
			// 
			// SpreadsheetViewer
			// 
			this.Controls.Add(this.panel1);
			this.Name = "SpreadsheetViewer";
			this.Size = new System.Drawing.Size(512, 256);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spreadsheet)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Handles an invalid region in the data portion of the spreadsheet control.
		/// </summary>
		/// <remarks>Generally triggered from a background thread for handling by the Message Loop thread.</remarks>
		/// <param name="region">The invalid area to be repainted.</param>
		private void DataInvalidatedHandler(Region region)
		{

			// Allow the foreground to invalidate the area.
			this.controlData.Invalidate(region);

		}

		/// <summary>
		/// Handles resizing of the data portion of the spreadsheet control.
		/// </summary>
		/// <remarks>Generally triggered from a background thread for handling by the Message Loop thread.</remarks>
		/// <param name="region">The new size of the data area of the spreadsheet control.</param>
		private void DataSizeChangedHandler(Size size)
		{

			// Allow the foreground to resize the area.
			this.controlData.Size = size;

		}

		/// <summary>
		/// Handles an invalid region in the header portion of the spreadsheet control.
		/// </summary>
		/// <remarks>Generally triggered from a background thread for handling by the Message Loop thread.</remarks>
		/// <param name="region">The invalid area to be repainted.</param>
		private void HeaderInvalidatedHandler(Region region)
		{

			// Allow the foreground to invalidate the header.
			this.controlHeader.Invalidate(region);

		}

		/// <summary>
		/// Locates the edit control over the active cell.
		/// </summary>
		private void LocateEditCell()
		{

			// This will position the edit control exactly on top of the active cell.
			SpreadsheetCell activeCell = this.ActiveCell;
			this.cellControl.Location = activeCell.DisplayRectangle.Location;
			this.cellControl.Size = new Size(activeCell.DisplayRectangle.Width - 1,
				activeCell.DisplayRectangle.Height - 1);

		}

		/// <summary>
		/// Handles the initialization of the edit control for a spreadsheet cell.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="endEditEventArgs">The event argument.</param>
		public virtual void OnStartEdit(StartEditArgs startEditArgs)
		{

			// This section will figure out which cell control to put in place based on information found in the stylesheet.  The
			// first step to selecting a control window to place over the cell is to remove any previous cell control.
			if (this.cellControl != null)
			{
				this.controlData.Controls.Remove(cellControl);
				this.cellControl = null;
			}

			// If an input contor hasn't been chosen in the code above, use the generic input window.
			if (this.cellControl == null)
				this.cellControl = textEditCell;

			// Add the chosen input control to the list of controls in the spreadsheet window.  When it has the input focus, all
			// mouse and keystrokes will go to this window instead of the spreadsheet.
			this.controlData.Controls.Add(this.cellControl);

			// Place the edit control over the active cell.
			this.LocateEditCell();
			SpreadsheetCell activeCell = this.ActiveCell;
			this.cellControl.Font = activeCell.Style.Font;
			this.cellControl.BackColor = activeCell.Style.InteriorPen.Color;
			this.cellControl.ForeColor = activeCell.Style.FontBrush.Color;

			// Translate the Excel cell alignment to one that the control window understands.  This mimics the effect of editing in
			// Excel with cell alignments.
			this.cellControl.Alignment = activeCell.Style.StringFormat.Alignment;
			this.cellControl.LineAlignment = activeCell.Style.StringFormat.LineAlignment;

			// After initializeing the cell, we give it the focus.  It will now accept the keyboard data instead of the
			// spreadsheet.  This will tell the edit control that the arrow keys are used for editing within the cell and not for
			// navigating around the spreadsheet.
			this.cellControl.IsNavigationAllowed = startEditArgs.IsNavigationAllowed;
			this.cellControl.Text = startEditArgs.InitialText;
			this.cellControl.IsValid = true;
			this.cellControl.Visible = true;
			this.cellControl.BringToFront();
			this.cellControl.Focus();

			// Certain editing modes will start the editing with the existing contents selected.  Others will leave the choice of
			// what should be selected up to the individual controls.
			if (startEditArgs.IsTextSelected)
				this.cellControl.SelectAll();

		}
		
		/// <summary>
		/// Handles the 'End of User Input' event from the edit control that overlays the spreadsheet.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event parameters.</param>
		private void cellControl_EndEditControl(object sender, MarkThree.Forms.EndEditEventArgs e)
		{

			// When the edit control has finished the input, we want to re-select the spreadsheet control
			this.controlData.Select();

			// re-broadcast the 'End Edit' event to any control that is listening.
			OnEndEdit(e);

		}

		/// <summary>
		/// Called when the user has finished entering data in the spreadsheet.
		/// </summary>
		/// <param name="endEditEventArgs"></param>
		protected virtual void OnEndEdit(EndEditEventArgs endEditEventArgs)
		{

			// Re-Multicast the event to anyone listening for the edit control to finish.  This hides the fact that the
			// spreadsheet isn't really responsible for the user input.  That is, we've put our own edit control over the
			// spreadsheet so it can immitate the act of entering data in a cell.  This edit control isn't impacted by
			// background refreshes the way the native in-cell editing is.
			if (this.EndEdit != null)
				this.EndEdit(this, endEditEventArgs);

		}

		/// <summary>
		/// Handles the 'End of User Input' event from the edit control that overlays the spreadsheet.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event parameters.</param>
		private void cellControl_NavigationControl(object sender, MarkThree.Forms.NavigationEventArgs e)
		{

			try
			{

				// The key used to terminate the input determines which direction to move the active cell.
				switch (e.NavigationCommand)
				{
	
				case NavigationCommand.Nothing: break;
				case NavigationCommand.Enter: Navigate(NavigationCommand.Down); break;
				case NavigationCommand.Right: Navigate(NavigationCommand.Right); break;
				case NavigationCommand.Left: Navigate(NavigationCommand.Left); break;
				case NavigationCommand.Down: Navigate(NavigationCommand.Down); break;
				case NavigationCommand.Up: Navigate(NavigationCommand.Up); break;

				}

			}
			catch (Exception exception)
			{

				// Log the exception.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}

		}

		/// <summary>
		/// Handles resizing of the header portion of the spreadsheet control.
		/// </summary>
		/// <remarks>Generally triggered from a background thread for handling by the Message Loop thread.</remarks>
		/// <param name="region">The new size of the header area of the spreadsheet control.</param>
		private void HeaderSizeChangedHandler(Size size)
		{

			// Chaging the height of the header area will impact the location of the data area.  This is the difference up or down
			// that the data area will be moved.
			int headerHeightChange = this.controlHeader.Height - size.Height;

			// Update the header area for the new height and reposition the data area to start right after the header.
			SuspendLayout();
			this.controlHeader.Size = size;
			this.controlData.Top = this.controlData.Top - headerHeightChange;
			ResumeLayout(true);
			
		}

		/// <summary>Transforms the input XML document into data that appears in the viewer.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public XmlDocument XslStylesheet
		{

			get
			{

				XmlDocument xmlDocument = new XmlDocument();

				try
				{

					xmlDocument.Load(this.xmlStylesheetReader);

				}
				catch (Exception exception)
				{

					EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

				}

				// This is the XML version of the internal data structure of a stylesheet.
				return xmlDocument;

			}

			set
			{

				try
				{

					this.stylesheet.BeginLoad();

					// Other threads must be prevented from reading while the structure is changed.
					this.stylesheet.Lock.AcquireWriterLock(Timeout.Infinite);

					// Load the XSL Stylesheet into an internal data structure.
					value.WriteTo(this.xmlStylesheetWriter);

					// Install the new stylesheet as the transform used by this viewer.
					LoadTransform();

				}
				catch (Exception exception)
				{

					// Dump the error to the console.
					Console.WriteLine("{0}, {1}", exception.Message, exception.StackTrace);

				}
				finally
				{

					// The stylesheet and data have been loaded into the viewer at this point.
					this.stylesheet.Lock.ReleaseWriterLock();

					this.stylesheet.EndLoad();

				}

			}

		}

		/// <summary>
		/// Compiles the stylesheet and installs it as the transform used to turn the XML data into viewable data.
		/// </summary>
		public void LoadTransform()
		{

			try
			{

				// This will prevent any access to the 'xslCompiledTransform' while it loads the stylesheet.  The lock on the
				// Compiled Transform object is not the same as the lock on the stylesheet.
				this.readerWriterLock.AcquireWriterLock(Timeout.Infinite);

				// Create a compiled XSL transform based on the reconstituted Stylesheet.
				this.xslCompiledTransform.Load(this.XslStylesheet);

#if DEBUGXML
			try
			{

				// Notice that it is the reconstituted stylesheet that is being saved, rather than use the original source.  This 
				// insures that the debug information contains the data in the internal structures.
				this.XslStylesheet.Save(string.Format("stylesheet.{0}.xml", this.GetType()));

			}
			catch (Exception exception)
			{

				// Make sure that writing the debug information doesn't kill the application.
				Console.WriteLine("{0}, {1}", exception.Message, exception.StackTrace);

			}
#endif

			}
			catch (Exception exception)
			{

				EventLog.Warning("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// The stylesheet and data have been loaded into the viewer at this point.
				this.readerWriterLock.ReleaseWriterLock();

			}				

		}

		/// <summary>
		/// The XML Document currently displayed in the viewer.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public XmlDocument XmlDocument
		{

			set
			{

				try
				{

#if DEBUGXML
					try
					{

						// Save the transformed document when debugging.
                        XmlTextWriter xmlDebugTextWriter = new XmlTextWriter(string.Format("spreadsheet.{0}.xml", this.GetType()),
							System.Text.Encoding.ASCII);
						xmlDebugTextWriter.Formatting = Formatting.Indented;
						this.xslCompiledTransform.Transform(new XPathDocument(new XmlNodeReader(value.DocumentElement)),
							xmlDebugTextWriter);
                        xmlDebugTextWriter.Close();

					}
					catch (Exception exception)
					{

						// Make sure that writing the debug information doesn't kill the application.
						Console.WriteLine("{0}, {1}", exception.Message, exception.StackTrace);

					}
#endif

					// IMPORTANT CONCEPT: This is where the compiled stylesheet is used to transform the incoming data into an XML
					// form that can be fed directly into the spreadsheet.
					DateTime time1 = DateTime.Now;
					this.xslCompiledTransform.Transform(new XmlNodeReader(value.DocumentElement), this.spreadsheetWriter);
					Console.WriteLine("Time to write document = {0}", DateTime.Now.Subtract(time1).TotalMilliseconds);

				}
				catch (Exception exception)
				{

					EventLog.Warning("{0}, {1}", exception.Message, exception.StackTrace);

				}

			}

		}

		/// <summary>
		/// Allows the user to select and order the columns in a viewer.
		/// </summary>
		public void SelectColumns()
		{


			// Initialize the column selection form with the current order of the columns and display the dialog.  This will give the
			// user the chance to order and show/hide any of the columns.
			ColumnSelector columnSelector = new ColumnSelector();
			columnSelector.Columns = this.stylesheet.Table.Columns;
			if (columnSelector.ShowDialog(this) == DialogResult.OK)
			{
				this.stylesheet.Table.Columns = columnSelector.Columns.Clone();
				this.spreadsheet.Refresh();
			}

		}

		public XmlDocument ToXml(bool isExtendedFormat)
		{

			return new XmlDocument();

		}

		private void controlHeader_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{

			try
			{

				this.spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);
			
				PaintHeader(e.Graphics, e.ClipRectangle);

				if (this.selectedColumn != null && e.ClipRectangle.IntersectsWith(this.selectedColumn.Rectangle))
				{

					Bitmap selectedBitmap = new Bitmap(this.selectedColumn.Rectangle.Width, this.selectedColumn.Rectangle.Height, e.Graphics);
					Graphics selectedGraphics = Graphics.FromImage(selectedBitmap);

					selectedGraphics.TranslateTransform(Convert.ToSingle(-this.selectedColumn.Rectangle.X),
						Convert.ToSingle(-this.selectedColumn.Rectangle.Y));

					PaintHeader(selectedGraphics, this.selectedColumn.Rectangle);

					e.Graphics.DrawImage(selectedBitmap, this.selectedColumn.Rectangle, 0, 0, selectedBitmap.Width,
						selectedBitmap.Height, GraphicsUnit.Pixel, this.invertedColorAttributes);

				}

			}
			finally
			{

				this.spreadsheet.Lock.ReleaseReaderLock();

			}

		}

		private void PaintHeader(Graphics graphics, Rectangle clipRectangle)
		{

			foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.ViewColumns)
				if (clipRectangle.IntersectsWith(spreadsheetColumn.Rectangle))
				{

					Style style = spreadsheetColumn.Style;

					Bitmap columnBitmap = new Bitmap(spreadsheetColumn.Rectangle.Width, spreadsheetColumn.Rectangle.Height, graphics);
					Graphics columnGraphics = Graphics.FromImage(columnBitmap);

					Rectangle rectangle = new Rectangle(0, 0, columnBitmap.Width, columnBitmap.Height);

					columnGraphics.Clear(this.controlHeader.BackColor);
			
					if (spreadsheetColumn.Image == null)
					{

						if (spreadsheetColumn.Rectangle.Left == this.spreadsheet.HeaderRectangle.Left)
							columnGraphics.DrawLine(Pens.White, rectangle.X, 0, rectangle.X, rectangle.Bottom - 4);
						else
							columnGraphics.DrawLine(Pens.White, rectangle.X, 2, rectangle.X, rectangle.Bottom - 5);

						columnGraphics.DrawLine(this.greyPen1, rectangle.X, rectangle.Bottom - 3, rectangle.X + rectangle.Width - 1,
							rectangle.Bottom - 3);
						columnGraphics.DrawLine(this.greyPen2, rectangle.X, rectangle.Bottom - 2, rectangle.X + rectangle.Width - 1,
							rectangle.Bottom - 2);
						columnGraphics.DrawLine(this.greyPen3, rectangle.X, rectangle.Bottom - 1, rectangle.X + rectangle.Width - 1,
							rectangle.Bottom - 1);

						columnGraphics.DrawString(spreadsheetColumn.Description, style.Font, style.FontBrush,
							new Rectangle(rectangle.X + 2, 2, rectangle.Right - 4, rectangle.Bottom - 4),
							style.StringFormat);

						if (spreadsheetColumn.Rectangle.Right == this.spreadsheet.HeaderRectangle.Right)
							columnGraphics.DrawLine(this.greyPen4, rectangle.Right - 1, rectangle.Y, rectangle.Right - 1,
								rectangle.Bottom - 4);
						else
							columnGraphics.DrawLine(this.greyPen4, rectangle.Right - 1, 2, rectangle.Right - 1, rectangle.Bottom - 5);

					}
					else
					{

						Bitmap bitmap = (Bitmap)spreadsheetColumn.Image;
						int left = rectangle.X + (rectangle.Width - bitmap.Width) / 2;
						int top = rectangle.Y + (rectangle.Height - bitmap.Height) / 2;
						Rectangle destinationRectangle = new Rectangle(new Point(left, top), bitmap.Size);

						// Draw the formatted data in the given text box with the alignment specified in the stylesheet.
						columnGraphics.CompositingMode = CompositingMode.SourceOver;
						columnGraphics.DrawImage(bitmap, destinationRectangle);
						columnGraphics.CompositingMode = CompositingMode.SourceCopy;

					}
					
					graphics.DrawImage(columnBitmap, spreadsheetColumn.Rectangle, 0, 0, columnBitmap.Width,
						columnBitmap.Height, GraphicsUnit.Pixel);

				}


		}

		/// <summary>
		/// Paints the data portion of the spreadsheet control.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event parameters.</param>
		private void controlData_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{

			// The strategy for painting the invalid area involves a bitmap in the exact shape and size of the area that needs to
			// be painted.  If this area is empty, it will cause problems with the creation of that bitmap.  This check will insure
			// that there is always a positive area to paint.
			if (e.ClipRectangle.Width == 0 || e.ClipRectangle.Height == 0)
				return;

			Console.WriteLine("Updating region {0}", e.ClipRectangle);

			Rectangle outerRectangle = Rectangle.Inflate(e.ClipRectangle, 2, 2);
			outerRectangle.Intersect(this.spreadsheet.DisplayRectangle);

			try
			{

				// Make sure the spreadsheet data isn't written while trying to draw the screen.
				this.spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

				// IMPORTANT CONCEPT: The strategy for painting the screen is to create a bitmap of the invalid area, draw the data
				// in that bitmap, then dump the composed image in the invalid part of the screen.  This eliminates the 'flicker'
				// you'll get if you try to erase even a tiny portion of the device and write directly to the device.  This section
				// creates a bitmap for the invalid space and associates a 'Graphics' context with that bitmap so drawing commands
				// can write directly to the bitmap.  The Graphics space is transformed to match the area where the invalid 
				// rectangle is located. This allows the painting logic to simply write to the document coordinates.
				Bitmap paintBitmap = new Bitmap(e.ClipRectangle.Width, e.ClipRectangle.Height, e.Graphics);
				Graphics paintGraphic = Graphics.FromImage(paintBitmap);
				paintGraphic.TranslateTransform(Convert.ToSingle(-e.ClipRectangle.X), Convert.ToSingle(-e.ClipRectangle.Y));

				// IMPORTANT CONCEPT: The area selected by the user appears surrounded with inverted colors that act like a
				// cursor.  The inversion of the document is accomplished by taking the invalid area and complimenting the entire
				// bitmap.  Then, the original image is painted with a transparent color that will allow the inverted image to show
				// through when the two images are combined.  Finally, the regular image and the inverted image are combined and
				// displayed on the device.  The 'Compositing' mode (if that is a word) must be set to 'SourceCopy' in order for
				// the Alpha value of the transparent pens and brushes to be writting to the bitmap.  Without this setting, the pen
				// and brushes are combined with the background colors and the alpha value always remains what was first written to
				// the bitmap.
				paintGraphic.CompositingMode = CompositingMode.SourceCopy;
				paintGraphic.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

				// The loop below will cycle through the columns in the row looking for any cells that are in the range of the 
				// invalid area.
				int maxColumns = this.spreadsheet.ViewColumns.Count;

				// This will find the index of the first row in the invalid area.  Each row is examined in turn to see if it falls 
				// inside the invalid area and painted if it does.
				int startRowIndex = FindRow(outerRectangle.Top);
				for (int rowIndex = startRowIndex; rowIndex != -1; rowIndex++)
				{

					// Get the row of data at the current row index.  This row has the data that will be displayed in the invalid
					// area.
					SpreadsheetRow spreadsheetRow = this.spreadsheet.ViewRows[rowIndex];

					// The coordinates of each cell are calculated when the Xml Document is read.  This loop will examine each cell
					// in the row to see if it is part of the invalid area and will draw the cell if it is.
					for (int columnIndex = 0; columnIndex < maxColumns; columnIndex++)
					{

						SpreadsheetColumn spreadsheetColumn = this.spreadsheet.ViewColumns[columnIndex];

						// The cell contains the data and style information.  The style data structures contain all the
						// instructions for formatting the data.
						SpreadsheetCell spreadsheetCell = spreadsheetRow[spreadsheetColumn];

						// This test will determine whether the cell needs to be painted.  Basically, if the cell is visible and
						// inside the invalid area of the window, then it is painted.
						if (outerRectangle.IntersectsWith(spreadsheetCell.DisplayRectangle))
						{

							// The style is used several times below.  This is a shortcut to the style of the current cell.
							Style style = spreadsheetCell.Style;
						
							// Erase the previous contents of this cell and fill it in with an appropriate background color.  If
							// the cell is selected, then a standard background is used.  Otherwise the preference set in the
							// stylesheet determines the background color for this cell.
							if (spreadsheetCell.IsSelected && !spreadsheetCell.IsActiveCell)
							{

								paintGraphic.FillRectangle(selectedBrush, spreadsheetCell.DisplayRectangle);

								if (!IsTopCellSelected(columnIndex, rowIndex))
									paintGraphic.DrawLine(style.InteriorPen, spreadsheetCell.DisplayRectangle.Left,
										spreadsheetCell.DisplayRectangle.Top, spreadsheetCell.DisplayRectangle.Right,
										spreadsheetCell.DisplayRectangle.Top);
							
								if (!IsRightCellSelected(columnIndex, rowIndex))
									paintGraphic.DrawLine(style.InteriorPen, spreadsheetCell.DisplayRectangle.Right - 2,
										spreadsheetCell.DisplayRectangle.Top, spreadsheetCell.DisplayRectangle.Right - 2,
										spreadsheetCell.DisplayRectangle.Bottom);

								if (!IsBottomCellSelected(columnIndex, rowIndex))
									paintGraphic.DrawLine(style.InteriorPen, spreadsheetCell.DisplayRectangle.Right - 2,
										spreadsheetCell.DisplayRectangle.Bottom - 2, spreadsheetCell.DisplayRectangle.Left,
										spreadsheetCell.DisplayRectangle.Bottom - 2);

								if (!IsLeftCellSelected(columnIndex, rowIndex))
									paintGraphic.DrawLine(style.InteriorPen, spreadsheetCell.DisplayRectangle.Left,
										spreadsheetCell.DisplayRectangle.Bottom, spreadsheetCell.DisplayRectangle.Left,
										spreadsheetCell.DisplayRectangle.Top - 2);

							}
							else
								paintGraphic.FillRectangle(style.InteriorBrush, spreadsheetCell.DisplayRectangle);

							// Left Border
							if (style.LeftBorder != null)
								paintGraphic.DrawLine(style.LeftBorder, spreadsheetCell.DisplayRectangle.Left - 1,
									spreadsheetCell.DisplayRectangle.Top, spreadsheetCell.DisplayRectangle.Left - 1,
									spreadsheetCell.DisplayRectangle.Bottom);

							// Top Border
							if (style.TopBorder != null)
								paintGraphic.DrawLine(style.TopBorder, spreadsheetCell.DisplayRectangle.Left,
									spreadsheetCell.DisplayRectangle.Top - 1, spreadsheetCell.DisplayRectangle.Right,
									spreadsheetCell.DisplayRectangle.Top - 1);

							// Right Border	
							if (style.RightBorder != null)
								paintGraphic.DrawLine(style.RightBorder, spreadsheetCell.DisplayRectangle.Right - 1,
									spreadsheetCell.DisplayRectangle.Top, spreadsheetCell.DisplayRectangle.Right - 1,
									spreadsheetCell.DisplayRectangle.Bottom);

							// Bottom Border	
							if (style.BottomBorder != null)
								paintGraphic.DrawLine(style.BottomBorder, spreadsheetCell.DisplayRectangle.Left,
									spreadsheetCell.DisplayRectangle.Bottom - 1, spreadsheetCell.DisplayRectangle.Right,
									spreadsheetCell.DisplayRectangle.Bottom - 1);

							// Paint the data when the cell is not empty.
							if (spreadsheetCell.Value is Bitmap)
							{

								Bitmap bitmap = (Bitmap)spreadsheetCell.Value;
								int left = spreadsheetCell.DisplayRectangle.Location.X + (spreadsheetCell.DisplayRectangle.Width - bitmap.Width) / 2;
								int top = spreadsheetCell.DisplayRectangle.Location.Y + (spreadsheetCell.DisplayRectangle.Height - bitmap.Height) / 2;
								Rectangle destinationRectangle = new Rectangle(new Point(left, top), bitmap.Size);

								// Draw the formatted data in the given text box with the alignment specified in the stylesheet.
								paintGraphic.CompositingMode = CompositingMode.SourceOver;
								paintGraphic.DrawImage(bitmap, destinationRectangle);
								paintGraphic.CompositingMode = CompositingMode.SourceCopy;

							}
							else
							{

								// Format the data according to the style.
								string dataString = string.Format(style.NumberFormat, spreadsheetCell.Value);

								// This defines the boundary of the text area (reversed engineered from Excel).
								Rectangle textRectangle = new Rectangle(spreadsheetCell.DisplayRectangle.X, spreadsheetCell.DisplayRectangle.Y,
									spreadsheetCell.DisplayRectangle.Width - 1, spreadsheetCell.DisplayRectangle.Height);

								// Draw the formatted data in the given text box with the alignment specified in the stylesheet.
								paintGraphic.DrawString(dataString, style.Font, style.FontBrush, textRectangle, style.StringFormat);

							}

						}

					}

					// Keep on drawing the rows until either the end of the document is reached, or the row is no longer visible 
					// in the invalid region of the device.
					if (rowIndex == this.spreadsheet.ViewRows.Count - 1 ||
						spreadsheetRow.rectangle.Top > outerRectangle.Bottom)
						break;

				}

				// IMPORTANT CONCEPT: This logic handles the visual ques for a selected area of the document.  The selected area 
				// will be outlined in a reverse video highlight.  To achieve this effect, the entire bitmap of the invalid area is
				// inverted.  Then the outline of the selected area is painted with a transparent brush in the original,
				// non-inverted bitmap.  When the normal image with the transparent outline is copied onto the inverted image, the 
				// inverted outline will show through the transparent paint.  The combined image is then copied directly to the
				// device.  As mentioned earlier, blasting the complete bitmap onto the device removes the flicker from dynamic
				// devices like screens.
				Bitmap invertedBitmap = new Bitmap(paintBitmap.Width, paintBitmap.Height, paintGraphic);
				Graphics invertedGraphic = Graphics.FromImage(invertedBitmap);
				invertedGraphic.DrawImage(paintBitmap, new Rectangle(Point.Empty, invertedBitmap.Size), 0, 0, invertedBitmap.Width,
					invertedBitmap.Height, GraphicsUnit.Pixel, this.invertedColorAttributes);

				if (this.selectedRanges.Count == 1)
				{

					for (int rowIndex = startRowIndex; rowIndex != -1; rowIndex++)
					{

						// Get the row of data at the current row index.  This row has the data that will be displayed in the
						// invalid area.
						SpreadsheetRow spreadsheetRow = this.spreadsheet.ViewRows[rowIndex];

						// The coordinates of each cell are calculated when the Xml Document is read.  This loop will examine each cell
						// in the row to see if it is part of the invalid area and will draw the cell if it is.
						for (int columnIndex = 0; columnIndex < maxColumns; columnIndex++)
						{

							// The cell contains the data and style information.  The style data structures contain all the
							// instructions for formatting the data.
							SpreadsheetColumn spreadsheetColumn = this.spreadsheet.ViewColumns[columnIndex];
							SpreadsheetCell spreadsheetCell = spreadsheetRow[spreadsheetColumn];

							if (outerRectangle.IntersectsWith(spreadsheetCell.DisplayRectangle) && spreadsheetCell.IsSelected)
							{

								Rectangle cellRectangle = spreadsheetCell.DisplayRectangle;

								if (!IsLeftCellSelected(columnIndex, rowIndex))
									paintGraphic.DrawLine(transparentPen, cellRectangle.Left - 1, cellRectangle.Bottom + 1,
										cellRectangle.Left - 1, cellRectangle.Top - 2);

								if (!IsTopCellSelected(columnIndex, rowIndex))
									paintGraphic.DrawLine(transparentPen, cellRectangle.Left - 1, cellRectangle.Top - 1,
										cellRectangle.Right + 1, cellRectangle.Top - 1);

								if (!IsRightCellSelected(columnIndex, rowIndex))
									paintGraphic.DrawLine(transparentPen, cellRectangle.Right - 1, cellRectangle.Top - 1,
										cellRectangle.Right - 1, cellRectangle.Bottom + 1);

								if (!IsBottomCellSelected(columnIndex, rowIndex))
									paintGraphic.DrawLine(transparentPen, cellRectangle.Right + 1, cellRectangle.Bottom - 1,
										cellRectangle.Left - 1, cellRectangle.Bottom - 1);
						
							}

						}

						// Keep on drawing the rows until either the end of the document is reached, or the row is no longer visible 
						// in the invalid region of the device.
						if (rowIndex == this.spreadsheet.ViewRows.Count - 1 ||
							spreadsheetRow.rectangle.Top > outerRectangle.Bottom)
							break;
					}

				}

				invertedGraphic.DrawImage(paintBitmap, new Rectangle(Point.Empty, paintBitmap.Size));
				e.Graphics.DrawImage(invertedBitmap, e.ClipRectangle);

				// This method will eat up resources if they aren't immediately released.
				invertedGraphic.Dispose();
				invertedBitmap.Dispose();

				paintGraphic.Dispose();
				paintBitmap.Dispose();

			}
			finally
			{

				if (this.spreadsheet.Lock.IsReaderLockHeld)
					this.spreadsheet.Lock.ReleaseLock();

			}

		}

		private bool IsLeftCellSelected(int columnIndex, int rowIndex)
		{

			if (columnIndex == 0)
				return false;

			SpreadsheetRow spreadsheetRow = this.spreadsheet.ViewRows[rowIndex];
			SpreadsheetColumn spreadsheetColumn = this.spreadsheet.ViewColumns[columnIndex - 1];
			return spreadsheetRow[spreadsheetColumn].IsSelected;

		}

		public bool IsRightCellSelected(int columnIndex, int rowIndex)
		{

			if (columnIndex == this.spreadsheet.ViewColumns.Count - 1)
				return false;

			SpreadsheetRow spreadsheetRow = this.spreadsheet.ViewRows[rowIndex];
			SpreadsheetColumn spreadsheetColumn = this.spreadsheet.ViewColumns[columnIndex + 1];
			return spreadsheetRow[spreadsheetColumn].IsSelected;

		}

		public bool IsTopCellSelected(int columnIndex, int rowIndex)
		{

			if (rowIndex == 0)
				return false;

			SpreadsheetRow spreadsheetRow = this.spreadsheet.ViewRows[rowIndex - 1];
			SpreadsheetColumn spreadsheetColumn = this.spreadsheet.ViewColumns[columnIndex];
			return spreadsheetRow[spreadsheetColumn].IsSelected;

		}

		public bool IsBottomCellSelected(int columnIndex, int rowIndex)
		{

			if (rowIndex == this.spreadsheet.ViewRows.Count - 1)
				return false;

			SpreadsheetRow  spreadsheetRow = this.spreadsheet.ViewRows[rowIndex + 1];
			SpreadsheetColumn spreadsheetColumn = this.spreadsheet.ViewColumns[columnIndex];
			return spreadsheetRow[spreadsheetColumn].IsSelected;

		}
		
		/// <summary>
		/// Finds a row given the location.
		/// </summary>
		/// <param name="location">A vertical coordinate in spreadsheet units.</param>
		/// <returns>The row found at the given vertical coordinate.</returns>
		private int FindRow(int location)
		{

			// A binary search is used to find the row.  The top and bottom bounds will come together until the row is located.
			// The starting values for the binary search are provided by the number of rows in the used part of the spreadsheet.
			int bottomIndex = 0;
			int topIndex = this.spreadsheet.ViewRows.Count - 1;

			// Search until the indices meet.
			while (bottomIndex <= topIndex)
			{

				// The test row is midway between the upper and lower bounds.
				int rowIndex = (bottomIndex + topIndex) / 2;
				SpreadsheetRow spreadsheetRow = this.spreadsheet.ViewRows[rowIndex];

				// Test the row to see if the location is in the row, below the row or above.  This is a standard binary search, if
				// the location falls inside the row, then the search is satisfied.  Otherwise, move the binary searc to the space
				// above or below the current row and try again.
				if (spreadsheetRow.rectangle.Top <= location && location < spreadsheetRow.rectangle.Bottom)
					return rowIndex;
				else
				{
					if (location < spreadsheetRow.rectangle.Top)
						topIndex = rowIndex - 1;
					else
					{
						if (location >= spreadsheetRow.rectangle.Bottom)
							bottomIndex = rowIndex + 1;
					}
				}

			}

			// At this point, the entire document was search and the coordinate couldn't be found.
			return -1;

		}
		
		/// <summary>
		/// Gets a list of the selected block orders.
		/// </summary>
		/// <returns>A list of the currently selected block orders.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object[] SelectedItems
		{
			get
			{

				// Create a list to hold the keys of the items selected.
				ArrayList keyList = new ArrayList();

				try
				{

					this.spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

					// Cycle through the current selection extracting the key elements of the rows.  Remember that every range is a set of
					// contiguous rows, so each range needs to be scanned for rows.
					foreach (DataRowView dataRowView in this.spreadsheet.ViewRows)
					{
						bool isRowSelected = false;
						SpreadsheetRow spreadsheetRow = (SpreadsheetRow)dataRowView.Row;
						foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.ViewColumns)
						{
							SpreadsheetCell spreadsheetCell = spreadsheetRow[spreadsheetColumn];
							if (spreadsheetCell.IsSelected)
								isRowSelected = true;
						}

						if (isRowSelected)
						{

							object[] key = new object[this.spreadsheet.PrimaryKey.Length];
							for (int keyIndex = 0; keyIndex < this.spreadsheet.PrimaryKey.Length; keyIndex++)
							{
								SpreadsheetCell spreadsheetCell = spreadsheetRow[this.spreadsheet.PrimaryKey[keyIndex]];
								key[keyIndex] = spreadsheetCell.Value;
							}
							keyList.Add(key);

						}

					}

				}
				finally
				{

					this.spreadsheet.Lock.ReleaseReaderLock();

				}

				// Return a list of the selected blocks.
				object[] keyArray = new object[keyList.Count];
				keyList.CopyTo(keyArray);
				return keyArray;

			}

			set
			{

				ClearSelection();

				try
				{

					this.spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

					foreach (object[] key in value)
					{

						ArrayList arrayList = new ArrayList();

						SpreadsheetRow spreadsheetRow = this.spreadsheet.ViewRows.Find(key[0]);
						if (spreadsheetRow == null)
							Console.WriteLine("Item not found");

						foreach (SpreadsheetCell innerSpreadsheetCell in spreadsheetRow.Cells)
						{
							innerSpreadsheetCell.IsSelected = true;
							arrayList.Add(innerSpreadsheetCell);
						}

						this.selectedRanges.Add(arrayList);

					}

				}
				finally
				{

					this.spreadsheet.Lock.ReleaseReaderLock();

				}

				this.spreadsheet.Refresh();

			}

		}

		public void ClearSelection()
		{

			bool isSelectionChanged = false;

			try
			{

				this.spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

				foreach (ArrayList selectedRange in this.selectedRanges)
				{
					foreach (SpreadsheetCell spreadsheetCell in selectedRange)
					{
						spreadsheetCell.IsSelected = false;
						spreadsheetCell.IsActiveCell = false;
						this.controlData.Invalidate(Rectangle.Inflate(spreadsheetCell.DisplayRectangle, 2, 2));
					}
					selectedRange.Clear();

					isSelectionChanged = true;

				}

				this.activeRow = null;
				this.activeColumn = null;
				this.selectedRanges.Clear();

				this.anchorRow = null;
				this.anchorColumn = null;

			}
			finally
			{
				this.spreadsheet.Lock.ReleaseReaderLock();
			}

			if (isSelectionChanged)
				OnSelectionChanged(this);

		}

		public void SelectAll()
		{

			ClearSelection();

			try
			{

				ArrayList range = new ArrayList();
				this.selectedRanges.Add(range);

				this.spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

				foreach (DataRowView dataRowView in this.spreadsheet.ViewRows)
				{

					SpreadsheetRow spreadsheetRow = (SpreadsheetRow)dataRowView.Row;

					foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.ViewColumns)
					{

						SpreadsheetCell spreadsheetCell = spreadsheetRow[spreadsheetColumn];
						spreadsheetCell.IsSelected = true;
						range.Add(spreadsheetCell);

					}

				}

			}
			finally
			{

				this.spreadsheet.Lock.ReleaseReaderLock();

			}

			this.controlData.Invalidate();

		}

		/// <summary>
		/// Scrolls the Spreadsheet control according to the action of the horizontal scroll bar.
		/// </summary>
		/// <param name="sender">The object that originated the event</param>
		/// <param name="event_args">Arguments passed by the event</param>
		protected virtual void hScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{

			// The data and the header both move with respect to the left edge to make it appear as if they're scrolling in
			// repsonse to the horizontal scroll bar commands.
			this.panel1.SuspendLayout();
			this.controlHeader.Left = -e.NewValue;
			this.controlData.Left = -e.NewValue;
			this.panel1.ResumeLayout(false);

		}

		/// <summary>
		/// Scrolls the Spreadsheet control according to the action of the vertical scroll bar.
		/// </summary>
		/// <param name="sender">The object that originated the event</param>
		/// <param name="event_args">Arguments passed by the event</param>
		protected virtual void vScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{

			// The scrollable area of the data window is moved with respect to the bottom edge of the header.  This makes the data
			// appear to move while the headers stay fixed.
			this.controlData.Top = this.controlHeader.Bottom - e.NewValue;

		}

		/// <summary>
		/// Measures the scroll bars and sets the thumb to the proper location in the document.
		/// </summary>
		private void panel1_Resize(object sender, System.EventArgs e)
		{

			// The usable window height and width will change if scroll bars are present.  This calculation is somewhat iterative
			// in that the presence of a vertical scroll bar can alter the need for a horizontal scroll bar and visa-verca.
			int windowHeight = this.panel1.Height - this.controlHeader.Height;
			int windowWidth = this.panel1.Width;
			if (this.controlData.Height > windowHeight)
				windowWidth = this.panel1.Width - this.vScrollBar.Width;
			if (this.controlData.Width > windowWidth)
				windowHeight = this.panel1.Height - this.controlHeader.Height - this.hScrollBar.Height;
			if (this.controlData.Height > windowHeight)
				windowWidth = this.panel1.Width - this.vScrollBar.Width;
			windowWidth = windowWidth < 0 ? 0 : windowWidth;
			windowHeight = windowHeight < 0 ? 0 : windowHeight;

			// This rectangle represents the visible window in the spreadsheet pixel space.  It is used to test whether the window
			// on the document needs to be scrolled back into the document's coordinate space once the scroll bars have been 
			// adjusted.
			Point windowLocation = new Point(-this.controlData.Left, this.controlHeader.Height - this.controlData.Top);
			Size windowSize = new Size(windowWidth, windowHeight);
			Rectangle windowRectangle = new Rectangle(windowLocation, windowSize);

			// The scroll bar range reflects the document's dimensions.
			this.vScrollBar.Minimum = 0;
			this.vScrollBar.Maximum = this.controlData.Height;
			this.hScrollBar.Minimum = 0;
			this.hScrollBar.Maximum = this.controlData.Width;
			
			// The layout of several controls inside the panel will be changed.  This will prevent those changes from modifying the
			// window until all the changes are complete.
			this.panel1.SuspendLayout();
			
			// The window size is used as the 'LargeChange' during the scrolling actions.  Note the 'fudge' factor of '+ 1' in the
			// assignment for the 'Large Change'.  There is no explanation given as to why this property functions this way, just a
			// cryptic desription: "The maximum value that can be reached is equal to the Maximum property value minus the
			// LargeChange property value plus one."
			this.vScrollBar.LargeChange = windowHeight + 1;
			this.vScrollBar.SmallChange = Convert.ToInt32(DefaultSpreadsheet.RowHeight);
			this.vScrollBar.Visible = windowHeight < this.controlData.Height;
			this.vScrollBar.Height = windowWidth >= this.controlData.Width ? this.panel1.Height :
				this.panel1.Height - this.hScrollBar.Height;

			// This will calculate the horizontal metrics for the scroll bar.  The size of 'thumb' is the size of each window pane,
			// so that by clicking in the scroll bar for a large change, the view area is moved forward or backward by one full
			// window.  Also note that the scroll bars will disappear if they are not needed and the space will be used by the data
			// screens and any other visible scroll bars.
			this.hScrollBar.LargeChange = windowWidth + 1;
			this.hScrollBar.SmallChange = Convert.ToInt32(DefaultSpreadsheet.ColumnWidth);
			this.hScrollBar.Visible = windowWidth < this.controlData.Width;
			this.hScrollBar.Width = windowHeight >= this.controlData.Height ? this.panel1.Width :
				this.panel1.Width - this.vScrollBar.Width;

			// This panel fills in the gap in the lower right hand corner when both the vertical and horizontal scroll bars are
			// visible.  It is hidden when there's no gap to fill.
			this.panel2.Visible = windowRectangle.Width < this.controlData.Width && windowRectangle.Height < this.controlData.Height;

			// If the document's top edge has ended up in the client area of the viewer, then scroll it back to the top.  This
			// scenario is possible if a column header that was previously visibile is hidden.
			if (0 > windowRectangle.Top)
			{
				this.vScrollBar.Value = 0;
				this.controlData.Top = this.controlHeader.Bottom - this.vScrollBar.Value;
			}
			
			// If the window is enlarged, it will expose space to the bottom or right if the virtual window is positioned near the
			// ends of the document.  This space needs to be filled in or it leaves an ugly blank space in the window.  This will
			// 'pull' the document downward to fill in the space.
			if (this.controlData.Height < windowRectangle.Bottom)
			{
				this.vScrollBar.Value = this.controlData.Height - windowRectangle.Height < 0 ? 0 :
					(this.controlData.Height - windowRectangle.Height);
				this.controlData.Top = this.controlHeader.Bottom - this.vScrollBar.Value;
			}

			// If the document's left edge has moved into the client area of the viewer, then scroll it back to the left edge. This
			// is possible when a row heading has disappeared, leaving a blank space on the left side of the viewer.
			if (0 > windowRectangle.Left)
				this.controlData.Left = this.controlHeader.Left = this.hScrollBar.Value = 0;

			// When the window is expanded to the right with the document position near the right edge of the viewing area, this
			// will pull the document back into the window to fill in the blank spot that is created.
			if (this.controlData.Width < windowRectangle.Right)
			{
				this.hScrollBar.Value = this.controlData.Width - windowRectangle.Width < 0 ?
					0 : this.controlData.Width - windowRectangle.Width;
				this.controlData.Left = this.controlHeader.Left = -this.hScrollBar.Value;
			}

			// Allow the changes to update the screen now that everything is in position.
			this.panel1.ResumeLayout(false);

		}

		private void controlData_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{

			try
			{

				this.spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

				if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
				{

					Point mouseLocation = new Point(e.X, e.Y);

					int rowIndex = FindRow(e.Y);

					SpreadsheetRow selectedRow = this.spreadsheet.ViewRows[rowIndex];
					SpreadsheetColumn selectedColumn = null;
					SpreadsheetCell selectedCell = null;
					for (int columnIndex = 0; columnIndex < this.spreadsheet.ViewColumns.Count; columnIndex++)
					{
						selectedColumn = this.spreadsheet.ViewColumns[columnIndex];
						selectedCell = selectedRow[selectedColumn];
						if (selectedCell.DisplayRectangle.Contains(mouseLocation))
							break;
					}

					int firstRowIndex = rowIndex;
					int lastRowIndex = rowIndex;
					int firstColumnIndex = selectedColumn.Ordinal;
					int lastColumnIndex = selectedColumn.Ordinal;

					if ((e.Button == MouseButtons.Left && (Control.ModifierKeys & (Keys.Shift | Keys.Control)) == 0) ||
						(e.Button == MouseButtons.Right && !selectedCell.IsSelected))
					{
						ClearSelection();
						this.anchorRow = selectedRow;
						this.anchorColumn = selectedColumn;
						this.selectedRanges.Add(new ArrayList());
					}

					if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
					{
						if (this.anchorRow == null)
						{
							this.anchorRow = selectedRow;
							this.anchorColumn = selectedColumn;
							this.selectedRanges.Add(new ArrayList());
						}
						firstRowIndex = this.anchorRow == null ? rowIndex : rowIndex < this.anchorRow.RowViewIndex ? rowIndex : this.anchorRow.RowViewIndex;
						lastRowIndex = this.anchorRow == null ? rowIndex : rowIndex > this.anchorRow.RowViewIndex ? rowIndex : this.anchorRow.RowViewIndex;
						firstColumnIndex = this.anchorColumn == null ? selectedColumn.Ordinal : selectedColumn.Ordinal < this.anchorColumn.Ordinal ? selectedColumn.Ordinal : this.anchorColumn.Ordinal;
						lastColumnIndex = this.anchorColumn == null ? selectedColumn.Ordinal : selectedColumn.Ordinal > this.anchorColumn.Ordinal ? selectedColumn.Ordinal : this.anchorColumn.Ordinal;

					}

					if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
					{
						if (this.selectedRanges.Count == 1)
							foreach (ArrayList selectedRange in this.selectedRanges)
								foreach (SpreadsheetCell spreadsheetCell in selectedRange)
									this.controlData.Invalidate(Rectangle.Inflate(spreadsheetCell.DisplayRectangle, 2, 2));

						this.anchorRow = selectedRow;
						this.anchorColumn = selectedColumn;

						lastRowIndex = firstRowIndex = rowIndex;
						this.selectedRanges.Add(new ArrayList());

					}
					
					if (this.spreadsheet.SelectionMode == SelectionMode.Row)
					{

						ArrayList arrayList = (ArrayList)this.selectedRanges[this.selectedRanges.Count - 1];

						if (e.Button != MouseButtons.Right)
						{
							foreach (SpreadsheetCell spreadsheetCell in arrayList)
							{
								spreadsheetCell.IsSelected = false;
								this.controlData.Invalidate(Rectangle.Inflate(spreadsheetCell.DisplayRectangle, 2, 2));
							}
							arrayList.Clear();
						}
						
						for (int selectedRowIndex = firstRowIndex; selectedRowIndex <= lastRowIndex; selectedRowIndex++)
						{
							SpreadsheetRow spreadsheetRow = this.spreadsheet.ViewRows[selectedRowIndex];
							foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.ViewColumns)
							{
								spreadsheetRow[spreadsheetColumn].IsSelected = true;
								arrayList.Add(spreadsheetRow[spreadsheetColumn]);
							}

							this.controlData.Invalidate(Rectangle.Inflate(spreadsheetRow.rectangle, 2, 2));

						}

						this.activeRow = selectedRow;
						this.activeColumn = null;

					}

					if (this.spreadsheet.SelectionMode == SelectionMode.Cell)
					{

						ArrayList arrayList = (ArrayList)this.selectedRanges[this.selectedRanges.Count - 1];

						if (this.activeColumn != null)
						{
							SpreadsheetCell activeCell = this.ActiveCell;
							activeCell.IsActiveCell = false;
							this.controlData.Invalidate(Rectangle.Inflate(activeCell.DisplayRectangle, 2, 2));
						}

						if (e.Button != MouseButtons.Right)
						{
							foreach (SpreadsheetCell spreadsheetCell in arrayList)
							{
								SpreadsheetRow spreadsheetRow = this.spreadsheet.ViewRows[this.anchorRow.RowViewIndex];
								SpreadsheetCell anchorCell = spreadsheetRow[this.anchorColumn];
								if (!Object.ReferenceEquals(spreadsheetCell, anchorCell))
									spreadsheetCell.IsSelected = false;
								this.controlData.Invalidate(Rectangle.Inflate(spreadsheetCell.DisplayRectangle, 2, 2));
							}
							arrayList.Clear();
						}

						// This will prevent a cell from being selected more than once.
						for (int selectedRowIndex = firstRowIndex; selectedRowIndex <= lastRowIndex; selectedRowIndex++)
						{

							SpreadsheetRow spreadsheetRow = this.spreadsheet.ViewRows[selectedRowIndex];

							for (int selectedColumnIndex = firstColumnIndex; selectedColumnIndex <= lastColumnIndex; selectedColumnIndex++)
							{

								SpreadsheetCell spreadsheetCell = spreadsheetRow[selectedColumnIndex];

								bool isAlreadySelected = false;
								foreach (ArrayList selectedRange in this.selectedRanges)
									foreach (SpreadsheetCell cell in selectedRange)
										if (Object.ReferenceEquals(spreadsheetCell, cell))
											isAlreadySelected = true;

								if (!isAlreadySelected)
								{

									spreadsheetCell.IsSelected = true;
									arrayList.Add(spreadsheetCell);

									this.controlData.Invalidate(Rectangle.Inflate(spreadsheetCell.DisplayRectangle, 2, 2));

								}
							}

						}

						this.activeRow = selectedRow;
						this.activeColumn = selectedColumn;
						this.activeRow[this.activeColumn].IsActiveCell = true;

					}

					OnSelectionChanged(this);

				}

			}
			catch (Exception exception)
			{

				// Write the exception to the event log.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				this.spreadsheet.Lock.ReleaseReaderLock();

			}

			this.controlData.Select();

		}

		/// <summary>
		/// Starts a drag and rop operation with the header columns.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The mouse event parameters.</param>
		private void controlHeader_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{

			// The only button of interest for drag-and-drop operations is the left mouse.
			if (e.Button != MouseButtons.Left)
				return;

			try
			{

				// Excluded writing on the spreadsheet while the columns in the spreadsheet are searched to find out what kinds of
				// drop operations are valid.
				this.spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

				// This keeps track of where the mouse button was pressed.
				this.mouseDownLocation = new Point(e.X, e.Y);

				// Locate the column that contains the mouse position.  Note that hidden columns are ignored.
				foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.ViewColumns)
				{

					// The columns can either be moved, removed or have their size changed using the mouse.  An area two pixels
					// from each edge is used for dragging the edges to make the columns wider or more narrow.  This rectangle is
					// used for the hit-test for the edge dragging operation.  If the mouse button is pressed inside this edge,
					// then a resizing operation is initiated.
					Rectangle edgeArea = new Rectangle(spreadsheetColumn.Rectangle.Right - 2, 0, 4, this.ClientRectangle.Height);
					if (edgeArea.Contains(this.mouseDownLocation))
					{

						// Have all the mouse movements passed on to the header window until the mouse button is released.  The
						// 'resizeColumn' captures the subject of this operation.
						this.controlHeader.Capture = true;
						this.resizeColumn = spreadsheetColumn;
						break;
					}

					// The user selectes a column movement (or removal) operation by selecting any other part of the column header
					// than the edge.  When this happens, the column header will reverse its color and a special cursor containing
					// the floating column will appear to give a visual cue for the drag-and-drop operation.
					if (spreadsheetColumn.Contains(this.mouseDownLocation))
					{

						this.controlHeader.Capture = true;
						if (this.selectedColumn != null && this.selectedColumn != spreadsheetColumn)
							this.controlHeader.Invalidate(this.selectedColumn.Rectangle);
						this.selectedColumn = spreadsheetColumn;
						if (this.selectedColumn.Table == null)
							Console.WriteLine("Column is not in table.");
						this.controlHeader.Invalidate(this.selectedColumn.Rectangle);
						break;
					}

				}

			}
			catch (Exception exception)
			{

				// Write the exception to the event log.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// End of writer exclusion on the spreadsheet data.
				this.spreadsheet.Lock.ReleaseReaderLock();

			}

		}

		/// <summary>
		/// Handles the movement of the mouse over the header window.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The mouse event parameters.</param>
		private void ControlHeader_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{

			// This is the position of the mouse in client coordinates.
			Point mouseClientLocation = new Point(e.X, e.Y);

			try
			{

				this.spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

				// When the user presses the left mouse button, they initiate some drag operation and the mouse activity is captured by
				// the column header window.  If the user is simply moving the mouse over the window, then feedback is given in the
				// shape of the cursor.
				if (this.controlHeader.Capture)
				{

					// There are three different kinds of operations that the user can perform with the mouse dragging: resizing the
					// column, moving a column or choosing the sort order for the document.  This is the start of the tests to figure
					// out what the user is going based on the mouse commands.  If they are not resizing, then check for column
					// movement or sorting.
					if (this.resizeColumn == null)
					{
					
						// The column cursor comes to life after the user has tried to 'drag' the column out of its location on the
						// header.  If it doesn't exist yet, the user hasn't moved the cursor very far from the original location where
						// the left mouse button was pressed.  If it does exist, then the user is in the process of moving the column.
						if (this.columnCursor == null)
						{

							// This formula determins if the mouse has moved an absolute distance of four pixels from the original 
							// location.  If it has, the user has selected a movement operation for the column.  Otherwise, the mouse
							// operation will be interpreted as a request for a new sort order when the left mouse button is lifted.
							if (Math.Sqrt(Math.Pow(e.X - this.mouseDownLocation.X, 2.0) +
								Math.Pow(e.Y - this.mouseDownLocation.Y, 2.0)) > 4.0)
							{

								// A column movement (or removal) operation is selected when the mouse has moved away from the original
								// location.  The destination cursor is a set of red arrows that straddle the header window.  Note 
								// that it is important that this cursor is created before the column cursor to preserve the Z order.
								// The destination cursor should appear between the column header and the column cursor.
								this.destinationCursor = new DestinationCursor(this.controlHeader.Height);

								// This will create a floating window that contains an image of the selected column.  The effect is to
								// make it appear that the user has ripped the column header off the page and can move it where they
								// please.  The 'ColumnCursor' is basically a floating window with a paintBitmap image of the selected 
								// column header.
								Bitmap selectedBitmap = new Bitmap(this.selectedColumn.Rectangle.Width,
									this.selectedColumn.Rectangle.Height, Graphics.FromHwnd(this.Handle));
								Graphics selectedGraphics = Graphics.FromImage(selectedBitmap);
								selectedGraphics.TranslateTransform(Convert.ToSingle(-this.selectedColumn.Rectangle.X),
									Convert.ToSingle(-this.selectedColumn.Rectangle.Y));
								PaintHeader(selectedGraphics, this.selectedColumn.Rectangle);
								this.columnCursor = new ColumnCursor(selectedBitmap);

								// Now that the column cursor has been created, it needs to be located on the screen with respect to
								// the current mouse position.  Remember that this window doesn't have a parent, so all coordinates 
								// must be converted to screen coordinates.
								this.columnCursor.Location = this.controlHeader.PointToScreen(mouseClientLocation);

							}

						}
						else
						{

							// The cursor column is really a floating window, not a cursor.  It needs to be moved to match the location
							// of the mouse.  Note that the floating window doesn't have a parent, so the coordinates are in screen
							// units.
							this.columnCursor.Location = this.controlHeader.PointToScreen(mouseClientLocation);

							// The mouse operation can only 'drop' something in the visible part of the column header bar.  This 
							// operation will clip the header with the enclosing panel to determine the bounds of the visible part of
							// the header.
							Rectangle parentScreen = this.panel1.RectangleToScreen(this.panel1.ClientRectangle);
							Rectangle parentClient = this.controlHeader.RectangleToClient(parentScreen);
							Rectangle visibleHeader = Rectangle.Intersect(parentClient, this.controlHeader.ClientRectangle);

							// These members will hold the state of the destination (selected or not) and the column header selected as
							// the destination.
							this.isDestinationSelected = false;
							this.destinationColumn = null;

							// A target column for the drag-and-drop operation can be selected if the mouse is inside the visible 
							// header.  Otherwise the user gets a big 'X' to show then that dropping will result in the loss of the
							// column from the viewer.
							if (visibleHeader.Contains(mouseClientLocation))
							{

								// Any operation inside the visible header gets the basic pointing arrow for a cursor.
								this.controlHeader.Cursor = Cursors.Arrow;

								// This attempts to find a destination for the column operation.
								foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.ViewColumns)
								{

									// If the mouse is over the right half of the previous column, then the current column in the loop
									// is the actual destination.  See below for the test that sets this flag.
									if (this.isDestinationSelected)
									{
										this.destinationColumn = spreadsheetColumn;
										break;
									}

									// A destination is selected if the left edge of the target column is visible and the left half of
									// the column header contains the current mouse location.  Not also that the selected column header
									// is never allowed to be its own destination.
									Rectangle testArea = spreadsheetColumn.Rectangle;
									testArea.Width /= 2;
									if (visibleHeader.Left <= spreadsheetColumn.Rectangle.Left && testArea.Contains(mouseClientLocation) &&
										this.selectedColumn.Rectangle.Left != spreadsheetColumn.Rectangle.Left &&
										this.selectedColumn.Rectangle.Right != spreadsheetColumn.Rectangle.Left)
									{
										this.isDestinationSelected = true;
										this.destinationColumn = spreadsheetColumn;
										break;
									}

									// This will test the right half of each of the colum headers.  If the cursor is over the right 
									// half and the rightmost part of the destination is visible, then it can be a destination.  Note
									// again that the selected column can't be used as a destination.  If this test finds a valid
									// destination, the next pass through the loop will assign the actual destination column (or it
									// will be null if this is the last column).
									testArea.X += testArea.Width;
									testArea.Width = spreadsheetColumn.Rectangle.Right - testArea.X;
									if (visibleHeader.Right >= spreadsheetColumn.Rectangle.Right && testArea.Contains(mouseClientLocation) &&
										this.selectedColumn.Rectangle.Left != spreadsheetColumn.Rectangle.Right &&
										this.selectedColumn.Rectangle.Right != spreadsheetColumn.Rectangle.Right)
										this.isDestinationSelected = true;

								}

							}
							else
							{

								// If the mouse isn't over the column header window, a big 'X' give the user feedback that the column 
								// will be dropped from the viewer if they release the mouse button.
								this.controlHeader.Cursor = this.bigEx;

							}

							// If a valid destination was found in the search above, move the set of red arrows over the exact spot
							// where the column will be moved.
							if (this.isDestinationSelected)
							{

								// The destination cursor is a set of red arrows that point to where the column will be once it's 
								// dropped.  Locate the cursor over the appropriate column.  If the rightmost point was selected, then
								// there won't be a selected column and the location is fudged up with the width of the header window.
								this.destinationCursor.Location = this.controlHeader.PointToScreen(this.destinationColumn == null ?
									new Point(this.controlHeader.Width, 0) : this.destinationColumn.Rectangle.Location);

								// Make the cursor window visible if it isn't already.  One may wonder why the cursor widow is made 
								// visible and invisible instead of being created and destroyed.  This is done to maintain the Z order
								// of the selection cursor to be above the spreadsheet control, but below the column cursor.
								if (!this.destinationCursor.Visible)
									this.destinationCursor.Visible = true;

							}
							else
							{

								// Hide the destination cursor when there is no valid destination.
								if (this.destinationCursor.Visible)
									this.destinationCursor.Visible = false;

							}

						}

					}
					else
					{

						// A cursor is drawn from the top of the panel to the bottom when resizing.  In order to have the line not
						// destroy the screen underneath the line as it moves, a special 'Reversible' line is used.  The trick to
						// making this line disappear is to draw it twice.  The first time compliments all the bits underneath it, the
						// second time compliments them back and restores the original state.  This first line will clear out the
						// previous line if it was drawn.
						if (lastLine != null)
							ControlPaint.DrawReversibleLine(lastLine[0], lastLine[1], System.Drawing.Color.DarkGray);
						else
							lastLine = new Point[2];

						// This gives the user feedback regarding the location of the new right edge of the selected column.  It draws
						// a vertical line over the client area at the current horizontal position of the mouse.
						lastLine[0] = this.controlHeader.PointToScreen(new Point(e.X, this.controlHeader.Top));
						lastLine[1] = this.controlHeader.PointToScreen(new Point(e.X, this.hScrollBar.Visible ? this.hScrollBar.Top :
							panel1.Bottom));
						ControlPaint.DrawReversibleLine(lastLine[0], lastLine[1], System.Drawing.Color.DarkGray);

					}

				}
				else
				{

					// This will determine which cursor should be used: a vertical size cursor or a regular arrow cursor.  If the mouse
					// is over the right or left edge of the column, then the vertical resizing cursor is used.
					bool isResizingColumn = false;
					foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.ViewColumns)
					{
						Rectangle edgeArea = new Rectangle(spreadsheetColumn.Rectangle.Right - 2, 0, 4, this.ClientRectangle.Height);
						if (edgeArea.Contains(mouseClientLocation))
						{
							isResizingColumn = true;
							break;
						}
					}

					// Select the resizing cursor when the mouse is over the edge of the column header.
					this.controlHeader.Cursor = isResizingColumn ? Cursors.VSplit : Cursors.Arrow;

				}

			}
			catch (Exception exception)
			{

				// Write the exception to the event log.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				this.spreadsheet.Lock.ReleaseReaderLock();

			}

		}

		/// <summary>
		/// Handles the 'drop' operation for the column headers.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The mouse event parameters.</param>
		private void controlHeader_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{

			// This is the position of the mouse in client coordinates.
			Point mouseClientLocation = new Point(e.X, e.Y);

			bool isModified = false;

			try
			{

				this.spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

				// It's possible for the stylesheet to refresh while the drag-and-drop operation is underway.  To prevent a crash,
				// the selected column is re-selected here using the name of the column that was selected as a key into the column
				// table to find the current column (with the same name).
				if (this.selectedColumn != null)
					this.selectedColumn = this.spreadsheet.ViewColumns[this.selectedColumn.ColumnName];
				if (this.destinationColumn != null)
					this.destinationColumn = this.spreadsheet.ViewColumns[this.destinationColumn.ColumnName];
				
				// This will complete the resizing operation.
				if (this.resizeColumn != null)
				{

					// If the reversible line that is used as a cursor for the new column position needs to be removed, draw it one
					// last time.  This has the effect of re-complimenting the bits underneath, which leaves the screen with the
					// original image.  Note that the 'lastLine' needs to be cleared out so it's initialized for the next drag-and-drop
					// operation.
					if (this.lastLine != null)
					{
						ControlPaint.DrawReversibleLine(this.lastLine[0], this.lastLine[1], System.Drawing.Color.DarkGray);
						this.lastLine = null;
					}
				
					// This calculates the size of the new column.  Note that the mouse action allows for a negative column width, but
					// that isn't allowed in the stylesheet.  Instead, it's interpreted as an instruction to hide the column.  Once the
					// stylesheet has been modified, the document is redrawn with the new stylesheet.
					Stylesheet.ColumnNode column = this.stylesheet.Table.Columns.Find(this.resizeColumn.ColumnName);
					column.Width = (float)(e.X - this.resizeColumn.Rectangle.X < 0 ? 0.0M : (e.X - this.resizeColumn.Rectangle.X));

					// Modify the spreadsheet.
					SpreadsheetColumn spreadsheetColumn = this.spreadsheet.ViewColumns[this.resizeColumn.ColumnName];
					Rectangle displayRectangle = spreadsheetColumn.Rectangle;
					displayRectangle.Width = e.X - this.resizeColumn.Rectangle.X;
					spreadsheetColumn.Rectangle = displayRectangle;

					isModified = true;

					// Clearing out the resize column initalizes the control for the next drag-and-drop operation.
					this.resizeColumn = null;

				}

				// A column cursor indicates either a movement or removal of a column.
				if (this.columnCursor != null)
				{

					// A valid destination indicates that columns are to be moved.
					if (this.isDestinationSelected)
					{

						// Move the columns.  Note that when a column is moved to the rightmost position, there's no column to reference,
						// so the total number of columns is used as the position.  Once the stylesheet has been modified, the entire
						// document is redrawn with the new stylesheet.
						int sourceIndex = this.spreadsheet.ViewColumns.IndexOf(selectedColumn);
						int destinationIndex = this.destinationColumn == null ? this.spreadsheet.ViewColumns.Count - 1 :
							this.spreadsheet.ViewColumns.IndexOf(destinationColumn);

						this.stylesheet.Table.Columns.Move(sourceIndex, destinationIndex);
						this.spreadsheet.ViewColumns.Move(sourceIndex, destinationIndex);

						// Release the set of red arrows used to indicate the destination position.
						this.destinationCursor.Dispose();

						// This will reset the state of the viewer for the next drag-and-drop operation.
						this.isDestinationSelected = false;
						this.destinationColumn = null;
						this.destinationCursor = null;

						isModified = true;
						
					}
					else
					{

						// HACK - This section should contain code to remove the column from the view in the stylesheet.

						// Remove the column from the spreadsheet.
						SpreadsheetColumn spreadsheetColumn = this.spreadsheet.ViewColumns[this.selectedColumn.ColumnName];
						this.spreadsheet.ViewColumns.Remove(spreadsheetColumn);

						isModified = true;
				
					}

					// Reset the state of the column header window for the next operation.
					this.columnCursor.Dispose();
					this.columnCursor = null;

				}
				else
				{

					// Sorting the viewer by the columns is accomplished by pressing a column heading and not moving it.  In this 
					// scenario, a column will be selected bu not column heading cursor will have been created.
					if (this.selectedColumn != null)
					{

						// If the column is already part of the sort key, then the sort order will be inverted.  Otherwise the default
						// sort direction is ascending.  If the shift key is held down, the columns are added together to form the sort
						// order for the viewer.  Otherwise, only the selected column is used.
						Stylesheet.SortColumnNode sortColumn = this.stylesheet.Table.Sort.Find(this.selectedColumn.ColumnName);
						SortDirection sortDirection = sortColumn == null ? SortDirection.Ascending :
							sortColumn.Direction == SortDirection.Ascending ? SortDirection.Descending :
							SortDirection.Ascending;
						if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
							this.stylesheet.AppendView(this.selectedColumn.ColumnName, sortDirection);
						else
							this.stylesheet.SetView(this.selectedColumn.ColumnName, sortDirection);

						if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
							this.spreadsheet.AppendView(this.selectedColumn.ColumnName, sortDirection);
						else
							this.spreadsheet.SetView(this.selectedColumn.ColumnName, sortDirection);

						isModified = true;

					}

				}

			}
			catch (Exception exception)
			{

				// Write the error out to the log.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				this.spreadsheet.Lock.ReleaseReaderLock();

			}

			if (isModified)
			{
				OnStylesheetChanged(this);
				spreadsheet.Refresh();
			}
			
			// This will reset the state of the column header for the next operation.
			this.selectedColumn = null;
			this.controlHeader.Capture = false;
			this.controlHeader.Cursor = Cursors.Default;

		}

		private void spreadsheet_HeaderInvalidated(object sender, InvalidRegionEventArgs invalidRegionEventArgs)
		{

			if (this.IsOpen)
			{
				if (this.InvokeRequired)
					BeginInvoke(this.invalidHeaderHandler, new object[] { invalidRegionEventArgs.Region });
				else
					HeaderInvalidatedHandler(invalidRegionEventArgs.Region);
			}

		}

		private void spreadsheet_DataInvalidated(object sender, InvalidRegionEventArgs invalidRegionEventArgs)
		{

			if (this.IsOpen)
			{
				if (this.InvokeRequired)
					BeginInvoke(this.invalidDataHandler, new object[] { invalidRegionEventArgs.Region });
				else
					DataInvalidatedHandler(invalidRegionEventArgs.Region);
			}

		}

		private void spreadsheet_DataSizeChanged(object sender, SizeEventArgs sizeEventArgs)
		{

			if (this.IsOpen)
			{
				if (this.InvokeRequired)
					BeginInvoke(this.sizeDataHandler, new object[] { sizeEventArgs.Size });
				else
					DataSizeChangedHandler(sizeEventArgs.Size);
			}

		}

		private void spreadsheet_HeaderSizeChanged(object sender, SizeEventArgs sizeEventArgs)
		{

			if (this.IsOpen)
			{
				if (this.InvokeRequired)
					BeginInvoke(this.sizeHeaderHandler, new object[] { sizeEventArgs.Size });
				else
					HeaderSizeChangedHandler(sizeEventArgs.Size);
			}

		}

		private void panel1_Click(object sender, System.EventArgs e)
		{
			ClearSelection();
		}

		private void childViewer_Click(object sender, System.EventArgs e)
		{

			OnClick(e);

		}

		private void childViewer_DoubleClick(object sender, System.EventArgs e)
		{

			OnDoubleClick(e);

		}

		/// <summary>
		/// Print the document.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		public void Print()
		{

			try
			{

				// This will prompt the user with a standard 'Print' dialog.  The 'try/catch' logic just makes sure that any
				// spurious problems don't kill the application.  Ideally, the print should never crash, but as of this writing,
				// its still a possiblity.  Note that the 'Print' method doesn't actually print the document.  It sets up some sort
				// of graphics context for the printer and begins a process of calling back to the viewer to print each page.
				this.printDialog.AllowSomePages = false;
				if (this.printDialog.ShowDialog() == DialogResult.OK)
					this.printDocument.Print();

			}
			catch {}

		}

		public void PrintPreview()
		{

			// This dialog will call back into the viewer to paint the document to a screen device context that mirrors the
			// printing units of a piece of paper.
			this.printPreviewDialog.ShowDialog();

		}

		public void PageSetup()
		{

			// This will modify the 'PrintDocument' structure that is shared by the 'Print' and the 'PrintPreview' functions.
			this.pageSetupDialog.ShowDialog();

		}

		private void Navigate(NavigationCommand navigationCommand)
		{

			int viewColumnIndex;
			int viewRowIndex;
			SpreadsheetColumn targetColumn;
			SpreadsheetRow targetRow;

			try
			{

				this.spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

				targetColumn = this.activeColumn;
				targetRow = this.activeRow;

				switch (navigationCommand)
				{
				case NavigationCommand.Left:

					if (this.spreadsheet.SelectionMode == SelectionMode.Cell)
					{
						viewColumnIndex = this.activeColumn.ColumnViewIndex;
						if (viewColumnIndex != 0)
							targetColumn = this.spreadsheet.ViewColumns[viewColumnIndex - 1];
					}
					break;

				case NavigationCommand.Right:

					if (this.spreadsheet.SelectionMode == SelectionMode.Cell)
					{
						viewColumnIndex = this.activeColumn.ColumnViewIndex;
						if (viewColumnIndex != this.spreadsheet.ViewColumns.Count - 1)
							targetColumn = this.spreadsheet.ViewColumns[viewColumnIndex + 1];
					}
					break;

				case NavigationCommand.Up:

					viewRowIndex = this.activeRow.RowViewIndex;
					if (viewRowIndex != 0)
						targetRow = this.spreadsheet.ViewRows[viewRowIndex - 1];
					break;

				case NavigationCommand.Down:

					viewRowIndex = this.activeRow.RowViewIndex;
					if (viewRowIndex != this.spreadsheet.ViewRows.Count - 1)
						targetRow = this.spreadsheet.ViewRows[viewRowIndex + 1];
					break;

				}

				if (this.activeRow != targetRow || this.activeColumn != targetColumn)
				{

					bool selectionChanged = this.activeRow != targetRow;

					this.ClearSelection();

					ArrayList range = new ArrayList();
					this.selectedRanges.Add(range);
						
					this.activeRow = targetRow;
					this.anchorRow = targetRow;

					if (this.spreadsheet.SelectionMode == SelectionMode.Cell)
					{
						this.activeColumn = targetColumn;
						this.anchorColumn = targetColumn;
						SpreadsheetCell activeCell = targetRow[targetColumn];
						activeCell.IsSelected = true;
						activeCell.IsActiveCell = true;
						range.Add(activeCell);
						this.controlData.Invalidate(Rectangle.Inflate(activeCell.DisplayRectangle, 2, 2));
					}
					else
					{
						foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.ViewColumns)
						{
							SpreadsheetCell spreadsheetCell = this.activeRow[spreadsheetColumn];
							spreadsheetCell.IsSelected = true;
							range.Add(spreadsheetCell);
							this.controlData.Invalidate(Rectangle.Inflate(spreadsheetCell.DisplayRectangle, 2, 2));
						}

					}

					if (selectionChanged)
						OnSelectionChanged(this);

				}

			}
			finally
			{

				this.spreadsheet.Lock.ReleaseReaderLock();

			}

		}

		protected override bool ProcessDialogKey(Keys keyData)
		{

			try
			{

				switch (keyData)
				{
				case Keys.Up: Navigate(NavigationCommand.Up); break;
				case Keys.Down: Navigate(NavigationCommand.Down); break;
				case Keys.Right: Navigate(NavigationCommand.Right); break;
				case Keys.Left: Navigate(NavigationCommand.Left); break;
				default: return base.ProcessDialogKey(keyData);

				}
			}
			catch (Exception exception)
			{

				// Log the error.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}

			return true;

		}

		/// <summary>
		/// Raises the 'SelectionChanged' event.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		protected virtual void OnSelectionChanged(object sender)
		{

			// This will notify any listeners that the stylesheet has been modified.  Generally handling this event will involve 
			// saving the modified stylesheet to some persistent store so the changes can be used the next time it is used.
			if (this.SelectionChanged != null)
				this.SelectionChanged(sender, EventArgs.Empty);

		}

		/// <summary>
		/// Compiles the modified stylesheet can notifies any listeners that the stylesheet has been changed.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		protected virtual void OnStylesheetChanged(object sender)
		{

			// Whenever the stylesheet has been modified, it needs to be recompiled.
			LoadTransform();

			// This will notify any listeners that the stylesheet has been modified.  Generally handling this event will involve 
			// saving the modified stylesheet to some persistent store so the changes can be used the next time it is used.
			if (this.StylesheetChanged != null)
				this.StylesheetChanged(sender, EventArgs.Empty);

		}

		/// <summary>
		/// Handles a change to the stylesheet.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void Stylesheet_Changed(object sender, EventArgs e)
		{

			// This will daisy-chain the events from the internal Stylesheet structure up to the Spreadsheet Viewer level.
			OnStylesheetChanged(sender);

		}

		/// <summary>
		/// Handles a key being pressed while the Data area of the spreadsheet control has the focus.
		/// </summary>
		/// <param name="sender">The object that originated the call.</param>
		/// <param name="e">The event arguments.</param>
		private void controlData_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{

			// Printable characters are used to invoke the Edit boxes.  All other characters are passed on to the default handlers.
			if (Char.IsLetterOrDigit(e.KeyChar) || Char.IsPunctuation(e.KeyChar))
			{

				// If the cell is locked, give the user a polite beep.
				if (this.ActiveCell.Style.IsProtected)
					Sounds.PlaySound(Sounds.MB_ICONASTERISK);
				else
				{

					// Overlay the cell with one of the customized windows for accepting input.  This gives the user much more
					// control over the input window, particularly if a complete document refresh comes in while they are editing.
					// These controls also allow us to add features such as 'AutoComplete' and drop-down ComboBoxes.
					OnStartEdit(new StartEditArgs(false, false, e.KeyChar.ToString()));
					e.Handled = true;

				}

			}

		}

	}

}
