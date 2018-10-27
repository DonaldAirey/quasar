namespace MarkThree.Forms
{

	// The element names parsed in from the XML are turned into tokens which then drive how the XML data is read into the data
	// structures.  These tokens basically represent element names in the modified Excel XML spreadsheet schema.
	public enum Token 
	{
		Alignment, Animation, ApplyTemplate, Attribute, Bold, Border, Borders, Cell, Choose, Color, Column, ColumnId,
		ColumnReference, Columns, Comment, Constraint, Constraints, Delete, Description, Direction, Document, Element, EndElement,
		Family, Font, FontName, Format, Formula, Fragment, HeaderHeight, Height, Horizontal, If, Image, Insert, Interior, Italic,
		LineStyle, Namespace, NumberFormat, Otherwise, Output, Parent, Pattern, Position, PrimaryKey, ProcessingInstruction,
		Protected, Protection, ReadingOrder, Root, Row, RowFilter, SelectionMode, Size, Sort, SortColumn, StartColor, Steps,
		Strikeout, Style, StyleId, Styles, Stylesheet, Table, Template, Text, Type, Underline, Unique, Update, ValueOf, Variable,
		Vertical, View, Weight, When, Width, match, name, select, test, version
	};
		
}
