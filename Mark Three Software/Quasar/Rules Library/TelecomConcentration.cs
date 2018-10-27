// This module contains a sample compliance rule that will test for excessive market value concentrations in an industry sector. It
// is event driven and will generate the violations at the moment the conditions arise.  Conversely, if the conditions are cleared,
// the violations will be removed automatically.  As with all event driven compliance rules, there is a process of initializing the
// violations table to the current state of the data model.  But once the initialization is complete, only the changes to the
// accounts defined in this rule need to be monitored.
namespace Shadows.Quasar.Rule
{

	using System;
	using System.Collections;
	using System.Windows.Forms;

	/// <summary>
	/// List of Restricted Accounts
	/// </summary>
	internal class BetaAccountList : AccountList
	{

		public BetaAccountList()
		{

			// Apply the concentration rule to the Bailey Family members, but not the family group account.  Note that the account
			// identifiers used here are the external ids shared with the accounting system.
			this.Add(Account.Find("0236"));
			this.Add(Account.Find("0237"));
			this.Add(Account.Find("0239"));
			this.Add(Account.Find("0241"));

		}

	}
	
	/// <summary>
	/// A sample industry concentration rule.
	/// </summary>
	public class TelecomConcentration
	{

		private static decimal sectorLimit;
		private static Sector telecomServices;
		private static Restriction restriction;
		private static BetaAccountList betaAccountList;
		private static AccountList updateAccountList;
		
		/// <summary>
		/// Installs an event driven industry concentration compliance check.
		/// </summary>
		static TelecomConcentration()
		{

			// This is a list of all the accounts targeted by this compliance check.
			TelecomConcentration.betaAccountList = new BetaAccountList();

			// This is the preset concentration limit.
			TelecomConcentration.sectorLimit = 0.1M;

			// Find the telecom sector based on the S&P GICS external codes.
			TelecomConcentration.telecomServices = new Sector("50");

			// Open up a restriction for the industry concentration rule.  The severity level of these errors is high and an
			// officer approval is needed to trade any positions.
			TelecomConcentration.restriction = Restriction.Find("TELECOMSECTOR");
			if (TelecomConcentration.restriction == null)
				TelecomConcentration.restriction = new Restriction("TELECOMSECTOR", Severity.High, Approval.Officer,
					"{0} in account {1} exceeds {2:#0%} in {3}.");

			// Cycle through each predefined account and synchronize the violations against the current state of the data model.
			// The method 'AccountHandler' will check the given account for an exceesive concentration in the Telecommunications
			// Sector.
			foreach (Account account in TelecomConcentration.betaAccountList)
				ValidateAccount(account);

			// Flush the command buffer.
			CommandBatch.Flush();
			
			// This list collects the positions that have been changed by the incoming events.
			TelecomConcentration.updateAccountList = new AccountList();

			// This compliance check is event driven.  When an event -- such as adding or deleting an order -- changes the state of
			// the data model, these tests will be called to insure that the new state doesn't violate this compliance rule.  In
			// the case of a simple list rule, the new position will be tested to see if adding or deleting a position results in a
			// violation.  These statements install the event handlers for tax lots, proposed orders, orders and allocations.  For
			// example, the method 'TaxLotHandler' will be called when the tax lot table changes.
			MarketData.BeginMerge += new EventHandler(BeginMerge);
			TaxLot.Changed += new TaxLotEvent(TaxLotHandler);
			ProposedOrder.Changed += new ProposedOrderEvent(ProposedOrderHandler);
			Order.Changed += new OrderEvent(OrderHandler);
			Allocation.Changed += new AllocationEvent(AllocationHandler);
			MarketData.EndMerge += new EventHandler(EndMerge);

		}

		/// <summary>
		/// Initializes the compliance rule to handle incoming events.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private static void BeginMerge(object sender, EventArgs eventArgs)
		{

			// Empty out the list each time a Merge is processed.
			TelecomConcentration.updateAccountList.Clear();

		}
		
		/// <summary>
		/// Checks to make sure that a tax lot doesn't violate a list of restricted securities.
		/// </summary>
		/// <param name="sender">The object that originated the event (ignored).</param>
		/// <param name="taxLotEventArgs">A proposed order that is to be checked.</param>
		private static void TaxLotHandler(object sender, TaxLotEventArgs taxLotEventArgs)
		{

			// Extract the event argument.
			TaxLot taxLot = taxLotEventArgs.TaxLot;

			// There is no need to recalculate the market and sector values if the the account is not part of this compliance
			// restriction.  A quick filter should be part of all compliance checks to prevent unnecessary calculations.
			if (TelecomConcentration.betaAccountList.Contains(taxLot.Position.Account))
				TelecomConcentration.updateAccountList.Add(taxLot.Position.Account);

		}

