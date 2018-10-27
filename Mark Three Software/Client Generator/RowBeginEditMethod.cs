namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description a method to handle the Row Changed event.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class RowBeginEditMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Changed event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public RowBeginEditMethod(TableSchema tableSchema)
		{

			// To reduce the frequency of deadlocking, the tables are always locked in alphabetical order.  This section collects
			// all the table locks that are used for this operation and organizes them in a list that is used to generate the
			// locking and releasing statements below.
			List<LockRequest> tableLockList = new List<LockRequest>();
			tableLockList.Add(new WriteRequest(tableSchema));
			foreach (KeyrefSchema parentKeyref in tableSchema.ParentKeyrefs)
				tableLockList.Add(new ReadRequest(parentKeyref.Refer.Selector));
			tableLockList.Sort();

			//			/// <summary>
			//			/// Starts an edit operation on a DataModel.EmployeeRow object.
			//			/// </summary>
			//			public new void BeginEdit()
			//			{
			//				// This will lock all tables for editing and suspend checking the constraints until the 'EndEdit' is called.
			//				DataModel.Department.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//				DataModel.Employee.ReaderWriterLock.AcquireWriterLock(System.Threading.Timeout.Infinite);
			//				DataModel.Object.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//				DataModel.Race.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//				base.BeginEdit();
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Starts an edit operation on a {0}.{1}Row object.", tableSchema.DataModelSchema.Name, tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.New;
			this.Name = "BeginEdit";
			this.Statements.Add(new CodeCommentStatement("This will lock all tables for editing and suspend checking the constraints until the 'EndEdit' is called."));
			foreach (LockRequest lockRequest in tableLockList)
			{
				CodeExpression tableExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(tableSchema.DataModelSchema.Name), lockRequest.TableSchema.Name);
				if (lockRequest is ReadRequest)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(tableExpression, "ReaderWriterLock"), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
				if (lockRequest is WriteRequest)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(tableExpression, "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			}
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "BeginEdit"));

		}

	}

}
