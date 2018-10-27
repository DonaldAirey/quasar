namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Text;
	using System.Threading;
	using System.Xml;

	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class SpreadsheetWriter : XmlWriter
	{

		// When this many rows have been processed, the SpreadsheetWriter will release the locks and broadcast an intermediate
		// document.  The interruption is used to allow other processes, most notably the foreground thread, to access the data
		// structures in the spreadsheet.  If this value is set too low, it will take a long time to draw a large document.  If the
		// value is too large, then other threads, like the foreground thread, will be suspended while the document is read.
		private const int updateTrigger = 100;

		// A row of data can be parsed while reading a table, or updates of data.  These stages are used to indicat what action
		// should be taken after reading a row of data.
		private enum ParsingRowState { Insert, Update, Delete };

		// Private Members
		private MarkThree.Forms.BorderPosition borderPosition;
		private MarkThree.Forms.Device device;
		private MarkThree.Forms.Lexicon lexicon;
		private MarkThree.Forms.Spreadsheet spreadsheet;
		private MarkThree.Forms.SpreadsheetColumn spreadsheetColumn;
		private MarkThree.Forms.SpreadsheetRow currentRow;
		private MarkThree.Forms.SpreadsheetWriter.Constraint constraint;
		private MarkThree.Forms.Style style;
		private MarkThree.Forms.Style defaultStyle;
		private MarkThree.Forms.ViewColumn viewColumn;
		private System.Collections.Generic.Stack<Driver> stack;
		private System.Boolean isVisible;
		private System.Boolean isPrinted;
		private System.Boolean isProtected;
		private System.Int32 rowCounter;
		private System.Int32 columnIndex;
		private System.Int32 rowIndex;
		private System.Int32 displayHeight;
		private System.Int32 printerHeight;
		private System.Int32 dislayWidth;
		private System.Int32 printerWidth;
		private System.Int32 steps;
		private System.Object[] searchKey;
		private System.Single weight;
		private System.Single size;
		private System.String attributePrefix;
		private System.String attributeNamespaceURI;
		private System.String attributeLocalName;
		private System.String text;
		private System.String dataType;
		private System.String description;
		private System.String image;
		private System.String fontName;
		private System.String format;
		private System.String direction;
		private System.String styleId;
		private System.String columnId;
		private System.String rowFilter = string.Empty;
		private System.Boolean isResortingRequired;
		private System.Drawing.StringFormat stringFormat;
		private System.Drawing.Color color;
		private System.Drawing.Color startColor;
		private System.Drawing.FontStyle fontStyle;
		private MarkThree.Forms.SpreadsheetWriter.ParsingRowState parsingRowState;

		/// <summary>
		/// A List of columns that define uniqueness rules on the data in the viewer.
		/// </summary>
		public class Constraint
		{

			public bool IsPrimaryKey;
			public bool IsUnique;
			public List<string> ColumnIds;

			/// <summary>
			/// Initializes uniqueness rules on the data in this structure.
			/// </summary>
			public Constraint() { ColumnIds = new List<string>(); }

		}

		public SpreadsheetWriter(Spreadsheet spreadsheet)
		{

			// Initialize the object.
			this.spreadsheet = spreadsheet;

			// This lexicon will change the incoming XSL element and attribute names into tokens.  These installed handlers will
			// call up a handler to deal with the incoming tokens as they are recognized by the analyzer.
			this.lexicon = new Lexicon();
			this.lexicon.InstallHandlers(Token.Alignment, new TokenHandler(this.ParseAlignmentStart), new TokenHandler(this.ParseAlignmentEnd));
			this.lexicon.InstallHandlers(Token.Animation, new TokenHandler(this.ParseAnimationStart), new TokenHandler(this.ParseAnimationEnd));
			this.lexicon.InstallHandlers(Token.Bold, null, new TokenHandler(this.ParseBoldEnd));
			this.lexicon.InstallHandlers(Token.Border, null, new TokenHandler(this.ParseBorderEnd));
			this.lexicon.InstallHandlers(Token.Cell, new TokenHandler(this.ParseCellStart), new TokenHandler(this.ParseCellEnd));
			this.lexicon.InstallHandlers(Token.Color, null, new TokenHandler(this.ParseColorEnd));
			this.lexicon.InstallHandlers(Token.Column, new TokenHandler(ParseColumnStart), new TokenHandler(this.ParseColumnEnd));
			this.lexicon.InstallHandlers(Token.ColumnId, null, new TokenHandler(this.ParseColumnIdEnd));
			this.lexicon.InstallHandlers(Token.Columns, new TokenHandler(this.ParseColumnsStart), new TokenHandler(this.ParseColumnsEnd));
			this.lexicon.InstallHandlers(Token.Constraint, new TokenHandler(this.ParseConstraintStart), new TokenHandler(this.ParseConstraintEnd));
			this.lexicon.InstallHandlers(Token.ConstraintColumn, new TokenHandler(this.ParseConstraintColumnStart), new TokenHandler(this.ParseConstraintColumnEnd));
			this.lexicon.InstallHandlers(Token.Data, new TokenHandler(this.ParseDataStart), null);
			this.lexicon.InstallHandlers(Token.Delete, new TokenHandler(this.ParseDeleteStart));
			this.lexicon.InstallHandlers(Token.Description, null, new TokenHandler(this.ParseDescriptionEnd));
			this.lexicon.InstallHandlers(Token.Direction, null, new TokenHandler(this.ParseDirectionEnd));
			this.lexicon.InstallHandlers(Token.Display, new TokenHandler(this.ParseDisplayStart), new TokenHandler(this.ParseDisplayEnd));
			this.lexicon.InstallHandlers(Token.DoNotSelectRows, new TokenHandler(this.ParseDoNotSelectRowsStart));
			this.lexicon.InstallHandlers(Token.Font, new TokenHandler(this.ParseFontStart), new TokenHandler(this.ParseFontEnd));
			this.lexicon.InstallHandlers(Token.FontName, null, new TokenHandler(this.ParseFontNameEnd));
			this.lexicon.InstallHandlers(Token.Format, null, new TokenHandler(this.ParseFormatEnd));
			this.lexicon.InstallHandlers(Token.Fragment, new TokenHandler(this.ParseFragmentStart), new TokenHandler(this.ParseFragmentEnd));
			this.lexicon.InstallHandlers(Token.HeaderHeight, null, new TokenHandler(this.ParseHeaderHeight));
			this.lexicon.InstallHandlers(Token.Height, null, new TokenHandler(this.ParseDisplayHeightEnd));
			this.lexicon.InstallHandlers(Token.Hidden, null, new TokenHandler(this.ParseHiddenEnd));
			this.lexicon.InstallHandlers(Token.Horizontal, null, new TokenHandler(this.ParseHorizontalEnd));
			this.lexicon.InstallHandlers(Token.ID, null, new TokenHandler(this.ParseIDEnd));
			this.lexicon.InstallHandlers(Token.Image, null, new TokenHandler(this.ParseImageEnd));
			this.lexicon.InstallHandlers(Token.Insert, new TokenHandler(this.ParseInsertStart));
			this.lexicon.InstallHandlers(Token.Interior, new TokenHandler(this.ParseInteriorStart), new TokenHandler(this.ParseInteriorEnd));
			this.lexicon.InstallHandlers(Token.Italic, null, new TokenHandler(this.ParseItalicEnd));
			this.lexicon.InstallHandlers(Token.NumberFormat, new TokenHandler(this.ParseNumberFormatStart), new TokenHandler(this.ParseNumberFormatEnd));
			this.lexicon.InstallHandlers(Token.Parent, null, new TokenHandler(this.ParseParentEnd));
			this.lexicon.InstallHandlers(Token.Position, null, new TokenHandler(this.ParsePositionEnd));
			this.lexicon.InstallHandlers(Token.PrimaryKey, null, new TokenHandler(this.ParsePrimaryKeyEnd));
			this.lexicon.InstallHandlers(Token.PrintHeight, null, new TokenHandler(this.ParsePrinterHeightEnd));
			this.lexicon.InstallHandlers(Token.PrintWidth, null, new TokenHandler(this.ParsePrinterWidthEnd));
			this.lexicon.InstallHandlers(Token.Printed, null, new TokenHandler(this.ParseIsPrintedEnd));
			this.lexicon.InstallHandlers(Token.Printer, new TokenHandler(this.ParsePrinterStart), new TokenHandler(this.ParsePrinterEnd));
			this.lexicon.InstallHandlers(Token.Protected, null, new TokenHandler(this.ParseProtectedEnd));
			this.lexicon.InstallHandlers(Token.Protection, new TokenHandler(this.ParseProtectionStart), new TokenHandler(this.ParseProtectionEnd));
			this.lexicon.InstallHandlers(Token.ReadingOrder, null, new TokenHandler(this.ParseReadingOrderEnd));
			this.lexicon.InstallHandlers(Token.Row, new TokenHandler(this.ParseRowStart), new TokenHandler(this.ParseRowEnd));
			this.lexicon.InstallHandlers(Token.Size, null, new TokenHandler(this.ParseSizeEnd));
			this.lexicon.InstallHandlers(Token.StartColor, null, new TokenHandler(this.ParseStartColorEnd));
			this.lexicon.InstallHandlers(Token.Steps, null, new TokenHandler(this.ParseStepsEnd));
			this.lexicon.InstallHandlers(Token.Style, new TokenHandler(this.ParseStyleStart), new TokenHandler(this.ParseStyleEnd));
			this.lexicon.InstallHandlers(Token.StyleID, null, new TokenHandler(this.ParseStyleIdEnd));
			this.lexicon.InstallHandlers(Token.Styles, new TokenHandler(this.ParseStylesStart));
			this.lexicon.InstallHandlers(Token.Table, new TokenHandler(this.ParseTableStart));
			this.lexicon.InstallHandlers(Token.Type, null, new TokenHandler(this.ParseTypeEnd));
			this.lexicon.InstallHandlers(Token.Underline, null, new TokenHandler(this.ParseUnderlineEnd));
			this.lexicon.InstallHandlers(Token.Unique, null, new TokenHandler(this.ParseUniqueEnd));
			this.lexicon.InstallHandlers(Token.Update, new TokenHandler(this.ParseUpdateStart));
			this.lexicon.InstallHandlers(Token.Vertical, null, new TokenHandler(this.ParseVerticalEnd));
			this.lexicon.InstallHandlers(Token.View, new TokenHandler(this.ParseViewStart), new TokenHandler(this.ParseViewEnd));
			this.lexicon.InstallHandlers(Token.ViewColumn, new TokenHandler(this.ParseViewColumnStart), new TokenHandler(this.ParseViewColumnEnd));
			this.lexicon.InstallHandlers(Token.Weight, null, new TokenHandler(this.ParseWeightEnd));
			this.lexicon.InstallHandlers(Token.Width, null, new TokenHandler(this.ParseWidthEnd));
			this.lexicon.InstallHandlers(Token.Workbook, new TokenHandler(this.ParseWorkbookStart), new TokenHandler(this.ParseWorkbookEnd));
			this.lexicon.InstallHandlers(Token.Worksheet, new TokenHandler(this.ParseWorksheetStart));

			// Initialize the object;
			this.stack = new Stack<Driver>();

			// A Default Style is always available.
			this.defaultStyle = new Style("Default", null);

		}

		private void OpenDocument()
		{

			// In order to keep the SpreadsheetWriter from consuming too much of the CPU time, control is surrendered to ther 
			// threads after reading a certain number of rows.  This counter keeps track of how many rows have been read from the
			// XML Document.
			this.rowCounter = 0;

			// Other threads must be prevented from reading while the structure is changed.
			this.spreadsheet.readerWriterLock.AcquireWriterLock(Timeout.Infinite);

		}

		private void CloseDocument()
		{

			// If rows were added or deleted, the table must be resorted.  Note that this is a hack.  There must be a more elegant way
			// to support the sorting without having to clear the sort string and re-add it.  Perhaps adding rows through the view would
			// keep the view in sync.
			if (this.isResortingRequired)
			{
				this.spreadsheet.spreadsheetRowView.Sort = string.Empty;
				this.spreadsheet.spreadsheetRowView.Sort = this.spreadsheet.sort;
				this.isResortingRequired = false;
			}

			// IMPORTANT CONCEPT: The incoming data may change the size of the document.  When the drawing is complete, an event 
			// message will be broadcast to indicate that the size of the document has changed.  While it is not strictly necessary
			// to release the locks before broadcasting this message, it makes the control much more responsive.  So, as a general
			// rule, these messages are sent out only after the locks on the data model have been released.  This variable will
			// capture the new size of the document after intergrating the changes and will be used to broadcast the size change
			// after the locks are released (to be consistent with the way the message is sent out in the middle of processing the
			// incoming XML data.
			Size size = Size.Empty;

			// The region that needs to be redrawn will also be calculated while the data model is locked, and broadcast after the 
			// locks have been released.
			Region region = null;

			// Measure the document and scan it for changed regions after all the data has been read.
			region = this.spreadsheet.MeasureData();

			// At this point, all the data has been read into the structure.
			this.spreadsheet.AcceptChanges();

			// The owner of this object may will be interested in the size when the document has been changed.  The size is
			// broadcast out on an event.  However, once the write lock is released, the size can change, so directly 
			// accessing the member variable 'this.displayRectangle' can lead to random changes in the size.  In order to broadcast
			// the change in a thread-safe way, a copy of the size is read here, then broadcast once the lock has been 
			// released.
			size = this.spreadsheet.displayRectangle.Size;

			// Generate an event when the size has changed.
			this.spreadsheet.OnDataSizeChanged(size);

			// Broadcast the changed regions when the data has changed.
			this.spreadsheet.OnDataInvalidated(region);

			this.spreadsheet.readerWriterLock.ReleaseWriterLock();

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

		public override void Close()
		{

		}

		public override void Flush()
		{

			// There is no flushing involved with writing to the spreadsheet.

		}

		public override string LookupPrefix(string ns)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteCData(string text)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteCharEntity(char ch)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteComment(string text)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteEndAttribute()
		{

			if (this.attributeLocalName != "xmlns" && this.attributePrefix != "xmlns")
			{

				Driver driver = this.lexicon.GetDriver(this.attributeNamespaceURI, this.attributeLocalName);
				if (driver.ElementEndHandler != null)
					driver.ElementEndHandler();

			}

		}

		public override void WriteEndDocument()
		{

			throw new Exception("The method or operation is not implemented.");

		}

		public override void WriteEndElement()
		{

			Driver driver = this.stack.Peek();

			if (driver.ElementEndHandler != null)
				driver.ElementEndHandler();

			this.stack.Pop();

		}

		public override void WriteEntityRef(string name)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteFullEndElement()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteRaw(string data)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{

			// Save the name of the attribute
			this.attributePrefix = prefix;
			this.attributeLocalName = localName;
			this.attributeNamespaceURI = ns;

			// Clear out the text from previously parsed attributes.
			this.text = string.Empty;

		}

		public override void WriteStartDocument(bool standalone)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteStartDocument()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{

			Driver driver = this.lexicon.GetDriver(ns, localName);

			if (driver.ElementStartHandler != null)
				driver.ElementStartHandler();

			this.stack.Push(driver);

		}

		public override WriteState WriteState
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override void WriteString(string text)
		{

			// Nothing is done when parsing the text except to save the value.  Once the entire attribute has been scanned, the
			// logic will determine if it is a namespace declaration or a simple attribute.
			this.text = text;

		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteWhitespace(string ws)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private void ParseHorizontalEnd()
		{

			// This will assign the Alignment settings to the Display.
			if (this.stringFormat != null)
			{
				this.stringFormat.Alignment = this.text == "Left" ? StringAlignment.Near :
					this.text == "Center" ? StringAlignment.Center :
					this.text == "Right" ? StringAlignment.Far : DefaultSpreadsheet.Alignment;
			}

		}

		private void ParseVerticalEnd()
		{

			this.stringFormat.LineAlignment = this.text == "Top" ? StringAlignment.Near :
				this.text == "Center" ? StringAlignment.Center :
				this.text == "Bottom" ? StringAlignment.Far : DefaultSpreadsheet.LineAlignment;

		}

		private void ParseReadingOrderEnd()
		{

			if (this.text == "RightToLeft")
				this.stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

		}

		/// <summary>
		/// Handler for the start of the Alignment tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseAlignmentEnd()
		{

			// This will assign the Alignment settings to the Display.
			if (this.device == Device.Common || this.device == Device.Display)
				this.style.DisplayStyle.StringFormat = this.stringFormat;

			// This will assign the Alignment settings to the Printer.
			if (this.device == Device.Common || this.device == Device.Printer)
				this.style.PrinterStyle.StringFormat = this.stringFormat;

		}

		private void ParseAlignmentStart()
		{

			// The alignment attributes will be translated to values in this object.
			this.stringFormat = new StringFormat();
			this.stringFormat.Alignment = DefaultSpreadsheet.Alignment;
			this.stringFormat.LineAlignment = DefaultSpreadsheet.LineAlignment;
			this.stringFormat.FormatFlags = DefaultSpreadsheet.FormatFlags;

		}

		/// <summary>
		/// Handler for the start of the Animation element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseAnimationEnd()
		{

			Movement movement = this.direction == string.Empty ? Movement.Detached :
				(Movement)Enum.Parse(typeof(Movement), this.direction);

			// This indicates that the style contains animation which needs to be analyzed when the data in cells that have this 
			// style are modified.
			this.style.DisplayStyle.IsAnimated = true;

			// The next step is to create an array of styles the cycle through a calculated color change from the start color to
			// the base color of the Font.  When the cell changes value, these styles will be used to make the color burst into the
			// StartColor and gradually fade over time to the font color.  The number of steps indicates how gradual and how long
			// the fade effect will be.  If the animation period is 1 second, and there are ten steps, the fade will take place
			// using 10 different styles over a total of 10 seconds.
			Style[] styleArray = new Style[steps];

			// Extract the color components of the start and end colors of this fade effect.
			Color endColor = this.color;
			decimal totalSteps = Convert.ToDecimal(this.steps - 1);
			decimal startRed = this.startColor.R;
			decimal startGreen = this.startColor.G;
			decimal startBlue = this.startColor.B;
			decimal endRed = endColor.R;
			decimal endGreen = endColor.G;
			decimal endBlue = endColor.B;

			// This will create a new style for each of the colors in the fade effect.  Animation is accomplished by changing the
			// style on the cell to each one of these styles in sequence.
			for (int newStyleIndex = 0; newStyleIndex < steps; newStyleIndex++)
			{

				// Create a new style and name it using a combination of the parent style name, the direction of the movement (up,
				// down, any, none) and the step involved.
				string styleId = string.Format("{0}{1}{2}", this.style.Id, movement, newStyleIndex);
				styleArray[newStyleIndex] = new Style(styleId, this.style);

				// The color used for the font is an involved calculation.  In round terms, the starting color is the color
				// specified in the 'Animation' element.  The ending color for the animated sequence is the color of the font.  The
				// steps are worked out so that the starting color morphs into the ending color.
				decimal index = Convert.ToDecimal(newStyleIndex);
				decimal red = startRed - ((startRed - endRed) / totalSteps) * index;
				decimal green = startGreen - ((startGreen - endGreen) / totalSteps) * index;
				decimal blue = startBlue - ((startBlue - endBlue) / totalSteps) * index;
				Color brushColor = Color.FromArgb(Convert.ToInt32(red), Convert.ToInt32(green), Convert.ToInt32(blue));
				styleArray[newStyleIndex].DisplayStyle.FontBrush = new SolidBrush(brushColor);

			}

			// This will assign the animation effect to the proper movement of the data in the cell.  Note that animation is only 
			// valid for a display device.
			if (this.device == Device.Display)
				switch ((Movement)Enum.Parse(typeof(Movement), this.direction))
				{
				case Movement.Nil:
					this.style.DisplayStyle.NilAnimation = styleArray;
					break;
				case Movement.Up:
					this.style.DisplayStyle.UpAnimation = styleArray;
					break;
				case Movement.Down:
					this.style.DisplayStyle.DownAnimation = styleArray;
					break;
				}

		}

		private void ParseStepsEnd()
		{

			this.steps = Convert.ToInt32(this.text);

		}

		private void ParseStartColorEnd()
		{

			this.startColor = ConvertToColor(this.text);

		}

		/// <summary>
		/// Handler for the start of the Animation element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseAnimationStart()
		{

			// Initialize the animation with the defaults.
			this.steps = DefaultSpreadsheet.Steps;
			this.startColor = DefaultSpreadsheet.StartColor;
			this.direction = string.Empty;

		}

		private void ParsePositionEnd()
		{

			this.borderPosition = (BorderPosition)Enum.Parse(typeof(BorderPosition), this.text);

		}

		private void ParseWeightEnd()
		{

			this.weight = Convert.ToSingle(this.text);

		}

		private void ParseColorEnd()
		{

			this.color = ConvertToColor(this.text);

		}

		/// <summary>
		/// Handler for the start of the Border tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseBorderStart()
		{

			// Set the defaults.
			this.borderPosition = BorderPosition.Detached;
			this.weight = DefaultSpreadsheet.BorderWeight;
			this.color = DefaultSpreadsheet.BorderColor;

		}

		/// <summary>
		/// Handler for the start of the Border tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseBorderEnd()
		{

			// Create a pen based on the color and weight parsed out of the XML stream.
			Pen pen = new Pen(new SolidBrush(this.color), this.weight);

			// This will assign a Border settings to the Display based on the position specified in the XML.
			if (this.device == Device.Common || this.device == Device.Display)
				switch (this.borderPosition)
				{
				case BorderPosition.Left:
					this.style.DisplayStyle.LeftBorder = pen;
					break;
				case BorderPosition.Top:
					this.style.DisplayStyle.TopBorder = pen;
					break;
				case BorderPosition.Right:
					this.style.DisplayStyle.RightBorder = pen;
					break;
				case BorderPosition.Bottom:
					this.style.DisplayStyle.BottomBorder = pen;
					break;
				}

			// This will assign a Border settings to the Printer based on the position specified in the XML.
			if (this.device == Device.Common || this.device == Device.Printer)
				switch (borderPosition)
				{
				case BorderPosition.Left:
					this.style.PrinterStyle.LeftBorder = pen;
					break;
				case BorderPosition.Top:
					this.style.PrinterStyle.TopBorder = pen;
					break;
				case BorderPosition.Right:
					this.style.PrinterStyle.RightBorder = pen;
					break;
				case BorderPosition.Bottom:
					this.style.PrinterStyle.BottomBorder = pen;
					break;
				}

		}

		private void ParseIDEnd()
		{

			this.style.Id = this.text;

			// Don't allow multiple styles with the same identifier.
			if (this.spreadsheet.styleTable.ContainsKey(this.style.Id))
				throw new Exception(string.Format("Style {0} already exists in the style table", this.style.Id));

			// An implicit "Default" style is created when the Workbook is initialized.  If an explicit "Default" style is 
			// defined, remove the original one so there isn't a conflict.
			if (this.style.Id == "Default")
				this.spreadsheet.styleTable.Remove(this.style.Id);


		}

		private void ParseStyleIdEnd()
		{

			this.styleId = this.text;

		}

		private void ParseColumnIdEnd()
		{

			this.columnId = this.text;

		}

		/// <summary>
		/// Handler for the start of the Cell element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseCellEnd()
		{

			try
			{


				// Find the column associated with the incoming data.  It is possible that a fragment can be processed before the 
				// document has created a structure, so a check is made here to see if there's a valid spreadsheet structure into
				// which the data can be parsed.
				SpreadsheetColumn currentColumn = (SpreadsheetColumn)this.spreadsheet.Columns[this.columnId];
				if (currentColumn != null)
				{

					SpreadsheetCell spreadsheetCell = this.currentRow[currentColumn];

					// This cell will use the style of the column it belongs to if a style isn't explicitly set for it.  Note that if the
					// cell is being reused, the operation to put the cell back in the row appears redundant, but this operation will
					// trigger a change in status in the data table to make this cell appear to be modified.  Also note that the size of 
					// the cell is fixed when it is parsed.  The location may change, but the size is fixed here.
					spreadsheetCell.Style = this.styleId == string.Empty ? currentColumn.Style :
						this.spreadsheet.styleTable[this.styleId];
					spreadsheetCell.IsModified = true;

					// Convert the incoming XML data into a native datatype.
					switch (this.dataType)
					{

					case "string":

						spreadsheetCell.Data = this.text;
						break;

					case "boolean":

						spreadsheetCell.Data = Convert.ToBoolean(this.text);
						break;

					case "short":

						spreadsheetCell.Data = Convert.ToInt16(this.text);
						break;

					case "int":

						spreadsheetCell.Data = Convert.ToInt32(this.text);
						break;

					case "long":

						spreadsheetCell.Data = Convert.ToInt64(this.text);
						break;

					case "decimal":

						spreadsheetCell.Data = this.text == string.Empty || this.text == "NaN" ? 0.0M : Convert.ToDecimal(this.text);
						break;

					case "float":

						spreadsheetCell.Data = Convert.ToSingle(this.text);
						break;

					case "double":

						spreadsheetCell.Data = Convert.ToDouble(this.text);
						break;

					case "dateTime":

						spreadsheetCell.Data = Convert.ToDateTime(this.text);
						break;

					case "image":

						if (this.text != string.Empty)
						{
							MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(this.text));
							spreadsheetCell.Data = new Bitmap(memoryStream);
						}
						break;

					default:

						spreadsheetCell.Data = DBNull.Value;
						break;

					}

				}

			}
			catch (Exception exception)
			{

				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

		}

		/// <summary>
		/// Handler for the start of the Cell element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseCellStart()
		{

			// Set the defaults
			this.styleId = string.Empty;
			this.columnId = string.Empty;
			this.dataType = string.Empty;

		}

		private void ParseWidthEnd()
		{

			this.dislayWidth = Convert.ToInt32(Math.Round(Convert.ToSingle(this.text) * DefaultSpreadsheet.PixelsPerUnit));

		}

		private void ParsePrinterWidthEnd()
		{

			this.printerWidth = Convert.ToInt32(Math.Round(Convert.ToSingle(this.text) * DefaultSpreadsheet.PixelsPerUnit));

		}

		private void ParseHiddenEnd()
		{

			this.isVisible = false;

		}

		private void ParseIsPrintedEnd()
		{

			this.isPrinted = Convert.ToBoolean(this.text);

		}

		private void ParseDescriptionEnd()
		{

			this.description = this.text;

		}

		private void ParseImageEnd()
		{

			this.image = this.text;

		}

		/// <summary>
		/// Handler for the start of the Column element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseColumnEnd()
		{

			// Check the integrity of the column attributes.
			if (this.columnId == string.Empty)
				throw new Exception("Columns require an identifier.");

			// Extract the simple column attributes
			this.spreadsheetColumn.ColumnName = this.columnId;
			this.spreadsheetColumn.IsVisible = this.isVisible;
			this.spreadsheetColumn.IsPrinted = this.isPrinted;
			this.spreadsheetColumn.Description = this.description;

			// Extract the Style attribute
			Style style;
			if (!this.spreadsheet.styleTable.TryGetValue(this.styleId, out style))
				throw new Exception(string.Format("Column {0} has a Parent Style {1} that is not part of the stylesheet",
					spreadsheetColumn.ColumnName, styleId));
			spreadsheetColumn.Style = style;

			// Set the width of the displayed column.
			Rectangle displayRectangle = spreadsheetColumn.DisplayRectangle;
			displayRectangle.Width = this.dislayWidth;
			spreadsheetColumn.DisplayRectangle = displayRectangle;

			// Set the width of the printed column.
			Rectangle printerRectangle = spreadsheetColumn.PrinterRectangle;
			printerRectangle.Width = this.printerWidth;
			spreadsheetColumn.PrinterRectangle = printerRectangle;

			// Extract the Image attribute
			if (this.image != string.Empty)
			{
				MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(this.image));
				spreadsheetColumn.Image = new Bitmap(memoryStream);
			}

			// The column can be added to the spreadsheet once it's been initialized.
			spreadsheetColumn.ColumnIndex = this.columnIndex++;
			this.spreadsheet.Columns.Add(spreadsheetColumn);

		}

		/// <summary>
		/// Handler for the start of the Column element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseColumnStart()
		{

			// This will initialize a new column and set its members to default values.
			this.styleId = "Default";
			this.columnId = string.Empty;
			this.dislayWidth = Convert.ToInt32(DefaultSpreadsheet.DisplayColumnWidth);
			this.printerWidth = Convert.ToInt32(DefaultSpreadsheet.PrinterColumnWidth);
			this.isVisible = true;
			this.isPrinted = true;
			this.description = string.Empty;
			this.image = string.Empty;
			this.spreadsheetColumn = new SpreadsheetColumn();

		}

		/// <summary>
		/// Handler for the end of the Column element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseColumnsEnd()
		{

			// Create a filter for the columns.  This should eventually be replaced with a section that describes the visible
			// columns and their order.  This is a stop-gap until this syntax is in place.
			string columnFilter = string.Empty;
			DataColumnCollection dataColumnCollection = this.spreadsheet.Columns;
			for (int columnIndex = 0; columnIndex < dataColumnCollection.Count; columnIndex++)
			{
				SpreadsheetColumn spreadsheetColumn = (SpreadsheetColumn)dataColumnCollection[columnIndex];
				if (spreadsheetColumn.IsVisible)
				{
					columnFilter += spreadsheetColumn.ColumnName;
					if (columnIndex < dataColumnCollection.Count - 1)
						columnFilter += ",";
				}
			}
			this.spreadsheet.ViewColumns.ColumnFilter = columnFilter;

			// This is the other side of the bug fix to clear reset the row filters after loading new results.  In the 'Workbook'
			// tag, the row filter was cleared out (set to an empty string), so they weren't active while new column definitions
			// were loaded in.  Now that the columns have been defined, the row filter can be put back and it will be able to read
			// the proper columns to see if they should be in the view or not.  Without this workaround, the filter attempt to use
			// the previous column definitions for a filter.
			this.spreadsheet.spreadsheetRowView.RowFilter = this.rowFilter;

			// Now that a complete definition of the header exists, it can be measured and displayed.
			Region region = this.spreadsheet.MeasureHeader();

			// The header can sometimes be part of a very big document.  By taking a quick break here and drawing the header area,
			// the user is given some valuable feedback that something is happening and they can expect a beautiful new document in
			// their viewer soon.
			this.spreadsheet.readerWriterLock.ReleaseWriterLock();
			this.spreadsheet.OnHeaderSizeChanged(this.spreadsheet.headerRectangle.Size);
			this.spreadsheet.OnHeaderInvalidated(region);
			this.spreadsheet.readerWriterLock.AcquireWriterLock(Timeout.Infinite);

		}

		private void ParseColumnsStart()
		{

			this.columnIndex = 0;

		}

		/// <summary>
		/// Handler for the start of the Constraint element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseConstraintStart()
		{

			// Initialize a new constraint with the defaults.  The constraint is used to impose a uniqueness rule on the data in
			// the viewer.  Most often this is used for a primary index.
			this.constraint = new Constraint();

		}

		/// <summary>
		/// Handler for the start of the ConstraintColumn element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseConstraintColumnEnd()
		{

			// Extract the ColumnId attribute
			if (this.columnId == string.Empty)
				throw new Exception("There is no ColumnId attribute in a Constraint Column element.");
			this.constraint.ColumnIds.Add(this.columnId);

		}

		/// <summary>
		/// Handler for the start of the ConstraintColumn element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseConstraintColumnStart()
		{

			this.columnId = string.Empty;

		}

		public void ParsePrimaryKeyEnd()
		{

			this.constraint.IsPrimaryKey = Convert.ToBoolean(this.text);

		}

		public void ParseUniqueEnd()
		{

			this.constraint.IsUnique = Convert.ToBoolean(this.text);

		}

		/// <summary>
		/// Handler for the end of the Constraint element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		public void ParseConstraintEnd()
		{

			// The primary key for the spreadsheet table can be constructed once all the columns in the constraint have been read.
			if (this.constraint.IsPrimaryKey)
			{

				DataColumn[] dataRowColumns = new DataColumn[constraint.ColumnIds.Count];
				int fieldIndex = 0;
				foreach (string columnId in constraint.ColumnIds)
					dataRowColumns[fieldIndex++] = this.spreadsheet.Columns[columnId];
				this.spreadsheet.PrimaryKey = dataRowColumns;

				// Since the size of the key is fixed at this point, a semi-permanent key holder can be allocated here instead of
				// each time a row is parsed in.  It may not seem significant here, but this same operation is repeated for every
				// row that is read.  Note that this search key is only used for parsing rows.  Any other use can corrupt the
				// importing of XML data.
				this.searchKey = new object[this.spreadsheet.PrimaryKey.Length];

			}

		}


		/// <summary>
		/// Handler for the start of the Data element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseTypeEnd()
		{

			this.dataType = this.text;
			this.text = string.Empty;

		}

		/// <summary>
		/// Handler for the start of the Data element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseDataStart()
		{

			this.dataType = "String";

		}

		/// <summary>
		/// Begin examining instructions for deleting a row.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private void ParseDeleteStart()
		{

			// This controls how a row is processed.
			this.parsingRowState = ParsingRowState.Delete;
			this.currentRow = null;

		}

		/// <summary>
		/// Handler for the End Display tag.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private void ParseDisplayEnd()
		{

			// Any attributes read after this end tag will modify the common output attributes.
			this.device = Device.Common;

		}

		/// <summary>
		/// Handler for the Display tag.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private void ParseDisplayStart()
		{

			// Any attributes read after this tag modify the display device settings.
			this.device = Device.Display;

		}

		/// <summary>
		/// Handler for the start of the Protection element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseDoNotSelectRowsStart()
		{

			// This will assign the protection settings to the Display.
			this.spreadsheet.SelectionMode = SelectionMode.Cell;

		}

		private void ParseFontNameEnd()
		{

			this.fontName = this.text;

		}

		private void ParseSizeEnd()
		{

			this.size = Convert.ToSingle(this.text);

		}

		private void ParseBoldEnd()
		{

			if (Convert.ToInt32(this.text) == 1)
				fontStyle |= FontStyle.Bold;

		}

		private void ParseItalicEnd()
		{

			if (Convert.ToInt32(this.text) == 1)
				fontStyle |= FontStyle.Italic;

		}

		private void ParseUnderlineEnd()
		{

			if (Convert.ToInt32(this.text) == 1)
				fontStyle |= FontStyle.Underline;

		}

		/// <summary>
		/// Handler for the start of the Font element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseFontEnd()
		{

			// These will be used to draw text in the cell in the specified color.
			Font font = new System.Drawing.Font(this.fontName, this.size, this.fontStyle);
			SolidBrush solidBrush = new SolidBrush(this.color);

			// This will assign the font settings to the Display.
			if (this.device == Device.Common || this.device == Device.Display)
			{
				this.style.DisplayStyle.Font = font;
				this.style.DisplayStyle.FontBrush = solidBrush;
			}

			// This will assign the font settings to the Printer.
			if (this.device == Device.Common || this.device == Device.Printer)
			{
				this.style.PrinterStyle.Font = font;
				this.style.PrinterStyle.FontBrush = solidBrush;
			}

		}

		/// <summary>
		/// Handler for the start of the Font element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseFontStart()
		{

			this.fontName = DefaultSpreadsheet.FontName;
			this.size = DefaultSpreadsheet.FontSize;
			this.color = DefaultSpreadsheet.ForeColor;
			this.fontStyle = FontStyle.Regular;

		}

		/// <summary>
		/// Handler for the start of the Interior element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseInteriorEnd()
		{

			// Create a brush for the interior area of the cells.
			Brush brush = new SolidBrush(this.color);
			Pen pen = new Pen(brush, 1.0f);

			// This will assign the interior brush settings to the Display.
			if (this.device == Device.Common || this.device == Device.Display)
			{
				style.DisplayStyle.InteriorBrush = brush;
				style.DisplayStyle.InteriorPen = pen;
			}

			// This will assign the interior brush settings to the Printer.
			if (this.device == Device.Common || this.device == Device.Printer)
			{
				style.PrinterStyle.InteriorBrush = brush;
				style.PrinterStyle.InteriorPen = pen;
			}

		}

		/// <summary>
		/// Handler for the start of the Interior element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseInteriorStart()
		{

			// Set up the defaults.
			this.color = DefaultSpreadsheet.BackColor;

		}

		private void ParseFormatEnd()
		{

			this.format = this.text;

		}

		/// <summary>
		/// Handler for the start of the NumberFormat element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseNumberFormatEnd()
		{

			// Initialize the Number Format settings with the spreadsheet defaults.
			string rawFormat = this.format.Replace("A/P", "t");
			this.format = this.format == "General" ? "{0}" : "{0:" + rawFormat + "}";

			// This will assign the number format settings to the Display.
			if (this.device == Device.Common || this.device == Device.Display)
				this.style.DisplayStyle.NumberFormat = this.format;

			// This will assign the number format settings to the Printer.
			if (this.device == Device.Common || this.device == Device.Printer)
				this.style.PrinterStyle.NumberFormat = this.format;

		}

		/// <summary>
		/// Handler for the start of the NumberFormat element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseNumberFormatStart()
		{

			// Initialize the Number Format settings with the spreadsheet defaults.
			this.format = DefaultSpreadsheet.Format;

		}

		/// <summary>
		/// Handler for the Printer tag.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private void ParsePrinterStart()
		{

			// Any attributes read after this tag modify the display device settings.
			this.device = Device.Printer;

		}

		/// <summary>
		/// Handler for the End Printer tag.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private void ParsePrinterEnd()
		{

			// Any attributes read after this end tag will modify the common output attributes.
			this.device = Device.Common;

		}

		private void ParseProtectedEnd()
		{

			this.isProtected = Convert.ToInt32(this.text) == 1;

		}

		/// <summary>
		/// Handler for the start of the Protection element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseProtectionEnd()
		{

			// This will assign the protection settings to the Display.
			if (this.device == Device.Display)
				this.style.DisplayStyle.IsProtected = this.isProtected;

		}

		/// <summary>
		/// Handler for the start of the Protection element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseProtectionStart()
		{

			this.isProtected = DefaultSpreadsheet.IsProtected;

		}

		private void ParseDisplayHeightEnd()
		{

			this.displayHeight = Convert.ToInt32(Math.Round(Convert.ToDecimal(this.text) * 4.0M / 3.0M));

		}

		private void ParsePrinterHeightEnd()
		{

			this.printerHeight = Convert.ToInt32(Math.Round(Convert.ToDecimal(this.text) * 4.0M / 3.0M));

		}

		/// <summary>
		/// Handler for the end of the Row element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseRowEnd()
		{

			// Extract the Height attribute
			this.currentRow.DisplayRectangle.Height = this.displayHeight;

			// Extract the PrinterHeight attribute
			this.currentRow.PrinterRectangle.Height = this.printerHeight;

			int keyIndex;
			SpreadsheetRow spreadsheetRow;

			// The row can come from three different parts of the XML document.  When the 'Row' is part of the 'Table' element, it
			// is interpreted as a straight-forward insert instruction.  When it is part of the 'Update' element, it is handled
			// as an incremental update to an existing record.  When it is part of the 'Remove' element, the row element contains
			// only the primary key of a row that is removed if found in the spreadsheet table.
			switch (this.parsingRowState)
			{

			case ParsingRowState.Insert:

				// Add the comleted row to the table.  It is now part of the data model that makes up the viewer.
				this.currentRow.RowIndex = this.rowIndex++;
				this.spreadsheet.ViewRows.Add(this.currentRow);
				this.isResortingRequired = true;

				// This is an instruction to create a new SpreadsheetRow for the next line of data read in from the XML stream.
				this.currentRow = null;

				break;

			case ParsingRowState.Update:

				// It is possible in a multithreaded application to get a fragment before there is a document structure.  This code
				// insures that there is a valid DataTable into which the data in the incoming row can be placed.
				if (this.spreadsheet.PrimaryKey.Length == 0)
					break;
				
				// Search the viewer database for the incoming row.  If it doesn't exist, it will be added.  If a row is found that
				// matches the primary key, it will be compared against the existing row to see what has changed.
				keyIndex = 0;
				foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.PrimaryKey)
					this.searchKey[keyIndex++] = this.currentRow[spreadsheetColumn].Data;
				spreadsheetRow = (SpreadsheetRow)this.spreadsheet.Rows.Find(this.searchKey);
				if (spreadsheetRow != null)
				{

					// If the incoming row already has been added to the data, then compare the new row, column by column, against 
					// the old row to see what has changed.
					foreach (DataColumn dataColumn in this.spreadsheet.Columns)
					{

						// Updates can contain only partial rows.  The columns that aren't included aren't compared or integrated
						// into the data model.  A DBNull.Value indiates that this column is to be ignored when comparing rows.
						if (!this.currentRow[dataColumn].IsModified)
							continue;

						// Compare the incoming (source) cell against the existing (destination) cell.  If the data or the style 
						// has changed, the source cell will replace the destination.
						SpreadsheetCell sourceCell = this.currentRow[dataColumn];
						SpreadsheetCell destinationCell = spreadsheetRow[dataColumn];

						if (!sourceCell.Data.Equals(destinationCell.Data) || sourceCell.Style != destinationCell.Style)
						{

                            if (sourceCell.Style.DisplayStyle.IsAnimated)
                            {

                                if (sourceCell.Data is IComparable && destinationCell.Data is IComparable)
                                {

                                    // This will test to see if the new cell should be animated and, if it is animated, which array of 
                                    // styles will be used to provide a color-fade effect.
                                    int comparision = ((IComparable)sourceCell.Data).CompareTo(destinationCell.Data);
                                    Movement movement = comparision > 0 ? Movement.Up : comparision < 0 ? Movement.Down : Movement.Nil;
                                    Style[] styleArray = null;
                                    switch (movement)
                                    {
                                        case Movement.Nil:
                                            styleArray = sourceCell.Style.DisplayStyle.NilAnimation;
                                            break;
                                        case Movement.Up:
                                            styleArray = sourceCell.Style.DisplayStyle.UpAnimation;
                                            break;
                                        case Movement.Down:
                                            styleArray = sourceCell.Style.DisplayStyle.DownAnimation;
                                            break;
                                    }

                                    // At ths point there's been an movement that has an associated animation effect.
                                    if (styleArray != null)
                                    {

                                        // If an animated sequence has been defined for this cell, then place the column and row 
                                        // coordinates in a list that will drive the animated cell through their sequence.  The sequence is
                                        // primed by grabbing the first color (the start color) out of the array of styles.  This color
                                        // will immediately be show as soon as the XML is finished reading.  The rest of the colors will be
                                        // displayed as the animation thread gets around to them.
                                        destinationCell.AnimationIndex = 0;
                                        destinationCell.AnimationArray = styleArray;
                                        destinationCell.Style = styleArray[0];
                                        this.spreadsheet.animatedList.Add(destinationCell);

                                    }

                                }

                            }
                            else
                            {
                                destinationCell.Style = sourceCell.Style;
                            }

							// This will cause the cell to be updated when measured again.
							destinationCell.IsModified = true;
							if (destinationCell.Data is Bitmap)
								((Bitmap)destinationCell.Data).Dispose();
							destinationCell.Data = sourceCell.Data;
							

						}

					}

					// NOTE: I have no idea why this seems to fix the weird price update, but it does.
					// We reset the spreadsheet row, and it is created when the new row element is parsed.
					// I think this is an optimization to re-use rows, but this seems to work.
					this.currentRow = null;
				}

				break;

			case ParsingRowState.Delete:

				// Find the row in the data table based on the primary key.
				keyIndex = 0;
				foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.PrimaryKey)
					this.searchKey[keyIndex++] = this.currentRow[spreadsheetColumn].Data;
				spreadsheetRow = (SpreadsheetRow)this.spreadsheet.Rows.Find(this.searchKey);

				// If the row exists, then remove any reference to the row from the list of animated cells (this is to prevent the
				// animation thread from trying to update a cell that no longer exists in the viewer).  Finally, remove the row
				// from the data in the viewer.
				if (spreadsheetRow != null)
				{
					spreadsheetRow.Delete();
					this.isResortingRequired = true;

				}

				break;

			}

			// In order to improve the response of the application using this database when a large document is being parsed,
			// control is returned to the other threads periodically.  A command is also broadcast to update the viewer with the
			// partial document so the viewer can present something to the user quickly while the rest of the document is 
			// constructed in this background thread.
			if (++this.rowCounter % updateTrigger == 0)
			{

				// Measure the document and get the new size.  These will be broadcast out to the viewer when the locks have been
				// removed.
				Region region = this.spreadsheet.MeasureData();
				Size size = this.spreadsheet.displayRectangle.Size;

				// Accept the changes to the spreadsheet database for this set of partial results.
				this.spreadsheet.AcceptChanges();

				// This will release the locks on the table so it can be read by the viewer, then it will broadcast the changes,
				// allow the other threads, including the foreground message thread, to run for a little while.  When control is
				// returned to this thread, the construction of the document will continue.
				this.spreadsheet.readerWriterLock.ReleaseWriterLock();
				this.spreadsheet.OnDataSizeChanged(size);
				this.spreadsheet.OnDataInvalidated(region);
				Thread.Sleep(0);
				this.spreadsheet.readerWriterLock.AcquireWriterLock(Timeout.Infinite);

			}

		}

		/// <summary>
		/// Handler for the start of the Row element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseRowStart()
		{

			// While constructing the document and handling incremental updates, it's very important to conserve the number of
			// memory allocation operations.  While the CPU would never notice a few scattered 'new' operations, the sheer volume
			// of data handled while reading in a large document can be a significant drain on not only the CPU, but the memory
			// reserves.  The general stragegy here is to reuse the rows whenever possible.  In all cases, a row is provided for
			// reading values in from the transformed document.  When the row is inserted, such as when a new document is 
			// constructed, that row is added to the SpreadsheetTable and a new row is allocated for the next line that is read.
			// Updates are handled differently: the values are copied from the row used for parsing and that row is reused for the
			// next line read.
			if (this.currentRow == null)
				this.currentRow = this.spreadsheet.NewSpreadsheetRow();

			if (parsingRowState == ParsingRowState.Update)
				foreach (DataColumn dataColumn in this.spreadsheet.Columns)
					this.currentRow[dataColumn].IsModified = false;

			// Set up the defaults for this row.
			this.displayHeight = Convert.ToInt32(DefaultSpreadsheet.DisplayRowHeight);
			this.printerHeight = Convert.ToInt32(DefaultSpreadsheet.PrinterRowHeight);

		}

		private void ParseParentEnd()
		{

			Style parentStyle;
			if (this.spreadsheet.styleTable.TryGetValue(this.text, out parentStyle))
				this.style.Parent = parentStyle;

			// Make sure that none of the animation sequences are inherited from the parent.
			this.style.DisplayStyle.NilAnimation = null;
			this.style.DisplayStyle.UpAnimation = null;
			this.style.DisplayStyle.DownAnimation = null;

		}

		/// <summary>
		/// Handler for the start of the Style tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseStyleEnd()
		{

			// Add this style to the list of styles in this spreadsheet.
			this.spreadsheet.styleTable.Add(this.style.Id, this.style);

			// This is a shortcut to the default style for this workbook.
			if (this.styleId == "Default")
				this.defaultStyle = this.style;

			// As the attributes of the style are parsed, they can either belong to the screen style or the printer style.  This
			// member keeps track of which device should be modified by the incoming attributes.
			this.device = Device.Common;

		}

		/// <summary>
		/// Handler for the start of the Style tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseStyleStart()
		{

			this.style = new Style();

		}

		/// <summary>
		/// Handler for the Styles tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseStylesStart()
		{

			// This element acts as a collection of styles.
			this.spreadsheet.styleTable = new Dictionary<string, Style>();

		}

		/// <summary>
		/// Handler for the start of the Table element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseTableStart()
		{

			// The columns are going to be blown away by this method.  This will render any row filtering slightly brain-damaged
			// due to some sort of bug in the way the columns relate to the filter.  Clearing the filter out before the columns are
			// reloaded and then restoring it seems to be an effective way around the bug.  It just makes sense that if some sort
			// of compiled expression tried to reference columns that have been cleared out that you'd run into an issue.
			this.spreadsheet.rowFilter = this.spreadsheet.spreadsheetRowView.RowFilter;
			this.spreadsheet.spreadsheetRowView.RowFilter = string.Empty;
			this.spreadsheet.sort = this.spreadsheet.spreadsheetRowView.Sort;
			this.spreadsheet.spreadsheetRowView.Sort = string.Empty;

			// The Table tag is a trigger to initialize the data structure.  Incremental changes -- such as price changes -- are
			// handled through the Update which bypasses this initialization sequence.
			this.spreadsheet.Reset();
			this.spreadsheet.ViewColumns.Clear();
			this.spreadsheet.PrimaryKey = new DataColumn[0];

			// The old animation list is no longer valid after reading a new document.  Note that there is an implicit Mutex in the
			// animated list because a there is a background thread that also uses this list.
			this.spreadsheet.animatedList.Clear();

			this.spreadsheet.headerRectangle = new Rectangle(0, 0, 0, Convert.ToInt32(DefaultSpreadsheet.HeaderHeight));

			// The dimensions of the printed and screen spreadsheet will be calculated as the data is read in from the XML source.
			this.spreadsheet.displayRectangle = Rectangle.Empty;
			this.spreadsheet.printerRectangle = Rectangle.Empty;

			this.rowIndex = 0;
			this.parsingRowState = ParsingRowState.Insert;

		}

		private void ParseHeaderHeight()
		{

			// Examine the parsing stack.
			Driver driver = this.stack.Peek();

			// The attributes of this element can override the default settings.
			this.spreadsheet.headerRectangle = new Rectangle(0, 0, 0,
				Convert.ToInt32(Math.Round(Convert.ToSingle(this.text) * DefaultSpreadsheet.PixelsPerUnit)));

		}

		private void ParseDirectionEnd()
		{

			this.direction = this.text;

		}

		/// <summary>
		/// Handler for the start of the ViewColumn element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseViewColumnEnd()
		{

			// This will be used to determine the sort order.
			this.viewColumn = new ViewColumn();
			this.viewColumn.ColumnId = this.columnId;
			this.viewColumn.Direction = (SortDirection)Enum.Parse(typeof(SortDirection), this.direction, true);
			this.spreadsheet.defaultView.Add(this.viewColumn);

		}

		/// <summary>
		/// Handler for the start of the ViewColumn element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseViewColumnStart()
		{

			// The view columns provide a column and direction instruction for sorting the viewer.
			this.columnId = string.Empty;
			this.direction = "Ascending";

		}

		/// <summary>
		/// Begin parsing an update to a row.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private void ParseFragmentEnd()
		{

			CloseDocument();

		}

		/// <summary>
		/// Begin parsing an update to a row.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private void ParseFragmentStart()
		{

			OpenDocument();

		}

		/// <summary>
		/// Begin parsing an update to a row.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private void ParseInsertStart()
		{

			// This controls how a row is processed.
			this.parsingRowState = ParsingRowState.Insert;
			this.currentRow = null;

		}

		/// <summary>
		/// Begin parsing an update to a row.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private void ParseUpdateStart()
		{

			// This controls how a row is processed.
			this.parsingRowState = ParsingRowState.Update;
			this.currentRow = null;

			// Since the size of the key is fixed at this point, a semi-permanent key holder can be allocated here instead of
			// each time a row is parsed in.  It may not seem significant here, but this same operation is repeated for every
			// row that is read.  Note that this search key is only used for parsing rows.  Any other use can corrupt the
			// importing of XML data.
			this.searchKey = new object[this.spreadsheet.PrimaryKey.Length];

		}

		/// <summary>
		/// Handler for the start of the View element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseViewStart()
		{

			// The view decides how the data in the viewer is ordered.  This will not necessarily be the same order as the
			// constraints or the primary index.
			this.spreadsheet.defaultView = new List<ViewColumn>();

		}

		/// <summary>
		/// Handler for the end of the View element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseViewEnd()
		{

			this.spreadsheet.ApplyView();

		}

		/// <summary>
		/// Handler for the Workbook tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseWorkbookEnd()
		{

			CloseDocument();

		}

		/// <summary>
		/// Handler for the Workbook tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseWorkbookStart()
		{

			OpenDocument();
			this.currentRow = null;

		}

		/// <summary>
		/// Handler for the start of the Protection element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private void ParseWorksheetStart()
		{

			// This is the default action for selecting objects in the view.
			this.spreadsheet.SelectionMode = DefaultSpreadsheet.SelectionMode;

		}

	}

}
