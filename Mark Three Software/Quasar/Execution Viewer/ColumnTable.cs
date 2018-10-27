using System;
using System.Collections;

namespace Shadows.Quasar.Viewers.Execution
{

	public class ColumnAddress
	{

		public int documentVersion;
		public int columnIndex;

		public int DocumentVersion {get {return this.documentVersion;}}
		public int ColumnIndex {get {return this.columnIndex;}}

		public ColumnAddress(int documentVersion, int columnIndex) {this.documentVersion = documentVersion; this.columnIndex = columnIndex;}

	}
		
	public class ColumnTable : System.Collections.Hashtable
	{

		/// <summary>
		/// Adds a lock to the hash table.
		/// </summary>
		/// <param name="readerWriterLock"></param>
		public void Add(int columnTypeCode, ColumnAddress columnAddress) {base.Add(columnTypeCode, columnAddress);}

		/// <summary>
		/// This is the strongly typed indexer for the Event hash table.
		/// </summary>
		public new ColumnAddress this[object columnTypeCode] {get {return (ColumnAddress)base[columnTypeCode];} set {base[columnTypeCode] = value;}}
	
	}
}
