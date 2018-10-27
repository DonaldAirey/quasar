namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Data;

	public class UpdateList
	{

		private System.Boolean isValid;
		private ArrayList arrayList;

		public UpdateList()
		{

			// Initialize the object
			this.isValid = true;
			this.arrayList = new ArrayList();

		}

		public bool IsValid {get {return this.isValid;} set {this.isValid = value;}}

		public int Count {get {return this.arrayList.Count;}}

		public void Clear() {this.arrayList.Clear(); this.isValid = true;}

		public Update Add(Update update)
		{

			int index = this.arrayList.BinarySearch(update);
			if (index < 0)
			{
				this.arrayList.Insert(~index, update);
				return update;
			}

			Update oldUpdate = (Update)this.arrayList[index];

			if (oldUpdate.DataAction == DataAction.Update)
			{
				if (update.DataAction == DataAction.Update)
					oldUpdate.Fields |= update.Fields;
				else
				{
					oldUpdate.DataAction = update.DataAction;
					oldUpdate.Key = update.Key;
				}

			}

			this.arrayList[index] = oldUpdate;

			return oldUpdate;

		}

		public Update Add(DataAction dataAction, Row row)
		{

			return Add(new Update(dataAction, row));

		}

		public Update Add(DataAction dataAction, Row row, Enum field)
		{

			FieldArray fieldArray = FieldArray.Clear;
			fieldArray[field] = true;

			return Add(new Update(dataAction, row, fieldArray));

		}

		public Update Add(DataAction dataAction, Row row, FieldArray fields)
		{

			return Add(new Update(dataAction, row, fields));

		}

		public IEnumerator GetEnumerator() {return this.arrayList.GetEnumerator();}

	}

}
