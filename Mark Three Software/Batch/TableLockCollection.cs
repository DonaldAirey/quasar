namespace MarkThree
{

	using System;
	using System.Data;

	public class TableLockCollection : System.Collections.CollectionBase
	{
        
		public virtual void Add(TableLock tableLock)
		{
			this.List.Add(tableLock);
		}
	}
    
}
