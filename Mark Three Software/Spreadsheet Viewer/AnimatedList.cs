namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Threading;

	/// <summary>Used to keep track of the cells that are going through an animation sequence.</summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class AnimatedList
	{

		private MarkThree.Forms.CellOrderComparer cellOrderComparer;
		private System.Collections.ArrayList arrayList;
		private System.Threading.Mutex mutex;

		/// <summary>
		/// Create the AnimatedList.
		/// </summary>
		public AnimatedList()
		{

			// Initialize the object.
			this.arrayList = new ArrayList();
			this.mutex = new Mutex();
			this.cellOrderComparer = new CellOrderComparer();

		}

		/// <summary>
		/// A Lock to exclude other threads from reading or modifying the list of animated cells.
		/// </summary>
		public Mutex Mutex {get {return this.mutex;}}

		/// <summary>
		/// Add an address to the list of animated cells.
		/// </summary>
		/// <param name="spreadsheetCell">A Cell Address to be animated.</param>
		/// <returns>The cell that was added to the list.</returns>
		public SpreadsheetCell Add(SpreadsheetCell spreadsheetCell)
		{

			try
			{

				// Make sure no other threads modify the list while an address is being added.
				this.mutex.WaitOne();

				// The list must be ordered to prevent thrashing during the animation.
				int index = this.arrayList.BinarySearch(spreadsheetCell, this.cellOrderComparer);
				if (index < 0)
					this.arrayList.Insert(~index, spreadsheetCell);

				// This cell will now be animated.
				return spreadsheetCell;

			}
			finally
			{

				// Other threads can now use the list.
				this.mutex.ReleaseMutex();

			}

		}

		/// <summary>
		/// Gets the AnimatedCell at the given index in the list.
		/// </summary>
		public SpreadsheetCell this[int index]
		{
			
			get
			{

				// Make sure the list is locked before extracing the requested cell.
				try
				{
					this.mutex.WaitOne();
					return (SpreadsheetCell)this.arrayList[index];
				}
				finally
				{
					this.mutex.ReleaseMutex();
				}

			}

		}

		/// <summary>
		/// Removes the AnimatedCell from the animation task.
		/// </summary>
		/// <param name="spreadsheetCell">The cell to be removed.</param>
		public void Remove(SpreadsheetCell spreadsheetCell)
		{

			try
			{

				// Make sure the list is locked before removing the cell.  Note that the list is sorted, so the binary search picks
				// up the element quickly and removing the item using the index into the array instead of the reference is quicker 
				// still.
				this.mutex.WaitOne();
				int index = this.arrayList.BinarySearch(spreadsheetCell, this.cellOrderComparer);
				if (index >= 0)
					this.arrayList.RemoveAt(index);

			}
			finally
			{
				this.mutex.ReleaseMutex();
			}

		}

		/// <summary>
		/// Returns the number of AnimatedCells in the animation task.
		/// </summary>
		public int Count
		{
			
			// Make sure the table is exclusively owned before counting the elements.
			get
			{
				try
				{
					this.mutex.WaitOne();
					return this.arrayList.Count;
				}
				finally
				{
					this.mutex.ReleaseMutex();
				}
			}

		}

		/// <summary>
		/// Clears the animated task list.
		/// </summary>
		public void Clear() {try {this.mutex.WaitOne(); this.arrayList.Clear();} finally {this.mutex.ReleaseMutex();}}

	}

}
