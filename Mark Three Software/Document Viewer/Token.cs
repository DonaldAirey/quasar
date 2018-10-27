namespace MarkThree.Forms
{

	// The element names parsed in from the XML are turned into tokens which then drive how the XML data is read into the data
	// structures.  These tokens basically represent element names in the modified Excel XML spreadsheet schema.
	public enum Token 
	{
		Alignment, Animation, ApplyTemplate, ApplyTemplates, Attribute, Background, Bold, BottomBorder, Color, Column, ColumnId,
		ColumnReference, Columns, Comment, Constraint, Constraints, Data, DataTransform, DataTransformId, Description, Direction,
		Down, Effect, Element, EndElement, Factor, Family, Filter, Font, FontBrush, FontName, Foreground, Format, Formula,
		Fragment, Height, Image, InteriorBrush, Italic, LeftBorder, LineAlignment, LineStyle, Lock, Locks, Match, Name, Namespace,
		NumberFormat, Off, On, Order, Parent, Pattern, Position, PrimaryKey, Protected, Protection, ReadingOrder, Repeat,
		RightBorder, Root, Row, RowId, Rows, Same, Scale, Scratch, Select, Size, Sort, SortApplyTemplate, SortColumn, SortRow,
		SortTemplate, Sorts, Source, Split, StartColor, Steps, Strikeout, StringFormat, Style, StyleId, Styles, Stylesheet, Table,
		TargetNamespace, Template, Templates, Text, Tile, TopBorder, Type, Underline, Unique, Up, Variable, VariableName, View,
		Visible, Width
	}; 
}
