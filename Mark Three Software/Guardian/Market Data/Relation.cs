using MarkThree.Guardian;
using System;
using System.Diagnostics;

namespace MarkThree.Guardian
{

	/// <summary>
	/// Retrieves the status of relationships in the Data Model.
	/// </summary>
	public class Relationship
	{

		/// <summary>
		/// Determines if an object is a child of another object.
		/// </summary>
		/// <param name="parentObject">The parent object.</param>
		/// <param name="childObject">The chld object.</param>
		/// <returns>True of the child object is an descendant of the parent object.</returns>
		public static bool IsChildObject(MarketData.ObjectRow parentObject, MarketData.ObjectRow childObject)
		{

			// If the parent is the same as the child, they are related.
			if (parentObject.ObjectId == childObject.ObjectId)
				return true;
			
			// Recurse through the hierarhy testing to see if the child is part of the famly tree of the parent object.
			foreach (MarketData.ObjectTreeRow objectTreeRow in parentObject.GetObjectTreeRowsByObjectObjectTreeParentId())
				if (IsChildObject(objectTreeRow.ObjectRowByObjectObjectTreeChildId, childObject))
					return true;

			// At this point, the child is not part of the parent hierarchy.
			return false;

		}

	}

}
