namespace MarkThree.Quasar
{

	using MarkThree.Quasar;
	using System;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>
	/// Summary description for Trading.
	/// </summary>
	public class Trading
	{

		/// <summary>
		/// Calculates the next valid trade date from the date given.
		/// </summary>
		/// <param name="CountryId">The country is used to find the holiday and weekend schedule.</param>
		/// <param name="securityTypeCode">Identifies what type of security we're trading.</param>
		/// <param name="startDate">The proposed trade date.</param>
		/// <returns>The valid trade date closest to the 'startDate'.</returns>
		public static DateTime TradeDate(int CountryId, int securityTypeCode, DateTime startDate)
		{

			// Stip off the time portion of the start date.  The proposed trade date is the starting point for the testing
			// below.  If we find out that the day is a weekend or a holiday, we'll move up to the next day and test it
			// again until we're sure the date is good for trading.
			DateTime tradeDate = startDate.Date;

			// This loop will continue until the proposed date passes all the tests for a valid trade date.  If we change 
			// the date, either for a weekend or a holiday, we need to check the date again to make sure that the new date
			// doesn't end up on a weekend or a holiday.
			while (true)
			{

				// The loop will continue until the trade date has passed all the tests for a valid trading day.
				startDate = tradeDate;

				// Adjust the trade date for weekends.  This can be parameterized by the country when we've sold out in
				// every country that uses Saturday and Sunday as a weekend.
				if (tradeDate.DayOfWeek == DayOfWeek.Saturday)
					tradeDate = tradeDate.AddDays(2);
				if (tradeDate.DayOfWeek == DayOfWeek.Sunday)
					tradeDate = tradeDate.AddDays(1);

				// Check the proposed trade date against the holiday schedule.  If we get a match for a trading
				// holiday, then we'll move up to the next date.  Note that we cycle back and check the dates again
				// after this in case the new date ends up on a weekend.  Only when we've passed the proposed trade
				// date successfully through all the tests will we accept the date.
				foreach (DataModel.HolidayRow holidayRow in DataModel.Holiday)
					if (holidayRow.CountryId == CountryId && holidayRow.SecurityTypeCode == securityTypeCode &&
						holidayRow.Date == tradeDate && (holidayRow.HolidayTypeCode == HolidayType.Both ||
						holidayRow.HolidayTypeCode == HolidayType.Trading))
						tradeDate = tradeDate.AddDays(1);

				// Keep on looping until the proposed trade date passes all the test for holiday and weekends without
				// being moved.
				if (startDate == tradeDate)
					break;
			
			}
			
			// This date is guaranteed to be good for trading.
			return tradeDate;

		}
	
		/// <summary>
		/// Calculates the next valid settlement date from the date given.
		/// </summary>
		/// <param name="CountryId">The country is used to find the holiday and weekend schedule.</param>
		/// <param name="securityTypeCode">Identifies what type of security we're trading.</param>
		/// <param name="startDate">The proposed trade date.</param>
		/// <returns>The valid trade date closest to the 'startDate'.</returns>
		public static DateTime SettlementDate(int CountryId, int securityTypeCode, DateTime startDate)
		{

			// HACK - the settlement days are fixed for now.  This should eventually be driven off the country and asset
			// class.  The 'dayCount' is used to count up the number of valid business days.
			int settlementDays = 3;
			int dayCount = 0;
			
			// Strip off the time portion of the start date.  The proposed trade date is the starting point for the
			// testing below.  If we find out that the day is a weekend or a holiday, we'll move up to the next day and
			// test it again until we're sure the date is good for trading.
			DateTime settlementDate = startDate.Date;

			// Keep on looping until we've counted the given number of settlement days.
			while (true)
			{

				// If the 'startDate' is the same as the 'settlementDate' at the end of this loop, then we've successfully
				// found a good settlement date.  If not, we continue moving the date up until a given date passes all our
				// tests without modification.
				startDate = settlementDate;

				// Adjust the settlement date for weekends.  This can be parameterized by the country when we've sold out 
				// in every country that uses Saturday and Sunday as a weekend.
				if (settlementDate.DayOfWeek == DayOfWeek.Saturday)
					settlementDate = settlementDate.AddDays(2);
				if (settlementDate.DayOfWeek == DayOfWeek.Sunday)
					settlementDate = settlementDate.AddDays(1);

				// Check the proposed settlement date against the holiday schedule.  If we get a match for a settlement 
				// holiday, then we'll move up to the next date.
				foreach (DataModel.HolidayRow holidayRow in DataModel.Holiday)
					if (holidayRow.CountryId == CountryId && holidayRow.SecurityTypeCode == securityTypeCode &&
						holidayRow.Date == settlementDate && (holidayRow.HolidayTypeCode == HolidayType.Both ||
						holidayRow.HolidayTypeCode == HolidayType.Settlement))
						settlementDate = settlementDate.AddDays(1);

				// If we get to the end of a loop and the 'startDate' is the same as the 'settlementDate', then we've
				// found a good settlement date.
				if (startDate == settlementDate)
				{

					// When we've found the given number of settlement days, we're through and 'settlementDate' contains
					// the proper date.
					if (dayCount == settlementDays)
						break;

					// Increase the number of good settlement days found.
					dayCount++;

					// Move up to the next day and test it in the loop above.
					settlementDate = settlementDate.AddDays(1);

				}

			}
			
			// This date is guaranteed to be good for settlement.
			return settlementDate;

		}

	}

}
