/*************************************************************************************************************************
*
*	File:			TechHackersAnalyst.cs
*	Description:	Tech Hackers Analyst Debt Calculation Library
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace Shadows.Quasar.TechHackers
{

	/// <summary>
	/// Summary description for TechHackersAnalyst.
	/// </summary>
	public class TechHackersAnalyst
	{

		[DllImport("t_date.dll")]
		public static extern int THIdate(int year, int month, int day, int[] status_ptr);

		[DllImport("t_bond.dll")] 
		public static extern double THIbyld(double[] coupon, int num_coupon, int[]maturity, int num_maturity,
			int settle, double price, int[]dated, int[]firstcoup, int[] debttype, int[] freq, int num_freq,
			double[] redem, double[] itax, double[] gtax, int[] cutoff, double[] issue_pr, int[] true_yld,
			int[] wkend, int[]holidays, int numholidays, double[] rout, int[]rstart, int num_rstart,
			int[]rend, int num_rend, double[] ramount, double[] voluntary, int[] numexdiv, int[] status);
		
		[DllImport("t_bond.dll")] 
		public static extern double THIpbyld(double[] coupon, int num_coupon, int anniv, int settle,
			double price, int[] dated, int num_dated, int[] debttype, int[] freq, double[] itax, int[] true_yld,
			int[] wkend, int[] holidays, int numholidays, int[] numexdiv, int[] status);

		[DllImport("t_bond.dll")] 
		public static extern double THIbai1(double[] coupon, int num_coupon, int[] maturity, int num_maturity,
			int settle, int[] dated, int[] firstcoup, int[] debttype, int[] freq, int num_freq, double[] itax,
			double[] rout, int[] rstart, int num_rstart, int[] rend, int num_rend, double[] ramount, double[] voluntary,
			int[] numexdiv, int[] wkend, int[] holidays, int numholidays, int[] status);

		[DllImport("t_bond.dll")] 
		public static extern double THIbpr(double[] coupon, int num_coupon, int[] maturity, int num_maturity,
			int settle, double yield, int[] dated, int[] firstcoup, int[] debttype, int[] freq, int num_freq,
			double[] redem, double[] itax, double[] gtax, int[] cutoff, double[] issue_pr, int[] true_yld,
			int[] wkend, int[] holidays, int numholidays, double[] rout, int[] rstart, int num_rstart, int[] rend,
			int num_rend, double[] ramount, double[] voluntary, int[] numexdiv, int[] status);

		[DllImport("t_bond.dll")] 
		public static extern double THIbconv(double[] coupon, int num_coupon, int[] maturity, int num_maturity,
			int settle, double yield, int[] dated, int[] firstcoup, int[] debttype, int[] freq, int num_freq,
			double[] redem, double[] itax, double[] gtax, int[] cutoff, double[] issue_pr, int[] true_yld, int[] wkend,
			int[] holidays, int numholidays, double[] rout, int[] rstart, int num_rstart, int[] rend, int num_rend,
			double[] ramount, double[] voluntary, int[] numexdiv, int[] status);

		[DllImport("t_bond.dll")] 
		public static extern double THIbdur(double[] coupon, int num_coupon, int[] maturity, int num_maturity, int settle,
			double yield, int[] dated, int[] firstcoup, int[] debttype, int[] freq, int num_freq, double[] redem, int[] macaulay,
			double[] itax, double[] gtax, int[] cutoff, double[] issue_pr, int[] true_yld, int[] wkend, int[] holidays,
			int numholidays, double[] rout, int[] rstart, int num_rstart, int[] rend, int num_rend, double[] ramount,
			double[] voluntary, int[] numexdiv, int[] status);

	}

}
