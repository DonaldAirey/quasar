namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Text;

	public class Generate
	{

		/// <summary>
		/// Generates a code expression from a generic value.
		/// </summary>
		/// <returns>A CodeDOM expression representing the value of the object.</returns>
		public static CodeExpression PrimativeExpression(object value)
		{

			// If the column provides a fixed value, then translate the default text into the proper datatype for the column.
			switch (value.GetType().ToString())
			{
			case "System.Boolean":
			case "System.Int16":
			case "System.Int32":
			case "System.Int64":
			case "System.Decimal":
			case "System.Double":
			case "System.String":
				return new CodePrimitiveExpression(value);
			case "System.DBNull":
				return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBNull)), "Value");
			}

			throw new Exception(string.Format("There is no CodeDOM expression for {0}", value));

		}

		public static string CamelCase(string value)
		{

			return value[0].ToString().ToLower() + value.Remove(0, 1);

		}

	}

}
