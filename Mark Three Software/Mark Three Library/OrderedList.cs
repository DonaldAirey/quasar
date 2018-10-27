namespace MarkThree
{

	using System;
	using System.Collections;

	public class OrderedList
	{

		private ArrayList orderedList;
		private ArrayList arrayList;

		public object this[int index] {get {return this.arrayList[index];}}

		public int Count {get {return this.arrayList.Count;}}

		public OrderedList()
		{

			// Initialize the object
			this.arrayList = new ArrayList();
			this.orderedList = new ArrayList();

		}

		public IComparable Add(IComparable sortedObject)
		{
			this.arrayList.Add(sortedObject);
			int index = this.orderedList.BinarySearch(sortedObject);
			if (index < 0)
				this.orderedList.Insert(~index, sortedObject);
			return sortedObject;
		}

		public IComparable Find(object key)
		{
			int index = this.orderedList.BinarySearch(key);
			if (index >= 0)
				return (IComparable)this.orderedList[index];
			return null;
		}

		public void Clear()
		{
			this.arrayList.Clear();
			this.orderedList.Clear();
		}

		public void CopyTo(System.Array array, int arrayIndex)
		{
			this.arrayList.CopyTo(array, arrayIndex);
		}

		public void Remove(IComparable sortedObject)
		{
			this.arrayList.Remove(sortedObject);
			int index = this.orderedList.BinarySearch(sortedObject);
			if (index >= 0)
				this.orderedList.RemoveAt(index);
		}

		public void Move(int sourceIndex, int destinationIndex)
		{
			object sourceObject = this.arrayList[sourceIndex];
			this.arrayList.RemoveAt(sourceIndex);
			this.arrayList.Insert(destinationIndex, sourceObject);
		}

		public void Move(object sourceColumn, object destinationColumn)
		{
			this.arrayList.Remove(sourceColumn);
			int index = destinationColumn == null ? this.arrayList.Count : this.arrayList.IndexOf(destinationColumn);
			this.arrayList.Insert(index, sourceColumn);
		}

		public int IndexOf(object column)
		{
			return this.arrayList.IndexOf(column);
		}

		public object Clone()
		{

			OrderedList orderedList = new OrderedList();
			foreach (IComparable iComparable in this.arrayList)
				orderedList.Add(iComparable);
			return orderedList;

		}

		public IEnumerator GetEnumerator() {return this.arrayList.GetEnumerator();}

	}

}
