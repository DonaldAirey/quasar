namespace MarkThree
{

	using System;

	/// <summary>
	/// Resource manager for durable (non-volatile) data stores.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public abstract class DurableResourceManager : ResourceManager
	{

		/// <summary>
		/// Creates a resource manager for durable (non-volatile) data stores.
		/// </summary>
		/// <param name="name">The name of the Resource Manager.</param>
		public DurableResourceManager(string name) : base(name) { }

	}

}
