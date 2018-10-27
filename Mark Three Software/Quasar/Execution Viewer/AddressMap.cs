/*************************************************************************************************************************
*
*	File:			AddressMap.cs
*	Description:	An address map is a quick lookup table for cells in the spreadsheet document.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Common;
using System;
using System.Diagnostics;
using System.Xml;

namespace Shadows.Quasar.Viewers.Execution
{

	/// <summary>
	/// Summary description for AddressMap.
	/// </summary>
	internal class AddressMap : AddressMapSet
	{

		private int placeholderRow;

		public int PlaceholderRow {get {return this.placeholderRow;}}

		/// <summary>
		/// Creates an empty, but still functional, AddressMap
		/// </summary>
		public AddressMap() : base() {}

		/// <summary>
		/// Creates an address map from an XML document.
		/// </summary>
		/// <param name="xmlDocument">The document to be parsed for addresses</param>
		/// <param name="documentVersion">Identifies a particular version of the document</param>
		public AddressMap(XmlDocument xmlDocument, int documentVersion) : base()
		{

			// This table is used to quickly map an element name to a token.
			ElementNameTable elementNameTable = new ElementNameTable();
		
			// These variables are used to track the states when parsing the XML Spreadsheet.
			int rowType = 0;
			int rowIndex = 0;
			int columnIndex = 0;
			int columnType = 0;
			ParsingState parsingState = ParsingState.None;

			// IMPORTANT CONCEPT: Create a fast, forward only reader on the XML Document.  This saves a lot of time over 
			// trying to use XPath commands on the version used for the document.  The main idea is that we are going to
			// rip through the spreadsheet XML looking only for specific element nodes.  Certain elements will cause the
			// state of the parsing to change (e.g. finding a 'ss:Row' element will increment the row counter.  Other
			// nodes contain data that helps to map the spreadsheet document into a data structure that can be used to
			// address individual cells in the spreadsheet, which is the end product of this method.
			XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlDocument);
		
			// Rip through the XML file looking for specific node types and names.
			while (xmlNodeReader.Read())
			{

				// The outer switch statement divides the nodes up into their types.  The inner switch statement will
				// examine their names.  We will switch from one state to another based on this combination of node type
				// and name.  The state ultimately tells us how to interpret the data when we run across it.
				switch (xmlNodeReader.NodeType)
				{

					case XmlNodeType.Element:

						// Turn the element name into a token so we can quickly jump to the code to handle it.
					switch (elementNameTable[xmlNodeReader.Name])
					{

						case ElementName.NamedRange:

							// Create a mapping of column names to column types and vica-versa.
							columnIndex = SpreadsheetColumn.Parse(xmlNodeReader["ss:RefersTo"]);
							columnType = ExecutionColumnType.Tokenize(xmlNodeReader["ss:Name"]);
							if (columnIndex != SpreadsheetColumn.DoesNotExist && columnType != ExecutionColumnType.None)
							{

								// Make sure that errors in the column names don't kill the whole parsing session.
								try
								{
									this.ColumnIndexMap.AddColumnIndexMapRow(columnType, columnIndex);
									this.ColumnIdMap.AddColumnIdMapRow(columnIndex, columnType);
								}
								catch (Exception exception)
								{

									// Write the error and stack trace out to the debug listener
									Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

								}

							}
							break;

						case ElementName.Row:

							// Increase the row index monotonically if no explicit index is given.
							string rowIndexText = xmlNodeReader["ss:Index"];
							rowIndex = (rowIndexText == null) ? rowIndex + 1 : Convert.ToInt32(rowIndexText);

							// Reset the state of the row when we recognize a new row.
							rowType = ExecutionRowType.Unused;
							columnIndex = 0;

							break;

						case ElementName.Cell:

							// Increase the column index monotonically if no explicit index is given.
							string columnIndexText = xmlNodeReader["ss:Index"];
							columnIndex = (columnIndexText == null) ? columnIndex + 1 : Convert.ToInt32(columnIndexText);
							break;

						case ElementName.Data:

							// If we're on the row type column (typically, the first column of every row in the 
							// spreadsheet), then the next Text node will contain the row type.
							if (columnIndex == GetColumnIndex(ExecutionColumnType.RowType))
							{
								parsingState = ParsingState.RowType;
								break;
							}

							// If the current column is a position type code on a position row, then look for the position
							// type code the next time a Text node is found.
							if (rowType == ExecutionRowType.LocalExecution && columnIndex == GetColumnIndex(ExecutionColumnType.ExecutionId))
							{
								parsingState = ParsingState.LocalExecutionId;
								break;
							}

							// If the current column is a position type code on a position row, then look for the position
							// type code the next time a Text node is found.
							if (rowType == ExecutionRowType.GlobalExecution && columnIndex == GetColumnIndex(ExecutionColumnType.ExecutionId))
							{
								parsingState = ParsingState.GlobalExecutionId;
								break;
							}

							// The placeholder is a special row with no key.  But since there is only one, we can keep 
							// track of where it is.  The placeholder is important for creating new executions.
							if (rowType == ExecutionRowType.Placeholder)
							{
								parsingState = ParsingState.Placeholder;
								break;
							}

							break;

						default:

							// The parsing state is only good for the scope of the 'Data' element.  If we didnt' 
							// recognize a parsing state above, then reset the state value so we don't get confused the
							// next time we read a 'Data' node.
							parsingState = ParsingState.None;
							break;

					}

						break;

					case XmlNodeType.Text:

						// The state variable tells us how to interpret the text nodes.
					switch (parsingState)
					{
	
						case ParsingState.RowType:

							// This node tells us what kind of row we are reading.
							rowType = Convert.ToInt32(xmlNodeReader.Value);
							break;

						case ParsingState.LocalExecutionId:

							// The position type code is found in this cell.
							int localExecutionId = Convert.ToInt32(xmlNodeReader.Value);
							this.LocalMap.AddLocalMapRow(localExecutionId, documentVersion, rowIndex);

							break;

						case ParsingState.GlobalExecutionId:

							// The position type code is found in this cell.
							int globalExecutionId = Convert.ToInt32(xmlNodeReader.Value);
							this.GlobalMap.AddGlobalMapRow(globalExecutionId, documentVersion, rowIndex);

							break;

						case ParsingState.Placeholder:

							// Keep track of where the placeholder is for new executions.
							this.placeholderRow = rowIndex;
							break;

					}

						// Reset the parsing state after each text node is read and interpreted.
						parsingState = ParsingState.None;

						break;

				}

			}

			// Commit all the changes made to the data set.
			this.AcceptChanges();

		}

		/// <summary>
		/// Returns the column index in the current document based on it's constant type.
		/// </summary>
		/// <param name="columnType">The type of column we are looking for.</param>
		/// <returns>The index of the column, 'SpreadsheetColumn.DoesNotExist' if the column isn't present in this document.
		/// </returns>
		public int GetColumnIndex(int columnType)
		{

			// The address map is constructed when the document is created.  It maps the variable elements of a report
			// to fixed values and provides a means for finding an row or cell based on fixed values.  See if an entry
			// was created when we parsed the document for the given column type.
			AddressMapSet.ColumnIndexMapRow columnIndexMapRow = this.ColumnIndexMap.FindByColumnId(columnType);
			return (columnIndexMapRow == null) ? SpreadsheetColumn.DoesNotExist : columnIndexMapRow.ColumnIndex;

		}

		/// <summary>
		/// Returns the column index in the current document based on it's constant type.
		/// </summary>
		/// <param name="columnType">The type of column we are looking for.</param>
		/// <returns>The index of the column, 'SpreadsheetColumn.DoesNotExist' if the column isn't present in this document.
		/// </returns>
		public int GetColumnType(int columnIndex)
		{

			// The address map is constructed when the document is created.  It maps the variable elements of a report
			// to fixed values and provides a means for finding an row or cell based on fixed values.  See if an entry
			// was created when we parsed the document for the given column type.
			AddressMapSet.ColumnIdMapRow columnIdMapRow = this.ColumnIdMap.FindByColumnIndex(columnIndex);
			return (columnIdMapRow == null) ? SpreadsheetColumn.DoesNotExist : columnIdMapRow.ColumnId;

		}
	
	}

	internal enum ParsingState {None, RowType, LocalExecutionId, GlobalExecutionId, Placeholder};

}
