namespace MarkThree.Forms
{

	using MarkThree;
	using System;
	using System.Collections;
	using System.Data;
	using System.Xml;

	public delegate XmlElement UpdateHandler(Row row, FieldArray fields);

	public struct Update : IComparable
	{

		public MarkThree.DataAction DataAction;
		public MarkThree.Forms.FieldArray Fields;
		public MarkThree.Row Row;
		public System.Int64 RowVersion;
		public System.Object[] Key;

		public Update(DataAction dataAction, Row row)
		{

			// Initialize the object
			this.DataAction = dataAction;
			this.Row = row;
			this.RowVersion = (long)row[row.Table.RowVersionColumn, row.RowState == DataRowState.Deleted ? DataRowVersion.Original :
				DataRowVersion.Current];
			this.Fields = FieldArray.Clear;

			if (dataAction == DataAction.Delete)
			{
				this.Key = new object[this.Row.Table.PrimaryKey.Length];
				for (int keyIndex = 0; keyIndex < this.Row.Table.PrimaryKey.Length; keyIndex++)
					this.Key[keyIndex] = this.Row[this.Row.Table.PrimaryKey[keyIndex], DataRowVersion.Original];
			}
			else
				this.Key = null;

		}

		public Update(DataAction dataAction, Row row, FieldArray fieldArray)
		{

			// Initialize the object
			this.DataAction = dataAction;
			this.Row = row;
			this.Key = null;
			this.RowVersion = (long)row[row.Table.RowVersionColumn, row.RowState == DataRowState.Deleted ? DataRowVersion.Original :
				DataRowVersion.Current];
			this.Fields = fieldArray;

		}

		#region IComparable Members

		public int CompareTo(object obj)
		{

			if (obj is Update)
			{

				Update update = (Update)obj;

				return this.RowVersion.CompareTo(update.RowVersion);

			}

			throw new Exception(string.Format("Can't compare a {0} to a {1}", this.GetType(), obj.GetType()));

		}

		#endregion

	}

}
