namespace MarkThree
{

	using System;
	using System.Threading;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// A queue of prices.
	/// </summary>
	public class ActionQueue
	{

		private Queue<Action> queue;
		private ManualResetEvent queueEvent;

		/// <summary>
		/// Represents a stream of price change records.
		/// </summary>
		public ActionQueue()
		{

			// This queue is used to hold the elements.
			this.queue = new Queue<Action>();

			// This event is used to signal a waiting thread that new data is avaiable in the ticker.
			this.queueEvent = new ManualResetEvent(false);

		}

		/// <summary>
		/// Thread safe access to the number of elements in the queue.
		/// </summary>
		public int Count {get {lock (this) return this.queue.Count;}}

		/// <summary>
		/// Suspends the process until the ticker has data in it.
		/// </summary>
		public void WaitOne()
		{

			// Wait for an event to signal us that there are new quotes in the price queue.
			this.queueEvent.WaitOne();

		}

		/// <summary>
		/// Place a price into the queue.
		/// </summary>
		/// <param name="priceRow">A price record.</param>
		public void Enqueue(Action action)
		{

			// Make sure that only one thread at a time has access to the queue.
			lock (this)
			{

				// Place the tick in the queue.
				this.queue.Enqueue(action);

				// Signal anyone waiting on a tick that one is ready in the queue.
				this.queueEvent.Set();

			}

		}

		/// <summary>
		/// Remove a price record from the queue.
		/// </summary>
		/// <returns>The next price record on the queue.</returns>
		public Action Dequeue()
		{
			
			// If there is nothing in the queue, wait until something shows up.
			if (this.Count == 0)
				this.WaitOne();

			// The object must be locked from other threads while extracting elements.
			lock (this)
			{

				// Remove the first item placed in the queue.
				Action action = this.queue.Dequeue();

				// If there is nothing left in the queue, then clear the event.  This will block any calls to 'Dequeue' until there
				// is something to extract from the queue.
				if (this.queue.Count == 0)
					this.queueEvent.Reset();

				// This is the first item placed in the queue.
				return action;

			}

		}

	}
	
}
