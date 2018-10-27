using System;

namespace MarkThree.Quasar
{
	/// <summary>
	/// Summary description for UserPrefereces.
	/// </summary>
	public class Preferences
	{

		private static object timeInForceCode = null;
		private static object blotterId = null;
		private static object brokerId = null;
		private static Pricing pricing = Pricing.Last;

		public static int TimeInForceCode
		{
			get {return (int)Preferences.TimeInForceCode;}
			set {Preferences.TimeInForceCode = value;}
		}

		public static int BlotterId
		{
			get {return (int)Preferences.BlotterId;}
			set {Preferences.blotterId = value;}
		}

		public static int BrokerId
		{
			get {return (int)Preferences.brokerId;}
			set {Preferences.BrokerId = value;}
		}

		public static Pricing Pricing
		{
			get {return Preferences.pricing;}
			set {Preferences.pricing = value;}
		}

		public static bool IsTimeInForceCodeNull() {return Preferences.timeInForceCode == null;}
		public static bool IsBlotterIdNull() {return Preferences.blotterId == null;}

	}
}
