namespace MarkThree.Guardian
{

	/// <summary>
	/// Summary description for Hierarchy.
	/// </summary>
	public class Hierarchy
	{

		/// <summary>
		/// Determines if a given blotter is visible on the current document.
		/// </summary>
		/// <param name="parentRow">The current object in the recursive search.</param>
		/// <param name="blotterId">The blotter id to be found.</param>
		/// <returns>true if the blotter is part of the tree structure.</returns>
		public static bool IsDescendant(MarketData.ObjectRow parentRow, MarketData.ObjectRow childRow)
		{

			// Don't attempt to search if there is no starting point.
			if (parentRow != null)
			{

				// If the parent is the same as the child, then the original records are related.
				if (parentRow == childRow)
					return true;

				// Recursively search each of the children of the current node.
				foreach (MarketData.ObjectTreeRow objectTreeRow in parentRow.GetObjectTreeRowsByObjectObjectTreeParentId())
					if (IsDescendant(objectTreeRow.ObjectRowByObjectObjectTreeChildId, childRow))
						return true;

			}

			// At this point, there were no children found on this tree that matched the blotter id.
			return false;

		}
	
	}

}
