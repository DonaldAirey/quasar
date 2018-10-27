using MarkThree.Quasar;
using System;
using System.Data;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Summary description for Object.
	/// </summary>
	public class Object
	{

		/// <summary>
		/// Indicates if a given object is a parent of another object.
		/// </summary>
		/// <param name="parent">The parent object</param>
		/// <param name="child">The child object</param>
		/// <returns>true if the parent object is a parent of the given child object.</returns>
		public static bool IsParent(DataModel.ObjectRow parent, DataModel.ObjectRow child)
		{

			// When we find the classification record, we know that the test is true.
			if (child.ObjectId == parent.ObjectId)
				return true;

			// Recursively test each parent of the current node to see if it is a ancestor of the current node.
			foreach (DataModel.ObjectTreeRow objectTreeRow in child.GetObjectTreeRowsByFKObjectObjectTreeChildId())
				if (IsParent(parent, objectTreeRow.ObjectRowByFKObjectObjectTreeParentId))
					return true;

			// If none of the parents are in the classification scheme, then this node can't be either.
			return false;

		}

	}

}
