namespace MarkThree.Forms
{

	using MarkThree;
	using System;
	using System.Reflection;
	using System.CodeDom;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Drawing.Drawing2D;
	using System.Drawing.Text;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;
	using System.Xml;

	/// <summary>
	/// Manages the drawing operations for a document viewer.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public partial class DocumentViewer : MarkThree.Forms.Viewer
	{

		// Constants
		private const int commandBufferSize = 50000;
		private const int maxThreadWait = 500;
		private const int animationPeriod = 1000;
		private const int warningLevel = 4;
		private const float selectionOuterBorder = 1.0f;
		private const float selectionInnerBorder = 2.0f;
		private const float selectionInterior = 3.0f;
		private const float headerDragTrigger = 4.0f;
		private const float splitBorder = 8.0f;
		private const string compilerOptions = "/optimize+ /warnaserror+";
		private const bool generateExecutable = false;
		private const bool generateInMemory = true;
		private const bool includeDebugInformation = false;

		// Enumerations
		private enum MouseState { ButtonUp, ButtonDown, DraggingColumn, DraggingTile, ResizingColumn };
		private enum DropAction { NoAction, Select, Delete };

		// Private Members
		private MarkThree.Forms.ColumnCursor columnCursor;
		private MarkThree.Forms.DataTransform dataTransform;
		private MarkThree.Forms.DestinationCursor destinationCursor;
		private MarkThree.Forms.DocumentViewer.DropAction destinationState;
		private MarkThree.Forms.DocumentViewer.MouseEventDelegate[] mouseDownHandler;
		private MarkThree.Forms.DocumentViewer.MouseEventDelegate[] mouseMoveHandler;
		private MarkThree.Forms.DocumentViewer.MouseEventDelegate[] mouseUpHandler;
		private MarkThree.Forms.DocumentViewer.MouseState mouseState;
		private MarkThree.Forms.DocumentViewer.PaintDelegate[] paintHandler;
		private MarkThree.Forms.TileDepthComparer viewerTileDepthComparer;
		private MarkThree.Forms.TileLocationComparer viewerTileLocationComparer;
		private MarkThree.Forms.ViewerTile selectedColumnTile;
		private System.Boolean isBackgroundThreadRunning;
		private System.CodeDom.Compiler.CodeDomProvider codeDomProvider;
		private System.CodeDom.Compiler.CodeGeneratorOptions codeGeneratorOptions;
		private System.Collections.Generic.Dictionary<int, Tile> tileTable;
		private System.Collections.Generic.Dictionary<string, Style> styleTable;
		private System.Collections.Generic.Dictionary<string, ViewerStyle> viewerStyleTable;
		private System.Collections.Generic.Dictionary<int, ViewerTile> columnViewerTileMap;
		private System.Collections.Generic.Dictionary<DataRow, Dictionary<int, ViewerTile>> rowColumnMap;
		private System.Collections.Generic.List<ViewerTile> viewerTileList;
		private System.Collections.Generic.List<ViewerStyle> viewerStyleList;
		private System.Collections.Generic.List<AnimationSequence> animationList;
		private System.Data.DataTable constantTable;
		private System.Drawing.Brush selectedInteriorBrush;
		private System.Drawing.Brush selectedHeaderBrush;
		private System.Drawing.Imaging.ImageAttributes invertedColorAttributes;
		private System.Drawing.PointF destinationLocation;
		private System.Drawing.Point[] lastLine;
		private System.Drawing.PointF mouseDownLocation;
		private System.Drawing.PointF virtualOffset;
		private System.Drawing.RectangleF rectangleF;
		private System.Drawing.RectangleF[] quadrants = new RectangleF[4];
		private System.Drawing.PointF[] offsets = new PointF[4];
		private System.Drawing.PointF anchorPoint;
		private System.Drawing.SizeF splitSize;
		private System.Int32 viewerTileCounter;
		private System.Int32 mouseDownQuadrant;
		private System.Single scaleFactor;
		private System.Single resizeEdge;
		private System.Threading.Thread backgroundThread;
		private System.Threading.Timer animationTimer;
		private System.Windows.Forms.Cursor bigEx;
		private System.Windows.Forms.Cursor selectColumn;
		private System.Windows.Forms.Cursor selectRow;
		private System.Windows.Forms.Cursor verticalSplit;
		private System.Windows.Forms.Cursor horizontalSplit;

		// Private Delegates
		private delegate void InvalidRegionDelegate(List<RectangleF> updateList);
		private delegate void ResizeDelegate(RectangleF previousRectangle);
		private delegate void PaintDelegate(Graphics graphics, Bitmap bitmap, RectangleF clipArea);
		private delegate void MouseEventDelegate(QuadrantMouseEventArgs mouseEventArgs);

		// Protected Members
		protected MarkThree.Forms.DocumentView documentView;
		protected System.Threading.ReaderWriterLock displayLock;
		protected System.Threading.ReaderWriterLock documentLock;

		// Public Members
		public readonly MarkThree.WaitQueue<ViewerCommand> ViewerCommandQueue;
		public readonly System.Data.DataRow ConstantRow;
		public readonly System.Collections.Generic.Dictionary<string, object> Variables;

		/// <summary>
		/// Creates a manager for the drawing operations of a document viewer.
		/// </summary>
		public DocumentViewer()
		{

			// Initialize the designer supported components.
			InitializeComponent();

			this.mouseState = MouseState.ButtonUp;
			this.destinationState = DropAction.NoAction;

			// Initialize the object.
			this.viewerTileCounter = 0;
			this.displayLock = new ReaderWriterLock();
			this.documentLock = new ReaderWriterLock();
			this.ViewerCommandQueue = new WaitQueue<ViewerCommand>();
			this.tileTable = new Dictionary<int, Tile>();
			this.viewerTileDepthComparer = new TileDepthComparer();
			this.viewerTileLocationComparer = new TileLocationComparer();
			this.styleTable = new Dictionary<string, Style>();
			this.viewerStyleTable = new Dictionary<string, ViewerStyle>();
			this.dataTransform = new DataTransform();
			this.columnViewerTileMap = new Dictionary<int, ViewerTile>();
			this.rowColumnMap = new Dictionary<DataRow, Dictionary<int, ViewerTile>>();
			this.viewerTileList = new List<ViewerTile>();
			this.viewerStyleList = new List<ViewerStyle>();
			this.animationList = new List<AnimationSequence>();
			this.Variables = new Dictionary<string, object>();
			this.constantTable = new DataTable();
			this.selectedInteriorBrush = new SolidBrush(System.Drawing.Color.FromArgb(82, 178, 178, 206));
			this.selectedHeaderBrush = new SolidBrush(System.Drawing.Color.FromArgb(255, 255, 195, 107));
			this.ConstantRow = this.constantTable.NewRow();

			// Initial screen metrics.
			this.splitSize = SizeF.Empty;
			this.scaleFactor = DefaultDocument.ScaleFactor;
			this.virtualOffset = PointF.Empty;

			// These delegates are used to select a paint routine for each of the four quadrants.
			this.paintHandler = new PaintDelegate[4];
			this.paintHandler[0] = PaintHeaders;
			this.paintHandler[1] = PaintHeaders;
			this.paintHandler[2] = PaintHeaders;
			this.paintHandler[3] = PaintTiles;

			// This is used to capture the quadrant where a drag operation was started.
			this.mouseDownQuadrant = int.MinValue;

			// This array will handle the "Mouse Down" event for each of the four quadrants.
			this.mouseDownHandler = new MouseEventDelegate[4];
			this.mouseDownHandler[0] = OnHeaderMouseDown;
			this.mouseDownHandler[1] = OnColumnHeaderMouseDown;
			this.mouseDownHandler[2] = OnRowHeaderMouseDown;
			this.mouseDownHandler[3] = OnTileMouseDown;

			// This array will handle the "Mouse Move" event for each of the four quadrants.
			this.mouseMoveHandler = new MouseEventDelegate[4];
			this.mouseMoveHandler[0] = OnHeaderMouseMove;
			this.mouseMoveHandler[1] = OnColumnHeaderMouseMove;
			this.mouseMoveHandler[2] = OnRowHeaderMouseMove;
			this.mouseMoveHandler[3] = OnTileMouseMove;

			// This array will handle the "Mouse Up" event for each of the four quadrants.
			this.mouseUpHandler = new MouseEventDelegate[4];
			this.mouseUpHandler[0] = OnHeaderMouseUp;
			this.mouseUpHandler[1] = OnColumnHeaderMouseUp;
			this.mouseUpHandler[2] = OnRowHeaderMouseUp;
			this.mouseUpHandler[3] = OnTileMouseUp;

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

			// These are the standard options for generating the CodeDOM modules from the DataTransform.
			this.codeDomProvider = CodeDomProvider.CreateProvider("C#");
			this.codeGeneratorOptions = new CodeGeneratorOptions();
			this.codeGeneratorOptions.BlankLinesBetweenMembers = false;
			this.codeGeneratorOptions.BracingStyle = "C";
			this.codeGeneratorOptions.IndentString = "\t";

			// This is the big, honkin' X that is used to indicate that if you drop and object during a drag-and-drop operation, it
			// will disappear.
			this.bigEx = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("MarkThree.Forms.BigEx.cur"));
			this.selectColumn = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("MarkThree.Forms.SelectColumn.cur"));
			this.selectRow = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("MarkThree.Forms.SelectRow.cur"));
			this.verticalSplit = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("MarkThree.Forms.VerticalSplit.cur"));
			this.horizontalSplit = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("MarkThree.Forms.HorizontalSplit.cur"));

		}

		/// <summary>
		/// Handles the creation of the window handle.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnHandleCreated(EventArgs e)
		{

			// Allow the base class to handle the event.
			base.OnHandleCreated(e);

			// This thread will read the queue of commands and execute the proper handler for the viewer objectes that are to be
			// processed.  It's active as long as there's a window to which to write.
			this.backgroundThread = new Thread(new ThreadStart(BackgroundThread));
			this.backgroundThread.Name = "ViewerPipeline";
			this.backgroundThread.Start();

			// This will start the timer that provides the animation in the tiles.
			this.animationTimer = new System.Threading.Timer(AnimationThread, null, 0, DocumentViewer.animationPeriod);

			// Lanch a background thread which will reload the description of the document format into memory.
			if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
				ThreadPool.QueueUserWorkItem(new WaitCallback(LoadView));

		}

		/// <summary>
		/// Handles the destruction of a window and its resources.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnHandleDestroyed(EventArgs e)
		{

			// If the thread is still running, then attempt to kill it gracefully by clearing the flag used in the thread to keep
			// looping.  If it can't be killed this way then abort it.
			if (this.IsBackgroundThreadRunning)
			{
				this.IsBackgroundThreadRunning = false;
				if (!this.backgroundThread.Join(DocumentViewer.maxThreadWait))
					this.backgroundThread.Abort();
			}

			// The animation timer is no longer needed.
			this.animationTimer.Dispose();

			// Allow the base class to complete the destruction of the window.
			base.OnHandleDestroyed(e);

		}

		/// <summary>
		/// The DataTransform describes how data is converted into a document that can be seen in this viewer.
		/// </summary>
		public DataTransform DataTransform
		{

			get
			{

				try
				{

					// The transform is considered part of the critical data that makes up the document part of the viewer and is
					// locked to prevent corruption in a multithreaded environment.
					this.documentLock.AcquireReaderLock(Timeout.Infinite);
					return this.dataTransform.Clone();

				}
				finally
				{

					// Other threads can now access the document parameters.
					this.documentLock.ReleaseReaderLock();

				}
			
			}

			set
			{

				try
				{

					// The transform is considered part of the critical data that makes up the document part of the viewer and is
					// locked to prevent corruption in a multithreaded environment.
					this.documentLock.AcquireWriterLock(Timeout.Infinite);
					this.dataTransform = value;

				}
				finally
				{

					// Other threads can now access the document parameters.
					this.documentLock.ReleaseWriterLock();

				}

			}

		}

		/// <summary>
		/// Gets or sets a variable which controls the execution of the background thread.
		/// </summary>
		private bool IsBackgroundThreadRunning
		{
			get { lock (this) return this.isBackgroundThreadRunning; }
			set { lock (this) this.isBackgroundThreadRunning = value; }
		}

		/// <summary>
		/// The scale (magnification) factor.
		/// </summary>
		public float ScaleFactor
		{

			get
			{

				try
				{

					// This will insure that the value is thread-safe.
					this.displayLock.AcquireReaderLock(Timeout.Infinite);
					return this.scaleFactor;

				}
				finally
				{

					// Allow other threads to set or view the scale factor.
					this.displayLock.ReleaseReaderLock();
				}
			
			}

			set
			{

				try
				{

					// This operation will modify the parameters that control the viewer.  The split size and offsets are used by
					// the background threads and must be locked to prevent corruption.
					this.displayLock.AcquireWriterLock(Timeout.Infinite);

					// This will inhibit the screen from attempting to redraw when the split hasn't changed.
					if (this.scaleFactor != value)
					{

						// The next time the screen is drawn, the magification will be changed.
						this.scaleFactor = value;

						// Now that the split has been changed, re-measure all the screen values and update the scroll bars to reflect the
						// new settings.
						Recalibrate();

						// Redraw the entire window after setting the splits.
						Invalidate();

					}

				}
				finally
				{

					// The display metrics can now be examined and modified by other threads.
					this.displayLock.ReleaseWriterLock();

				}
		
			}

		}

		/// <summary>
		/// The location of the top and right header area.
		/// </summary>
		public SizeF SplitSize
		{

			get 
			{

				try
				{

					// This will insure that the value is thread-safe.
					this.displayLock.AcquireReaderLock(Timeout.Infinite);
					return this.splitSize;

				}
				finally
				{

					// Allow other threads to set or view the split size.
					this.displayLock.ReleaseReaderLock();
				}

			}
			
			set
			{

				try
				{

					// This operation will modify the parameters that control the viewer.  The split size and offsets are used by
					// the background threads and must be locked to prevent corruption.
					this.displayLock.AcquireWriterLock(Timeout.Infinite);

					// This will inhibit the screen from attempting to redraw when the split hasn't changed.
					if (this.splitSize != value)
					{

						// The absolute position of the scroll bars is maintained when splitting and unsplitting.  This will 
						// calculate the current offsets into the document in absolute document coordinates.  Then the new document
						// offsets for the scroll bars are calcualted from the absolute position and the new location of the
						// headers.
						PointF absoluteOffset = new PointF(this.virtualOffset.X - this.splitSize.Width,
							this.virtualOffset.Y - this.splitSize.Height);
						this.splitSize = value;
						this.virtualOffset = new PointF(absoluteOffset.X + this.splitSize.Width,
							absoluteOffset.Y + this.splitSize.Height);

						// Now that the split has been changed, re-measure all the screen values and update the scroll bars to reflect the
						// new settings.
						Recalibrate();

						// Redraw the entire window after setting the splits.
						Invalidate();

					}

				}
				finally
				{

					// The display metrics can now be examined and modified by other threads.
					this.displayLock.ReleaseWriterLock();

				}

			}

		}

		/// <summary>
		/// Clear the viewer of all data.
		/// </summary>
		public void Clear()
		{

			try
			{

				// This insures no other threads access the data while it is cleared.
				this.displayLock.AcquireWriterLock(Timeout.Infinite);

				// This will reset the drawing surface.
				this.rectangleF = Rectangle.Empty;
				this.splitSize = SizeF.Empty;
				this.scaleFactor = DefaultDocument.ScaleFactor;
				this.styleTable.Clear();
				this.columnViewerTileMap.Clear();
				this.rowColumnMap.Clear();
				this.tileTable.Clear();

			}
			finally
			{

				// The viewer data can be accessed again.
				this.displayLock.ReleaseWriterLock();

			}

		}

		/// <summary>
		/// Paints the background of the window control.
		/// </summary>
		/// <param name="e">Paint event arguments.</param>
		protected override void OnPaintBackground(PaintEventArgs e)
		{

			// Don't allow the default action to clear the surface.  The default action is to clear the graphics context to the
			// Background color.  This will result in a choppy display.

		}

		/// <summary>
		/// Paints the window control.
		/// </summary>
		/// <param name="e">Paint event arguments.</param>
		protected override void OnPaint(PaintEventArgs e)
		{

			// I don't know why the GDI+ is invoking the Paint method when there is nothing to be redrawn, but it screws up the
			// creation of the bitmap on which the invalid area is drawn.  This will check for an empty drawing area and return if
			// there's nothing to draw.
			if (e.ClipRectangle == Rectangle.Empty)
				return;

			// The document is managed in world coordinates, however, the paint event describes an area to be painted in device
			// coordinates using the clip rectangle.  This operation will create a clip rectangle in world coordinates that
			// describes the areas of the document that must be redrawn.
			RectangleF worldClip = new RectangleF((float)e.ClipRectangle.X / this.scaleFactor,
				(float)e.ClipRectangle.Y / this.scaleFactor, (float)e.ClipRectangle.Width / this.scaleFactor,
				(float)e.ClipRectangle.Height / this.scaleFactor);

			//  The screen is divided up into quadrants for the purpose of scrolling and column/row headings.  Similar to the Excel
			// feature, the top and left side of the screen can be frozen.  When the rest of the document is scrolled, the frozen
			// areas remain fixed with respect to their vertical or horizontal anchors.  That is, the column headers will not move
			// when the document is scrolled vertically, the row headers will not move when the document is scrolled horizontally.
			// And the upper, left hand corner of the window will never move.  This involved calcuation will create four regions,
			// adjusted to the world coordinates of each of these quadrants, that need to be redrawn based on the incoming clipping
			// area.
			RectangleF[] clipRectangles = new RectangleF[4];
			clipRectangles[0] = RectangleF.Intersect(quadrants[0], new RectangleF(new PointF(this.rectangleF.X + worldClip.X,
				this.rectangleF.Y + worldClip.Y), worldClip.Size));
			clipRectangles[1] = RectangleF.Intersect(quadrants[1], new RectangleF(new PointF(this.virtualOffset.X -
				this.splitSize.Width + worldClip.X, this.rectangleF.Y + worldClip.Y), worldClip.Size));
			clipRectangles[2] = RectangleF.Intersect(quadrants[2], new RectangleF(new PointF(this.rectangleF.X + worldClip.X,
				this.virtualOffset.Y - this.splitSize.Height + worldClip.Y), worldClip.Size));
			clipRectangles[3] = RectangleF.Intersect(quadrants[3], new RectangleF(new PointF(this.virtualOffset.X -
				this.splitSize.Width + worldClip.X, this.virtualOffset.Y - this.splitSize.Height + worldClip.Y),
				worldClip.Size));

			try
			{

				// Make sure the spreadsheet data isn't written while trying to draw the screen.
				this.displayLock.AcquireReaderLock(Timeout.Infinite);

				// IMPORTANT CONCEPT: The strategy for painting the screen is to create a bitmap of the invalid area, draw the data
				// in that bitmap, then dump the composite image in the invalid part of the screen.  This eliminates the 'flicker'
				// you'll get if you try to erase even a tiny portion of the device and write directly to the device.  This section
				// creates a bitmap for the invalid space and associates a 'Graphics' context with that bitmap so drawing commands
				// can write directly to the bitmap.
				Bitmap paintBitmap = new Bitmap(e.ClipRectangle.Width, e.ClipRectangle.Height, e.Graphics);
				Graphics paintGraphic = Graphics.FromImage(paintBitmap);
				paintGraphic.Clear(this.BackColor);

				// Paint any part of each of the quadrants that is invalid.
				for (int quadrantIndex = 0; quadrantIndex < 4; quadrantIndex++)
					if (clipRectangles[quadrantIndex].Height != 0 && clipRectangles[quadrantIndex].Width != 0)
					{

						// The Graphics space is transformed to match the area where the invalid rectangle is located in document 
						// (world) coordinates. This allows the painting logic to simply write to the world coordinates.
						paintGraphic.Transform = new Matrix(this.scaleFactor, 0.0f, 0.0f, this.scaleFactor,
							(this.offsets[quadrantIndex].X - worldClip.X) * this.scaleFactor,
							(this.offsets[quadrantIndex].Y - worldClip.Y) * this.scaleFactor);

						// Create a clipping area inside the window where the data for this quadrant is to be redrawn.  This will
						// prevent data from one quadrant from overwriting data in another quadrant when their position on the
						// device overlaps.
						paintGraphic.Clip = new Region(clipRectangles[quadrantIndex]);

						// This delegate will paint each of the four quadrants in turn.  The 'paintBitmap' is required for reverse
						// color highlighting.
						paintHandler[quadrantIndex](paintGraphic, paintBitmap, clipRectangles[quadrantIndex]);

					}

				// Draw the final composit image to the device.  Doing this in one operation effectively eliminates the flicker
				// that multiple updates to the screen device would cause.
				e.Graphics.DrawImage(paintBitmap, e.ClipRectangle);

				// Return the resources used to paint the image back to the GDI.  Because of the potential frequency of the drawing
				// operations and the relatively large resources needed to paint the document, these are forced back into the free
				// pool immediately rather than waiting for garbage collection to come along and clean up.
				paintGraphic.Dispose();
				paintBitmap.Dispose();

			}
			finally
			{

				// Other thread can now read and modify the display parameters.
				this.displayLock.ReleaseLock();

			}

		}

		/// <summary>
		/// Paint the header areas of the document.
		/// </summary>
		/// <param name="paintGraphic">The graphics context on which to paint.</param>
		/// <param name="paintBitmap">A Bitmap of the paint context used for reverse video effects.</param>
		/// <param name="clipRectangle">The invalid area that needs to be redrawn in document coordinates.</param>
		private void PaintHeaders(Graphics paintGraphic, Bitmap paintBitmap, RectangleF clipRectangle)
		{

			// The viewerTiles are stored chaotically in a hashtable.  While a more sober architecture might try to organize the 
			// viewerTiles spacially by vertical, horizontal and depth, this organization turned out to be more trouble that it was
			// worth, particularly when it came to moving viewerTiles from one location to another.  The hashtable has the
			// advantage that the viewerTile doens't need to move in the data structure when it moves in the virtual space.  The
			// intuitive weakness of a hash table would be the time it takes to iterate through it.  However, it turns out that
			// iterating through a hash table when the viewerTiles are drawn is much less work than maintaining a spacially
			// organized set of viewerTiles.  This step selects the viewerTiles that appear in the clip area and puts them in a
			// list that is organized by depth.  If the viewerTiles are drawn in the order of the depth, they will appear to have a
			// three dimensional quality where viewerTiles that fall behind other viewerTiles are obscured.
			List<Tile> tiles = new List<Tile>();
			List<Tile> selectedTiles = new List<Tile>();
			foreach (Tile viewerTile in this.tileTable.Values)
			{

				// This will select the cells that need to be redrawn based on the invalid area of the screen.  They are placed in
				// a list according to the depth of the viewerTile.  When drawing the cells, the viewerTiles that are deeper are 
				// drawn first, the viewerTiles that are closest to the top will appear to cover up the ones further down.
				if (clipRectangle.IntersectsWith(viewerTile.RectangleF))
				{

					// This will put the viewerTiles in a list that is sorted by the depth.  If the viewerTiles are later drawn in 
					// this order, the ones that are shallow will appear to obscure the ones further down giving a three
					// dimensional look to the document.
					int viewerTileIndex = tiles.BinarySearch(viewerTile, this.viewerTileDepthComparer);
					tiles.Insert(viewerTileIndex < 0 ? ~viewerTileIndex : viewerTileIndex, viewerTile);

					// This will make an ordered list of all the viewerTiles that are selected in the invalid area of the screen.
					if (viewerTile.IsSelected)
						selectedTiles.Add(viewerTile);

				}

			}

			// Now that the viewerTiles are ornaized by depth, then can be drawn.  Note that there is no need to organize them by
			// any other dimension because the coordinates of the viewerTile will place them where they need to be in the clip 
			// area.
			foreach (Tile tile in tiles)
			{

				// The rectangle and the style of the current viewerTile are used several times below.  Note that the style can be
				// animated and run through a sequence of hidden styles in order to appear to flash, fade, etc.  For that reason an
				// index is used into an array of style identifiers.  As another thread changes the index, the style will change to
				// produce the animation effect.
				RectangleF tileRectangle = tile.RectangleF;
				Style style = this.styleTable[tile.StyleArray[tile.StyleIndex]];

				// Fill in the area to be redrawn with the background color specified by the style.
				paintGraphic.FillRectangle(tile.IsSelected ? this.selectedHeaderBrush : style.InteriorBrush,
					tileRectangle);

				// Left Border: a polygon if joined with another border, a rectangle otherwise.
				if (style.LeftBorder != null)
					paintGraphic.FillPolygon(style.LeftBorder.Brush, new PointF[]
						{
							new PointF(tileRectangle.Left, tileRectangle.Top),
							new PointF(tileRectangle.Left + style.LeftBorder.Width,
								style.TopBorder == null ? tileRectangle.Top : tileRectangle.Top + style.LeftBorder.Width),
							new PointF(tileRectangle.Left + style.LeftBorder.Width,
								style.BottomBorder == null ? tile.RectangleF.Bottom : tile.RectangleF.Bottom - style.LeftBorder.Width),
							new PointF(tileRectangle.Left, tile.RectangleF.Bottom)
						});

				// Top Border: a polygon if joined with another border, a rectangle otherwise.
				if (style.TopBorder != null)
					paintGraphic.FillPolygon(style.TopBorder.Brush, new PointF[]
						{
							new PointF(tileRectangle.Left, tileRectangle.Top),
							new PointF(tileRectangle.Right, tileRectangle.Top),
							new PointF(style.RightBorder == null ? tileRectangle.Right :
								tileRectangle.Right - style.TopBorder.Width, tile.RectangleF.Top + style.TopBorder.Width),
							new PointF(style.LeftBorder == null ? tile.RectangleF.Left :
								tile.RectangleF.Left + style.TopBorder.Width, tile.RectangleF.Top + style.TopBorder.Width)
						});

				// Right Border: a polygon if joined with another border, a rectangle otherwise.
				if (style.RightBorder != null)
					paintGraphic.FillPolygon(style.RightBorder.Brush, new PointF[]
						{
							new PointF(tileRectangle.Right - style.RightBorder.Width,
								style.TopBorder == null ? tile.RectangleF.Top : tile.RectangleF.Top + style.RightBorder.Width),
							new PointF(tileRectangle.Right, tileRectangle.Top),
							new PointF(tileRectangle.Right, tile.RectangleF.Bottom),
							new PointF(tile.RectangleF.Right - style.RightBorder.Width,
								style.BottomBorder == null ? tile.RectangleF.Bottom : tile.RectangleF.Bottom - style.RightBorder.Width)
						});

				// Bottom Border: a polygon if joined with another border, a rectangle otherwise.
				if (style.BottomBorder != null)
					paintGraphic.FillPolygon(style.BottomBorder.Brush, new PointF[]
						{
							new PointF(style.LeftBorder == null ? tileRectangle.Left :
								tileRectangle.Left + style.BottomBorder.Width, tile.RectangleF.Bottom - style.BottomBorder.Width),
							new PointF(style.RightBorder == null ? tileRectangle.Right :
								tileRectangle.Right - style.BottomBorder.Width, tile.RectangleF.Bottom - style.BottomBorder.Width),
							new PointF(tileRectangle.Right, tile.RectangleF.Bottom),
							new PointF(tile.RectangleF.Left, tile.RectangleF.Bottom)
						});

				// The viewer displays both image and data.
				if (tile.Data is Bitmap || style.Image is Bitmap)
				{

					// This image will be displayed in the viewerTile.
					Image image = style.Image == null ? tile.Data as Image : style.Image;

					// This is where the image will be placed within the viewerTile unless an instruction below chooses a different
					// alignment.
					PointF location = new PointF(tileRectangle.X, tileRectangle.Y);

					// This will align the image horizontally.
					switch (style.StringFormat.Alignment)
					{
					case StringAlignment.Center:

						// This will center the image horizontally in the viewerTile.
						location.X = tileRectangle.X + (tileRectangle.Width - image.Width) / 2.0f;
						break;

					case StringAlignment.Far:

						// This will align the image with the right side of the viewerTile.
						location.X = tileRectangle.Right - image.Width;
						break;

					}

					// This will align the image vertically.
					switch (style.StringFormat.LineAlignment)
					{

					case StringAlignment.Center:

						// This will center the image vertically in the viewerTile.
						location.Y = tileRectangle.Y + (tileRectangle.Height - image.Height) / 2.0f;
						break;

					case StringAlignment.Far:

						// This will align the image with the bottom of the viewerTile.
						location.Y = tileRectangle.Bottom - image.Height;
						break;

					}

					// Draw the image in the viewerTile's rectangle with the alignment specified.
					paintGraphic.DrawImage(image, new RectangleF(location, image.Size));

				}
				else
				{

					// Draw the formatted data in the given text box with the alignment specified in the stylesheet.
					paintGraphic.DrawString(string.Format(style.NumberFormat, tile.Data), style.Font, style.FontBrush,
						tileRectangle, style.StringFormat);

				}

			}

			// This will draw the header in reverse video to indicate that it is selected.  The reverse video is accomplished by
			// creating a region where the headings are and then a reverse image of the current painting surface is drawn in a
			// clipping region created from the space occupied by the selected header viewerTiles.  Note that the region "Union"
			// operation is preceeded by an "Exclude" operation.  This is due to a bug in some of the early versions of the GDI+
			// library that came out with .NET.  The "Exclude" insures there are no overlapping regions which tend to confuse the
			// buggy GDI libraries and leave artifacts on the painting surface.
			Region outerOutline = new Region(RectangleF.Empty);
			foreach (Tile selectedTile in selectedTiles)
			{
				outerOutline.Exclude(selectedTile.RectangleF);
				outerOutline.Union(selectedTile.RectangleF);
			}

			// The reverse video effect will paint into a clip region created from the shape of the selected viewerTiles in the 
			// header. This will insure that this new mask doesn't extend out of the original painting surface.
			outerOutline.Intersect(paintGraphic.Clip);
			Region oldRegion = paintGraphic.Clip;
			paintGraphic.Clip = outerOutline;

			// The operation below will copy a reverse image of the painting surface onto the painting surface using the selected
			// viewerTiles as an inclusive mask.  The end effect is that the viewerTiles will appear in reverse video wherever 
			// there was a selected viewerTile in the header.  This will require an identity matrix for a transform as anything
			// that involves a scaling operation will leave a smeared image.  This will save the current transform so it can be
			// restored after the paint operation.
			Matrix oldTransform = paintGraphic.Transform;

			// Copying the current image onto itself with an inverted color matrix produces a reverse video effect when the image
			// is masked by a clipping region constructed from the selected viewerTiles.
			paintGraphic.ResetTransform();
			paintGraphic.DrawImage(paintBitmap, new Rectangle(0, 0, paintBitmap.Width, paintBitmap.Height), 0, 0,
				paintBitmap.Width, paintBitmap.Height, GraphicsUnit.Pixel, this.invertedColorAttributes);

			// The world transform and clipping area for this quadrant are restored after the reverse video effect.
			paintGraphic.Transform = oldTransform;
			paintGraphic.Clip = oldRegion;

			// The regions are no longer needed and should be disposed of immediately so they don't collect up during frequent
			// paint operations.
			outerOutline.Dispose();

		}

		/// <summary>
		/// Paint the viewerTiles in the third quadrant of the viewer.
		/// </summary>
		/// <param name="paintGraphic">The graphics context on which to paint.</param>
		/// <param name="paintBitmap">A Bitmap of the paint context used for reverse video effects.</param>
		/// <param name="clipRectangle">The invalid area that needs to be redrawn in document coordinates.</param>
		private void PaintTiles(Graphics paintGraphic, Bitmap paintBitmap, RectangleF clipRectangle)
		{

			// The highlight around selected viewerTiles bleeds into other cells.  Because of this, the test to see if a viewerTile
			// should be painted included not just the viewerTiles in the invalid area, but also the neighboring viewerTiles.  This
			// process is further confused when viewerTiles are at the boundaries of quadrants.  This rectangle defines the
			// quadrant that normal 'body' viewerTiles will occupy.  The test for whether a selected viewerTile should be redrawn
			// will include the area where the selection highlighting is shown, but will exclude viewerTiles that are selected in
			// other quadrants.
			RectangleF viewerTileQuadrant = new RectangleF(new PointF(splitSize.Width, splitSize.Height),
				new SizeF(float.PositiveInfinity, float.PositiveInfinity));

			// The viewerTiles are stored chaotically in a hashtable.  While a more sober architecture might try to organize the
			// viewerTiles spacially by vertical, horizontal and depth, this organization turned out to be more trouble that it was
			// worth, particularly when it came to moving viewerTiles from one location to another.  The hashtable has the
			// advantage that the viewerTile doens't need to move in the data structure when the coordinates change.  The intuitive
			// weakness of a hash table would be the time it takes to iterate through it.  However, it turns out that iterating
			// through a hash table when the viewerTiles are drawn is much less work than maintaining a spacially organized set of
			// viewerTiles.  This step selects the viewerTiles that appear in the clip area and puts them in a list that is
			// organized by depth.  If the viewerTiles are drawn in the order of the depth, they will appear to have a three
			// dimensional quality where viewerTiles that fall behind other viewerTiles are obscured.
			List<Tile> tiles = new List<Tile>();
			List<Tile> selectedTiles = new List<Tile>();
			foreach (Tile tile in this.tileTable.Values)
			{

				// This will select the cells that need to be redrawn based on the invalid area of the screen.  They are placed in
				// a list according to the depth of the viewerTile.  When drawing the cells, the viewerTiles that are deeper are
				// drawn first, the viewerTiles that are closest to the top will appear to cover up the ones further down.
				if (clipRectangle.IntersectsWith(tile.RectangleF))
				{
					int viewerTileIndex = tiles.BinarySearch(tile, this.viewerTileDepthComparer);
					tiles.Insert(viewerTileIndex < 0 ? ~viewerTileIndex : viewerTileIndex, tile);
				}

				// This will make an ordered list of all the viewerTiles that are selected in the invalid area of the screen and
				// their immediate neighbors.  It will exclude viewerTiles that don't belong in this quadrant (for example, the
				// header viewerTiles).  The selected viewerTiles immediately around the selection are needed to properly draw the
				// borders (e.g. the corner of a selected area needs to know that the viewerTile below and to the right are not
				// selected).
				if (tile.IsSelected && clipRectangle.IntersectsWith(RectangleF.Inflate(tile.RectangleF,
					DocumentViewer.selectionInterior, DocumentViewer.selectionInterior)) &&
					viewerTileQuadrant.IntersectsWith(tile.RectangleF))
				{
					int selectedTileIndex = selectedTiles.BinarySearch(tile, this.viewerTileLocationComparer);
					selectedTiles.Insert(selectedTileIndex < 0 ? ~selectedTileIndex : selectedTileIndex, tile);
				}

			}

			// Now that the viewerTiles are ornaized by depth, then can be drawn.  Note that there is no need to organize them by
			// any other dimension because the coordinates of the viewerTile will place them where they need to be in the clip 
			// area.
			foreach (Tile tile in tiles)
			{

				// The rectangle and the style of the current viewerTile are used several times below.  Note that the style can be
				// animated and run through a sequence of hidden styles in order to appear to flash, fade, etc.  For that reason an
				// index is used into an array of style identifiers.  As another thread changes the index, the style will change to
				// produce the animation effect.
				RectangleF tileRectangle = tile.RectangleF;
				Style style = this.styleTable[tile.StyleArray[tile.StyleIndex]];

				// Fill in the area to be redrawn with the background color specified by the style.
				paintGraphic.FillRectangle(style.InteriorBrush, tileRectangle);

				// Left Border: a polygon if joined with another border, a rectangle otherwise.
				if (style.LeftBorder != null)
					paintGraphic.FillPolygon(style.LeftBorder.Brush, new PointF[]
						{
							new PointF(tileRectangle.Left, tileRectangle.Top),
							new PointF(tileRectangle.Left + style.LeftBorder.Width,
								style.TopBorder == null ? tileRectangle.Top : tileRectangle.Top + style.LeftBorder.Width),
							new PointF(tileRectangle.Left + style.LeftBorder.Width,
								style.BottomBorder == null ? tile.RectangleF.Bottom : tile.RectangleF.Bottom - style.LeftBorder.Width),
							new PointF(tileRectangle.Left, tile.RectangleF.Bottom)
						});

				// Top Border: a polygon if joined with another border, a rectangle otherwise.
				if (style.TopBorder != null)
					paintGraphic.FillPolygon(style.TopBorder.Brush, new PointF[]
						{
							new PointF(tileRectangle.Left, tileRectangle.Top),
							new PointF(tileRectangle.Right, tileRectangle.Top),
							new PointF(style.RightBorder == null ? tileRectangle.Right :
								tileRectangle.Right - style.TopBorder.Width, tile.RectangleF.Top + style.TopBorder.Width),
							new PointF(style.LeftBorder == null ? tile.RectangleF.Left :
								tile.RectangleF.Left + style.TopBorder.Width, tile.RectangleF.Top + style.TopBorder.Width)
						});

				// Right Border: a polygon if joined with another border, a rectangle otherwise.
				if (style.RightBorder != null)
					paintGraphic.FillPolygon(style.RightBorder.Brush, new PointF[]
						{
							new PointF(tileRectangle.Right - style.RightBorder.Width,
								style.TopBorder == null ? tile.RectangleF.Top : tile.RectangleF.Top + style.RightBorder.Width),
							new PointF(tileRectangle.Right, tileRectangle.Top),
							new PointF(tileRectangle.Right, tile.RectangleF.Bottom),
							new PointF(tile.RectangleF.Right - style.RightBorder.Width,
								style.BottomBorder == null ? tile.RectangleF.Bottom : tile.RectangleF.Bottom - style.RightBorder.Width)
						});

				// Bottom Border: a polygon if joined with another border, a rectangle otherwise.
				if (style.BottomBorder != null)
					paintGraphic.FillPolygon(style.BottomBorder.Brush, new PointF[]
						{
							new PointF(style.LeftBorder == null ? tileRectangle.Left :
								tileRectangle.Left + style.BottomBorder.Width, tile.RectangleF.Bottom - style.BottomBorder.Width),
							new PointF(style.RightBorder == null ? tileRectangle.Right :
								tileRectangle.Right - style.BottomBorder.Width, tile.RectangleF.Bottom - style.BottomBorder.Width),
							new PointF(tileRectangle.Right, tile.RectangleF.Bottom),
							new PointF(tile.RectangleF.Left, tile.RectangleF.Bottom)
						});

			}

			// This will highlight the selected viewerTiles with an inverse border around the viewerTiles and a slate-blue
			// interior. The inverse video highlight is accomplished by creating a region around the selected viewerTiles, then
			// excluding a region with the same shape but a smaller size from the interior.  When an image of the screen is passed
			// through this outlined region with a reverse color matrix, it will make the outline appear with an inverse video
			// effect.
			Region outerOutline = new Region(RectangleF.Empty);
			Region innerOutline = new Region(RectangleF.Empty);

			// The inner part of the selected area is given a slate-blue shading.  The effect is accomplished using a simple
			// transparent brush.
			Region innerRegion = new Region(RectangleF.Empty);

			// This rectangle is where the current focus is.  It will be excluded from the interior highlighting.
			RectangleF activeRectangle = RectangleF.Empty;

			// The main idea here is to draw an outline around all the congigous regions that are selected.  This is done by taking
			// a viewerTile and examining all the viewerTiles to see if there are any neighbors that are selected.  This is a
			// little tricky because the viewerTiles are not organized spacially.  However, even this brute force approach to
			// finding contigous areas in a chaotically organized data structure is faster overall than trying to keep the
			// viewerTiles organized by location.
			for (int outerTileIndex = 0; outerTileIndex < selectedTiles.Count; outerTileIndex++)
			{

				// This tile will be compared against its neighbors to determine the shape of the outline.
				Tile outerTile = selectedTiles[outerTileIndex];

				// This is the viewerTile that is going to be examined for selected neighboring viewerTiles.
				RectangleF outerRectangle = outerTile.RectangleF;

				// This is the part of the viewerTile which will be drawn in a slate-blue color in the interior section.
				RectangleF interiorRectangle = outerTile.RectangleF;

				// The focus viewerTile, if present in the visible selected area, is removed from the interior area that is shaded 
				// with a slate-blue coloring.
				if (outerTile.IsActive)
					activeRectangle = outerRectangle;

				// This rectangle represents an outline around the selected viewerTile.  Note that it is inflated to spill over to
				// the surrounding viewerTiles.  The 'outerOutline' region will be the outside part of the perimeter around all the
				// selected viewerTiles.  The hard part is removing the interior pieces so as to make it look like a pen was used
				// to draw the permiter.  Note also that the are is excluded from the region before it is added.  This is to fix a
				// bug with the current GDI+ code that causes artifacts when there are overlapping area and concave shapes.  The
				// 'Exclude' method should be removed in future releases of the XP operating system.
				RectangleF outlineRectangle = new RectangleF(outerRectangle.X - DocumentViewer.selectionOuterBorder - 1.0f,
					outerRectangle.Y - DocumentViewer.selectionOuterBorder - 1.0f,
					outerRectangle.Width + DocumentViewer.selectionOuterBorder * 2.0f + 1.0f,
					outerRectangle.Height + DocumentViewer.selectionOuterBorder * 2.0f + 1.0f);
				outerOutline.Exclude(outlineRectangle);
				outerOutline.Union(outlineRectangle);

				// The inner loop is responsible for removing the interior parts of the outline around the viewerTiles.  That is,
				// it will make it look like a pen drew an outline around all the selected viewerTiles even though we're painting a
				// region.  It will determine if there is a neighbor to the right or below the viewerTile that is selected and that
				// will determine what kind of interior angles are cut into the region.  The main idea is to construct a proper
				// outline around the viewerTiles taking into account that contigous areas should be shaded contigously.
				bool isRightEdgeConnected = false;
				bool isBottomEdgeConnected = false;
				bool isKittyCornerConnected = false;
				RectangleF rightRectangle = RectangleF.Empty;
				RectangleF bottomRectangle = RectangleF.Empty;
				RectangleF kittyCornerRectangle = RectangleF.Empty;

				// This loop will search the unordered set of viewerTiles looking for neighbors and determining their position with
				// respect to the viewerTile being tested for neighbors.
				for (int innerTileIndex = outerTileIndex + 1; innerTileIndex < selectedTiles.Count; innerTileIndex++)
				{

					// This tile is the neighbor that is compared against the outer loop's tile to see what shape will be used to
					// highlight the cell.
					Tile innerTile = selectedTiles[innerTileIndex];
						
					// Never compare a cell to itself: it will always appear to have a contigous right and bottom edge.
					if (innerTile.TileId == outerTile.TileId)
						continue;

					// This is the area occupied by the viewerTile that is to be tested to see if it is a neighbor.
					RectangleF innerRectangle = innerTile.RectangleF;

					// This will test whether there is a selected neighbor to the right.
					if (innerRectangle.Right >= outerRectangle.Right && innerRectangle.Left <= outerRectangle.Right &&
						innerRectangle.Top == outerRectangle.Top && innerRectangle.Bottom == outerRectangle.Bottom)
					{
						isRightEdgeConnected = true;
						rightRectangle = innerRectangle;
					}

					// This will test whether there is a selected neighbor below.
					if (innerRectangle.Bottom >= outerRectangle.Bottom && innerRectangle.Top <= outerRectangle.Bottom &&
						innerRectangle.Left == outerRectangle.Left && innerRectangle.Right == outerRectangle.Right)
					{
						isBottomEdgeConnected = true;
						bottomRectangle = innerRectangle;
					}

					// This will test whether there is a tile kitty corner to the test viewerTile.
					if (outerRectangle.Bottom == innerRectangle.Top && outerRectangle.Right == innerRectangle.Left)
					{
						isKittyCornerConnected = true;
						kittyCornerRectangle = innerRectangle;
					}

					// This will break out of the inner loop when there are no longer any tiles to consider.  Remember that the
					// selected tiles are ordered by cartesean coordinates with the vertical (Y) coordinate as the primary sort and
					// the horizontal (X) as the secondary.
					if ((innerRectangle.Top == outerRectangle.Bottom && innerRectangle.Left > outerRectangle.Right) &&
						innerRectangle.Top > outerRectangle.Bottom)
						break;

				}

				// Now that all the selected viewerTiles in the invalid area have been considered, an outline for the current test
				// viewerTile can be constructed.  A 'kitty corner' means there are selected viewerTiles to the right, below and
				// down and to the right.  This indicates there are no borders to the left and right and the interior area should
				// be connected to the cells below, to the right and kitty corner to the outer viewerTile.
				if (isKittyCornerConnected && bottomRectangle.Top == kittyCornerRectangle.Top &&
					bottomRectangle.Bottom == kittyCornerRectangle.Bottom && rightRectangle.Left == kittyCornerRectangle.Left &&
					rightRectangle.Right == kittyCornerRectangle.Right)
				{

					// This will cut out the left and top part of the perimeter, but leave the right and bottom parts.
					outerRectangle = new RectangleF(outerRectangle.X + DocumentViewer.selectionInnerBorder - 1.0f,
						outerRectangle.Y + DocumentViewer.selectionInnerBorder - 1.0f, outerRectangle.Width,
						outerRectangle.Height);

					// This will cut out the left and top part of the interior shaded area, but not the right and bottom parts.
					interiorRectangle = new RectangleF(interiorRectangle.X + DocumentViewer.selectionInterior - 1.0f,
						interiorRectangle.Y + DocumentViewer.selectionInterior - 1.0f, interiorRectangle.Width,
						interiorRectangle.Height);

				}
				else
				{

					// At this point, there is at least one unselected viewerTile to the right, below or kitty corner.  The
					// strategy is to shrink the interior rectangles by a fixed amount and then draw additional rectangles to
					// connect the contigous areas.  Note the constant -- 1.0f -- is used to make the areas spill over to the left
					// and above. It makes the selection more symetrical looking with respect to the border lines.
					outerRectangle = new RectangleF(outerRectangle.X + DocumentViewer.selectionInnerBorder - 1.0f,
						outerRectangle.Y + DocumentViewer.selectionInnerBorder - 1.0f,
						outerRectangle.Width - DocumentViewer.selectionInnerBorder * 2.0f + 1.0f,
						outerRectangle.Height - DocumentViewer.selectionInnerBorder * 2.0f + 1.0f);
					interiorRectangle = new RectangleF(interiorRectangle.X + DocumentViewer.selectionInterior - 1.0f,
						interiorRectangle.Y + DocumentViewer.selectionInterior - 1.0f,
						interiorRectangle.Width - DocumentViewer.selectionInterior * 2.0f + 1.0f,
						interiorRectangle.Height - DocumentViewer.selectionInterior * 2.0f + 1.0f);

					// If there is a neighbor to the right, then cut out the connecting space from the perimeter and add the
					// connecting space to the interior shaded region.
					if (isRightEdgeConnected)
					{

						// This will remove the connecting area to the right from the area used to calculate the perimeter.
						RectangleF innerOutlineRectangle = new RectangleF(outerRectangle.Right, outerRectangle.Top,
							DocumentViewer.selectionInnerBorder * 2.0f, outerRectangle.Height);
						innerOutline.Exclude(innerOutlineRectangle);
						innerOutline.Union(innerOutlineRectangle);

						// This will add the area to the shaded interior region for the selected viewerTiles.
						RectangleF innerAreaRectangle = new RectangleF(interiorRectangle.Right, interiorRectangle.Top,
							DocumentViewer.selectionInterior * 2.0f, interiorRectangle.Height);
						innerRegion.Exclude(innerAreaRectangle);
						innerRegion.Union(innerAreaRectangle);

					}

					// If there is a neighbor below, then cut out the connecting space from the perimeter and add the connecting
					// space to the interior shaded region.
					if (isBottomEdgeConnected)
					{

						// This will remove the connecting area below from the area used to calculate the perimeter.
						RectangleF innerOutlineRectangle = new RectangleF(outerRectangle.Left, outerRectangle.Bottom,
							outerRectangle.Width, DocumentViewer.selectionInnerBorder * 2.0f);
						innerOutline.Exclude(innerOutlineRectangle);
						innerOutline.Union(innerOutlineRectangle);

						// This will add the area to the shaded interior region for the selected viewerTiles.
						RectangleF innerAreaRectangle = new RectangleF(interiorRectangle.Left, interiorRectangle.Bottom,
							interiorRectangle.Width, DocumentViewer.selectionInterior * 2.0f);
						innerRegion.Exclude(innerAreaRectangle);
						innerRegion.Union(innerAreaRectangle);

					}

				}

				// At this point, the inner part of the perimeter of this viewerTile has been calculated and can be merged in with 
				// the other viewerTiles that comprise the outline of the selected area.
				innerOutline.Exclude(outerRectangle);
				innerOutline.Union(outerRectangle);

				// The interior area of this viewerTile can be added to the other interior areas.
				innerRegion.Exclude(interiorRectangle);
				innerRegion.Union(interiorRectangle);

			}

			// At this point the both the exterior and interior part of the outline has been calculated properly to show the
			// correct set of angles and straigh lines to make a path around the selected viewerTiles.  Removing the interior 
			// region of the outline will make it appear that a pen has drawn an outline around the selected cells.  We could fil
			// this area with a brush to highlight the selected viewerTiles, but it would be more dramatic to use a reverse video
			// effect on this region.
			outerOutline.Exclude(innerOutline);

			// This operation will replace the clipping area of the viewer with the outline of the selected viewerTiles.  The big
			// idea is copy a reverse image of the current bitmap used to construct the invalid area of the viewer over this clip 
			// area.  The end effect is that the currently constructed image will show through the outline region in reverse
			// video.  It is important when playing games with the clipping region that the painting doesn't end up outside the
			// original clipping area.  That is why painting region is the intersection of the outline region and the current
			// clipping region.  Note that the current clipping area is saved so it can be restored after the reverse video effect.
			outerOutline.Intersect(paintGraphic.Clip);
			Region oldRegion = paintGraphic.Clip;
			paintGraphic.Clip = outerOutline;

			// There were some very ugly early versions of this operation.  The next step is to draw the reverse video effect
			// around the selected viewerTiles.  This is done by taking the current image of the drawing area, which is constructed
			// in a bitmap, and copy it onto the Graphics area using a matrix that reverses the colors.  Because the clipping area
			// is a thin line around the selected viewerTiles, the end effect is to create a line in reverse video around the
			// selected viewerTiles. An identity matrix is needed for the transform operation to prevent the 'DrawImage' operation
			// from smearing the image, which is what would happen if we tried to copy the image into a Graphics using anything but
			// 1.0f for a scaling factor.
			Matrix oldMatrix = paintGraphic.Transform;
			paintGraphic.ResetTransform();

			// This will draw an image of the current painting surface on to the current painting surface but reversing all of the
			// colors that appear in the outline region constructed above.
			paintGraphic.DrawImage(paintBitmap, new Rectangle(0, 0, paintBitmap.Width, paintBitmap.Height), 0, 0,
				paintBitmap.Width, paintBitmap.Height, GraphicsUnit.Pixel, this.invertedColorAttributes);

			// Now that the image operation is complete, the original transform matrix used to paint in world coordinates can be
			// restored.
			paintGraphic.Transform = oldMatrix;

			// This will restore the original clipping region.
			paintGraphic.Clip = oldRegion;

			// The operation to shade the interior is much less involved.  Just remove the active (focus) viewerTile and paint the
			// remainig region with a semi-transparent brush.
			innerRegion.Exclude(activeRectangle);
			paintGraphic.FillRegion(this.selectedInteriorBrush, innerRegion);

			// The regions are no longer needed and should be disposed of immediately so they don't collect up during frequent
			// paint operations.
			innerRegion.Dispose();
			innerOutline.Dispose();
			outerOutline.Dispose();

			// Now that the viewerTiles are organized by depth and the backgrounds drawn and highlighted, the body of the 
			// viewerTiles can be drawn.  Note that there is no need to organize them by any other dimension because the row and
			// column coordinates of the viewerTile will place them where they need to be on the device.
			foreach (Tile tile in tiles)
			{

				// The rectangle and the style of the current viewerTile are used several times below.
				RectangleF viewerTileRectangle = tile.RectangleF;
				Style style = this.styleTable[tile.StyleArray[tile.StyleIndex]];

				// The viewer displays both image and data.
				if (tile.Data is Bitmap || style.Image is Bitmap)
				{

					// This image will be displayed in the viewerTile.
					Image image = style.Image == null ? tile.Data as Image : style.Image;

					// This is where the image will be placed within the viewerTile unless an instruction below chooses a different
					// alignment.
					PointF location = new PointF(viewerTileRectangle.X, viewerTileRectangle.Y);

					// This will align the image horizontally.
					switch (style.StringFormat.Alignment)
					{
					case StringAlignment.Center:

						// This will center the image horizontally in the viewerTile.
						location.X = viewerTileRectangle.X + (viewerTileRectangle.Width - image.Width) / 2.0f;
						break;

					case StringAlignment.Far:

						// This will align the image with the right side of the viewerTile.
						location.X = viewerTileRectangle.Right - image.Width;
						break;

					}

					// This will align the image vertically.
					switch (style.StringFormat.LineAlignment)
					{

					case StringAlignment.Center:

						// This will center the image vertically in the viewerTile.
						location.Y = viewerTileRectangle.Y + (viewerTileRectangle.Height - image.Height) / 2.0f;
						break;

					case StringAlignment.Far:

						// This will align the image with the bottom of the viewerTile.
						location.Y = viewerTileRectangle.Bottom - image.Height;
						break;

					}

					// Draw the image in the viewerTile's rectangle with the alignment specified.
					paintGraphic.DrawImage(image, new RectangleF(location, image.Size));

				}
				else
				{

					// Draw the formatted data in the given text box with the alignment specified in the stylesheet.
					paintGraphic.DrawString(string.Format(style.NumberFormat, tile.Data), style.Font, style.FontBrush,
						viewerTileRectangle, style.StringFormat);

				}

			}

		}

		/// <summary>
		/// This thread handles the incoming stream of objects which describe how the screen is painted.
		/// </summary>
		private void BackgroundThread()
		{

			// This flag is used to shut down the thread gracefully from another.
			this.IsBackgroundThreadRunning = true;

			// This buffer collects logical groups of drawing operations which should be drawn as a unit.
			List<ViewerCommand> viewerCommandBuffer = new List<ViewerCommand>();

			// This is a general purpose datastructure that is shared by all the command handlers to control the construction and
			// maintainence of the of the viewer data structures.  Most importantly, it contains the shared list of modified areas
			// that need to be redrawn.
			BackgroundContext backgroundContext = new BackgroundContext();

			// Keep working until the thread is shut down.
			while (this.IsBackgroundThreadRunning)
			{

				// This queue provides the means to send data and instructions to the viewer from other threads.  This will pull
				// the next object off the incoming queue.
				viewerCommandBuffer.Add(this.ViewerCommandQueue.Dequeue());

				// Drawing instructions are queued up until a critical mass is ready to be drawn, or the queue is empty.
				if (viewerCommandBuffer.Count == DocumentViewer.commandBufferSize || this.ViewerCommandQueue.IsEmpty)
				{

					try
					{

						// Prevent the foreground thread from reading the viewer data while it is being updated.
						this.displayLock.AcquireWriterLock(Timeout.Infinite);

						// The incoming ranges will have collection of objects that modify the viewer.  This loop will pull apart
						// the list and handle each of the incoming objects.
						foreach (ViewerCommand viewerCommand in viewerCommandBuffer)
							viewerCommand.ViewerDelegate(viewerCommand.ViewerObject, backgroundContext);

					}
					catch (Exception exception)
					{

						// Log the location and description of any errors updating the screen.
						EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

					}
					finally
					{

						// At this point, the foreground thread can access the viewer data.
						this.displayLock.ReleaseWriterLock();

					}

					// Everything in this list has been handled.  Clear it out for the next batch of incoming viewer instructions.
					viewerCommandBuffer.Clear();

					// This will allow other threads to do some work while the background is processing the incoming viewer 
					// commands.
					Thread.Sleep(0);

				}

			}

		}

		/// <summary>
		/// Provides highlighting of modified cells that fade over time.
		/// </summary>
		private void AnimationThread(object state)
		{

			// This will collect all the areas of the document that need to be redrawn because they've got an animation effect that
			// changed the appearance of the tile.
			List<RectangleF> updateList = new List<RectangleF>();

			try
			{

				// The display needs to be locked while the tiles are examined.
				this.displayLock.AcquireWriterLock(Timeout.Infinite);

				// Each tile that has a running animation sequence is advanced to the next frame in the animation.  The tile's area
				// is added to the list of areas that will be redrawn.
				foreach (Tile tile in this.tileTable.Values)
					if (tile.StyleIndex < tile.StyleArray.Length - 1)
					{
						tile.StyleIndex++;
						updateList.Add(tile.RectangleF);
					}

			}
			finally
			{

				// The viewer's data and the animated list can be use by other threads now.
				this.displayLock.ReleaseWriterLock();

			}

			// Call the foreground to invalidate the rectangles that are calcualted above.
			BeginInvoke(new InvalidRegionDelegate(InvalidateRegion), new object[] { updateList });

		}

		/// <summary>
		/// Creates header areas for the rows and columns.
		/// </summary>
		/// <param name="viewerObject">The generic object of the command.</param>
		/// <param name="backgroundContext">Information about document update currently in progress.</param>
		public void SetSplit(ViewerObject viewerObject, BackgroundContext backgroundContext)
		{

			// This method is used by the view code during initialization of the document to set the header area for the rows and 
			// columns.
			this.splitSize = (viewerObject as ViewerSplit).SizeF;

		}

		/// <summary>
		/// Sets the magnification factor for the viewer.
		/// </summary>
		/// <param name="viewerObject">The generic object of the command.</param>
		/// <param name="backgroundContext">Information about document update currently in progress.</param>
		public void SetScale(ViewerObject viewerObject, BackgroundContext backgroundContext)
		{

			// This method is used by the view code during initialization of the document to set the header area for the rows and 
			// columns.
			this.scaleFactor = (viewerObject as ViewerScale).Factor;

		}

		/// <summary>
		/// Adds a style to the Style Table of this document.
		/// </summary>
		/// <param name="viewerObject">The styles to be added.</param>
		/// <param name="backgroundContext">The context data for maintaining the viewer area.</param>
		public void UpdateStyle(ViewerObject viewerObject, BackgroundContext backgroundContext)
		{

			// The incoming object is a style which defines how data is drawn in a viewerTile in the viewer.
			ViewerStyle viewerStyle = viewerObject as ViewerStyle;

			// The incoming 'ViewerStyle' is really a set of instructions for creating a style.  The 'Style' used by the viewer can
			// be based on a previous style with additional attributes.  It also has the actual GDI resources (pens, brushes,
			// fonts) that are used to draw on the device context.  This section will create the style if it doesn't exist, then
			// build the style from the attributes of the parent and the incoming instructoins in the ViewerStyle.
			Style style;
			if (!styleTable.TryGetValue(viewerStyle.StyleId, out style))
			{

				// If the style doesn't exist yet, then create it and add it to the hash table used to manage the styles.
				style = new Style(viewerStyle.StyleId);
				this.styleTable.Add(style.StyleId, style);

			}

			// This will copy the attributes of the parent style into the style being constructed.
			if (viewerStyle.ParentId != string.Empty)
				style.Parent = this.styleTable[viewerStyle.ParentId];

			// The Animation attribute for this style requires that all the other attributes be set because it will generate clones
			// of this style for the different frames.  This variable is used to defer the calculation of the animation frames
			// until after the other attributes have been processed.
			ViewerAnimation viewerAnimation = null;

			// The incoming 'ViewerStyle' has a list of attributes which overwrite the default/parent attributes.
			foreach (ViewerAttribute viewerAttribute in viewerStyle.Attributes)
			{
				if (viewerAttribute is ViewerAnimation)
					viewerAnimation = viewerAttribute as ViewerAnimation;
				if (viewerAttribute is ViewerImage)
					style.Image = (viewerAttribute as ViewerImage).Image;
				if (viewerAttribute is ViewerFont)
					style.Font = (viewerAttribute as ViewerFont).Font;
				if (viewerAttribute is ViewerFontBrush)
					style.FontBrush = (viewerAttribute as ViewerFontBrush).SolidBrush;
				if (viewerAttribute is ViewerInteriorBrush)
					style.InteriorBrush = (viewerAttribute as ViewerInteriorBrush).Brush;
				if (viewerAttribute is ViewerNumberFormat)
					style.NumberFormat = (viewerAttribute as ViewerNumberFormat).Format;
				if (viewerAttribute is ViewerStringFormat)
					style.StringFormat = (viewerAttribute as ViewerStringFormat).StringFormat;
				if (viewerAttribute is ViewerBottomBorder)
					style.BottomBorder = viewerAttribute as ViewerBorder;
				if (viewerAttribute is ViewerLeftBorder)
					style.LeftBorder = viewerAttribute as ViewerBorder;
				if (viewerAttribute is ViewerRightBorder)
					style.RightBorder = viewerAttribute as ViewerBorder;
				if (viewerAttribute is ViewerTopBorder)
					style.TopBorder = viewerAttribute as ViewerBorder;
			}

			// All styles will have an animation processor.  For static styles, the animation is a single frame.  The more advanced
			// styles can be specified through the ViewerStyles created in the view.
			if (viewerAnimation == null)
				style.Animation = new DefaultAnimation(style);
			else
			{
				if (viewerAnimation is ViewerFadeAnimation)
					style.Animation = new FadeAnimation(this.styleTable, style, viewerAnimation as ViewerFadeAnimation);
				if (viewerAnimation is ViewerFlashAnimation)
					style.Animation = new FlashAnimation(this.styleTable, style, viewerAnimation as ViewerFlashAnimation);
			}

			// This will cause each viewerTile that uses the modified style to be redrawn.
			foreach (Tile tile in this.tileTable.Values)
				if (tile.StyleArray[tile.StyleIndex] == style.StyleId)
					backgroundContext.UpdateList.Add(tile.RectangleF);

		}

		/// <summary>
		/// Deletes a viewerTile from the viewer.
		/// </summary>
		/// <param name="viewerObject">The viewer object to be deleted.</param>
		/// <param name="backgroundContext">The context data for maintaining the viewer area.</param>
		private void DeleteTile(ViewerObject viewerObject, BackgroundContext backgroundContext)
		{

			// This method operates on a viewerTile.
			Tile viewerTile = viewerObject as Tile;

			// IMPORTANT CONCEPT: Each of the viewerTiles has a unique identifier.  If the incoming viewerTile has been moved or
			// resized, then the previous location of this viewerTile will be erased using the old coordinates.  Two data
			// structures are used to manage the data in the viewer.  The first one is a hash table that uses the primary key of
			// the viewerTile as an identifier.  This table is used to find the previous coordinates of the viewerTile when the
			// object is already part of the viewer.  The second data structure is a list of list of list of viewerTiles ordered by
			// their three dimensional location in the document.  This is what the 'Paint' method uses to draw invalid areas of the
			// screen.
			Tile oldViewerTile = null;

			// A hash table is used to quickly find the viewerTiles by their identifiers.  The binary sort ended up being too
			// inefficient when the viewerTiles were read in the wrong order (i.e. if the document was sorted in the opposite order
			// the viewerTiles were generated, then inserting these viewerTiles at the start of the list was very slow operation).
			if (this.tileTable.TryGetValue(viewerTile.TileId, out oldViewerTile))
			{

				// This will indicate that the size of the document should be recalculated.
				backgroundContext.HasSizeChanged = true;

				// Remove the previous instance of the viewerTile from the identifier lookup table and from the ordered list of viewerTiles by
				// location.
				this.tileTable.Remove(oldViewerTile.TileId);

				// The 'UpdateList' is used to calculate the part of the screen that should be redrawn after this set of
				// viewerTiles has been deleted. The 'Region.Union' operation is horribly inefficient when it comes to small
				// rectangles, so the area of all the deleted viewerTiles will be added to an ordered list and then compressed
				// until the smallest number of rectangular areas is found.  Note that the invalid area is increased to cover the
				// part of the neighboring cells that may have been highlighted to show a selected area.
				backgroundContext.UpdateList.Add(RectangleF.Inflate(oldViewerTile.RectangleF, DocumentViewer.selectionInterior,
					DocumentViewer.selectionInterior));

			}

		}

		/// <summary>
		/// Adds or updates a viewerTile in the viewer space.
		/// </summary>
		/// <param name="viewerObject">The viewer viewerTile to be added/updated.</param>
		/// <param name="backgroundContext">The context data for maintaining the viewer area.</param>
		public void UpdateTile(ViewerObject viewerObject, BackgroundContext backgroundContext)
		{

			// The 'ViewerTile' contains the actual data, the location and the formatting instructions for a given rectangle in the
			// document.
			Tile newViewerTile = viewerObject as Tile;

			// IMPORTANT CONCEPT: Each of the ViewerTiles has a unique identifier.  If the incoming viewerTile has been moved or
			// resized, then the previous location of this viewerTile will be erased using the old coordinates.  Two data
			// structures are used to manage the data in the viewer.  The first one is a hash table that uses the primary key of
			// the viewerTile as an identifier.  This table is used to find the previous coordinates of the viewerTile when the
			// object is already part of the viewer.  The second data structure is a list of list of list of viewerTiles ordered by
			// their location in the three dimensions of the document.
			Tile oldViewerTile = null;

			// A hash table is used to quickly find the viewerTiles by their identifiers.  The binary sort ended up being too
			// inefficient when the viewerTiles were read in the wrong order (i.e. if the document was sorted in the opposite order
			// the viewerTiles were generated, then inserting these viewerTiles at the start of the list was very slow operation).
			if (this.tileTable.TryGetValue(newViewerTile.TileId, out oldViewerTile))
			{

				// The previous location of the cell and its neighbors is added to the update region when the cell has moved or
				// changed size.  The neighbors are included so that the selection highlights are properly redrawn.
				if (oldViewerTile.RectangleF != newViewerTile.RectangleF)
				{

					// An inflated version of the tile's rectangle is added to the update list when a tile's size or position has
					// changed so that all the neighbors of that tile can re-evaluate their border areas.
					backgroundContext.UpdateList.Add(RectangleF.Inflate(oldViewerTile.RectangleF, DocumentViewer.selectionInterior,
						DocumentViewer.selectionInterior));
					oldViewerTile.RectangleF = newViewerTile.RectangleF;

					// Changing the size of a tile has the potential to change the size of the document.
					backgroundContext.HasSizeChanged = true;

				}

				// Ask the style associated with the tile to initiate any animation sequence when the data has changed.  Many
				// animation sequence involve a state change, so the previous and current tiles are examined.  Note that even
				// non-animated tiles are given a simple sequence of a single style to make the logic more consistent.
				if (oldViewerTile.Data != newViewerTile.Data)
					this.styleTable[newViewerTile.StyleId].Animation.SetSequence(oldViewerTile, newViewerTile);
				
				// This will add the viewerTile's area to the list of areas that need to be redrawn.  Note that the surrounding
				// viewerTiles are also redrawn then the selected state of this viewerTile has changed.
				backgroundContext.UpdateList.Add(oldViewerTile.IsSelected != newViewerTile.IsSelected ?
					RectangleF.Inflate(newViewerTile.RectangleF, DocumentViewer.selectionInterior,
					DocumentViewer.selectionInterior) : newViewerTile.RectangleF);

				// Copy the new values into the viewerTile rather than replace it with the new one.  This will save the hash table
				// the work of adding the new viewerTile and removing the old one when the only difference is the values.
				oldViewerTile.IsActive = newViewerTile.IsActive;
				oldViewerTile.IsSelected = newViewerTile.IsSelected;
				oldViewerTile.Depth = newViewerTile.Depth;
				oldViewerTile.StyleId = newViewerTile.StyleId;
				oldViewerTile.Data = newViewerTile.Data;

			}
			else
			{

				// If the viewerTile is outside the bounds of the document size then the size needs to be recalculated.
				if (!this.rectangleF.Contains(newViewerTile.RectangleF))
					backgroundContext.HasSizeChanged = true;

				// This will initialize the animation sequence for a new tile.  Even tiles without animation are given a simple
				// sequence involving a single style so that all tiles -- animated and static -- have the same logic.
				this.styleTable[newViewerTile.StyleId].Animation.SetSequence(newViewerTile);

				// This will add the viewerTile to the viewer.
				this.tileTable.Add(newViewerTile.TileId, newViewerTile);

				// This will add the viewerTile's area and that of the immediate neighbors to the list of areas that need to be 
				// redrawn.
				backgroundContext.UpdateList.Add(RectangleF.Inflate(newViewerTile.RectangleF, DocumentViewer.selectionInterior,
					DocumentViewer.selectionInterior));

			}

		}

		/// <summary>
		/// Generates a command that will redraw the invalid sections of the screen.
		/// </summary>
		/// <param name="viewerObject">Unused.</param>
		/// <param name="backgroundContext">The context data for maintaining the viewer area.</param>
		public void InvalidateDocument(ViewerObject viewerObject, BackgroundContext backgroundContext)
		{

			// As viewerTiles are added, removed or resized they have the potential to change the size of the virtual document. This
			// value will be passed to a foreground thread to handle each batch of updates to the document.  If this size isn't the
			// same as the known size of the document, then all the metrics concerning the document will be recalculated.
			RectangleF documentRectangle = this.rectangleF;

			// After handling the update of a range of values, recalculate the size of the document.  If the size of the document
			// changes, then the metrics (scroll bars) will be recalcualted.
			if (backgroundContext.HasSizeChanged)
			{
				IEnumerator<Tile> viewerTileEnumerator = this.tileTable.Values.GetEnumerator();
				if (viewerTileEnumerator.MoveNext())
				{
					documentRectangle = viewerTileEnumerator.Current.RectangleF;
					while (viewerTileEnumerator.MoveNext())
						documentRectangle = RectangleF.Union(documentRectangle, viewerTileEnumerator.Current.RectangleF);
				}
			}

			// This thread acts as a proxy to send the invalid areas to the foreground after the locks have been release.  The
			// 'Invoke' methods can't be called while the display lock is in place because it can force a deadlock when the paint
			// operation tries to obtain the lock.  That is, the message loop thread is locked waiting for the display lock and the
			// background thread is locked waiting for the 'Invoke' statement to complete.
			ThreadPool.QueueUserWorkItem(CalculateInvalidRegion, new object[] { documentRectangle, backgroundContext.UpdateList });

			// This will clear the background context for the next batch of viewer modification commands.
			backgroundContext.HasSizeChanged = false;
			backgroundContext.UpdateList = new List<RectangleF>();

		}

		/// <summary>
		/// Background thread used as a proxy to pass the invalid area to the foreground outside the multithreading locks.
		/// </summary>
		/// <param name="state">The thread parameters.</param>
		private void CalculateInvalidRegion(object state)
		{

			// Extract the thread parameters.
			object[] parameters = state as object[];
			RectangleF documentRectangle = (RectangleF)parameters[0];
			List<RectangleF> updateList = (List<RectangleF>)parameters[1];

			// Call the foreground to invalidate the rectangles that are calcualted above.  Also, provide the previous size of the
			// document to be used to determine if the world coordinates have changed.
			BeginInvoke(new ResizeDelegate(ResizeDocument), new object[] { documentRectangle });
			BeginInvoke(new InvalidRegionDelegate(InvalidateRegion), new object[] { updateList });

		}

		/// <summary>
		/// Called from a background thread resize the document.
		/// </summary>
		/// <param name="updateRectangle">The calculated size of the document.</param>
		private void ResizeDocument(RectangleF updateRectangle)
		{

			// Recalibrate the scrollbars, scrolling area, painting offsets and all the other screen metrics when the size of the
			// document has changed.
			if (this.rectangleF != updateRectangle)
			{
				this.rectangleF = updateRectangle;
				Recalibrate();
			}

		}
		
		/// <summary>
		/// Called from a background thread to redraw the document with a list of invalid areas.
		/// </summary>
		/// <param name="updateList">A list of rectangles that have been invalidated.</param>
		private void InvalidateRegion(List<RectangleF> updateList)
		{

			// The document is managed in world coordinates, however, the paint event describes an area to be painted in device
			// coordinates using the clip rectangle.  This operation will create a clip rectangle in world coordinates that
			// describes the areas of the document that must be redrawn.
			RectangleF worldWindow = new RectangleF(0.0f, 0.0f, (float)this.ClientRectangle.Width / this.scaleFactor,
				(float)this.ClientRectangle.Height / this.scaleFactor);

			// The screen is divided up into quadrants for the purpose of scrolling and column/row headings.  Similar to the Excel
			// feature, the top and left side of the screen can be frozen.  When the rest of the document is scrolled, the frozen
			// areas remain fixed with respect to their vertical or horizontal anchors.  That is, the column headers will not move
			// when the document is scrolled vertically, the row headers will not move when the document is scrolled horizontally.
			// And the upper, left hand corner of the window will never move.  The calcuation below will create four regions,
			// adjusted to the world coordinates of each of these quadrants.  These rectangles are used to calculate what part of
			// the screen, in device units, needs to be redrawn based on the updated rectangles which are managed in document
			// coordinates.
			RectangleF[] windows = new RectangleF[4];
			windows[0] = RectangleF.Intersect(quadrants[0], new RectangleF(new PointF(this.rectangleF.X + worldWindow.X,
				this.rectangleF.Y + worldWindow.Y), worldWindow.Size));
			windows[1] = RectangleF.Intersect(quadrants[1], new RectangleF(new PointF(this.virtualOffset.X - this.splitSize.Width +
				worldWindow.X, this.rectangleF.Y + worldWindow.Y), worldWindow.Size));
			windows[2] = RectangleF.Intersect(quadrants[2], new RectangleF(new PointF(this.rectangleF.X + worldWindow.X,
				this.virtualOffset.Y - this.splitSize.Height + worldWindow.Y), worldWindow.Size));
			windows[3] = RectangleF.Intersect(quadrants[3], new RectangleF(new PointF(this.virtualOffset.X - this.splitSize.Width +
				worldWindow.X, this.virtualOffset.Y - this.splitSize.Height + worldWindow.Y), worldWindow.Size));

			// This will scale and offset each of the rectangles that need to be redrawn for the device.  A single transform on the
			// region would be faster, but there are four transforms that must be done depending on which quadrant contains the
			// invalid rectangle.
			Region region = new Region(RectangleF.Empty);
			foreach (RectangleF updateArea in updateList)
				for (int quadrant = 0; quadrant < 4; quadrant++)
					if (windows[quadrant].IntersectsWith(updateArea))
					{
						RectangleF rectangleF = new RectangleF((updateArea.X + offsets[quadrant].X) * this.scaleFactor,
							(updateArea.Y + offsets[quadrant].Y) * this.scaleFactor,
							updateArea.Width * this.scaleFactor,
							updateArea.Height * this.scaleFactor);
						region.Union(rectangleF);
					}

			// Send the invalid area of the screen to the GDI+ to be repainted.
			Invalidate(region);

		}

		/// <summary>
		/// Redraws the document.
		/// </summary>
		public override void Refresh()
		{

			// This allows this method to be called from any thread.  The Windows Message Loop thread will not want to wait for
			// this method to complete, so a worker thread is queued to handle the refresh task.
			if (this.InvokeRequired)
				RefreshThread(null);
			else
				ThreadPool.QueueUserWorkItem(new WaitCallback(RefreshThread));

		}

		/// <summary>
		/// Background thead used to redraw the document.
		/// </summary>
		/// <param name="parameter">Unused thread initialization parameter.</param>
		private void RefreshThread(object parameter)
		{

			try
			{

				// Prevent other threads from accessing the document data while the view is rebuilt.
				this.documentLock.AcquireWriterLock(Timeout.Infinite);

				// The main idea of the refresh is to run around and collect up all the modified viewerTiles and erase the ones 
				// that are no longer part of the document.  Two flags are needed on the viewerTiles to calculate this state.  The
				// 'IsVisible' flag is cleared on each element in the document.  The viewerTiles that remain invisible after the
				// 'BuildView' will be purged from the viewer.  Similarly, the viewerTiles that have been modified after the
				// document is built will be sent to the viewer through the queue to be updated or added.
				foreach (ViewerTile viewerTile in this.viewerTileList)
				{
					viewerTile.IsObsolete = true;
					viewerTile.IsModified = false;
				}

				// This will recursively build the document, calculate which viewerTiles need to be updated and determine the 
				// position of the viewerTiles in the document.
				PointF cursor = PointF.Empty;
				Measure(this.documentView.BuildView(), ref cursor);

				// At this point, all the viewerTiles in the visible document have been added to the queue and will be processed 
				// by the viewer in turn.  This code will clean up the viewerTiles that are no longer part of the viewer.
				foreach (ViewerTile viewerTile in this.viewerTileList)
					if (viewerTile.IsObsolete)
					{
						this.ViewerCommandQueue.Enqueue(new ViewerCommand(new Tile(viewerTile), DeleteTile));
						viewerTile.RectangleF = RectangleF.Empty;
					}

				// This will force any invalid areas of the screen to be updated once all the viewerTiles have been processed.
				this.ViewerCommandQueue.Enqueue(new ViewerCommand(null, InvalidateDocument));

			}
			finally
			{

				// Allow other threads to access the document data.
				this.documentLock.ReleaseWriterLock();

			}

			// This will force the viewer to re-evaluate the cursor shown.  It is possible when moving columns around and other
			// such drag-and-drop operations that the current cursor is no longer valid.  For example, when a column is resized the
			// drag-and-drop cursor is a vertical split.  After the final column width is chosen with the mouse operation, chances
			// are the cursor is over the body of a column heading which displays as a down arrow.  However, when the document
			// recompiles, the position where the cursor is will, by definition, have a column boundary, which should always use
			// the split cursor.
			if (this.IsHandleCreated)
				BeginInvoke(new EventHandler(PingCursor), new object[] { this, EventArgs.Empty });

		}

		/// <summary>
		/// Forces an update 
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void PingCursor(object sender, EventArgs eventArgs)
		{

			// Pick up the latest location of the mouse and and force the viewer to reevaluate which cursor should be displayed.
			Point mouseLocation = this.PointToClient(Control.MousePosition);
			if (this.ClientRectangle.Contains(mouseLocation))
				OnMouseMove(new System.Windows.Forms.MouseEventArgs(Control.MouseButtons, 0, mouseLocation.X, mouseLocation.Y, 0));

		}

		/// <summary>
		/// Provides absolute coordinates to the viewerTiles in the document.
		/// </summary>
		/// <param name="viewerTable">A table of rows.</param>
		/// <param name="cursor">The document coordinates.</param>
		protected void Measure(ViewerTable viewerTable, ref PointF cursor)
		{

			// This will take each viewerTile in each row of the table and give it an absolute location in the document.
			foreach (ViewerRow viewerRow in viewerTable)
			{

				// Give each of the viewerTiles in the row an absolute position.  Then take all the modified viewerTiles and pass them through
				// to the viewer using the queue.
				foreach (ViewerTile viewerTile in viewerRow.Tiles)
				{

					// Every viewerTile that makes it here is part of the visible viewer.  At the end of the refresh cycle, all 
					// the viewerTiles that are not visible will be purged from the viewer.
					viewerTile.IsObsolete = false;

					// If the current location doesn't agree with the viewerTile, then the viewerTile has moved.
					if (cursor != viewerTile.RectangleF.Location)
					{
						viewerTile.RectangleF.Location = cursor;
						viewerTile.IsModified = true;
					}

					// If the viewerTile has been modified, then it is placed in a queue.  The viewer has a background thread that 
					// pulls these instructions out and executes them.  This is the primary mechanism for updating only the parts
					// of the screen that have changed.
					if (viewerTile.IsModified)
						this.ViewerCommandQueue.Enqueue(new ViewerCommand(new Tile(viewerTile), UpdateTile));

					// The viewerTile contains instructions on where the next cell will be located relative to this one.
					cursor += viewerTile.Cursor;

				}

				// Each row can have zero or more relations to child tables.  These are handled recursively with the cursor keeping
				// track of the location all the way down into the node structures.
				if (viewerRow.Children != null)
					foreach (ViewerTable childTable in viewerRow.Children)
						Measure(childTable, ref cursor);

			}

		}

		/// <summary>
		/// Handles a change to the size of the viewer's exposed area on the device.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnResize(EventArgs e)
		{

			// Recalculate the scrolling and drawing metrics of the viewer based on the new window size.
			Recalibrate();

			// Allow the base class to handle the rest of the event.
			base.OnResize(e);

		}

		/// <summary>
		/// Recalculate the scrolling and drawing metrics of the viewer.
		/// </summary>
		public void Recalibrate()
		{

			// The entire window will be redrawn if any of the viewer is exposed by recalculating the metrics.
			PointF originalOffset = this.virtualOffset;

			// The scroll bars can either appear or disappear during this operation.  This will force them to update as a single
			// operation when everything has been calculated.
			this.SuspendLayout();

			// The usable window height and width will change if scroll bars are present.  This calculation is somewhat iterative
			// in that the presence of a vertical scroll bar can alter the need for a horizontal scroll bar and visa-verca.  The
			// basic idea here is to convert the visible area on the screen to world coordinates (that involves the magnification
			// factor) and to remove the space occupied by the scroll bars.
			float magnificationFactor = this.scaleFactor == 0.0f ? 1.0f : this.scaleFactor;
			float windowHeight = Convert.ToSingle(this.ClientRectangle.Height) / magnificationFactor;
			float windowWidth = Convert.ToSingle(this.ClientRectangle.Width) / magnificationFactor;
			float hScrollHeight = Convert.ToSingle(this.hScrollBar.Height) / magnificationFactor;
			float vScrollWidth = Convert.ToSingle(this.vScrollBar.Width) / magnificationFactor;
			if (this.rectangleF.Size.Height > windowHeight)
			{
				windowWidth = windowWidth - vScrollWidth;
				if (this.rectangleF.Size.Width > windowWidth)
					windowHeight = windowHeight - hScrollHeight;
			}
			else
			{
				if (this.rectangleF.Size.Width > windowWidth)
					windowHeight = windowHeight - hScrollHeight;
				if (this.rectangleF.Size.Height > windowHeight)
					windowWidth = windowWidth - vScrollWidth;
			}
			windowWidth = windowWidth < 0.0f ? 0.0f : windowWidth;
			windowHeight = windowHeight < 0.0f ? 0.0f : windowHeight;

			// The operating parameters for the vertical scroll bar.  The most important concept here is that the value of the
			// scroll bar is the same as the offset for top edge the vertical scrolling pane.  The minimum and maximum represent
			// the total area that can be scrolled in document coordinates.
			float splitHeight = this.splitSize.Height > this.rectangleF.Height ? 0.0f : this.splitSize.Height;
			float scrollableHeight = windowHeight < splitHeight ? 0.0f : windowHeight - splitHeight;
			this.virtualOffset.Y = this.virtualOffset.Y < splitHeight || scrollableHeight > this.rectangleF.Height ? splitHeight :
				this.virtualOffset.Y + scrollableHeight >= this.rectangleF.Height && this.rectangleF.Size.Height >= windowHeight ?
				this.rectangleF.Height - scrollableHeight : this.virtualOffset.Y;
			this.vScrollBar.Minimum = Convert.ToInt32(splitHeight);
			this.vScrollBar.Maximum = Convert.ToInt32(this.rectangleF.Height);
			this.vScrollBar.SmallChange = Convert.ToInt32(DefaultDocument.RowHeight * magnificationFactor);
			this.vScrollBar.LargeChange = Convert.ToInt32(scrollableHeight);
			this.vScrollBar.Value = Convert.ToInt32(this.virtualOffset.Y);
			this.vScrollBar.Visible = windowHeight < this.rectangleF.Size.Height;
			this.vScrollBar.Left = this.ClientRectangle.Width - this.vScrollBar.Width;
			this.vScrollBar.Height = windowWidth >= this.rectangleF.Size.Width ? this.ClientRectangle.Height :
				this.ClientRectangle.Height - this.hScrollBar.Height;

			// The operating parameters for the horizontal scroll bar.  The most important concept here is that the value of the
			// scroll bar is the same as the offset for left edge the horizontal scrolling pane.  The minimum and maximum represent
			// the total area that can be scrolled in document coordinates.
			float splitWidth = this.splitSize.Width > this.rectangleF.Width ? 0.0f : this.splitSize.Width;
			float scrollableWidth = windowWidth < splitWidth ? 0.0f : windowWidth - splitWidth;
			this.virtualOffset.X = this.virtualOffset.X < splitWidth || scrollableWidth > this.rectangleF.Width ? splitWidth :
				this.virtualOffset.X + scrollableWidth >= this.rectangleF.Width && this.rectangleF.Size.Width >= windowWidth ?
				this.rectangleF.Width - scrollableWidth : this.virtualOffset.X;
			this.hScrollBar.Minimum = Convert.ToInt32(splitWidth);
			this.hScrollBar.Maximum = Convert.ToInt32(this.rectangleF.Width);
			this.hScrollBar.Value = Convert.ToInt32(this.virtualOffset.X);
			this.hScrollBar.SmallChange = Convert.ToInt32(DefaultDocument.ColumnWidth * magnificationFactor);
			this.hScrollBar.LargeChange = Convert.ToInt32(scrollableWidth);
			this.hScrollBar.Visible = windowWidth < this.rectangleF.Size.Width;
			this.hScrollBar.Top = this.ClientRectangle.Bottom - this.hScrollBar.Height;
			this.hScrollBar.Width = windowHeight >= this.rectangleF.Size.Height ? this.ClientRectangle.Width :
				this.ClientRectangle.Width - this.vScrollBar.Width;

			// This is the panel at the lower, right hand part of the screen and is visible only when both the vertical and
			// horizontal scroll bars are present.
			this.panel.Visible = windowWidth < this.rectangleF.Size.Width && windowHeight < this.rectangleF.Size.Height;

			// The quadrants represent the four areas created by the split screen effect.  This calculates the areas of each in
			// document coordinates.
			this.quadrants = new RectangleF[4];
			this.quadrants[0] = new RectangleF(this.rectangleF.Location, this.splitSize);
			this.quadrants[1] = new RectangleF(this.virtualOffset.X, this.rectangleF.Y, float.PositiveInfinity, this.splitSize.Height);
			this.quadrants[2] = new RectangleF(this.rectangleF.X, this.virtualOffset.Y, this.splitSize.Width,
				float.PositiveInfinity);
			this.quadrants[3] = new RectangleF(this.virtualOffset.X, this.virtualOffset.Y,
				float.PositiveInfinity, float.PositiveInfinity);

			// These are the offsets to the start of each of the quadrants taking into account how far the panes have been 
			// scrolled.
			this.offsets = new PointF[4];
			this.offsets[0] = new PointF(-(this.rectangleF.X), -(this.rectangleF.Y));
			this.offsets[1] = new PointF(-(this.virtualOffset.X - this.splitSize.Width), -(this.rectangleF.Y));
			this.offsets[2] = new PointF(-(this.rectangleF.X), -(this.virtualOffset.Y - this.splitSize.Height));
			this.offsets[3] = new PointF(-(this.virtualOffset.X - this.splitSize.Width), -(this.virtualOffset.Y - this.splitSize.Height));

			// This will redraw the entire contents of the viewer if any of the operations above required the virtual position in
			// the document to change.
			if (originalOffset != this.virtualOffset)
				Invalidate();

			// This will allow all the controls to update in a single operation now that all the controls have been set to reflect 
			// the state of the document in the viewer.
			ResumeLayout();

		}

		/// <summary>
		/// Handles a change in the value of the vertical scroll bar.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
		{

			// There is no need to use the EndScroll event as all scroll bar events are handled as they are raised.  This event is
			// always raised at the end of a scroll bar change, so it needs to be filtered out of the stream of events to prevent
			// updating the window twice.
			if (e.Type != ScrollEventType.EndScroll)
			{

				// The vertical scroll bar is calibrated to the document coordinate system.  The value in the scroll bar describes 
				// the amount the pane is offset into the document to provide its view.
				this.virtualOffset.Y = e.NewValue;

				// The offsets for the quadrants need to be recalculated when the virtual offset is changed.
				Recalibrate();

				// Redrawing the screen with the new metrics will make it appear to scroll smoothly as the scroll bar value changes.
				Invalidate();

			}

		}

		/// <summary>
		/// Handles a change in the value of the horizontal scroll bar.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
		{

			// There is no need to use the EndScroll event as all scroll bar events are handled as they are raised.  This event is
			// always raised at the end of a scroll bar change, so it needs to be filtered out of the stream of events to prevent
			// updating the window twice.
			if (e.Type != ScrollEventType.EndScroll)
			{

				// The horizontal scroll bar is calibrated to the document coordinate system.  The value in the scroll bar 
				// describes the amount the pane is offset into the document to provide its view.
				this.virtualOffset.X = e.NewValue;

				// The offsets for the quadrants need to be recalculated when the virtual offset is changed.
				Recalibrate();

				// Redrawing the screen with the new metrics will make it appear to scroll smoothly as the scroll bar value changes.
				Invalidate();

			}

		}

		/// <summary>
		/// Creates the C# source, compiles and loads into memory the document view.
		/// </summary>
		public void Compile()
		{

			// This will compile the view in the background if called from a Windows Message thread.
			if (this.InvokeRequired)
				CompileThread(null);
			else
				ThreadPool.QueueUserWorkItem(new WaitCallback(CompileThread));

		}

		/// <summary>
		/// Creates the C# source, compiles and loads into memory the document view.
		/// </summary>
		private void CompileThread(object parameter)
		{

			try
			{

				// Prevent other threads from accessing the document while the view is constructed.
				this.documentLock.AcquireWriterLock(Timeout.Infinite);

				// This will create the source code from the description of how to transform the data.
				StringWriter stringWriter = new StringWriter();
				CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
				codeCompileUnit.Namespaces.Add(new DataTransformNamespace(this.dataTransform));
				this.codeDomProvider.GenerateCodeFromCompileUnit(codeCompileUnit, stringWriter, codeGeneratorOptions);

#if DEBUG
				// Write the generated C# code out to a file during debug mode.
				StreamWriter streamWriter = new StreamWriter(string.Format("../../{0}.cs", this.dataTransform.DataTransformId));
				streamWriter.Write(stringWriter);
				streamWriter.Close();
#endif

				// Compile the source code and load it into memory.
				MetaAssembly metaAssembly = new MetaAssembly();
				metaAssembly.OutputAssembly = string.Format("{0}.dll", this.dataTransform.DataTransformId);
				metaAssembly.OutputType = string.Format("{0}.{1}", this.dataTransform.TargetNamespace, this.dataTransform.DataTransformId);
				metaAssembly.CompilerOptions = DocumentViewer.compilerOptions;
				metaAssembly.GenerateExecutable = DocumentViewer.generateExecutable;
				metaAssembly.GenerateInMemory = DocumentViewer.generateInMemory;
				metaAssembly.IncludeDebugInformation = DocumentViewer.includeDebugInformation;
				metaAssembly.SourceCode = new string[] { stringWriter.ToString() };
				metaAssembly.Dependancies = new string[] {"System.dll", "System.Data.dll", "System.Drawing.dll",
				"System.Windows.Forms.dll", "System.Xml.dll", "Mark Three Forms.dll", "Mark Three Library.dll",
				"Document Viewer.dll", Assembly.GetEntryAssembly().Location};
				this.documentView = CSharpCompiler.Compile(metaAssembly, this) as DocumentView;

				// Rebuild the styles in this document.  This only needs to be done after the document is compiled.  That is, it
				// shouldn't be done when just the data is changing such as during a refresh.
				InitializeView();

			}
			catch (Exception exception)
			{

				// Log any error compiling the code.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Allow other threads to access the document data.
				this.documentLock.ReleaseWriterLock();

			}

			// Refresh the contents of the document in the viewer once it has compiled.
			RefreshThread(null);

		}

		/// <summary>
		/// Initializes the view of the document.
		/// </summary>
		public void InitializeView()
		{

			// The styles are preserved through a compilation of the View.  When a new View is ready, it will attempt to update the
			// styles.  Those styles that haven't changed from the original ones should not be forced back into the viewer because
			// it will cause all the viewerTiles that depend on those styles to redraw.  To prevent this performance drain each of 
			// the sytles is tested to see if it has changed since the last time this method was called.
			foreach (ViewerStyle viewerStyle in this.viewerStyleList)
				viewerStyle.IsModified = false;

			// Build/Rebuild the styles.
			this.documentView.InitializeView();

			// Any of the styles that have been modified are send to the viewer to be added or updated.
			foreach (ViewerStyle viewerStyle in this.viewerStyleList)
				if (IsViewerStyleModified(viewerStyle))
					this.ViewerCommandQueue.Enqueue(new ViewerCommand(viewerStyle.Clone(), UpdateStyle));

		}

		/// <summary>
		/// Recursively tests whether a style or its parent has been modified
		/// </summary>
		/// <param name="viewerStyle">The style to be tested.</param>
		/// <returns>True if the style has been updated, false otherwise.</returns>
		public bool IsViewerStyleModified(ViewerStyle viewerStyle)
		{

			// Unwind the recursion when either a modified ancestor is found, or there are no more ancestors to check.
			return viewerStyle.IsModified ? true : viewerStyle.ParentId == string.Empty ? false :
				IsViewerStyleModified(this.viewerStyleTable[viewerStyle.ParentId]);

		}

		/// <summary>
		/// Finds or creates a style from a common table.
		/// </summary>
		/// <param name="styleId">The name of the style.</param>
		/// <returns>A unique style from the common table of ViewerStyles.</returns>
		public ViewerStyle GetStyle(string styleId)
		{

			ViewerStyle viewerStyle;

			// This will create a new style if one doesn't already exist in the pool of styles.  Note that there is an parallel 
			// list of styles that is used for simple iteration.
			if (!this.viewerStyleTable.TryGetValue(styleId, out viewerStyle))
			{
				viewerStyle = new ViewerStyle(styleId);
				this.viewerStyleTable.Add(styleId, viewerStyle);
				this.viewerStyleList.Add(viewerStyle);
			}

			// This value will always be the same for a given styleId.
			return viewerStyle;

		}

		/// <summary>
		/// Finds a viewer viewerTile based on a unique DataRow and Column address.
		/// </summary>
		/// <param name="dataRow">The DataRow associated with the ViewerTile.</param>
		/// <param name="viewerTileId">The column associated with the ViewerTile.</param>
		/// <returns>A unique ViewerTile associated with the given address.</returns>
		public ViewerTile GetTile(DataRow dataRow, int viewerTileId)
		{

			ViewerTile viewerTile;
			Dictionary<int, ViewerTile> columnMap;

			// The Tiles are organized in Dictionaries.  The first one maps the DataRow to a second hashtable which maps the column
			// ordinal to the viewerTile.  These dimensions are created on the fly as new DataRows or column ordinals are made 
			// part of the document.  If a viewerTile has been associated with the given DataRow and column ordinal, it is
			// returned.  Otherwise a new one is created and associated with that unique address.
			if (!this.rowColumnMap.TryGetValue(dataRow, out columnMap))
			{
				columnMap = new Dictionary<int, ViewerTile>();
				this.rowColumnMap.Add(dataRow, columnMap);
			}
			if (!columnMap.TryGetValue(viewerTileId, out viewerTile))
			{
				viewerTile = new ViewerTile();
				viewerTile.ViewerTileId = this.viewerTileCounter++;
				columnMap.Add(viewerTileId, viewerTile);
				this.viewerTileList.Add(viewerTile);
			}

			// This value will always be the same for a given combination of DataRow, ColumnOrdinal.
			return viewerTile;

		}

		/// <summary>
		/// Gets or sets the instructions for transforming data into viewer commands.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public XmlDocument XmlDataTransform
		{

			get
			{

				// The DataTransform will be written to this XML Document.
				XmlDocument xmlDocument = new XmlDocument();

				try
				{

					// Read the data into the XML structure while other threads are prevented from accessing the document data.
					this.documentLock.AcquireReaderLock(Timeout.Infinite);
					DataTransformReader dataTransformReader = new DataTransformReader(this.DataTransform);
					xmlDocument.Load(dataTransformReader);

				}
				finally
				{

					// Allow other threads to access the document data.
					this.documentLock.ReleaseReaderLock();

				}

				// This is the XML version of the current set of instructions to transform the data into viewer commands.
				return xmlDocument;

			}

			set
			{

				try
				{

					// Lock the document while the XML is interpreted into data transformation instructions.
					this.documentLock.AcquireWriterLock(Timeout.Infinite);
					XmlDataTransformWriter xmlDataTransformWriter = new XmlDataTransformWriter(this.dataTransform);
					value.WriteTo(xmlDataTransformWriter);

				}
				finally
				{

					// Other threads can now access the document data.
					this.documentLock.ReleaseWriterLock();

				}

			}

		}

		/// <summary>
		/// Allows the user to select and order the columns in a viewer.
		/// </summary>
		public void ChooseColumns()
		{

			// Initialize the column selection form with the current order of the columns and display the dialog.  This will give 
			// the user the chance to order and show/hide any of the columns.
			ColumnSelector columnSelector = new ColumnSelector(this.dataTransform);
			if (columnSelector.ShowDialog(this) == DialogResult.OK)
			{
				this.dataTransform = columnSelector.DataTransform;
				Compile();
			}

		}

		/// <summary>
		/// Select all the tiles in the document.
		/// </summary>
		public void SelectAll()
		{

			try
			{

				// The display must be locked while the tiles are selected.
				this.displayLock.AcquireWriterLock(Timeout.Infinite);

				// This will select every tile in the document and make the one closest to the split the active (input) tile.
				foreach (ViewerTile viewerTile in this.viewerTileList)
				{
					viewerTile.IsSelected = true;
					viewerTile.IsActive = viewerTile.RectangleF.Contains(this.virtualOffset);
					this.ViewerCommandQueue.Enqueue(new ViewerCommand(new Tile(viewerTile), UpdateTile));
				}

				// This will force the viewer to redraw the invalid sections of the viewer.
				this.ViewerCommandQueue.Enqueue(new ViewerCommand(null, InvalidateDocument));

			}
			finally
			{

				// The display can now be accessed by other threads.
				this.displayLock.ReleaseWriterLock();

			}

		}

		/// <summary>
		/// Compile and load the view from the DataTransform.
		/// </summary>
		/// <param name="parameter">Unused thread initialization parameter.</param>
		public virtual void LoadView(object parameter)
		{

			// This will take the DataTransform and use it to construct a DOM of the code needed to transform the data into a
			// document.  The DOM is then compiled and loaded into memory and a refresh forces an update of the document in the
			// viewer.
			Compile();

		}

		/// <summary>
		/// The four quadrants of the client window in device coordinates.
		/// </summary>
		/// <returns>An array of the four quadrants of the client area after it is split.</returns>
		private RectangleF[] SplitScreen()
		{

			// The coordinates of this property are device coordinates but they are calculated in floating point numbers to
			// preserve the accuracy during the scaling operations.
			SizeF deviceSplit = new SizeF(this.splitSize.Width * this.scaleFactor, this.splitSize.Height * this.scaleFactor);

			// Convert the client area into floating point coordinates.
			RectangleF clientRectangle = (RectangleF)this.ClientRectangle;

			// The document is divided up into four quadrants with three having the ability to scroll.  When converting between
			// device and document coordinates, the first step is to know which quadrant of the screen is the target of the
			// operation.  This divides the client area of the window into the four quadrants.
			RectangleF[] section = new RectangleF[4];
			section[0] = new RectangleF(clientRectangle.Left, clientRectangle.Top, deviceSplit.Width, deviceSplit.Height);
			section[1] = new RectangleF(clientRectangle.Left + deviceSplit.Width, clientRectangle.Top,
				clientRectangle.Width - deviceSplit.Width, deviceSplit.Height);
			section[2] = new RectangleF(clientRectangle.Left, clientRectangle.Top + deviceSplit.Height, deviceSplit.Width,
				clientRectangle.Height - deviceSplit.Height);
			section[3] = new RectangleF(clientRectangle.Left + deviceSplit.Width, clientRectangle.Top + deviceSplit.Height,
				clientRectangle.Width - deviceSplit.Width, clientRectangle.Height - deviceSplit.Height);

			// Note that the units of these four rectangles are device units.  This array of rectangles can be used to determine
			// which quadrant should handle a device event.
			return section;

		}

		/// <summary>
		/// Raises the MouseDown event for the document header quadrant.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnHeaderMouseDown(QuadrantMouseEventArgs quadrantMouseEventArgs)
		{

			// Cycle through all the tiles in the document and select them.  The very first tile after the split is going to get
			// the input focus.
			foreach (ViewerTile viewerTile in this.viewerTileList)
			{
				viewerTile.IsSelected = true;
				viewerTile.IsActive = viewerTile.RectangleF.Contains(this.virtualOffset);
				this.ViewerCommandQueue.Enqueue(new ViewerCommand(new Tile(viewerTile), UpdateTile));
			}

			// This will force the viewer to redraw the invalid sections of the viewer.
			this.ViewerCommandQueue.Enqueue(new ViewerCommand(null, InvalidateDocument));

		}

		/// <summary>
		/// Raises the MouseDown event for the document column header quadrant.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnColumnHeaderMouseDown(QuadrantMouseEventArgs quadrantMouseEventArgs)
		{

			// Evaluate the state of the keyboard.  Key combinations involving the shift and control keys will alter the areas that
			// are selected.
			bool isShiftKeyPressed = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
			bool isControlKeyPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;

			// The main idea in handling the column header is to find the tile that was selected by the mouse.  Then determine the
			// action (is the column size being changed?  Is the column being moved?  Should the column be resorted?).  The "mouse
			// down" event is the place to initialize the variables that keep track of these actions.
			foreach (ViewerTile viewerTile in this.viewerTileList)
				if (viewerTile.RectangleF.Contains(quadrantMouseEventArgs.Location))
				{

					// The column can be selected by either the right or left mouse button.  The left button can also be used to
					// change the size of the column.  After considering the state of the mouse buttons, this variable indicates
					// that the column should be tested to see if it can be selected and, if it is, it should be selected.
					bool isColumnSelected = false;

					// The mouse buttons determine the action to take.
					switch (quadrantMouseEventArgs.Button)
					{

					case MouseButtons.Right:

						// This will signal that the column should be tested to see if it can be selected below.
						isColumnSelected = true;
						break;

					case MouseButtons.Left:

						// This is a 'Hit Test' for the right edge of the column header tile to see if the user is trying to change
						// the size of the column.  If the mouse is close to the right edge, then the drag operation to change the 
						// size of the tile is begun.
						if (new RectangleF(viewerTile.RectangleF.Right - DocumentViewer.splitBorder, viewerTile.RectangleF.Top,
							DocumentViewer.splitBorder, viewerTile.RectangleF.Height).Contains(quadrantMouseEventArgs.Location))
						{
							this.resizeEdge = viewerTile.RectangleF.Right;
							this.mouseState = MouseState.ResizingColumn;
							break;
						}

						// This is a 'Hit Test' for the left edge of the column header tile to see if the user is trying to change
						// the size of the column.  Note that the left edge of the column is used.  When the 'Mouse Up' action is
						// handled, a search is made for a tile that has a right edge that matches.  So this action, in a sense, is
						// selecting the previous tile in the header for the resizing operation.
						if (new RectangleF(viewerTile.RectangleF.Left, viewerTile.RectangleF.Top, DocumentViewer.splitBorder,
							viewerTile.RectangleF.Height).Contains(quadrantMouseEventArgs.Location))
						{
							this.resizeEdge = viewerTile.RectangleF.Left;
							this.mouseState = MouseState.ResizingColumn;
							break;
						}

						// At this point, neither the right or left edge has been selected.  Clicking in the remaining 'body' of 
						// the header tile begins an operation to select or sort the column.
						isColumnSelected = true;
						break;

					}

					// The DataTransform contains attributes on the column definitions that determine whether clicking on a column
					// header will sort the column or allow it to be selected.  Generally, read-only values will be in a sortable
					// column and user-entered fields will be in a selectable column.
					if (isColumnSelected)
					{

						// This state variable will control how the 'Mouse Move' and 'Mouse Up' event handlers interpret the user
						// action.  The 'selectedColumnTile' is also used as the starting point for any drag-and-drop type of
						// operations.
						this.mouseState = MouseState.ButtonDown;
						this.selectedColumnTile = viewerTile;

						// The presence of a 'SortTemplate' in the column definition is interpreted as an instruction to sort the
						// column instead of selecting it.  The absense of this attribute indicates that the column should be
						// selected 'Excel' style.
						DataTransform.ColumnNode columnNode = this.dataTransform.FindLeftEdge(viewerTile.RectangleF.Left).Column;
						if (columnNode.SortTemplate == string.Empty)
						{

							// This is an area that is calculated to intersect with all the selected tiles.  When the 'Shift' key
							// is pressed, the area includes everything from the anchor point to the current location of the
							// cursor.  All other combinations create effectively a point rectangle where the mouse is currently
							// located.
							RectangleF selectedRectangle = isShiftKeyPressed ? new RectangleF(
								quadrantMouseEventArgs.Location.X < this.anchorPoint.X ? quadrantMouseEventArgs.Location.X :
								this.anchorPoint.X, 0.0f, Math.Abs(quadrantMouseEventArgs.Location.X - this.anchorPoint.X), float.PositiveInfinity) :
								new RectangleF(quadrantMouseEventArgs.Location.X, 0.0f, 0.0f, float.PositiveInfinity);

							// The input tile is the first one directly under the anchor column.
							PointF activePoint = new PointF(this.anchorPoint.X, this.splitSize.Height + 1.0f);

							// This will select every tile that matches the horizontal location of the column header tile and
							// clear the selection of everything else.
							foreach (ViewerTile innerTile in this.viewerTileList)
							{

								bool isSelected;
								bool isActive;

								// The tiles that match the column header tile's horizontal position are selected.  The first tile
								// after the split is made to have the input focus.  All other tiles are removed from the selected
								// region.
								if (innerTile.RectangleF.IntersectsWith(selectedRectangle))
								{
									isSelected = true;
									isActive = innerTile.RectangleF.Contains(activePoint);
								}
								else
								{
									isSelected = innerTile.IsSelected && isControlKeyPressed;
									isActive = false;
								}

								// Any tile that has modified selection attributes is passed through the queue to the viewer to be 
								// updated.
								if (innerTile.IsSelected != isSelected || innerTile.IsActive != isActive)
								{
									innerTile.IsSelected = isSelected;
									innerTile.IsActive = isActive;
									this.ViewerCommandQueue.Enqueue(new ViewerCommand(new Tile(innerTile), UpdateTile));
								}

							}

							// This will force the viewer to redraw the invalid sections of the viewer.
							this.ViewerCommandQueue.Enqueue(new ViewerCommand(null, InvalidateDocument));

						}

					}

					// Thi will break out of the loop that searches the viewer for a tile that contains the mouse position.
					break;

				}

		}

		/// <summary>
		/// Raises the MouseDown event for the document column header quadrant.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnRowHeaderMouseDown(QuadrantMouseEventArgs quadrantMouseEventArgs)
		{

			// Evaluate the state of the keyboard.  Key combinations involving the shift and control keys will alter the areas that
			// are selected.
			bool isShiftKeyPressed = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
			bool isControlKeyPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;

			// The main idea in handling the row header is to find the tile that was selected by the mouse.  Rows are not as 
			// complicated as columns as there is no sorting or resizing operations that are allowed, so a row header is mean to
			// simply select the row.  The structure of the 'ColumnHeaderMouseDown' is kept here in case more complicated
			// processing of the row headers is required in the future.
			foreach (ViewerTile viewerTile in this.viewerTileList)
				if (viewerTile.RectangleF.Contains(quadrantMouseEventArgs.Location))
				{

					// The column can be selected by either the right or left mouse button.  The left button can also be used to
					// change the size of the column.  After considering the state of the mouse buttons, this variable indicates
					// that the column should be tested to see if it can be selected and, if it is, it should be selected.
					bool isRowSelected = false;

					// The mouse buttons determine the action to take.
					switch (quadrantMouseEventArgs.Button)
					{

					case MouseButtons.Right:

						// This will signal that the column should be tested to see if it can be selected below.
						isRowSelected = true;
						break;

					case MouseButtons.Left:

						// This will signal that the column should be tested to see if it can be selected below.
						isRowSelected = true;
						break;

					}

					// The DataTransform contains attributes on the column definitions that determine whether clicking on a column
					// header will sort the column or allow it to be selected.  Generally, read-only values will be in a sortable
					// column and user-entered fields will be in a selectable column.
					if (isRowSelected)
					{

						// This is an area that is calculated to intersect with all the selected tiles.  When the 'Shift' key is
						// pressed, the area includes everything from the anchor point to the current location of the cursor.  All
						// other combinations create effectively a point rectangle where the mouse is currently located.
						RectangleF selectedRectangle = isShiftKeyPressed ? new RectangleF(0.0f,
							quadrantMouseEventArgs.Location.Y < this.anchorPoint.Y ? quadrantMouseEventArgs.Location.Y :
							this.anchorPoint.Y, float.PositiveInfinity,
							Math.Abs(quadrantMouseEventArgs.Location.Y - this.anchorPoint.Y)) :
							new RectangleF(0.0f, quadrantMouseEventArgs.Location.Y, float.PositiveInfinity, 0.0f);

						// This is a point where the active (input) tile will be located.
						PointF activePoint = new PointF(this.splitSize.Width + 1.0f, this.anchorPoint.Y);
						
						// This will select every tile that matches the horizontal location of the column header tile and
						// clear the selection of everything else.
						foreach (ViewerTile innerTile in this.viewerTileList)
						{

							bool isSelected;
							bool isActive;

							// The tiles that match the column header tile's horizontal position are selected.  The first tile
							// after the split is made to have the input focus.  All other tiles are removed from the selected
							// region.
							if (innerTile.RectangleF.IntersectsWith(selectedRectangle))
							{
								isSelected = true;
								isActive = innerTile.RectangleF.Contains(activePoint);
							}
							else
							{
								isSelected = innerTile.IsSelected && isControlKeyPressed;
								isActive = false;
							}

							// Any tile that has modified selection attributes is passed through the queue to the viewer to be 
							// updated.
							if (innerTile.IsSelected != isSelected || innerTile.IsActive != isActive)
							{
								innerTile.IsSelected = isSelected;
								innerTile.IsActive = isActive;
								this.ViewerCommandQueue.Enqueue(new ViewerCommand(new Tile(innerTile), UpdateTile));
							}

						}

						// This will force the viewer to redraw the invalid sections of the viewer.
						this.ViewerCommandQueue.Enqueue(new ViewerCommand(null, InvalidateDocument));

					}

					// Thi will break out of the loop that searches the viewer for a tile that contains the mouse position.
					break;

				}

		}

		/// <summary>
		/// Raises the MouseDown event for the quadrant that contains the main body of tiles.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnTileMouseDown(QuadrantMouseEventArgs e)
		{

			// Evaluate the state of the keyboard.  Key combinations involving the shift and control keys will alter the areas that
			// are selected.
			bool isShiftKeyPressed = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
			bool isControlKeyPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;

			// The selection logic is based on the Microsoft Excel model.
			if (e.Button == MouseButtons.Left)
			{

				// This is the start of a dragging operation to select a range of cells.
				this.mouseState = MouseState.DraggingTile;

				// This is an area that is calculated to intersect with all the selected tiles.  When the 'Shift' key is pressed,
				// the area includes everything from the anchor point to the current location of the cursor.  All other
				// combinations create effectively a point rectangle where the mouse is currently located.
				RectangleF selectedRectangle = isShiftKeyPressed ?
					new RectangleF(e.Location.X < this.anchorPoint.X ? e.Location.X : this.anchorPoint.X,
					e.Location.Y < this.anchorPoint.Y ? e.Location.Y : this.anchorPoint.Y,
					Math.Abs(e.Location.X - this.anchorPoint.X), Math.Abs(e.Location.Y - this.anchorPoint.Y)) :
					new RectangleF(e.Location, Size.Empty);

				// Run through each tile to see if it should be selected or have the selection removed.
				foreach (ViewerTile viewerTile in this.viewerTileList)
				{

					bool isSelected;
					bool isActive;

					// This will select the tile if it is part of the selected area and clear the selection if it is not. The
					// active tile is the one that contains the anchor point.
					if (viewerTile.RectangleF.IntersectsWith(selectedRectangle))
					{
						isSelected = true;
						isActive = viewerTile.RectangleF.Contains(this.anchorPoint);
					}
					else
					{
						isSelected = viewerTile.IsSelected && isControlKeyPressed;
						isActive = false;
					}

					// As an optimization, only the tiles that been modified are passed to the viewer for updating.
					if (viewerTile.IsSelected != isSelected || viewerTile.IsActive != isActive)
					{
						viewerTile.IsSelected = isSelected;
						viewerTile.IsActive = isActive;
						this.ViewerCommandQueue.Enqueue(new ViewerCommand(new Tile(viewerTile), UpdateTile));
					}

				}

				// This will force the viewer to redraw the invalid sections of the viewer.
				this.ViewerCommandQueue.Enqueue(new ViewerCommand(null, InvalidateDocument));

			}

		}

		/// <summary>
		/// Handles the pressing of any of the mouse buttons.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{

			// This keeps track of where the mouse button was pressed.  Note that it is in device units.
			PointF clientMouseLocation = new PointF(Convert.ToSingle(e.X), Convert.ToSingle(e.Y));

			try
			{

				// Lock the document while the data is scanned.
				this.displayLock.AcquireReaderLock(Timeout.Infinite);
				this.documentLock.AcquireWriterLock(Timeout.Infinite);

				// The location of the mouse in document (world) coordinates will depend on the quadrant where the mouse is
				// located. Because three of the quadrants can scroll, each quadrant has its own offset into the document
				// coordinate system and its own way of dealing with the mouse. This will check each section of the screen to see
				// if it contains the mouse and then adjusting the location of the mouse for the virtual coordinate system of that
				// quadrant.  The end result is a mouse location in the proper world coordinates for the quadrant in which it is
				// found.  This will also call the handler of the "Mouse Down" action for the given quadrant.
				RectangleF[] section = SplitScreen();
				for (int quadrant = 0; quadrant < 4; quadrant++)
					if (section[quadrant].Contains(clientMouseLocation))
					{

						// This will calculate the quadrant that owns the mouse during the remaining mouse operations until the
						// button is relesased.  The 'mouseDownLocation' is the location of the mouse in document (world) 
						// coordinates.
						this.mouseDownQuadrant = quadrant;
						this.mouseDownLocation = new PointF(clientMouseLocation.X / this.scaleFactor - this.offsets[quadrant].X,
							clientMouseLocation.Y / this.scaleFactor - this.offsets[quadrant].Y);

						// The anchor point isn't moved when the shift key is held.  The anchor point is used to select a range of
						// tiles similar to the way Excel selects cells.
						if (((Control.ModifierKeys & Keys.Shift) != Keys.Shift))
							this.anchorPoint = this.mouseDownLocation;

						// Invoke the handler for the quadrant that owns the mouse.
						this.mouseDownHandler[quadrant](new QuadrantMouseEventArgs(e.Button, this.mouseDownLocation));

					}

			}
			catch (Exception exception)
			{

				// Write the exception to the event log.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// The lock on the document and the display are no longer needed.
				this.displayLock.ReleaseReaderLock();
				this.documentLock.ReleaseWriterLock();

			}

		}

		/// <summary>
		/// Raises the MouseMove event for the headers quadrant.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnHeaderMouseMove(QuadrantMouseEventArgs mouseEventArgs)
		{

			// The arrow cursor is selected for any movement inside the upper left quadrant.
			this.Cursor = Cursors.Arrow;

		}

		/// <summary>
		/// Raises the MouseMove event for the quadrant the column header quadrant.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnColumnHeaderMouseMove(QuadrantMouseEventArgs quadrantMouseEventArgs)
		{

			// The document is managed in world coordinates, however, the mouse event describes a events in device coordinates.
			// This operation will translate the client window into world coordinates that can be use for the 'hit test' operations
			// below.
			float width = Convert.ToSingle(this.vScrollBar.Visible ? this.ClientRectangle.Width - this.vScrollBar.Width :
				this.ClientRectangle.Width);
			float height = Convert.ToSingle(this.hScrollBar.Visible ? this.ClientRectangle.Height - this.hScrollBar.Height :
				this.ClientRectangle.Height);
			RectangleF worldWindow = new RectangleF((float)this.ClientRectangle.Left / this.scaleFactor,
				(float)this.ClientRectangle.Top / this.scaleFactor, width / this.scaleFactor, height / this.scaleFactor);
			RectangleF window = RectangleF.Intersect(this.quadrants[1], new RectangleF(new PointF(this.virtualOffset.X -
				this.splitSize.Width + worldWindow.X, this.rectangleF.Y + worldWindow.Y), worldWindow.Size));

			Point clientLocation = new Point(Convert.ToInt32((quadrantMouseEventArgs.Location.X + this.offsets[1].X) * this.scaleFactor),
				Convert.ToInt32((quadrantMouseEventArgs.Location.Y + this.offsets[1].Y) * this.scaleFactor));

			// The action taken by a mouse movement in the column heading of a viewer is driven by a set of states.  These states
			// are driven, in turn, by where the mouse started and what buttons are pressed.  They can instruct the viewer to 
			// resize the columns, move the column, delete the column or sort the column.
			switch (this.mouseState)
			{

			case MouseState.ResizingColumn:

				// This will test whether the horizontal position of the mouse is still inside the client area of the window.
				if (clientLocation.X >= this.ClientRectangle.Left && clientLocation.X <= this.ClientRectangle.Right)
				{

					// A reversible cursor line is drawn from the top of the panel to the bottom when resizing.  The trick to
					// making this line disappear is to draw it twice.  The first time compliments all the bits underneath it, the
					// second time compliments them back and restores the original state.  This first line will clear out the
					// previous line if it was drawn.
					if (this.lastLine != null)
						ControlPaint.DrawReversibleLine(this.lastLine[0], this.lastLine[1], System.Drawing.Color.DarkGray);
					else
						this.lastLine = new Point[2];

					// This gives the user feedback regarding the location of the new right edge of the selected column.  It draws
					// a vertical line over the client area at the current horizontal position of the mouse.
					this.lastLine[0] = this.PointToScreen(new Point(clientLocation.X, this.ClientRectangle.Top));
					this.lastLine[1] = this.PointToScreen(new Point(clientLocation.X, this.hScrollBar.Visible ?
						this.hScrollBar.Top : this.ClientRectangle.Bottom));
					ControlPaint.DrawReversibleLine(this.lastLine[0], this.lastLine[1], System.Drawing.Color.DarkGray);

				}

				break;

			case MouseState.ButtonDown:

				// When the user presses the left mouse button, they initiate some drag operation and the mouse activity is
				// captured by the column header window.  If the user is simply moving the mouse over the window, then feedback is
				// given in the shape of the cursor. This formula determins if the mouse has moved an absolute distance of four
				// pixels from the original location. If it has, the user has selected a movement operation for the column.
				// Otherwise, the mouse operation will be interpreted as a request for a new sort order when the left mouse button
				// is lifted.
				if (Math.Sqrt(Math.Pow(quadrantMouseEventArgs.Location.X - this.mouseDownLocation.X, 2.0) +
					Math.Pow(quadrantMouseEventArgs.Location.Y - this.mouseDownLocation.Y, 2.0)) > DocumentViewer.headerDragTrigger)
				{

					// At this point the mouse movements are intepreted as drag-and-drop operations for the column headers.  The
					// drop states determines what happens when the mouse button is released.  It can either be moved, deleted or
					// have no action taken.
					this.mouseState = MouseState.DraggingColumn;
					this.destinationState = DropAction.NoAction;

					// A column movement (or removal) operation is selected when the mouse has moved away from the original
					// location.  The destination cursor is a set of red arrows that straddle the header window.  Note that it is
					// important that this cursor is created before the column cursor to preserve the Z order. The destination
					// cursor should appear between the column header and the column cursor.
					this.destinationCursor = new DestinationCursor(this.splitSize.Height * this.scaleFactor);

					// This will create a floating window that contains an image of the selected column.  The effect is to make it
					// appear that the user has ripped the column header off the page and can move it where they please.  The
					// 'ColumnCursor' is basically a floating window with a paintBitmap image of the selected column header.
					this.columnCursor = new ColumnCursor(this.tileTable[this.selectedColumnTile.ViewerTileId],
						this.styleTable[this.selectedColumnTile.ViewerStyleId], this.scaleFactor);

					// Now that the column cursor has been created, it needs to be located on the screen with respect to the
					// current mouse position.  Remember that the column cursor window doesn't have a parent, so all coordinates
					// must be converted to screen coordinates.
					this.columnCursor.Location = this.PointToScreen(clientLocation);

				}

				break;

			case MouseState.DraggingColumn:

				// The cursor column is really a floating window, not a cursor.  It needs to be moved to match the location of the
				// mouse.  Note that the floating window doesn't have a parent, so the coordinates are in screen units.
				this.columnCursor.Location = this.PointToScreen(clientLocation);

				// If the window that contains the column headings contains the cursor, then it's possible that a destination is
				// selected for the column drag-and-drop operation.  If the cursor is outside of the header quadrant, the column
				// will be deleted when the mouse button is released.
				if (window.Contains(quadrantMouseEventArgs.Location))
				{

					// Any operation inside the visible header gets the basic pointing arrow for a cursor.
					this.Cursor = Cursors.Arrow;

					// When the mouse is inside the header quadrant but there is no destination selected, then nothing will happen
					// when the mouse button is release.
					this.destinationState = DropAction.NoAction;

					// This attempts to find a destination for the column operation.
					foreach (Tile viewerTile in this.tileTable.Values)
					{

						// A column can't be its own destination.
						if (this.selectedColumnTile.ViewerTileId == viewerTile.TileId)
							continue;

						// A destination is selected if the left edge of the target column is entirely visible in the header
						// quadrant and the left half of the column header contains the current mouse location.
						RectangleF testArea = viewerTile.RectangleF;
						testArea.Width /= 2.0f;
						if (window.Contains(testArea) && testArea.Contains(quadrantMouseEventArgs.Location) &&
							this.selectedColumnTile.RectangleF.Right != viewerTile.RectangleF.Left)
						{
							this.destinationState = DropAction.Select;
							this.destinationLocation = viewerTile.RectangleF.Location;
							break;
						}

						// This will test the right half of each of the colum headers.  If the cursor is over the right half and
						// the rightmost part of the destination is entirely visible in the header, then it can be a destination. 
						testArea.X += testArea.Width;
						testArea.Width = viewerTile.RectangleF.Right - testArea.X;
						if (window.Contains(testArea) && testArea.Contains(quadrantMouseEventArgs.Location) &&
							this.selectedColumnTile.RectangleF.Left != viewerTile.RectangleF.Right)
						{
							this.destinationState = DropAction.Select;
							this.destinationLocation = new PointF(viewerTile.RectangleF.Right, viewerTile.RectangleF.Top);
							break;
						}

					}

					// If a valid destination was found in the search above, move the set of red arrows (the destination cursor) 
					// over the exact spot where the column will be moved.
					if (this.destinationState == DropAction.Select)
					{

						// The destination cursor is a set of red arrows that point to where the column will be once it's dropped.
						// Locate the cursor over the appropriate column.  If the rightmost point was selected, then there won't be
						// a selected column and the location is fudged up with the width of the header window.
						int cursorX = Convert.ToInt32((this.destinationLocation.X + this.offsets[1].X) * this.scaleFactor);
						int cursorY = Convert.ToInt32((this.destinationLocation.Y + this.offsets[1].Y) * this.scaleFactor);
						this.destinationCursor.Location = this.PointToScreen(new Point(cursorX, cursorY));

						// Make the cursor window visible if it isn't already.  One may wonder why the cursor widow is made visible
						// and invisible instead of being created and destroyed.  This is done to maintain the Z order of the
						// selection cursor to be above the spreadsheet control, but below the column cursor.
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
				else
				{

					// If the cursor isn't in the colum header quadrant then there can be no destination for the column.
					if (this.destinationCursor.Visible)
						this.destinationCursor.Visible = false;

					// If the mouse isn't over the column header quadrant, a big 'X' give the user feedback that the column will be
					// dropped from the viewer if they release the mouse button.
					this.destinationState = DropAction.Delete;
					this.Cursor = this.bigEx;

				}

				break;

			case MouseState.ButtonUp:

				// This will determine which cursor should be used when the button isn't pressed while moving the mouse: a vertical
				// size cursor or a regular arrow cursor.  If the mouse is over the right or left edge of the column, then the
				// vertical resizing cursor is used.
				bool isResizingColumn = false;
				foreach (Tile viewerTile in this.tileTable.Values)
				{
					RectangleF rightHitTest = RectangleF.Intersect(window,
						new RectangleF(viewerTile.RectangleF.Right - DocumentViewer.splitBorder, viewerTile.RectangleF.Top,
						DocumentViewer.splitBorder, viewerTile.RectangleF.Height));
					RectangleF leftHitTest = RectangleF.Intersect(window,
						new RectangleF(viewerTile.RectangleF.Left, viewerTile.RectangleF.Top, DocumentViewer.splitBorder,
						viewerTile.RectangleF.Height));
					if (leftHitTest.Contains(quadrantMouseEventArgs.Location) || rightHitTest.Contains(quadrantMouseEventArgs.Location))
					{
						isResizingColumn = true;
						break;
					}
				}

				// Select the resizing cursor when the mouse is over the edge of the column header.
				this.Cursor = isResizingColumn ? this.verticalSplit : this.selectColumn;

				break;

			}

		}

		/// <summary>
		/// Raises the MouseMove event for the quadrant that contains the main body of tiles.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnRowHeaderMouseMove(QuadrantMouseEventArgs quadrantMouseEventArgs)
		{

			// Evaluate the state of the keyboard.  Key combinations involving the shift and control keys will alter the areas that
			// are selected.
			bool isShiftKeyPressed = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
			bool isControlKeyPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;

			// The cursor for all row selection operations is a black arrow pointing to the right.
			this.Cursor = this.selectRow;

			// The main idea in handling the row header is to find the tile that was selected by the mouse.  Rows are not as 
			// complicated as columns as there is no sorting or resizing operations that are allowed, so a row header is mean to
			// simply select the row.  The structure of the 'ColumnHeaderMouseDown' is kept here in case more complicated
			// processing of the row headers is required in the future.
			foreach (ViewerTile viewerTile in this.viewerTileList)
				if (viewerTile.RectangleF.Contains(quadrantMouseEventArgs.Location))
				{

					// The column can be selected by either the right or left mouse button.  The left button can also be used to
					// change the size of the column.  After considering the state of the mouse buttons, this variable indicates
					// that the column should be tested to see if it can be selected and, if it is, it should be selected.
					bool isRowSelected = false;

					// The mouse buttons determine the action to take.
					switch (quadrantMouseEventArgs.Button)
					{

					case MouseButtons.Right:

						// This will signal that the column should be tested to see if it can be selected below.
						isRowSelected = true;
						break;

					case MouseButtons.Left:

						// This will signal that the column should be tested to see if it can be selected below.
						isRowSelected = true;
						break;

					}

					// The DataTransform contains attributes on the column definitions that determine whether clicking on a column
					// header will sort the column or allow it to be selected.  Generally, read-only values will be in a sortable
					// column and user-entered fields will be in a selectable column.
					if (isRowSelected)
					{

						// This is an area that is calculated to intersect with all the selected tiles.  When the 'Shift' key is
						// pressed, the area includes everything from the anchor point to the current location of the cursor.  All
						// other combinations create effectively a point rectangle where the mouse is currently located.
						RectangleF selectedRectangle = new RectangleF(0.0f,
							quadrantMouseEventArgs.Location.Y < this.anchorPoint.Y ? quadrantMouseEventArgs.Location.Y :
							this.anchorPoint.Y, float.PositiveInfinity,
							Math.Abs(quadrantMouseEventArgs.Location.Y - this.anchorPoint.Y));

						// This is a point where the active (input) tile will be located.
						PointF activePoint = new PointF(this.splitSize.Width + 1.0f, this.anchorPoint.Y);

						// This will select every tile that matches the horizontal location of the column header tile and
						// clear the selection of everything else.
						foreach (ViewerTile innerTile in this.viewerTileList)
						{

							bool isSelected;
							bool isActive;

							// The tiles that match the column header tile's horizontal position are selected.  The first tile
							// after the split is made to have the input focus.  All other tiles are removed from the selected
							// region.
							if (innerTile.RectangleF.IntersectsWith(selectedRectangle))
							{
								isSelected = true;
								isActive = innerTile.RectangleF.Contains(activePoint);
							}
							else
							{
								isSelected = innerTile.IsSelected && isControlKeyPressed;
								isActive = false;
							}

							// Any tile that has modified selection attributes is passed through the queue to the viewer to be 
							// updated.
							if (innerTile.IsSelected != isSelected || innerTile.IsActive != isActive)
							{
								innerTile.IsSelected = isSelected;
								innerTile.IsActive = isActive;
								this.ViewerCommandQueue.Enqueue(new ViewerCommand(new Tile(innerTile), UpdateTile));
							}

						}

						// This will force the viewer to redraw the invalid sections of the viewer.
						this.ViewerCommandQueue.Enqueue(new ViewerCommand(null, InvalidateDocument));

					}

					// Thi will break out of the loop that searches the viewer for a tile that contains the mouse position.
					break;

				}

		}

		/// <summary>
		/// Raises the MouseMove event for the quadrant that contains the main body of tiles.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnTileMouseMove(QuadrantMouseEventArgs quadrantMouseEventArgs)
		{

			// Evaluate the state of the keyboard.  Key combinations involving the shift and control keys will alter the areas that
			// are selected.
			bool isShiftKeyPressed = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
			bool isControlKeyPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;

			// The arrow cursor is used for all selection operations involving the tiles in the body of the viewer.
			this.Cursor = Cursors.Arrow;

			// This will select a range of tiles when the mouse button is pressed and the mouse is moved.
			if (this.mouseState == MouseState.DraggingTile)
			{

				// This is an area that is calculated to intersect with all the selected tiles.  When the 'Shift' key is pressed,
				// the area includes everything from the anchor point to the current location of the cursor.  All other
				// combinations create effectively a point rectangle where the mouse is currently located.
				RectangleF selectedRectangle = new RectangleF(
					quadrantMouseEventArgs.Location.X < this.anchorPoint.X ? quadrantMouseEventArgs.Location.X :
					this.anchorPoint.X,
					quadrantMouseEventArgs.Location.Y < this.anchorPoint.Y ? quadrantMouseEventArgs.Location.Y :
					this.anchorPoint.Y,
					Math.Abs(quadrantMouseEventArgs.Location.X - this.anchorPoint.X),
					Math.Abs(quadrantMouseEventArgs.Location.Y - this.anchorPoint.Y));

				// Run through each tile to see if it should be selected or have the selection removed.
				foreach (ViewerTile viewerTile in this.viewerTileList)
				{

					bool isSelected;
					bool isActive;

					// This will select the tile if it is part of the selected area and clear the selection if it is not. The
					// active tile is the one that contains the anchor point.
					if (viewerTile.RectangleF.IntersectsWith(selectedRectangle))
					{
						isSelected = true;
						isActive = viewerTile.RectangleF.Contains(this.anchorPoint);
					}
					else
					{
						isSelected = viewerTile.IsSelected && isControlKeyPressed;
						isActive = false;
					}

					// As an optimization, only the tiles that been modified are passed to the viewer for updating.
					if (viewerTile.IsSelected != isSelected || viewerTile.IsActive != isActive)
					{
						viewerTile.IsSelected = isSelected;
						viewerTile.IsActive = isActive;
						this.ViewerCommandQueue.Enqueue(new ViewerCommand(new Tile(viewerTile), UpdateTile));
					}

				}

				// This will force the viewer to redraw the invalid sections of the viewer.
				this.ViewerCommandQueue.Enqueue(new ViewerCommand(null, InvalidateDocument));

			}

		}

		/// <summary>
		/// Handles the movement of the mouse over the client area.
		/// </summary>
		/// <param name="e">The mouse event arguments.</param>
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{

			// This is a floating point version of the mouse location in client device coordinates.
			PointF mouseLocation = new PointF(Convert.ToSingle(e.X), Convert.ToSingle(e.Y));

			try
			{

				// The display and the document are locked while the mouse action is evaluated.
				this.displayLock.AcquireReaderLock(Timeout.Infinite);
				this.documentLock.AcquireReaderLock(Timeout.Infinite);

				// This will convert the location of the mouse from device coordinates to document coordinates.  The client area of
				// the viewer is divided up into four quadrants.  Three of those quadrants can scroll.  If that wasn't confusing
				// enough, when the mouse is captured by the client window, the location of the cursor may not even be over on of
				// the four quadrants. In that situation a variable is maintained with the quadrant in which the mouse was located
				// when it was captured.  If the mouse hasn't been captured, then it must be over one of the four quadrants.  This
				// will determine the quadrant that currently owns the mouse.
				int mouseQuadrant = this.mouseDownQuadrant;
				if (mouseQuadrant == int.MinValue)
				{
					RectangleF[] section = SplitScreen();
					for (int quadrant = 0; quadrant < 4; quadrant++)
						if (section[quadrant].Contains(mouseLocation))
							mouseQuadrant = quadrant;
				}

				// Once the quadrant that owns the mouse has been determined, the client coordinates can be scaled and offset to
				// document coordinates before the handler is called.
				if (mouseQuadrant != int.MinValue)
					this.mouseMoveHandler[mouseQuadrant](new QuadrantMouseEventArgs(e.Button,
						new PointF(mouseLocation.X / this.scaleFactor - this.offsets[mouseQuadrant].X,
							mouseLocation.Y / this.scaleFactor - this.offsets[mouseQuadrant].Y)));

			}
			catch (Exception exception)
			{

				// Write the exception to the event log.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// The locks are no longer needed now that the event has been handled.
				this.displayLock.ReleaseReaderLock();
				this.documentLock.ReleaseReaderLock();

			}

		}

		/// <summary>
		/// Raises the MouseUp event for the header quadrant.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnHeaderMouseUp(QuadrantMouseEventArgs mouseEventArgs) { }

		/// <summary>
		/// Raises the MouseUp event for the column header quadrant.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnColumnHeaderMouseUp(QuadrantMouseEventArgs mouseEventArgs)
		{

			// Evaluate the state of the keyboard.  Key combinations involving the shift and control keys will alter the areas that
			// are selected.
			bool isShiftKeyPressed = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
			bool isControlKeyPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;

			// The mouse state indications what action should be taken when the mouse button is released.
			switch (this.mouseState)
			{

			case MouseState.ResizingColumn:

				// If the reversible line that is used as a cursor for the new column position needs to be removed, draw it one
				// last time.  This has the effect of re-complimenting the bits underneath, which leaves the screen with the
				// original image.  Note that the 'lastLine' needs to be cleared out so it's initialized for the next drag-and-drop
				// operation.
				if (this.lastLine != null)
				{
					ControlPaint.DrawReversibleLine(this.lastLine[0], this.lastLine[1], System.Drawing.Color.DarkGray);
					this.lastLine = null;
				}

				// The main idea when the column width is changed is to find the record in the DataTransform that contains the
				// column seetings, change the size and then recompile the view.  This step will find a reference to the column
				// that matches the right edge of the column that is to be resized.
				DataTransform.ColumnReferenceNode resizeColumnNode = this.dataTransform.FindRightEdge(this.resizeEdge);

				// This calculates the size of the new column.  Note that the mouse action allows for a negative column width, but
				// that isn't allowed in the stylesheet.  Instead, it's interpreted as an instruction to hide the column.  Also 
				// note that the view is modified, note the master list of columns available.
				float columnWidth = resizeColumnNode.Column.Width +
					Convert.ToSingle(Math.Round(mouseEventArgs.Location.X - this.resizeEdge));
				if (columnWidth <= 0.0f)
					this.dataTransform.View.Remove(resizeColumnNode);
				else
					resizeColumnNode.Column.Width = columnWidth;

				// Now that the size of the column has been changed (or the column removed), rebuild the view and apply it against
				// the current document data.  Any tiles that have changed will be redrawn.
				Compile();

				break;

			case MouseState.ButtonDown:

				// This will sort the document by one or more selected columns.  The control key is used to select multiple columns
				// similar to the selection mode of Microsoft Office.  The first step to sorting is to find out which column in the
				// view has been selected for sorting.
				DataTransform.ColumnNode columnNode =
					this.dataTransform.FindLeftEdge(this.selectedColumnTile.RectangleF.Left).Column;

				// The 'SortTemplate' attribute is used to distinguish columns that can be selected (usually columns that have user
				// input can be selected) versus columns that can be sorted.
				if (columnNode.SortTemplate != string.Empty)
				{

					// The DataTransform data structure contains a complex method of displaying hierarchical information.  Since
					// the documents can display data in outline format, finding the location of the sort is more complicated than
					// in an Excel type of cartesean layout.  The column specification in the DataTransform can optionally specify
					// the location of the sort fields in the template.  It takes the form of a 'Template'/'Row'/'ApplySort'
					// address. Sometime in the future this could be made into a single string and parsed, but for now the fields
					// are discrete and extracted this way.  If the 'Row' or 'ApplyTemplate' is not specified in the DataTransform,
					// then the first one found in the template is used.
					DataTransform.TemplateNode templateNode = this.dataTransform.Templates[columnNode.SortTemplate];
					DataTransform.RowNode rowNode = columnNode.SortRow == string.Empty ? templateNode.Rows[0] :
						templateNode.Rows[columnNode.SortRow];
					DataTransform.ApplyTemplateNode applyTemplateNode = columnNode.SortApplyTemplate == string.Empty ?
						rowNode.ApplyTemplates[0] : rowNode.ApplyTemplates[columnNode.SortApplyTemplate];

					// If the control key is kept pressed, then the selected sort columns will be combined.  Otherwise, any
					// previously selected sort column is discarded.
					if (!isControlKeyPressed)
						while (applyTemplateNode.Sorts.Count > 1)
							applyTemplateNode.Sorts.Remove(applyTemplateNode.Sorts[0]);

					// The direction of the sorting in a column can be toggled each time it is selected.  This requires a test to
					// make sure that the same column has been selected.  Of course, if there is nothing in the list of sorting
					// columns, then the list is initialize with an ascending sort.
					if (applyTemplateNode.Sorts.Count != 0)
					{

						// Find the last sort column in the sort specification.  If it matches the last column selected, then
						// the order of the sorting is toggled.  If it doesn't match, then the column is added to the list
						// of columns when the control key is pressed or used to initialize the list when the control key is
						// not pressed.
						DataTransform.SortNode sortNode = applyTemplateNode.Sorts[applyTemplateNode.Sorts.Count - 1];
						if (sortNode.Column == columnNode)
						{
							sortNode.Direction = sortNode.Direction == SortOrder.Ascending ? SortOrder.Descending :
								SortOrder.Ascending;
						}
						else
						{
							if (!isControlKeyPressed)
								applyTemplateNode.Sorts.Remove(sortNode);
							applyTemplateNode.Add(new DataTransform.SortNode(columnNode, SortOrder.Ascending));
						}

					}
					else
					{

						// This will initialize the list of sort columns when the list is empty.
						applyTemplateNode.Add(new DataTransform.SortNode(columnNode, SortOrder.Ascending));

					}

					// Now that the list of sort orders has been worked out, rebuild the view and apply it against the current
					// document data.  Any tiles that have changed will be redrawn.
					Compile();

				}

				break;

			case MouseState.DraggingColumn:

				// The action taken when the dragging operation is complete depends on whether a valid destination is selected or
				// whether the column is meant to be deleted.
				switch (this.destinationState)
				{

				case DropAction.Select:

					// This will move the column from one place in the document to another.
					DataTransform.ColumnReferenceNode sourceColumn =
						this.dataTransform.FindLeftEdge(this.selectedColumnTile.RectangleF.Left);
					DataTransform.ColumnReferenceNode destinationColumn =
						this.dataTransform.FindLeftEdge(this.destinationLocation.X);
					if (sourceColumn != destinationColumn)
						this.dataTransform.View.Move(sourceColumn, destinationColumn);

					// Release the set of red arrows used to indicate the destination position.
					this.destinationCursor.Dispose();
					this.destinationCursor = null;

					// Now that the column has been moved, rebuild the view and apply it against the current document data.  Any
					// tiles that have changed will be redrawn.
					Compile();

					break;

				case DropAction.Delete:

					// This will remove the column from the view.
					DataTransform.ColumnReferenceNode columnReferenceNode =
						this.dataTransform.FindLeftEdge(this.selectedColumnTile.RectangleF.Left);
					this.dataTransform.View.Remove(columnReferenceNode);

					// Now that the column has been removed, rebuild the view and apply it against the current document data.  Any
					// tiles that have changed will be redrawn.
					Compile();

					break;

				}

				// Reset the state of the column header window for the next operation.
				this.columnCursor.Dispose();
				this.columnCursor = null;

				break;

			}

		}

		/// <summary>
		/// Raises the MouseUp event for the row header quadrant.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnRowHeaderMouseUp(QuadrantMouseEventArgs mouseEventArgs) { }

		/// <summary>
		/// Raises the MouseUp event for the quadrant containing the document tiles.
		/// </summary>
		/// <param name="quadrantMouseEventArgs">A QuadrantMouseEventArgs that contains the event data.</param>
		protected virtual void OnTileMouseUp(QuadrantMouseEventArgs mouseEventArgs) { }
		
		/// <summary>
		/// Handles the 'Mouse Up' event.
		/// </summary>
		/// <param name="e">The mouse event parameters.</param>
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{

			// This is a floating point version of the mouse location in client device coordinates.
			PointF mouseLocation = new PointF(Convert.ToSingle(e.X), Convert.ToSingle(e.Y));

			try
			{

				// The display and the document are locked while the mouse action is evaluated.
				this.displayLock.AcquireReaderLock(Timeout.Infinite);
				this.documentLock.AcquireReaderLock(Timeout.Infinite);

				// This will convert the location of the mouse from device coordinates to document coordinates.  The client area of
				// the viewer is divided up into four quadrants.  Three of those quadrants can scroll.  If that wasn't confusing
				// enough, when the mouse is captured by the client window, the location of the cursor may not even be over on of
				// the four quadrants. In that situation a variable is maintained with the quadrant in which the mouse was located
				// when it was captured.  If the mouse hasn't been captured, then it must be over one of the four quadrants.  This
				// will determine the quadrant that currently owns the mouse.
				int mouseQuadrant = this.mouseDownQuadrant;
				if (mouseQuadrant == int.MinValue)
				{
					RectangleF[] section = SplitScreen();
					for (int quadrant = 0; quadrant < 4; quadrant++)
						if (section[quadrant].Contains(mouseLocation))
							mouseQuadrant = quadrant;
				}

				// Once the quadrant that owns the mouse has been determined, the client coordinates can be scaled and offset to
				// document coordinates before the handler is called.
				this.mouseUpHandler[mouseQuadrant](new QuadrantMouseEventArgs(e.Button,
					new PointF(mouseLocation.X / this.scaleFactor - this.offsets[mouseQuadrant].X,
						mouseLocation.Y / this.scaleFactor - this.offsets[mouseQuadrant].Y)));

			}
			catch (Exception exception)
			{

				// Write the error out to the log.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// The locks are no longer needed now that the event has been handled.
				this.displayLock.ReleaseReaderLock();
				this.documentLock.ReleaseReaderLock();

			}

			// This will reset the state of the column header for the next operation.
			this.mouseState = MouseState.ButtonUp;
			this.mouseDownQuadrant = int.MinValue;
			this.selectedColumnTile = null;

		}

	}

}
