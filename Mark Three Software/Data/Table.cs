namespace MarkThree
{

	using System;
	using System.Data;
	using System.Collections.Generic;
	using System.Threading;

	/// <summary>
	/// Represents one table of in-memory data.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Table : DataTable
	{

		// Public Properties
		public readonly MarkThree.Column RowVersionColumn;
		public new readonly MarkThree.ColumnCollection Columns;
		public new readonly MarkThree.RelationCollection ChildRelations;
		public new readonly MarkThree.RelationCollection ParentRelations;
		public new readonly MarkThree.RowCollection Rows;
		public MarkThree.RowFilterDelegate UserFilter;
		public System.Boolean IsPersistent;
		public readonly System.Collections.Generic.List<Index> Indices;
		public readonly System.Threading.ReaderWriterLock ReaderWriterLock;

		/// <summary>
		/// Create a table of in-memory data.
		/// </summary>
		/// <param name="name"></param>
		public Table()
		{

			// Initialize the object.
			this.IsPersistent = true;
			this.Columns = new ColumnCollection(base.Columns);
			this.Rows = new RowCollection(this, base.Rows);
			this.ChildRelations = new RelationCollection(base.ChildRelations);
			this.ParentRelations = new RelationCollection(base.ParentRelations);
			this.Indices = new List<Index>();
			this.ReaderWriterLock = new ReaderWriterLock();

			// IMPORTANT CONCEPT: All rows have a row version which is used to reconcile the client data model with the server 
			// data model. When the row version of a server row is newer than the row version on the client, that record and only
			// that record is transmitted to the client.  In this way, the client data model is reconciled to the server data model
			// with a minimum amount of traffic between the two.
			this.RowVersionColumn = new Column("RowVersion", typeof(long), string.Empty, MappingType.Attribute);
			this.RowVersionColumn.IsPersistent = true;
			this.Columns.Add(this.RowVersionColumn);

			// The 'Indicies' is a subset of the constraint collection.  It is often clumsy to search all the constraints looking
			// for just an index, so a list of just the indices is maintained through this event handler.
			this.Constraints.CollectionChanged += new System.ComponentModel.CollectionChangeEventHandler(Constraints_CollectionChanged);

		}

		/// <summary>
		/// Create a table of in-memory data.
		/// </summary>
		/// <param name="name"></param>
		public Table(string name) : 
			base(name)
		{

			// Initialize the object.
			this.IsPersistent = true;
			this.Columns = new ColumnCollection(base.Columns);
			this.Rows = new RowCollection(this, base.Rows);
			this.ChildRelations = new RelationCollection(base.ChildRelations);
			this.ParentRelations = new RelationCollection(base.ParentRelations);
			this.Indices = new List<Index>();
			this.ReaderWriterLock = new ReaderWriterLock();

			// IMPORTANT CONCEPT: All rows have a row version which is used to reconcile the client data model with the server 
			// data model. When the row version of a server row is newer than the row version on the client, that record and only
			// that record is transmitted to the client.  In this way, the client data model is reconciled to the server data model
			// with a minimum amount of traffic between the two.
			this.RowVersionColumn = new Column("RowVersion", typeof(long), string.Empty, MappingType.Attribute);
			this.RowVersionColumn.IsPersistent = true;
			this.Columns.Add(this.RowVersionColumn);

			// The 'Indicies' is a subset of the constraint collection.  It is often clumsy to search all the constraints looking
			// for just an index, so a list of just the indices is maintained through this event handler.
			this.Constraints.CollectionChanged += new System.ComponentModel.CollectionChangeEventHandler(Constraints_CollectionChanged);

		}

		protected Table(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }

		/// <summary>
		/// Maintains the list of indices.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void Constraints_CollectionChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
		{

			// This will add indices to the specialized 'Indices' list when they are added as a constraint.
			if (e.Action == System.ComponentModel.CollectionChangeAction.Add)
				if (e.Element is Index)
					this.Indices.Add(e.Element as Index);

			// This will remove an index from the specialized 'Indices' list when they are removed as a constraint.
			if (e.Action == System.ComponentModel.CollectionChangeAction.Remove)
				if (e.Element is Index)
					this.Indices.Remove(e.Element as Index);

		}
        
		/// <summary>
		/// Creates a new MarkThree.Row with the same schema as the table.
		/// </summary>
		/// <returns>A new MarkThree.Row with the same schema as the table.</returns>
		public new Row NewRow() { return base.NewRow() as Row; }

		/// <summary>
		/// Creates a new MarkThree.Row with the same schema as the table.
		/// </summary>
		/// <param name="dataRowBuilder">The System.Data.DataRowBuilder type supports the .NET Framework and is not intended to be
		/// used directly by your code.</param>
		/// <returns>A new MarkThree.Row with the same schema as the table.</returns>
		protected override System.Data.DataRow NewRowFromBuilder(System.Data.DataRowBuilder dataRowBuilder)
		{

			// This will build a new from from the schema information of the table.
			return new Row(dataRowBuilder);

		}

	}
    
}
