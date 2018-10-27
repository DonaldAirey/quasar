namespace MarkThree
{

	using System;
	using System.Threading;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// A queue of prices.
	/// </summary>
	public class WaitQueue
	{

		private Queue queue;
		private ManualResetEvent queueEvent;

		/// <summary>
		/// Represents a stream of price change records.
		/// </summary>
		public WaitQueue()
		{

			// This queue is used to hold the elements.
			this.queue = new Queue();

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
		public void Enqueue(object queueObject)
		{

			// Make sure that only one thread at a time has access to the queue.
			lock (this)
			{

				// Place the tick in the queue.
				this.queue.Enqueue(queueObject);

				// Signal anyone waiting on a tick that one is ready in the queue.
				this.queueEvent.Set();

			}

		}

		/// <summary>
		/// Remove a price record from the queue.
		/// </summary>
		/// <returns>The next price record on the queue.</returns>
		public object Dequeue()
		{
			
			// If there is nothing in the queue, wait until something shows up.
			if (this.Count == 0)
				this.WaitOne();

			// The object must be locked from other threads while extracting elements.
			lock (this)
			{

				// Remove the first item placed in the queue.
				object queueObject = this.queue.Dequeue();

				// If there is nothing left in the queue, then clear the event.  This will block any calls to 'Dequeue' until there
				// is something to extract from the queue.
				if (this.queue.Count == 0)
					this.queueEvent.Reset();

				// This is the first item placed in the queue.
				return queueObject;

			}

		}

	}

	/// <summary>
	/// A queue of prices.
	/// </summary>
	public class WaitQueue<Type>
	{

		private System.Collections.Generic.Queue<Type> queue;
		private ManualResetEvent queueEvent;

		/// <summary>
		/// Represents a stream of price change records.
		/// </summary>
		public WaitQueue()
		{

			// This queue is used to hold the elements.
			this.queue = new Queue<Type>();

			// This event is used to signal a waiting thread that new data is avaiable in the ticker.
			this.queueEvent = new ManualResetEvent(false);

		}

		/// <summary>
		/// Place a price into the queue.
		/// </summary>
		/// <param name="priceRow">A price record.</param>
		public void Enqueue(Type queueObject)
		{

			Monitor.Enter(this.queue);

			// Place the tick in the queue.
			this.queue.Enqueue(queueObject);

			// Signal anyone waiting on a tick that one is ready in the queue.
			if (this.queue.Count == 1)
				this.queueEvent.Set();

			Monitor.Exit(this.queue);

		}

		public bool IsEmpty
		{

			get
			{

				Monitor.Enter(this.queue);
				bool isEmpty = this.queue.Count == 0;
				Monitor.Exit(this.queue);
				return isEmpty;
			}

		}

		public int Count
		{
			
			get
			{
				
				int count;
				Monitor.Enter(this.queue);
				count = this.queue.Count;
				Monitor.Exit(this.queue);
				return count;
			}
		
		}

		/// <summary>
		/// Returns the element at the beginning of the MarkThree.WaitQueue&lt;T&gt; without removing it.
		/// </summary>
		/// <returns>The next price record on the queue.</returns>
		public Type Peek()
		{

			try
			{

				// Insure thread safety.
				Monitor.Enter(this.queue);

				// If there is nothing in the queue, wait until something is put in the other end.
				if (this.queue.Count == 0)
				{
					Monitor.Exit(this.queue);
					this.queueEvent.WaitOne();
					Monitor.Enter(this.queue);
				}

				// Remove the first item placed in the queue.
				return this.queue.Peek();

			}
			finally
			{

				// The queue doesn't need to be blocked any longer.
				Monitor.Exit(this.queue);

			}

		}

		/// <summary>
		/// Remove a price record from the queue.
		/// </summary>
		/// <returns>The next price record on the queue.</returns>
		public Type Dequeue()
		{

			try
			{

				// Insure thread safety.
				Monitor.Enter(this.queue);

				// If there is nothing in the queue, wait until something is put in at the other end.
				if (this.queue.Count == 0)
				{
					Monitor.Exit(this.queue);
					this.queueEvent.WaitOne();
					Monitor.Enter(this.queue);
				}

				// Remove the first item placed in the queue.
				Type queueObject = this.queue.Dequeue();

				// If there is nothing left in the queue, then clear the event.  This will block any calls to 'Dequeue' until there
				// is something to extract from the queue.
				if (this.queue.Count == 0)
					this.queueEvent.Reset();

				// This is the first item placed in the queue.
				return queueObject;

			}
			finally
			{

				// The queue can be accessed by other threads now.
				Monitor.Exit(this.queue);

			}

		}

	}
	
}
