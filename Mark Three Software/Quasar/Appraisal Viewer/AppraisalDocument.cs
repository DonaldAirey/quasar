/*************************************************************************************************************************
*
*	File:			AppraisalDocument.cs
*	Description:	This class builds a Document Object Model for an appraisal.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Client;
using Shadows.Quasar.Common;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace Shadows.Quasar.Viewers.Appraisal
{

	/// <summary>
	/// An appraisal document is a hierarchical organization of account positions by industry classification scheme.
	/// </summary>
	class AppraisalDocument : XmlDocument
	{

		private ClientMarketData.ModelRow modelRow;
		private ClientMarketData.AccountRow accountRow;
		private Common.Appraisal appraisal;
		public DataView proposedOrderView;
		public DataView orderView;
		public DataView allocationView;

		// Public Properties
		public ClientMarketData.ModelRow ModelRow {get {return this.modelRow;}}
		public ClientMarketData.AccountRow AccountRow {get {return this.accountRow;}}

		/// <summary>
		/// Constructs a well formed, but empty, AppraisalDocument.
		/// </summary>
		public AppraisalDocument()
		{

			// Initialize the members.
			this.accountRow = null;
			this.modelRow = null;

			// Create the root element and add it to the document.
			this.AppendChild(new AppraisalElement(this));

		}

		/// <summary>
		/// Constructs an AppraisalDocument.
		/// </summary>
		/// <param name="accountRow">A record containing the account or fund data.</param>
		/// <param name="modelRow">A record containing the model that is superimposed on the appraisal data for
		/// rebalancing.</param>
		public AppraisalDocument(ClientMarketData.AccountRow accountRow, ClientMarketData.ModelRow modelRow)
		{

			// Create a view of the proposed orders that makes it easy to aggregate by position.
			this.proposedOrderView = new DataView(ClientMarketData.ProposedOrder);
			this.proposedOrderView.Sort = "[AccountId], [SecurityId], [PositionTypeCode]";

			// Create a view of the orders that makes it easy to aggregate by position.
			this.orderView = new DataView(ClientMarketData.Order);
			this.orderView.Sort = "[AccountId], [SecurityId], [PositionTypeCode]";

			// Create a view of the allocations that makes it easy to aggregate by position.
			this.allocationView = new DataView(ClientMarketData.Allocation);
			this.allocationView.Sort = "[AccountId], [SecurityId], [PositionTypeCode]";

			// This makes the account and model avaiable to the recursive methods.
			this.accountRow = accountRow;
			this.modelRow = modelRow;
			
			// Create the root element and add it to the document.
			AppraisalElement rootElement = new AppraisalElement(this);
			this.AppendChild(rootElement);

			// Create and populate the DataSet that represents the outline of the appraisal.  This function creates a
			// set of linked structures that contains only the sectors, securities and positions that will appear in
			// this document. This driver is built from the bottom up, meaning that we start with the tax lots,
			// proposed orders, orders and allocations associated with the given account and build the driver up to
			// the topmost security classification scheme.  The alternative -- starting with the classification scheme
			// and building down until we join with the tax lots, etc, -- turned out to be six times slower.
			this.appraisal = new Common.Appraisal(accountRow, modelRow, true);

			// This sector is a catch-all heading for securities not mapped to the given hierarchy.  If everything is
			// mapped, then this sector won't appear in the document.
			SectorElement unclassifiedSector = null;

			// Now that the driver is built, we can begin constructing the document.  The first section is the
			// 'Unclassified' sector.  This section catches all securities that aren't explicitly mapped to the
			// hierarchy.  This is important because classification schemes are not guaranteed to map every security.
			// Without this section, those unmapped securities wouldn't appear on the appraisal and wouldn't be
			// included in the NAV calculation.  That is very bad.  Note also that we skip over the classification
			// scheme record during the check.  We know that the security classification scheme is at the top of the
			// hierarchy and won't have any parents.  Every other record that doesn't have a parent in the hierarchy
			// is 'Unclassified'.
			foreach (AppraisalSet.SecurityRow parentSecurity in this.appraisal.Security)
				if (parentSecurity.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeChildId().Length == 0)
				{

					// If the document doesn't have an 'Unclassified' sector yet, then add the sector heading.  All 
					// secutiries that are not mapped to the given hierarchy will appear under this catch-all header.
					if (unclassifiedSector == null)
						rootElement.InsertBySortOrder(unclassifiedSector = new SectorElement(this));

					// Attach the each of the unclassified securities to the unclassified sector heading.
					BuildDocument(unclassifiedSector, parentSecurity.ObjectRow);

				}
				
			// The report is built recursively.  The 'AppraisalSet', constructed above, represents an 'inner join' of the
			// hierarchy information to the active position information.  We'll begin traversing the 'AppraisalSet' from
			// the top level security: a single node representing the classification scheme.
			foreach (AppraisalSet.SchemeRow schemeRow in this.appraisal.Scheme)
				foreach (AppraisalSet.ObjectTreeRow objectTreeRow in schemeRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
					BuildDocument(rootElement, objectTreeRow.ObjectRowByFKObjectObjectTreeChildId);

		}

		/// <summary>
		/// Recusively builds the XML document from the document outline.
		/// </summary>
		/// <param name="parentElement">The parent XML element the node being built.</param>
		/// <param name="parentSecurity">The parent security.</param>
		private void BuildDocument(CommonElement parentElement, AppraisalSet.ObjectRow parentObject)
		{

			// The code below will test to see if the current node is a sector or security, then create the appropriate
			// element. The outline doesn't have the full information about the securities, so we need to get a record
			// into the data model that can reference the security master.
			ClientMarketData.ObjectRow childObject = ClientMarketData.Object.FindByObjectId(parentObject.ObjectId);

			// Attach the sectors to this level of the document.
			foreach (ClientMarketData.SectorRow sectorRow in childObject.GetSectorRows())
			{

				// Create the new sector element and attach it to the document.
				SectorElement sectorElement = new SectorElement(this, sectorRow);
					parentElement.InsertBySortOrder(sectorElement);

				// Recurse down into the children securities looking for more sectors or securities to attach.
				foreach (AppraisalSet.ObjectTreeRow objectTreeRow in parentObject.GetObjectTreeRowsByFKObjectObjectTreeParentId())
					BuildDocument(sectorElement, objectTreeRow.ObjectRowByFKObjectObjectTreeChildId);

			}

			// Attach the securities to this level of the document.
			foreach (ClientMarketData.SecurityRow securityRow in childObject.GetSecurityRows())
			{
			
				// Attach long and short Debts to this level of the document.
				foreach (ClientMarketData.DebtRow debtRow in securityRow.GetDebtRowsByFKSecurityDebtDebtId())
					foreach (AppraisalSet.SecurityRow driverSecurity in parentObject.GetSecurityRows())
						foreach (AppraisalSet.PositionRow driverPosition in driverSecurity.GetPositionRows())
							parentElement.InsertByName(new DebtElement(this, debtRow, driverPosition));
			
				// Attach long and short Currencies to this level of the document.
				foreach (ClientMarketData.CurrencyRow currencyRow in securityRow.GetCurrencyRows())
					foreach (AppraisalSet.SecurityRow driverSecurity in parentObject.GetSecurityRows())
						foreach (AppraisalSet.PositionRow driverPosition in driverSecurity.GetPositionRows())
							parentElement.InsertByName(new CurrencyElement(this, currencyRow, driverPosition));

				// Attach long and short Equities to this level of the document.
				foreach (ClientMarketData.EquityRow equityRow in securityRow.GetEquityRowsByFKSecurityEquityEquityId())
					foreach (AppraisalSet.SecurityRow driverSecurity in parentObject.GetSecurityRows())
						foreach (AppraisalSet.PositionRow driverPosition in driverSecurity.GetPositionRows())
							parentElement.InsertByName(new EquityElement(this, equityRow, driverPosition));

			}

		}
		
	}

	/// <summary>
	/// A common Xml Element for the AppraisalDocument.
	/// </summary>
	class AppraisalElement : CommonElement
	{

		/// <summary>
		/// Creates a common element for the AppraisalDocument.
		/// </summary>
		/// <param name="name">The local name of the node.</param>
		/// <param name="appraisalDocument">The parent document.</param>
		public AppraisalElement(AppraisalDocument appraisalDocument) : base("Appraisal", appraisalDocument)
		{
		
			// The top level account information is used to add the user data to the top level of the appraisal.
			ClientMarketData.AccountRow accountRow = appraisalDocument.AccountRow;
			
			// Add any top-level data associated with this account (such as aggregate risk, account level quantitative
			// calculations, etc.
			if (accountRow != null)
			{
				if (!accountRow.IsUserData0Null()) AddAttribute("UserData0", accountRow.UserData0.ToString());
				if (!accountRow.IsUserData1Null()) AddAttribute("UserData1", accountRow.UserData1.ToString());
				if (!accountRow.IsUserData2Null()) AddAttribute("UserData2", accountRow.UserData2.ToString());
				if (!accountRow.IsUserData3Null()) AddAttribute("UserData3", accountRow.UserData3.ToString());
				if (!accountRow.IsUserData4Null()) AddAttribute("UserData4", accountRow.UserData4.ToString());
				if (!accountRow.IsUserData5Null()) AddAttribute("UserData5", accountRow.UserData5.ToString());
				if (!accountRow.IsUserData6Null()) AddAttribute("UserData6", accountRow.UserData6.ToString());
				if (!accountRow.IsUserData7Null()) AddAttribute("UserData7", accountRow.UserData7.ToString());
			}

		}

	}
	
	/// <summary>
	/// An element that represents a sector in the hierarchy.
	/// </summary>
	class SectorElement : CommonElement
	{

		/// <summary>
		/// Creates an 'Unclassified' Sector.
		/// </summary>
		/// <param name="appraisalDocument">The parent document.</param>
		public SectorElement(AppraisalDocument appraisalDocument) :
			base("Sector", appraisalDocument)
		{

			// The sector id and name are taken directly from the SectorRow record.
			AddAttribute("SectorId", "0");
			AddAttribute("SortOrder", "0");
			AddAttribute("Name", "Unclassified");

		}

		/// <summary>
		/// Creates a sector element.
		/// </summary>
		/// <param name="appraisalDocument">The parent document.</param>
		/// <param name="sectorRow">The sector record used to create the Xml element.</param>
		public SectorElement(AppraisalDocument appraisalDocument, ClientMarketData.SectorRow sectorRow) :
			base("Sector", appraisalDocument)
		{

			// The sector id and name are taken directly from the SectorRow record.
			AddAttribute("SectorId", sectorRow.SectorId.ToString());
			AddAttribute("SortOrder", sectorRow.SortOrder.ToString());
			AddAttribute("Name", sectorRow.ObjectRow.Name.ToString());

			// If there is a target percentage associated with this sector, add it to the attribute list.
			ClientMarketData.SectorTargetRow sectorTargetRow =
				ClientMarketData.SectorTarget.FindByModelIdSectorId(appraisalDocument.ModelRow.ModelId,
				sectorRow.SectorId);
			if (sectorTargetRow != null)
				AddAttribute("ModelPercent", sectorTargetRow.Percent.ToString());

			// If there is a position record associated with this sector, then add the externally supplied data to the
			// record.  Since the tax lots are always aggregated as we need them into a position, there's no static table
			// that keeps position data.  This information is generally from an outside system that is related to the
			// position, such as risk metrics or quantitative calculations.
			ClientMarketData.PositionRow position = ClientMarketData.Position.FindByAccountIdSecurityIdPositionTypeCode(
				appraisalDocument.AccountRow.AccountId, sectorRow.SectorId, Shadows.Quasar.Common.PositionType.Long);
			if (position != null)
			{
				if (!position.IsUserData0Null()) AddAttribute("UserData0", position.UserData0.ToString());
				if (!position.IsUserData1Null()) AddAttribute("UserData1", position.UserData1.ToString());
				if (!position.IsUserData2Null()) AddAttribute("UserData2", position.UserData2.ToString());
				if (!position.IsUserData3Null()) AddAttribute("UserData3", position.UserData3.ToString());
				if (!position.IsUserData4Null()) AddAttribute("UserData4", position.UserData4.ToString());
				if (!position.IsUserData5Null()) AddAttribute("UserData5", position.UserData5.ToString());
				if (!position.IsUserData6Null()) AddAttribute("UserData6", position.UserData6.ToString());
				if (!position.IsUserData7Null()) AddAttribute("UserData7", position.UserData7.ToString());
			}

		}
		
	}
	
	/// <summary>
	/// Debt Element of an Appraisal Document.
	/// </summary>
	class DebtElement : CommonElement
	{

		/// <summary>
		/// Creates a Debt Element for an Appraisal Document.
		/// </summary>
		/// <param name="appraisalDocument">The parent Document.</param>
		/// <param name="debtRow">The record used to create the Xml element.</param>
		/// <param name="PositionTypeCode">Whether a short or long position.</param>
		public DebtElement(AppraisalDocument appraisalDocument, ClientMarketData.DebtRow debtRow,
			AppraisalSet.PositionRow positionRow) : base("Debt", appraisalDocument)
		{

			// These records are used to access information held in the ancestors.
			ClientMarketData.SecurityRow securityRow = debtRow.SecurityRowByFKSecurityDebtDebtId;
			ClientMarketData.ObjectRow objectRow = securityRow.ObjectRow;

			// Add the essential attributes for this element.
			AddAttribute("SecurityId", debtRow.DebtId.ToString());
			AddAttribute("PositionTypeCode", positionRow.PositionTypeCode.ToString());
			AddAttribute("Name", objectRow.Name.ToString());
			AddAttribute("PriceFactor", securityRow.PriceFactor.ToString());
			AddAttribute("QuantityFactor", securityRow.QuantityFactor.ToString());

			// Add the price info.
			// Find the price based on the default currency found in the equity record.  If the security master doesn't
			// have a price for this security, provide dummy values to insure a market value can be calculated.  Zero is a
			// perfectly reasonable interpretation of a missing price.
			ClientMarketData.PriceRow securityPrice = ClientMarketData.Price.FindBySecurityIdCurrencyId(
				securityRow.SecurityId, debtRow.SettlementId);
			if (securityPrice == null)
				AddAttribute("Price", "0.0");
			else
			{
				if (ClientPreferences.Pricing == Pricing.Close)
					AddAttribute("Price", securityPrice.ClosePrice.ToString());
				if (ClientPreferences.Pricing == Pricing.Last)
					AddAttribute("Price", securityPrice.LastPrice.ToString());
			}

			// Find the crossing price.  This is used to convert any local price into the account's base currency. Provide
			// defaults if the crossing price can be found.  While this will lead to wrong values, they will be obvious on
			// the appraisal.  These defaults make the appraisal faster because it doesn't have to account for an absent
			// crossing price.
			ClientMarketData.PriceRow currencyPrice = ClientMarketData.Price.FindBySecurityIdCurrencyId(
				debtRow.SettlementId, appraisalDocument.AccountRow.CurrencyRow.CurrencyId);
			if (currencyPrice == null)
			{
				AddAttribute("CloseCrossPrice", "0.0");
				AddAttribute("LastCrossPrice", "0.0");
			}
			else
			{
				AddAttribute("CloseCrossPrice", currencyPrice.ClosePrice.ToString());
				AddAttribute("LastCrossPrice", currencyPrice.LastPrice.ToString());
			}

			// Add a target percentage if one is associated with this security.
			ClientMarketData.PositionTargetRow positionTargetRow =
				ClientMarketData.PositionTarget.FindByModelIdSecurityIdPositionTypeCode(
				appraisalDocument.ModelRow.ModelId, debtRow.DebtId, positionRow.PositionTypeCode);
			if (positionTargetRow != null)
				AddAttribute("ModelPercent", positionTargetRow.Percent.ToString());

			// If there is a position record associated with this, er..  position, then add the externally supplied data
			// to the record.  Since the tax lots are always aggregated as we need them into a position, there's no static
			// table that keeps position data.  This information is generally from an outside system that is related to 
			// the position, such as risk metrics or quantitative calculations.
			ClientMarketData.PositionRow position = ClientMarketData.Position.FindByAccountIdSecurityIdPositionTypeCode(
				appraisalDocument.AccountRow.AccountId, positionRow.SecurityId, positionRow.PositionTypeCode);
			if (position != null)
			{
				if (!position.IsUserData0Null()) AddAttribute("UserData0", position.UserData0.ToString());
				if (!position.IsUserData1Null()) AddAttribute("UserData1", position.UserData1.ToString());
				if (!position.IsUserData2Null()) AddAttribute("UserData2", position.UserData2.ToString());
				if (!position.IsUserData3Null()) AddAttribute("UserData3", position.UserData3.ToString());
				if (!position.IsUserData4Null()) AddAttribute("UserData4", position.UserData4.ToString());
				if (!position.IsUserData5Null()) AddAttribute("UserData5", position.UserData5.ToString());
				if (!position.IsUserData6Null()) AddAttribute("UserData6", position.UserData6.ToString());
				if (!position.IsUserData7Null()) AddAttribute("UserData7", position.UserData7.ToString());
			}

			// Add the account level aggregates for this security/position type pair.
			foreach (AppraisalSet.AccountRow accountRow in positionRow.GetAccountRows())
				this.AppendChild(new AccountElement(appraisalDocument, accountRow));
		
		}

	}

	/// <summary>
	/// Currency Element of an Appraisal Document.
	/// </summary>
	class CurrencyElement : CommonElement
	{

		/// <summary>
		/// Creates a Currency Element for an Appraisal Document.
		/// </summary>
		/// <param name="appraisalDocument">The parent Xml Document.</param>
		/// <param name="currencyRow">A currency record in the data model.</param>
		/// <param name="PositionTypeCode">Whether a short or long position.</param>
		public CurrencyElement(AppraisalDocument appraisalDocument, ClientMarketData.CurrencyRow currencyRow,
			AppraisalSet.PositionRow positionRow) : base("Currency", appraisalDocument)
		{

			// These records are used to access information held in the ancestors.
			ClientMarketData.SecurityRow securityRow = currencyRow.SecurityRow;
			ClientMarketData.ObjectRow objectRow = securityRow.ObjectRow;

			// Add the essential attributes for this element.
			AddAttribute("SecurityId", currencyRow.CurrencyId.ToString());
			AddAttribute("PositionTypeCode", positionRow.PositionTypeCode.ToString());
			AddAttribute("Name", objectRow.Name.ToString());

			// Add the price info.
			ClientMarketData.PriceRow priceRow =
				ClientMarketData.Price.FindBySecurityIdCurrencyId(securityRow.SecurityId,
				appraisalDocument.AccountRow.CurrencyRow.CurrencyId);
			if (priceRow == null)
				AddAttribute("Price", "0.0");
			else
			{
				if (ClientPreferences.Pricing == Pricing.Close)
					AddAttribute("Price", priceRow.ClosePrice.ToString());
				if (ClientPreferences.Pricing == Pricing.Last)
					AddAttribute("Price", priceRow.LastPrice.ToString());
			}

			// Add a target percentage if one is associated with this security.
			ClientMarketData.PositionTargetRow positionTargetRow =
				ClientMarketData.PositionTarget.FindByModelIdSecurityIdPositionTypeCode(
				appraisalDocument.ModelRow.ModelId, currencyRow.CurrencyId, positionRow.PositionTypeCode);
			if (positionTargetRow != null)
				AddAttribute("ModelPercent", positionTargetRow.Percent.ToString());

			// If there is a position record associated with this, er..  position, then add the externally supplied data
			// to the record.  Since the tax lots are always aggregated as we need them into a position, there's no static
			// table that keeps position data.  This information is generally from an outside system that is related to 
			// the position, such as risk metrics or quantitative calculations.
			ClientMarketData.PositionRow position = ClientMarketData.Position.FindByAccountIdSecurityIdPositionTypeCode(
				appraisalDocument.AccountRow.AccountId, positionRow.SecurityId, positionRow.PositionTypeCode);
			if (position != null)
			{
				if (!position.IsUserData0Null()) AddAttribute("UserData0", position.UserData0.ToString());
				if (!position.IsUserData1Null()) AddAttribute("UserData1", position.UserData1.ToString());
				if (!position.IsUserData2Null()) AddAttribute("UserData2", position.UserData2.ToString());
				if (!position.IsUserData3Null()) AddAttribute("UserData3", position.UserData3.ToString());
				if (!position.IsUserData4Null()) AddAttribute("UserData4", position.UserData4.ToString());
				if (!position.IsUserData5Null()) AddAttribute("UserData5", position.UserData5.ToString());
				if (!position.IsUserData6Null()) AddAttribute("UserData6", position.UserData6.ToString());
				if (!position.IsUserData7Null()) AddAttribute("UserData7", position.UserData7.ToString());
			}

			// Append the account level aggregates to this security/position type element.
			foreach (AppraisalSet.AccountRow accountRow in positionRow.GetAccountRows())
				this.AppendChild(new AccountElement(appraisalDocument, accountRow));

		}

	}

	/// <summary>
	/// Equity Element of an Appraisal Document
	/// </summary>
	class EquityElement : CommonElement
	{

		/// <summary>
		/// Creates an Equity Element for an Appriasal Document.
		/// </summary>
		/// <param name="appraisalDocument">The parent Xml Document.</param>
		/// <param name="equityRow">An equity record from the data model.</param>
		/// <param name="PositionTypeCode">Whether a short or long position.</param>
		public EquityElement(AppraisalDocument appraisalDocument, ClientMarketData.EquityRow equityRow,
			AppraisalSet.PositionRow positionRow) : base("Equity", appraisalDocument)
		{

			// These records are used to access information held in the ancestors.
			ClientMarketData.SecurityRow securityRow = equityRow.SecurityRowByFKSecurityEquityEquityId;
			ClientMarketData.ObjectRow objectRow = securityRow.ObjectRow;

			// Add the essential attributes for this element.
			AddAttribute("SecurityId", equityRow.EquityId.ToString());
			AddAttribute("PositionTypeCode", positionRow.PositionTypeCode.ToString());
			AddAttribute("Name", objectRow.Name.ToString());
			AddAttribute("PriceFactor", securityRow.PriceFactor.ToString());
			AddAttribute("QuantityFactor", securityRow.QuantityFactor.ToString());

			// Find the price based on the default currency found in the equity record.  If the security master doesn't
			// have a price for this security, provide dummy values to insure a market value can be calculated.  Zero is a
			// perfectly reasonable interpretation of a missing price.
			ClientMarketData.PriceRow securityPrice = ClientMarketData.Price.FindBySecurityIdCurrencyId(
				securityRow.SecurityId, equityRow.SettlementId);
			if (securityPrice == null)
				AddAttribute("Price", "0.0");
			else
			{
				if (ClientPreferences.Pricing == Pricing.Close)
					AddAttribute("Price", securityPrice.ClosePrice.ToString());
				if (ClientPreferences.Pricing == Pricing.Last)
					AddAttribute("Price", securityPrice.LastPrice.ToString());
			}

			// Find the crossing price.  This is used to convert any local price into the account's base currency. Provide
			// defaults if the crossing price can be found.  While this will lead to wrong values, they will be obvious on
			// the appraisal.
			ClientMarketData.PriceRow currencyPrice = ClientMarketData.Price.FindBySecurityIdCurrencyId(
				equityRow.SettlementId, appraisalDocument.AccountRow.CurrencyRow.CurrencyId);
			if (currencyPrice == null)
			{
				AddAttribute("CloseCrossPrice", "0.0");
				AddAttribute("LastCrossPrice", "0.0");
			}
			else
			{
				AddAttribute("CloseCrossPrice", currencyPrice.ClosePrice.ToString());
				AddAttribute("LastCrossPrice", currencyPrice.LastPrice.ToString());
			}

			// Add a target percentage if one is associated with this security.
			ClientMarketData.PositionTargetRow positionTargetRow =
				ClientMarketData.PositionTarget.FindByModelIdSecurityIdPositionTypeCode(
				appraisalDocument.ModelRow.ModelId, equityRow.EquityId, positionRow.PositionTypeCode);
			if (positionTargetRow != null)
				AddAttribute("ModelPercent", positionTargetRow.Percent.ToString());

			// If there is a position record associated with this, er..  position, then add the externally supplied data
			// to the record.  Since the tax lots are always aggregated as we need them into a position, there's no static
			// table that keeps position data.  This information is generally from an outside system that is related to 
			// the position, such as risk metrics or quantitative calculations.
			ClientMarketData.PositionRow position = ClientMarketData.Position.FindByAccountIdSecurityIdPositionTypeCode(
				appraisalDocument.AccountRow.AccountId, positionRow.SecurityId, positionRow.PositionTypeCode);
			if (position != null)
			{
				if (!position.IsUserData0Null()) AddAttribute("UserData0", position.UserData0.ToString());
				if (!position.IsUserData1Null()) AddAttribute("UserData1", position.UserData1.ToString());
				if (!position.IsUserData2Null()) AddAttribute("UserData2", position.UserData2.ToString());
				if (!position.IsUserData3Null()) AddAttribute("UserData3", position.UserData3.ToString());
				if (!position.IsUserData4Null()) AddAttribute("UserData4", position.UserData4.ToString());
				if (!position.IsUserData5Null()) AddAttribute("UserData5", position.UserData5.ToString());
				if (!position.IsUserData6Null()) AddAttribute("UserData6", position.UserData6.ToString());
				if (!position.IsUserData7Null()) AddAttribute("UserData7", position.UserData7.ToString());
			}

			// Add the account level aggregates for this security/position type pair.
			foreach (AppraisalSet.AccountRow accountRow in positionRow.GetAccountRows())
				this.AppendChild(new AccountElement(appraisalDocument, accountRow));

		}

	}

	/// <summary>
	/// An element in the Appriasal Document that represents a fund's or account's positions.
	/// </summary>
	class AccountElement : CommonElement
	{

		/// <summary>
		/// Creates an element in the Appraisal Document that represents a fund's or account's position.
		/// </summary>
		/// <param name="appraisalDocument">The parent document.</param>
		/// <param name="driverAccount">Identifies the individual position at the account/security/position level.</param>
		public AccountElement(AppraisalDocument appraisalDocument, AppraisalSet.AccountRow driverAccount) :
			base("Account", appraisalDocument)
		{

			// Get the account record from the account id.  This record drives most of the data that appears in this element.
			ClientMarketData.AccountRow accountRow =
				ClientMarketData.Account.FindByAccountId(driverAccount.AccountId);

			// Count up the compliance violations
			int violationCount = 0;
			foreach (DataRowView dataRowView in
				ClientMarketData.Violation.UKViolationAccountIdSecurityIdPositionTypeCode.FindRows(
				new object[] {driverAccount.AccountId, driverAccount.SecurityId, driverAccount.PositionTypeCode}))
			{
				ClientMarketData.ViolationRow violationRow = (ClientMarketData.ViolationRow)dataRowView.Row;
				if (violationRow.RestrictionRow.Severity > 0)
					violationCount++;
			}
			AddAttribute("Violation", violationCount);

			// Add the essential attributes to the element.
			AddAttribute("AccountId", accountRow.AccountId.ToString());

			// Aggregate the tax lot positions and cost.
			decimal taxLotQuantity = 0.0M;
			decimal taxLotCost = 0.0M;
			foreach (ClientMarketData.TaxLotRow taxLotRow in accountRow.GetTaxLotRows())
				if (taxLotRow.SecurityId == driverAccount.SecurityId &&
					taxLotRow.PositionTypeCode == driverAccount.PositionTypeCode)
				{
					taxLotQuantity += taxLotRow.Quantity;
					taxLotCost += taxLotRow.Cost * taxLotRow.Quantity;
				}
			AddAttribute("TaxLotQuantity", taxLotQuantity.ToString());
			AddAttribute("TaxLotCost", taxLotCost.ToString());

			// Aggregate the proposed orders positions.
			decimal proposedOrderQuantity = 0.0M;
			foreach (DataRowView dataRowView in
				appraisalDocument.proposedOrderView.FindRows(new object[] {driverAccount.AccountId, driverAccount.SecurityId,
																	 driverAccount.PositionTypeCode}))
			{
				ClientMarketData.ProposedOrderRow proposedOrderRow = (ClientMarketData.ProposedOrderRow)dataRowView.Row;
				proposedOrderQuantity += proposedOrderRow.Quantity *
					proposedOrderRow.TransactionTypeRow.QuantitySign;
			}
			AddAttribute("ProposedOrderQuantity", proposedOrderQuantity.ToString());

			// Aggregate the orders.
			decimal orderQuantity = 0.0M;
			foreach (DataRowView dataRowView in
				appraisalDocument.orderView.FindRows(new object[] {driverAccount.AccountId, driverAccount.SecurityId,
																	 driverAccount.PositionTypeCode}))
			{
				ClientMarketData.OrderRow orderRow = (ClientMarketData.OrderRow)dataRowView.Row;
				orderQuantity += orderRow.Quantity *
					orderRow.TransactionTypeRow.QuantitySign;
			}
			AddAttribute("OrderQuantity", orderQuantity.ToString());

			// Aggregate the allocations.
			decimal allocationQuantity = 0.0M;
			foreach (DataRowView dataRowView in
				appraisalDocument.allocationView.FindRows(new object[] {driverAccount.AccountId, driverAccount.SecurityId,
																	 driverAccount.PositionTypeCode}))
			{
				ClientMarketData.AllocationRow allocationRow = (ClientMarketData.AllocationRow)dataRowView.Row;
				allocationQuantity += allocationRow.Quantity *
					allocationRow.TransactionTypeRow.QuantitySign;
			}
			AddAttribute("AllocationQuantity", allocationQuantity.ToString());

		}

	}

}
