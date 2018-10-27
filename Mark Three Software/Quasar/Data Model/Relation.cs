/*************************************************************************************************************************
*
*	File:			Relation.cs
*	Description:	Retrieves status about relationships in the Data Model.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using MarkThree.Quasar;
using System;
using System.Diagnostics;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Retrieves the status of relationships in the Data Model.
	/// </summary>
	public class Relationship
	{

		/// <summary>
		/// Determines if the record is a child proposedOrder of another order.
		/// </summary>
		/// <param name="proposedOrderRow"></param>
		/// <returns>True if the order is a child of another propsosed order.</returns>
		public static bool IsChildProposedOrder(DataModel.ProposedOrderRow proposedOrderRow)
		{

			// This assumes only one level of hierarchy to the order, but returns true of the order is a child of another
			// proposedOrder.
			return (proposedOrderRow.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeChildId().Length > 0) ?
				true : false;

		}
		
		/// <summary>
		/// Determines if an object is a child of another object.
		/// </summary>
		/// <param name="parentObject">The parent object.</param>
		/// <param name="childObject">The chld object.</param>
		/// <returns>True of the child object is an descendant of the parent object.</returns>
		public static bool IsChildObject(DataModel.ObjectRow parentObject, DataModel.ObjectRow childObject)
		{

			// If the parent is the same as the child, they are related.
			if (parentObject.ObjectId == childObject.ObjectId)
				return true;
			
			// Recurse through the hierarhy testing to see if the child is part of the famly tree of the parent object.
			foreach (DataModel.ObjectTreeRow objectTreeRow in parentObject.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				if (IsChildObject(objectTreeRow.ObjectRowByFKObjectObjectTreeChildId, childObject))
					return true;

			// At this point, the child is not part of the parent hierarchy.
			return false;

		}

	}

}
