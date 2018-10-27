namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Xml.Schema;
	using System.Data;

	public class TypeConverter
	{

		public static CodeExpression Convert(Type type)
		{

			string sqlDataType = string.Empty;

			switch (type.ToString())
			{
				case "System.Boolean": sqlDataType = "Bit"; break;
				case "System.Byte": sqlDataType = "TinyInt"; break;
				case "System.Byte[]": sqlDataType = "Image"; break;
				case "System.DateTime": sqlDataType = "DateTime"; break;
				case "System.Decimal": sqlDataType = "Decimal"; break;
				case "System.Double": sqlDataType = "Float"; break;
				case "System.Float": sqlDataType = "Real"; break;
				case "System.Guid": sqlDataType = "UniqueIdentifier"; break;
				case "System.Int16": sqlDataType = "SmallInt"; break;
				case "System.Int32": sqlDataType = "Int"; break;
				case "System.Int64": sqlDataType = "BigInt"; break;
				case "System.Object": sqlDataType = "Variant"; break;
				case "System.String": sqlDataType = "NVarChar"; break;
			}

			return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("SqlDbType"), sqlDataType);

		}

	}

}
