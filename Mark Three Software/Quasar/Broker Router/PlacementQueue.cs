namespace Shadows.Quasar.Market.Router
{

	using System;
	using System.Collections;
	using System.Threading;

	/// <summary>
	/// A queue of prices.
	/// </summary>
	public class PlacementQueue : Queue
	{

		private AutoResetEvent placementIderEvent;

		/// <summary>
		/// Represents a stream of price change records.
		/// </summary>
		public PlacementQueue()
		{

			// This event is used to signal a waiting thread that new data is avaiable in the placementIder.
			placementIderEvent = new AutoResetEvent(false);

		}

		/// <summary>
		/// Thread safe access to the number of elements in the queue.
		/// </summary>
		public new int Count {get {lock (this) return base.Count;}}

		/// <summary>
		/// Suspends the process until the placementIder has data in it.
		/// </summary>
		public void WaitOne()
		{

			// Wait for an event to signal us that there are new quotes in the price queue.
			this.placementIderEvent.WaitOne();

		}

		/// <summary>
		/// Place a price into the queue.
		/// </summary>
		/// <param name="priceRow">A price record.</param>
		public void Enqueue(int placementId)
		{

			// Make sure that only one thread at a time has access to the queue.
			lock (this)
			{

				// Place the placementId in the queue.
				base.Enqueue(placementId);

				// Signal anyone waiting on a placementId that one is ready in the queue.
				this.placementIderEvent.Set();

			}

		}

		/// <summary>
		/// Remove a price record from the queue.
		/// </summary>
		/// <returns>The next price record on the queue.</returns>
		public new int Dequeue()
		{
			
			// Make sure that only one thread at a time has access to the queue.
			lock (this)
				return (int)base.Dequeue();
		
		}

	}

}
