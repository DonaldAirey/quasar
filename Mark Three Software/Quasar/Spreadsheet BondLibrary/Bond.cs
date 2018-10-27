/*************************************************************************************************************************
*
*	File:			Debt.cs
*	Description:	External Spreadsheet Functions for Debt Calculations
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Common;
using Shadows.Quasar.TechHackers;
using System;
using System.Runtime.InteropServices;

namespace Shadows.Quasar.Viewers.DebtLibrary
{

	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class Debt
	{

		public double EquityCommission(double quantity)
		{

			// Flat rate for every 1,000 shares.
			double commission = 0.0;
			
			if (quantity > 0.0)
			{
				
				commission = 29.95;

				// Rate per share after 1,000.
				if (quantity > 1000.0)
					commission += (quantity - 1000.0) * 0.02;

				// Apply rounding rules.
				commission = Math.Round(commission, 2);

			}

			// Return the commission.
			return commission;

		}
		
		public double DebtCommission(double quantity)
		{

			// Flat rate for every 1,000 shares.
			double commission = 0.0;
			
			if (quantity > 0.0)
			{
				
				commission = 36.00;

				// Rate per share after 1,000.
				if (quantity > 25000.0)
				{
					commission += ((quantity - 25000.0) / 1000.0) * 3.0;
					quantity -= 25000.0;
				}

				commission += (quantity / 1000.0) * 4.0;

				// Apply rounding rules.
				commission = Math.Round(commission, 2);

			}

			// Return the commission.
			return commission;

		}

		public double SECFee(double marketValue, double secFactor)
		{

			return Math.Round(marketValue * secFactor, 2);

		}
		
		public double YieldFromPrice(double coupon, DateTime maturityDate, DateTime settlementDate, double price)
		{

			int tHIMaturityDate = TechHackersAnalyst.THIdate(maturityDate.Year, maturityDate.Month, maturityDate.Day, null);
			int tHISettlementDate = TechHackersAnalyst.THIdate(settlementDate.Year, settlementDate.Month, settlementDate.Day, null);

			double []couponArray = new double[1];
			couponArray[0] = coupon;

			int []maturityDateArray = new int[1];
			maturityDateArray[0] = tHIMaturityDate;

			return TechHackersAnalyst.THIbyld(couponArray, couponArray.Length, maturityDateArray,
				maturityDateArray.Length, tHISettlementDate, price, null, null, null, null, 0,
				null, null, null, null, null, null, null, null, 0, null, null, 0,
				null, 0, null, null, null, null);

		}

		public double PriceFromYield(double coupon, DateTime maturityDate, DateTime settlementDate, double yield)
		{

			double[] couponArray = new double[1];
			couponArray[0] = coupon;

			int tHIMaturityDate = TechHackersAnalyst.THIdate(maturityDate.Year, maturityDate.Month, maturityDate.Day, null);
			int tHISettlementDate = TechHackersAnalyst.THIdate(settlementDate.Year, settlementDate.Month, settlementDate.Day, null);
						
			int[] maturityDateArray = new int[1];
			maturityDateArray[0] = tHIMaturityDate;
					
			return TechHackersAnalyst.THIbpr(couponArray, couponArray.Length,
				maturityDateArray, maturityDateArray.Length, tHISettlementDate, yield, null, null, null, null,
				0, null, null, null, null, null, null, null, null, 0, null, null, 0, null, 0, null, null,
				null, null);

		}

		public double DebtAccruedInterest(double coupon, DateTime maturityDate, DateTime settlementDate)
		{

			int tHIMaturityDate = TechHackersAnalyst.THIdate(maturityDate.Year, maturityDate.Month, maturityDate.Day, null);
			int tHISettlementDate = TechHackersAnalyst.THIdate(settlementDate.Year, settlementDate.Month, settlementDate.Day, null);

			double []couponArray = new double[1];
			couponArray[0] = coupon;

			int []maturityDateArray = new int[1];
			maturityDateArray[0] = tHIMaturityDate;

			return TechHackersAnalyst.THIbai1(couponArray, couponArray.Length, maturityDateArray,
				maturityDateArray.Length, tHISettlementDate, null, null, null, null, 0, null,
				null, null, 0, null, 0, null, null, null, null, null, 0, null);

		}

		public double Convexity(double coupon, DateTime maturityDate, DateTime settlementDate, double yield)
		{

			int tHIMaturityDate = TechHackersAnalyst.THIdate(maturityDate.Year, maturityDate.Month, maturityDate.Day, null);
			int tHISettlementDate = TechHackersAnalyst.THIdate(settlementDate.Year, settlementDate.Month, settlementDate.Day, null);

			double []couponArray = new double[1];
			couponArray[0] = coupon;

			int []maturityDateArray = new int[1];
			maturityDateArray[0] = tHIMaturityDate;

			return TechHackersAnalyst.THIbconv(couponArray, couponArray.Length, maturityDateArray, maturityDateArray.Length,
				tHISettlementDate, yield, null, null, null, null, 0, null, null, null, null, null, null, null, null, 0, null,
				null, 0, null, 0, null, null, null, null);

		}

		public double Duration(double coupon, DateTime maturityDate, DateTime settlementDate, double yield)
		{

			int tHIMaturityDate = TechHackersAnalyst.THIdate(maturityDate.Year, maturityDate.Month, maturityDate.Day, null);
			int tHISettlementDate = TechHackersAnalyst.THIdate(settlementDate.Year, settlementDate.Month, settlementDate.Day, null);

			double []couponArray = new double[1];
			couponArray[0] = coupon;

			int []maturityDateArray = new int[1];
			maturityDateArray[0] = tHIMaturityDate;

			return TechHackersAnalyst.THIbdur(couponArray, couponArray.Length, maturityDateArray, maturityDateArray.Length,
				tHISettlementDate, yield, null, null, null, null, 0, null, null, null, null, null, null, null, null, null,
				0, null, null, 0, null, 0, null, null, null, null);

		}

	}
	
}
