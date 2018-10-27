namespace MarkThree
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Threading;
	using System.Reflection;

	/// <summary>
	/// Used to execute transactions containing methods to be executed or rejected as a unit of work.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	[Serializable]
	public class Batch : IDisposable
	{

		// Private Static Members
		public static Dictionary<int, Batch> threadTable;

		// Public Readonly Members
		public readonly TransactionCollection Transactions;
		public readonly AssemblyCollection Assemblies;

		/// <summary>
		/// Describes an assembly.
		/// </summary>
		[Serializable]
		public class Assembly : IComparable
		{

			// Private Members
			public readonly string DisplayName;
			public readonly TypeCollection Types;

			/// <summary>
			/// Constructs a description of an assembly.
			/// </summary>
			/// <param name="displayName">The display name of the assembly.</param>
			public Assembly(string displayName)
			{

				// Initialize the object
				this.DisplayName = displayName;
				this.Types = new TypeCollection(this);

			}

			/// <summary>
			/// Returns a string that represents the current Assembly.
			/// </summary>
			/// <returns>A string that represents the current Assembly.</returns>
			public override string ToString() { return this.DisplayName; }

			/// <summary>
			/// Compares this instance with the specified System.Object.
			/// </summary>
			/// <param name="value">An Object that evaluates to a String.</param>
			/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
			public int CompareTo(object value) { return this.DisplayName.CompareTo(value); }

		}

		/// <summary>
		/// A unique collection of Assemblies.
		/// </summary>
		[Serializable]
		public class AssemblyCollection : ArrayList
		{

			/// <summary>
			/// Adds an assembly to the collection if the same name hasn't been added already.
			/// </summary>
			/// <param name="name">The display name of the assembly.</param>
			/// <returns>The newly added assembly specification or an existing one matching the display name.</returns>
			public Assembly Add(string displayName)
			{

				// This will order the list of display names and keep them unique.  If the name has already been entered, then
				// the existing Batch.Assembly is returned to the caller.  Otherwise a new entry is created and inserted in
				// alphabetical order in the collection.
				int index = BinarySearch(displayName);
				if (index < 0)
					Insert(index = ~index, new Assembly(displayName));
				return (Assembly)this[index];

			}

		}

		/// <summary>
		/// A description of the types found in an assembly.
		/// </summary>
		[Serializable]
		public class Type : IComparable
		{

			// Public Readonly Members
			public readonly string Name;
			public readonly Assembly Assembly;

			/// <summary>
			/// Create a description of a CLR Type.
			/// </summary>
			/// <param name="name">The name of the type.</param>
			public Type(Assembly assembly, string name)
			{

				// Initialize the object.
				this.Assembly = assembly;
				this.Name = name;

			}

			/// <summary>
			/// Returns a string that represents the current Assembly.
			/// </summary>
			/// <returns>A string that represents the current Assembly.</returns>
			public override string ToString() { return this.Name; }

			/// <summary>
			/// Compares this instance with the specified System.Object.
			/// </summary>
			/// <param name="value">An Object that evaluates to a String.</param>
			/// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.</returns>
			public int CompareTo(object value) { return this.Name.CompareTo(value); }

		}

		/// <summary>
		/// A collection of Types.
		/// </summary>
		[Serializable]
		public class TypeCollection : ArrayList
		{

			// Private Members
			private Assembly assembly;

			/// <summary>
			/// Construct a collection of types.
			/// </summary>
			/// <param name="assembly">The parent assembly of the types.</param>
			public TypeCollection(Batch.Assembly assembly)
			{

				// Initialize the object.
				this.assembly = assembly;

			}

			/// <summary>
			/// Add a member to the list of types found in the parent assembly.
			/// </summary>
			/// <param name="name">The name of the type.</param>
			/// <returns>A specification for a Type that is loaded when the batch is run.</returns>
			public Type Add(string name)
			{

				// This will order the list of type names and keep them unique.  If the name has already been entered, then the
				// existing Batch.Type is returned to the caller.  Otherwise a new entry is created and inserted in alphabetical
				// order in the collection.
				int index = BinarySearch(name);
				if (index < 0)
					Insert(index = ~index, new Type(this.assembly, name));
				return (Type)this[index];

			}

		}

		/// <summary>
		/// A collection of operations which must be completed or rejected as a unit.
		/// </summary>
		[Serializable]
		public class Transaction : IDisposable
		{

			// Private Members
			private TransactionCollection transactionCollection;
			private Transaction previousTransaction;

			// Public Readonly Members
			public readonly int Index;
			public readonly List<Exception> Exceptions;
			public readonly MethodCollection Methods;

			/// <summary>
			/// Creates an object that commits or rejects several batched operations as a unit.
			/// </summary>
			public Transaction(TransactionCollection transactionCollection)
			{

				// Initialize the object
				this.Index = transactionCollection.Count;
				this.Exceptions = new List<Exception>();
				this.Methods = new MethodCollection();
				this.transactionCollection = transactionCollection;

				// This transaction is the current transaction.
				this.previousTransaction = this.transactionCollection.Current;
				this.transactionCollection.current = this;

			}

			#region IDisposable Members

			public void Dispose()
			{

				this.transactionCollection.current = this.previousTransaction;
				
			}

			#endregion

		}

		/// <summary>
		/// A collection of transactions.
		/// </summary>
		[Serializable]
		public class TransactionCollection : List<Transaction>
		{

			// Internal Members
			internal Transaction current;

			/// <summary>
			/// Adds a new, blank transaction to the batch.
			/// </summary>
			/// <returns></returns>
			public Transaction Add()
			{

				// Create a new transaction and add it to the list managed by the batch.
				Transaction batchTransaction = new Transaction(this);
				base.Add(batchTransaction);
				return batchTransaction;

			}

			/// <summary>
			/// Gets the most recent transaction in the batch.
			/// </summary>
			public Transaction Current { get { return this.current; } }

		}

		/// <summary>
		/// A description of a method to be run on the remote host.
		/// </summary>
		[Serializable]
		public class Method
		{

			// Public Members
			public object[] Results;

			// Public Readonly Members
			public readonly int Index;
			public readonly string Name;
			public readonly Type Type;
			public readonly object[] Parameters;
			public readonly List<Exception> Exceptions;

			/// <summary>
			/// Construct a description of a method to be run on a remote host.
			/// </summary>
			/// <param name="type">The type to which this method belongs.</param>
			/// <param name="name">The method name.</param>
			internal Method(int index, Type type, string name, object[] parameters)
			{

				// Initialize the object
				this.Index = index;
				this.Name = name;
				this.Type = type;
				this.Parameters = parameters;
				this.Exceptions = new List<Exception>();

			}

			public object Return { get { return this.Results[0]; } }

		}

		/// <summary>
		/// A collection of the methods in a transaction.
		/// </summary>
		[Serializable]
		public class MethodCollection : List<Method>
		{

			/// <summary>
			/// Adds a method to the collection of methods in a transaction.
			/// </summary>
			/// <param name="type">The type to which this method belongs.</param>
			/// <param name="method">The method name.</param>
			/// <returns>A method description.</returns>
			public Method Add(Type type, string name, params object[] parameters)
			{

				Method method = new Method(base.Count, type, name, parameters);
				base.Add(method);
				return method;

			}

		}

		static Batch()
		{

			Batch.threadTable = new Dictionary<int, Batch>();

		}

		public Batch()
		{

			// Initialize the object
			this.Transactions = new TransactionCollection();
			this.Assemblies = new AssemblyCollection();

			lock (Batch.threadTable)
			{
				if (Batch.threadTable.ContainsKey(Thread.CurrentThread.ManagedThreadId))
					Batch.threadTable[Thread.CurrentThread.ManagedThreadId] = this;
				else
					Batch.threadTable.Add(Thread.CurrentThread.ManagedThreadId, this);
			}

		}

		public static Batch Current
		{

			get
			{

				lock (Batch.threadTable)
				{
					Batch batch = null;
					Batch.threadTable.TryGetValue(Thread.CurrentThread.ManagedThreadId, out batch);
					return batch;
				}

			}

		}

		public void Merge(Result result)
		{

			// This will copy the results from the 'BatchResult' back to the original structure.  The 'ResultBatch' returned 
			// from the execution of the batch only has the return codes and the exceptions.  This will correlate and copy
			// those values back into the original batch so the caller will have the impression that the entire structure made
			// the round trip to the server and back.  The 'ResultBatch' was designed to minimize the amount of traffic and
			// deserializing that had to be done during the trip to the server and back.
			foreach (Result.Transaction resultTransaction in result.Transactions)
			{

				// Correlate the current result transaction with the original transaction.
				Batch.Transaction batchTransaction = this.Transactions[resultTransaction.Index];

				// Copy each of the transaction level exceptions back into the original structure.
				foreach (Exception exception in resultTransaction.Exceptions)
					batchTransaction.Exceptions.Add(exception);

				foreach (Result.Method resultMethod in resultTransaction.Methods)
				{

					// Correlate the original method with the resulting method.
					Batch.Method batchMethod = batchTransaction.Methods[resultMethod.Index];

					// Copy each of the output parameters back into the original method.
					batchMethod.Results = resultMethod.Results;

					// Copy each of the exceptions back into the original method.
					foreach (Exception exception in resultMethod.Exceptions)
						batchMethod.Exceptions.Add(exception);

				}

			}

		}


		#region IDisposable Members

		public void Dispose()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}

}
