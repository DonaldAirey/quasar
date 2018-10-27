namespace MarkThree.Forms
{

	using System;

	public struct FieldArray
	{

		// Private Members
		private int field0;
		private int field1;
		private int field2;
		private int field3;
		private int field4;
		private int field5;
		private int field6;
		private int field7;

		public FieldArray(params Enum[] enumerations)
		{

			this.field0 = this.field1 = this.field2 = this.field3 = this.field4 = this.field5 = this.field6 = this.field7 = 0;

			foreach (Enum enumeration in enumerations)
				this[enumeration] = true;

		}

		public static FieldArray Clear {get {return new FieldArray();}}

		public static FieldArray Set
		{
			get 
			{

				FieldArray fieldArray = new FieldArray();

				fieldArray.field0 = fieldArray.field1 = fieldArray.field2 = fieldArray.field3 = fieldArray.field4 =
					fieldArray.field5 = fieldArray.field6 = fieldArray.field7 = ~0;

				return fieldArray;

			}

		}

		public bool this[Enum field]
		{

			get
			{

				int index = Convert.ToInt32(field);
				switch (index / 32)
				{
				case 0: return ((field0 & (1 << (index % 32))) != 0);
				case 1: return ((field1 & (1 << (index % 32))) != 0);
				case 2: return ((field2 & (1 << (index % 32))) != 0);
				case 3: return ((field3 & (1 << (index % 32))) != 0);
				case 4: return ((field4 & (1 << (index % 32))) != 0);
				case 5: return ((field5 & (1 << (index % 32))) != 0);
				case 6: return ((field6 & (1 << (index % 32))) != 0);
				case 7: return ((field7 & (1 << (index % 32))) != 0);

				}

				throw new IndexOutOfRangeException();

			}
			
			set
			{

				int index = Convert.ToInt32(field);
				switch (index / 32)
				{
				case 0: field0 = value ? field0 | 1 << (index % 32) : field0 & ~(1 << (index % 32)); return;
				case 1: field1 = value ? field1 | 1 << (index % 32) : field1 & ~(1 << (index % 32)); return;
				case 2: field2 = value ? field2 | 1 << (index % 32) : field2 & ~(1 << (index % 32)); return;
				case 3: field3 = value ? field3 | 1 << (index % 32) : field3 & ~(1 << (index % 32)); return;
				case 4: field4 = value ? field4 | 1 << (index % 32) : field4 & ~(1 << (index % 32)); return;
				case 5: field5 = value ? field5 | 1 << (index % 32) : field5 & ~(1 << (index % 32)); return;
				case 6: field6 = value ? field6 | 1 << (index % 32) : field6 & ~(1 << (index % 32)); return;
				case 7: field7 = value ? field7 | 1 << (index % 32) : field7 & ~(1 << (index % 32)); return;
				}
				
				throw new IndexOutOfRangeException();

			}
		
		}

		public static implicit operator bool(FieldArray fieldArray)
		{

			return (fieldArray.field0 | fieldArray.field1 | fieldArray.field2 | fieldArray.field3 | fieldArray.field4 |
				fieldArray.field5 | fieldArray.field6 | fieldArray.field7) != 0;

		}

		public static FieldArray operator |(FieldArray fieldArray0, FieldArray fieldArray1)
		{

			FieldArray fieldArray = new FieldArray();

			fieldArray.field0 = fieldArray0.field0 | fieldArray1.field0;
			fieldArray.field1 = fieldArray0.field1 | fieldArray1.field1;
			fieldArray.field2 = fieldArray0.field2 | fieldArray1.field2;
			fieldArray.field3 = fieldArray0.field3 | fieldArray1.field3;
			fieldArray.field4 = fieldArray0.field4 | fieldArray1.field4;
			fieldArray.field5 = fieldArray0.field5 | fieldArray1.field5;
			fieldArray.field6 = fieldArray0.field6 | fieldArray1.field6;
			fieldArray.field7 = fieldArray0.field7 | fieldArray1.field7;

			return fieldArray;

		}

	}

}
