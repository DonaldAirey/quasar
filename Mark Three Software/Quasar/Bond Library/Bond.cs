/*************************************************************************************************************************
*
*	File:			Debt.cs
*	Description:	Library of Debt Calculations
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.TechHackers;
using System;
using System.Runtime.InteropServices;

namespace Shadows.Quasar.Library.Debt
{

	public class Debt
	{

		public static decimal PriceFromYield
			(
			decimal coupon,
			DateTime maturityDate,
			DateTime settlementDate,
			decimal price
			)
		{

			int tHIMaturityDate = TechHackersAnalyst.THIdate(maturityDate.Year, maturityDate.Month, maturityDate.Day, null);
			int tHISettlementDate = TechHackersAnalyst.THIdate(settlementDate.Year, settlementDate.Month, settlementDate.Day, null);

			double []couponArray = new double[1];
			couponArray[0] = Convert.ToDouble(coupon);

			int []maturityDateArray = new int[1];
			maturityDateArray[0] = tHIMaturityDate;

			return Convert.ToDecimal(TechHackersAnalyst.THIbyld(couponArray, couponArray.Length, maturityDateArray,
				maturityDateArray.Length, tHISettlementDate, Convert.ToDouble(price), null, null, null, null, 0,
				null, null, null, null, null, null, null, null, 0, null, null, 0,
				null, 0, null, null, null, null));

		}

		public static decimal YieldFromPrice
			(
			decimal coupon,
			DateTime maturityDate,
			DateTime settlementDate,
			decimal yield
			)
		{

			double[] couponArray = new double[1];
			couponArray[0] = Convert.ToDouble(coupon);

			int tHIMaturityDate = TechHackersAnalyst.THIdate(maturityDate.Year, maturityDate.Month, maturityDate.Day, null);
			int tHISettlementDate = TechHackersAnalyst.THIdate(settlementDate.Year, settlementDate.Month, settlementDate.Day, null);
						
			int[] maturityDateArray = new int[1];
			maturityDateArray[0] = tHIMaturityDate;
					
			return Convert.ToDecimal(TechHackersAnalyst.THIbpr(couponArray, couponArray.Length,
				maturityDateArray, maturityDateArray.Length, tHISettlementDate, Convert.ToDouble(yield), null, null, null, null,
				0, null, null, null, null, null, null, null, null, 0, null, null, 0, null, 0, null, null,
				null, null));

		}

		public decimal DebtAccruedInterest
			(
			decimal coupon,
			DateTime maturityDate,
			DateTime settlementDate
			)
		{

			int tHIMaturityDate = TechHackersAnalyst.THIdate(maturityDate.Year, maturityDate.Month, maturityDate.Day, null);
			int tHISettlementDate = TechHackersAnalyst.THIdate(settlementDate.Year, settlementDate.Month, settlementDate.Day, null);

			double []couponArray = new double[1];
			couponArray[0] = Convert.ToDouble(coupon);

			int []maturityDateArray = new int[1];
			maturityDateArray[0] = tHIMaturityDate;

			return Convert.ToDecimal(TechHackersAnalyst.THIbai1(couponArray, couponArray.Length, maturityDateArray,
				maturityDateArray.Length, tHISettlementDate, null, null, null, null, 0, null,
				null, null, 0, null, 0, null, null, null, null, null, 0, null));

		}

	}
	
}
