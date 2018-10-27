/*************************************************************************************************************************
*
*	File:			ThreadQueue.cs
*	Description:	Provides a stream of threads that are guaranteed to execute sequentially.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace MarkThree.Quasar
{

	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>
	/// Delegate used to launch a thread with initialization argument.
	/// </summary>
	public delegate void ThreadHandler(params object[] argument);

	/// <summary>
	/// Arguments used to initiate a thread.
	/// </summary>
	public class ThreadArgument
	{

		private string name;
		private object[] argument;
		private ThreadHandler threadDelegate;

		/// <summary>The name of the thread</summary>
		public string Name {get {return this.name;} set {this.name = value;}}

		/// <summary>The initial parameters for a thread.</summary>
		public object[] Argument {get {return this.argument;}}

		/// <summary>The delegate used to start a thread.</summary>
		public ThreadHandler ThreadHandler {get {return this.threadDelegate;}}

		/// <summary>
		/// Creates starting argument for a thread.
		/// </summary>
		/// <param name="threadDelegate">A thread with initialization parameters.</param>
		/// <param name="argument">Initialization Arguments</param>
		public ThreadArgument(ThreadHandler threadDelegate)
		{

			// Initialize the object
			this.name = string.Empty;
			this.argument = null;
			this.threadDelegate = threadDelegate;

		}

		/// <summary>
		/// Creates starting argument for a thread.
		/// </summary>
		/// <param name="threadDelegate">A thread with initialization parameters.</param>
		/// <param name="argument">Initialization Arguments</param>
		public ThreadArgument(ThreadHandler threadDelegate, params object[] argument)
		{

			// Initialize the object
			this.name = string.Empty;
			this.argument = argument;
			this.threadDelegate = threadDelegate;

		}

		/// <summary>
		/// Creates starting argument for a thread.
		/// </summary>
		/// <param name="threadDelegate">A thread with initialization parameters.</param>
		/// <param name="argument">Initialization Arguments</param>
		public ThreadArgument(ThreadHandler threadDelegate, string name, params object[] argument)
		{

			// Initialize the object
			this.name = name;
			this.argument = argument;
			this.threadDelegate = threadDelegate;

		}

		/// <summary>
		/// Starts a thread with argument.
		/// </summary>
		public void StartThread()
		{

			// This will helpIdentify the thread to the debugger.
			Thread.CurrentThread.Name = this.name;

			// Call the thread body with the thread's initialization parameters.
			this.threadDelegate(this.argument);

		}

	}

	public class WorkerThread
	{

		public WorkerThread(ThreadHandler threadHandler, params object[] arguments)
		{
	
			ThreadArgument threadArgument = new ThreadArgument(threadHandler, arguments);
			Thread thread = new Thread(new ThreadStart(threadArgument.StartThread));
			thread.IsBackground = true;
			thread.Start();

		}

		public WorkerThread(ThreadHandler threadHandler, string name, params object[] arguments)
		{
	
			ThreadArgument threadArgument = new ThreadArgument(threadHandler, name, arguments);
			Thread thread = new Thread(new ThreadStart(threadArgument.StartThread));
			thread.IsBackground = true;
			thread.Start();

		}

	}

	/// <summary>
	/// Used to manage the sequential execution of several threads.
	/// </summary>
	public class ThreadQueue : Queue
	{

		private class ThreadQueueArgument : ThreadArgument
		{

			private ThreadQueue threadQueue;

			/// <summary>
			/// Creates starting argument for a thread.
			/// </summary>
			/// <param name="threadDelegate">A thread with initialization parameters.</param>
			/// <param name="argument">Initialization Arguments</param>
			public ThreadQueueArgument(ThreadQueue threadQueue, ThreadHandler threadDelegate) : base(threadDelegate)
			{

				// Initialize the object
				this.threadQueue = threadQueue;

			}

			/// <summary>
			/// Creates starting argument for a thread.
			/// </summary>
			/// <param name="threadDelegate">A thread with initialization parameters.</param>
			/// <param name="argument">Initialization Arguments</param>
			public ThreadQueueArgument(ThreadQueue threadQueue, ThreadHandler threadDelegate, string name,
				params object[] argument) : base(threadDelegate, name, argument)
			{

				// Initialize the object
				this.threadQueue = threadQueue;

			}

			/// <summary>
			/// Creates starting argument for a thread.
			/// </summary>
			/// <param name="threadDelegate">A thread with initialization parameters.</param>
			/// <param name="argument">Initialization Arguments</param>
			public ThreadQueueArgument(ThreadQueue threadQueue, ThreadHandler threadDelegate, params object[] argument) :
				base(threadDelegate, argument)
			{

				// Initialize the object
				this.threadQueue = threadQueue;

			}

			/// <summary>
			/// Starts a thread with argument.
			/// </summary>
			public new void StartThread()
			{

				// This will insure that the thread is removed from the queue when we're done.
				try
				{

					// This will helpIdentify the thread to the debugger.
					Thread.CurrentThread.Name = this.Name;

					// Set the priority based on the default for the thread.
					Thread.CurrentThread.Priority = this.threadQueue.DefaultPriority;

					// Joining with the previous thread will insure that the threads are executed in the order the were
					// initiated.
					this.threadQueue.Join();

					// Call the thread body with the thread's initialization parameters.
					this.ThreadHandler(this.Argument);

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
				finally
				{

					// When the thread is finished, remove it from the queue.
					this.threadQueue.Dequeue();

				}

			}

		}

		private ThreadPriority defaultPriority;
		private ManualResetEvent manualResetEvent = new ManualResetEvent(true);

		/// <summary>
		/// The default priority used to create threads in this queue.
		/// </summary>
		public ThreadPriority DefaultPriority {get {return this.defaultPriority;} set {this.defaultPriority = value;}}

		/// <summary>
		/// Waits for every thread in the queue to complete.
		/// </summary>
		public void Wait() {this.manualResetEvent.WaitOne();}

		/// <summary>
		/// Places a thread with no initialization parameters in the queue
		/// </summary>
		/// <param name="threadStart">A thread handler with no initialization parameters.</param>
		/// <param name="name">The name of the thread.</param>
		public void Enqueue(ThreadHandler threadDelegate)
		{

			// Use the name of the thread handler to identify the thread.
			string threadName = String.Format("{0}.{1}", threadDelegate.Target.GetType().ToString(), threadDelegate.Method.Name);

			// Create the argument used to start the thread.  This includes a start up routine that joins the new thread
			// to the chain of threads.  Being placed in a queue and joined to the previous thread guarantees that the
			// threads execute in the order they were created.
			QueueThread(new ThreadQueueArgument(this, threadDelegate, threadName, null));

		}

		public void Enqueue(ThreadHandler threadDelegate, params object[] argument)
		{

			// Use the name of the thread handler to identify the thread.
			string threadName = String.Format("{0}.{1}", threadDelegate.Method.DeclaringType.Name, threadDelegate.Method.Name);

			// Create the argument used to start the thread.  This includes a start up routine that joins the new thread
			// to the chain of threads.  Being placed in a queue and joined to the previous thread guarantees that the
			// threads execute in the order they were created.
			QueueThread(new ThreadQueueArgument(this, threadDelegate, threadName, argument));

		}

		private void QueueThread(ThreadQueueArgument threadQueueArgument)
		{

			// Create the thread and use the standard start up method in the ThreadArgument to launch the thread.
			Thread thread = new Thread(new ThreadStart(threadQueueArgument.StartThread));
			thread.IsBackground = true;

			// Place the new thread in the queue is the only part of the initialization procedure that needs to be
			// protected from other threads.  The 'lastThread' member is where we join new threads to insure sequential
			// execution.
			lock (this)
			{
				this.Enqueue(thread);
			}

			// This signal can be used to wait until all threads are complete.
			manualResetEvent.Reset();

			// The thread is now initialized and can be started.  The body of it will not execute until the other threads
			// that are before it have completed.
			thread.Start();

		}

		/// <summary>
		/// Removes a thread from the queue
		/// </summary>
		/// <returns>The thread that was removed from the queue</returns>
		private new Thread Dequeue()
		{

			// Make sure this method is safe from other threads.
			lock (this)
			{

				// Remove the first thread on the queue.
				Thread thread = (Thread)base.Dequeue();

				// Consistency Check: make sure that the thread removed is the current thread.  Anything else and we've 
				// got a corrupted thread.
				if (thread != Thread.CurrentThread)
					throw new Exception("Corrupted ThreadQueue");

				// Release any waiting threads when the queue is empty.
				if (this.Count == 0) manualResetEvent.Set();

				// This should be the current thread.  It isn't really used, but returning a value makes this method
				// appear more like a queue.
				return thread;

			}

		}

		/// <summary>
		/// Join this thread to the previous thread in the queue.
		/// </summary>
		private void Join()
		{

			bool found = false;
			Thread lastThread = null;
			
			// The followingCode needs to be protected from other threads so the queue isn't corrupted while we're 
			// transversing it.
			lock (this)
			{

				// This will find the previous thread in the queue, if it exists.  The mainIdea is that we want to want 
				// to execute all the threads in this queue in sequence.  This is the part where the sequence is organized
				// and the threads are chained together.  Note that this critical section only finds the previous thread.
				// We can't 'join' to the previous thread in a critical section because the execution is suspended until
				// the thread is called.  If the queue is locked when this thread is suspended, then we will deadlock the 
				// next time one of the other threads attempts to modify the queue.
				foreach (Thread thread in this)
				{
					if (thread == Thread.CurrentThread)
					{
						found = true;
						break;
					}
					lastThread = thread;
				}

			}

			// This must be done outside the 'try' logic because the thread will suspend after calling 'join'.  If we
			// suspend inside the 'lock' logic, the object will remain locked forever.
			if (lastThread != null && found)
				lastThread.Join();

		}

		/// <summary>
		/// Abort all threads in the queue
		/// </summary>
		public void Abort()
		{

			// Run through the queue and abort every thread found.  This is usually used when we want to abort the
			// application immediately.
			lock (this)
				foreach (Thread thread in this) thread.Abort();

			// Release any object that is waiting for the threads to complete.
			manualResetEvent.Set();

		}

	}

}
