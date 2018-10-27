namespace MarkThree
{

	using System;
	using System.Drawing;
	using System.Reflection;
	using System.Text.RegularExpressions;

	internal class ConversionString : IConvertible
	{

		private string internalString;

		public ConversionString(string internalString)
		{

			this.internalString = internalString;

		}

		public object ToType(Type conversionType, IFormatProvider provider)
		{

			try
			{

				// Enums can be handled efficiently and effectively by using the 'Parse' method.
				if (conversionType.IsEnum)
					return Enum.Parse(conversionType, this.internalString, false);

				switch (conversionType.ToString())
				{

					case "System.Drawing.Point": return ConvertPoint();
					case "System.Drawing.Size": return ConvertSize();

				}

			}
			catch
			{

				// Make sure that we catch any conversion errors here.

			}

			// At this point, there was no valid conversion to be made into the destination type.
			throw new InvalidCastException(string.Format("Can't convert {0} to {1}", this.internalString, conversionType.ToString()));

		}

		private object ConvertPoint()
		{

			Point point = new Point();

			// Parse the X and Y values out of the text associated with the key.  This is the same text that is generated
			// with the ToString() method in 'Point'.
			Regex point_regex = new Regex("{X=(?<X>[^,]+), *Y=(?<Y>.+)}");
			Match match = point_regex.Match(this.internalString);
			if (match.Success)
			{
				point.X = Convert.ToInt32(match.Groups["X"].Value);
				point.Y = Convert.ToInt32(match.Groups["Y"].Value);
			}

			// Return the Point.
			return point;

		}

		private object ConvertSize()
		{

			Size size = new Size();

			// Parse the X and Y values out of the text associated with the key.  This is the same text that is generated
			// with the ToString() method in 'Point'.
			Regex size_regex = new Regex("{Width=(?<Width>[^,]+), *Height=(?<Height>.+)}");
			Match match = size_regex.Match(this.internalString);
			if (match.Success)
			{
				size.Width = Convert.ToInt32(match.Groups["Width"].Value);
				size.Height = Convert.ToInt32(match.Groups["Height"].Value);
			}

			// Return the Size.
			return size;

		}

		#region IConvertible Members

		public ulong ToUInt64(IFormatProvider provider)
		{

			// Convert to System.UInt64.
			return (System.UInt64)Convert.ChangeType(this.internalString, typeof(System.UInt64), provider);

		}

		public sbyte ToSByte(IFormatProvider provider)
		{

			// Convert to System.SByte.
			return (System.SByte)Convert.ChangeType(this.internalString, typeof(System.SByte), provider);

		}

		public double ToDouble(IFormatProvider provider)
		{

			// Convert to System.Double.
			return (System.Double)Convert.ChangeType(this.internalString, typeof(System.Double), provider);

		}

		public DateTime ToDateTime(IFormatProvider provider)
		{

			// Convert to System.DateTime.
			return (System.DateTime)Convert.ChangeType(this.internalString, typeof(System.DateTime), provider);

		}

		public float ToSingle(IFormatProvider provider)
		{

			// Convert to System.Single.
			return (System.Single)Convert.ChangeType(this.internalString, typeof(System.Single), provider);

		}

		public bool ToBoolean(IFormatProvider provider)
		{

			// Convert to System.Boolean.
			return (System.Boolean)Convert.ChangeType(this.internalString, typeof(System.Boolean), provider);

		}

		public int ToInt32(IFormatProvider provider)
		{

			// Convert to System.Int32.
			return (System.Int32)Convert.ChangeType(this.internalString, typeof(System.Int32), provider);

		}

		public ushort ToUInt16(IFormatProvider provider)
		{

			// Convert to System.UInt16.
			return (System.UInt16)Convert.ChangeType(this.internalString, typeof(System.UInt16), provider);

		}

		public short ToInt16(IFormatProvider provider)
		{

			// Convert to short System.UInt16.
			return (System.Int16)Convert.ChangeType(this.internalString, typeof(System.Int16), provider);

		}

		public string ToString(IFormatProvider provider)
		{

			// No Convertion Needed.
			return this.internalString;

		}

		public byte ToByte(IFormatProvider provider)
		{

			// Convert to short System.Byte.
			return (System.Byte)Convert.ChangeType(this.internalString, typeof(System.Byte), provider);

		}

		public char ToChar(IFormatProvider provider)
		{

			// Convert to short System.Char.
			return (System.Char)Convert.ChangeType(this.internalString, typeof(System.Char), provider);

		}

		public long ToInt64(IFormatProvider provider)
		{

			// Convert to short System.Int64.
			return (System.Int64)Convert.ChangeType(this.internalString, typeof(System.Int64), provider);

		}

		public System.TypeCode GetTypeCode()
		{

			// Return the type code of the source object in the conversion.
			return this.internalString.GetTypeCode();

		}

		public decimal ToDecimal(IFormatProvider provider)
		{

			// Convert to short System.Decimal.
			return (System.Decimal)Convert.ChangeType(this.internalString, typeof(System.Decimal), provider);

		}

		public uint ToUInt32(IFormatProvider provider)
		{

			// Convert to short System.UInt32.
			return (System.UInt32)Convert.ChangeType(this.internalString, typeof(System.UInt32), provider);

		}

		#endregion

	}

}
