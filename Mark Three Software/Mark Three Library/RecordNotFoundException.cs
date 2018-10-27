namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Text;

	[Serializable]
	public class RecordNotFoundException : Exception
	{

		public RecordNotFoundException(string format, params object[] args) : base(string.Format(format, args)) { }

		protected RecordNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

	}
}
