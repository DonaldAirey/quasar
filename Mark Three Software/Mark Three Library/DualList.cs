namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// An unsorted and sorted list that is useful for random access and maintaining the original order of the items.
	/// </summary>
	/// <typeparam name="TKey">The primary key into the list.</typeparam>
	/// <typeparam name="TValue">The type of object containted in the list</typeparam>
	public class DualList<TKey, TValue>
	{

		// Private Members
		public System.Collections.Generic.List<TValue> unsortedList;
		public System.Collections.Generic.SortedList<TKey, TValue> sortedList;

		/// <summary>
		/// Create a list that maintains the original order and allows for random access of the elements by a key.
		/// </summary>
		public DualList()
		{

			// Initialize the object
			this.unsortedList = new List<TValue>();
			this.sortedList = new SortedList<TKey, TValue>();

		}

		/// <summary>
		/// Add an item to the Dual List.
		/// </summary>
		/// <param name="key">A key used to uniquely identify the item.</param>
		/// <param name="value">The item to be stored in the lists.</param>
		public void Add(TKey key, TValue value)
		{

			// Add the object to the unsorted and sorted list.  The unsorted list can be used to maintain the order of the items
			// independent of a sort order.  The sorted list is used to find the item quickly.
			this.unsortedList.Add(value);
			this.sortedList.Add(key, value);

		}

		/// <summary>
		/// Remove the item from the Dual List.
		/// </summary>
		/// <param name="key">A key to uniquely identify the item to be removed.</param>
		public void Remove(TKey key)
		{

			// Attempt to find the key quickly using the unique identifier and remove it from both lists when the item is found.
			TValue value;
			if (this.sortedList.TryGetValue(key, out value))
			{
				this.sortedList.Remove(key);
				this.unsortedList.Remove(value);
			}

		}

		/// <summary>
		/// Allows enumeration through the items using the original order of the items.
		/// </summary>
		/// <returns>An enumerator that allows to access the items in the original order.</returns>
		public IEnumerator<TValue> GetEnumerator() { return this.unsortedList.GetEnumerator(); }

		/// <summary>
		/// Attempts to find an item using the unique key.
		/// </summary>
		/// <param name="key">The unique key that identifies the item.</param>
		/// <param name="value">The found value or null if the item can't be found.</param>
		/// <returns>True if the item is found, false otherwise.</returns>
		public bool TryGetValue(TKey key, out TValue value)
		{

			// Use the sorted list for random access of the items.
			return this.sortedList.TryGetValue(key, out value);

		}

		/// <summary>
		/// Gets an item from the Dual List based on a unique identifier.
		/// </summary>
		/// <param name="key">A key that uniquely identifies the item.</param>
		/// <returns>The item that matches the key.</returns>
		public TValue this[TKey key] { get { return this.sortedList[key]; } set { this.sortedList[key] = value; } }

		public TValue this[int index] { get { return this.unsortedList[index]; } set { this.unsortedList[index] = value; } }

		public TValue Find(TKey tKey) { TValue tValue; return TryGetValue(tKey, out tValue) ? tValue : default(TValue); }

		public int Count { get { return this.unsortedList.Count; } }


	}

}
