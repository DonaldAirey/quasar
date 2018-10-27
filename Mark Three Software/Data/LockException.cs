namespace MarkThree
{

	using System;

	public class LockException : Exception
	{

		public LockException(string format, params object[] parameters) : base(string.Format(format, parameters)) { }

	}

}
