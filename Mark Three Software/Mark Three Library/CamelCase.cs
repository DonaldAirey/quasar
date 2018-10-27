namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public class CamelCase
	{

		public static string Convert(string text)
		{

			// Convert the variable to its camel case equivalent.
			return text[0].ToString().ToLower() + text.Remove(0, 1);

		}

	}

}