		/// <summary>
		/// Checks to make sure that a new proposed order doesn't violate a list of restricted securities.
		/// </summary>
		/// <param name="sender">The object that originated the event (ignored).</param>
		/// <param name="proposedOrderEventArgs">A proposed order that is to be checked.</param>
		private static void ProposedOrderHandler(object sender, ProposedOrderEventArgs proposedOrderEventArgs)
		{

			// Extract the event argument.
			ProposedOrder proposedOrder = proposedOrderEventArgs.ProposedOrder;

			// There is no need to recalculate the market and sector values if the the account is not part of this compliance
			// restriction.  A quick filter should be part of all compliance checks to prevent unnecessary calculations.
			if (TelecomConcentration.betaAccountList.Contains(proposedOrder.Position.Account))
				TelecomConcentration.updateAccountList.Add(proposedOrder.Position.Account);

		}

		/// <summary>
		/// Checks to make sure that a new proposed order doesn't violate a list of restricted securities.
		/// </summary>
		/// <param name="sender">The object that originated the event (ignored).</param>
		/// <param name="orderEventArgs">A proposed order that is to be checked.</param>
		private static void OrderHandler(object sender, OrderEventArgs orderEventArgs)
		{

			// Extract the event argument.
			Order order = orderEventArgs.Order;

			// There is no need to recalculate the market and sector values if the the account is not part of this compliance
			// restriction.  A quick filter should be part of all compliance checks to prevent unnecessary calculations.
			if (TelecomConcentration.betaAccountList.Contains(order.Position.Account))
				TelecomConcentration.updateAccountList.Add(order.Position.Account);

		}

		/// <summary>
		/// Checks to make sure that a new proposed order doesn't violate a list of restricted securities.
		/// </summary>
		/// <param name="sender">The object that originated the event (ignored).</param>
		/// <param name="allocationEventArgs">A proposed order that is to be checked.</param>
		private static void AllocationHandler(object sender, AllocationEventArgs allocationEventArgs)
		{

			// Extract the event argument.
			Allocation allocation = allocationEventArgs.Allocation;

			// There is no need to recalculate the market and sector values if the the account is not part of this compliance
			// restriction.  A quick filter should be part of all compliance checks to prevent unnecessary calculations.
			if (TelecomConcentration.betaAccountList.Contains(allocation.Position.Account))
				TelecomConcentration.updateAccountList.Add(allocation.Position.Account);

		}

		/// <summary>
		/// Apply the telecommmunication concentration rule to the given account.
		/// </summary>
		/// <param name="account">The account that should be checked for an industry concentration.</param>
		private static void EndMerge(object sender, EventArgs eventArgs)
		{

			if (TelecomConcentration.updateAccountList != null && TelecomConcentration.updateAccountList.Count != 0)
				foreach (Account account in TelecomConcentration.updateAccountList)
					ValidateAccount(account);

		}

		private static void ValidateAccount(Account account)
		{

			// Calculate the total market value of the given account and the sector market value for the telecommunications
			// sector.  Note how simple, object oriented operations (e.g. "GetMarketValue", "GetPositions") make a very complicated
			// tasks on the account data appear relatively easy to program.
			decimal totalMarketValue = account.GetMarketValue();
			decimal sectorMarketValue = account.GetMarketValue(telecomServices);

			// If the sector market value of the industry is greater than the predefined limit, a violation will be generated for
			// every security in that sector that belongs to this account.  Conversely, If the industry concentration is below the
			// limit, any existing violations will be cleared.
			if (sectorMarketValue / totalMarketValue >= sectorLimit)
				foreach (Position position in account.GetPositions(TelecomConcentration.telecomServices))
				{
					Violation violation = Violation.Find(TelecomConcentration.restriction, position);
					if (violation == null)
						Violation.Add(TelecomConcentration.restriction, position, position.Security.Symbol, position.Account.Name,
							sectorLimit, TelecomConcentration.telecomServices.Name);
				}
			else
				foreach (Violation violation in Violation.Find(restriction, account))
					violation.Remove();

		}

	}

}
