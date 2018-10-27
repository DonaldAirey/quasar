namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>Default values for the stylesheet and spreadsheet.</summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class DefaultSpreadsheet
	{
		public static System.Single XmlVersion = 1.0f;
		public static System.Boolean IsProtected = false;
		public static MarkThree.Forms.SelectionMode SelectionMode = SelectionMode.Cell;
		public static System.Drawing.Color ForeColor = Color.Black;
		public static System.Drawing.Color BackColor = Color.White;
		public static System.Drawing.Color BorderColor = Color.DarkGray;
		public static System.Drawing.Color StartColor = Color.Black;
		public static System.Drawing.FontStyle FontStyle = FontStyle.Regular;
		public static System.Drawing.StringAlignment Alignment = StringAlignment.Near;
		public static System.Drawing.StringAlignment LineAlignment = StringAlignment.Center;
		public static System.Drawing.StringFormatFlags FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox;
		public static System.Object Value = DBNull.Value;
		public static System.Single RowHeight = 17.0f;
		public static System.Single ColumnWidth = 64.0f;
		public static System.Single HeaderHeight = 20.0f;
		public static System.Int32 Steps = 10;
		public static System.Single BorderWeight = 1.0f;
		public static System.Single FontSize = 8.25f;
		public static System.String FontName = "Microsoft Sans Serif";
		public static System.String Format = "{0:}";
		public static System.String DataType = "string";
		public static System.String StyleId = "Default";

	}
}
