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
	class RowEndEditMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Changed event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public RowEndEditMethod(TableSchema tableSchema)
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
			//			/// Ends the edit occuring on a row.
			//			/// </summary>
			//			public new void EndEdit()
			//			{
			//				// The table can be accessed by other threads once the row editing is completed.
			//				base.EndEdit();
			//				DataModel.Department.ReaderWriterLock.ReleaseReaderLock();
			//				DataModel.Employee.ReaderWriterLock.ReleaseWriterLock();
			//				DataModel.Object.ReaderWriterLock.ReleaseReaderLock();
			//				DataModel.Race.ReaderWriterLock.ReleaseReaderLock();
			//			}
			//		}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Ends the edit occuring on a row.", tableSchema.DataModelSchema.Name, tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.New;
			this.Name = "EndEdit";
			this.Statements.Add(new CodeCommentStatement("The table can be accessed by other threads once the row editing is completed."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "EndEdit"));
			foreach (LockRequest lockRequest in tableLockList)
			{
				CodeExpression tableExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(tableSchema.DataModelSchema.Name), lockRequest.TableSchema.Name);
				if (lockRequest is ReadRequest)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(tableExpression, "ReaderWriterLock"), "ReleaseReaderLock"));
				if (lockRequest is WriteRequest)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(tableExpression, "ReaderWriterLock"), "ReleaseWriterLock"));
			}

		}

	}

}
