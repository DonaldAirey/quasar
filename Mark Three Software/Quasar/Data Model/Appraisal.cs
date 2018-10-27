/*************************************************************************************************************************
*
*	File:			Appraisal.cs
*	Description:	This class is used to construct an outline of an appraisal.  This outline organizes the data
*					hierarchically (industry sector within industry sector) and across all sub-account.  It's useful
*					for building the document as well as calculating market values by sector and security.  It's important
*					to note that the Appraisal is not guaranteed referential integrity with the MarketData unless the
*					tables are locked to prevent writing (to the MarketData).
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using MarkThree.Quasar;
using System;
using System.Collections;
using System.Data;
using System.Threading;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Diagnostics;

namespace MarkThree.Quasar
{

	/// <summary>
	/// A set of data structures that describe the hierarchical organization of an appraisal document.
	/// </summary>
	public class Appraisal : AppraisalSet
	{

		private DataModel.SchemeRow schemeRow;

		/// <summary>
		/// Constructs a hierarchical view of the appraisal including the active position and the positionTarget.
		/// </summary>
		/// <param name="AccountId"> identifier of the account.</param>
		/// <remarks>
		/// Table locks needed:
		///		Read:	Allocation
		///		Read:	Account
		///		Read:	Model
		///		Read:	PositionTarget
		///		Read:	ProposedOrder
		///		Read:	Object
		///		Read:	ObjectTree
		///		Read:	Order
		///		Read:	Scheme
		///		Read:	Security
		///		Read:	TaxLot
		/// </remarks>
		public Appraisal(DataModel.AccountRow accountRow, bool includeChildren)
		{

			// Clearing out the schema record will exclude any sector information in the outline.
			this.schemeRow = null;

			// Recursively build the document outline from the active position.
			BuildAppraisalSet(accountRow, includeChildren);

		}

		/// <summary>
		/// Constructs a hierarchical view of the appraisal including the active position and the positionTarget.
		/// </summary>
		/// <param name="AccountId">The account identifier.</param>
		/// <param name="ModelId">The model identifier.</param>
		/// <remarks>
		/// Table locks needed:
		///		Read:	Allocation
		///		Read:	Account
		///		Read:	Model
		///		Read:	PositionTarget
		///		Read:	ProposedOrder
		///		Read:	Object
		///		Read:	ObjectTree
		///		Read:	Order
		///		Read:	Scheme
		///		Read:	Security
		///		Read:	TaxLot
		/// </remarks>
		public Appraisal(DataModel.AccountRow accountRow, DataModel.ModelRow modelRow, bool includeChildren)
		{

			// Only the sector that are in this scheme are included in the outline.  If no scheme is found, there 
			// won't be any security classification data in the outline.
			int SchemeId = (modelRow.IsSchemeIdNull()) ? accountRow.SchemeId : modelRow.SchemeId;
			this.schemeRow = DataModel.Scheme.FindBySchemeId(SchemeId);

			// Add each of the SecurityIdentified in the model to the document outline.
			foreach (DataModel.PositionTargetRow positionTargetRows in modelRow.GetPositionTargetRows())
				BuildSecurity(accountRow.AccountId, positionTargetRows.SecurityId, positionTargetRows.PositionTypeCode);

			// Recursively build the document outline from the active position.
			BuildAppraisalSet(accountRow, includeChildren);

		}

		/// <summary>
		/// Constructs a hierarchical view of the appraisal including the active position and the positionTarget.
		/// </summary>
		/// <param name="accountRow">The parent account used to construct the outline.</param>
		/// <param name="schemeRow">The security classification scheme used to construct the outline.</param>
		/// <remarks>
		/// Table locks needed:
		///		Read:	Allocation
		///		Read:	Account
		///		Read:	Model
		///		Read:	PositionTarget
		///		Read:	ProposedOrder
		///		Read:	Object
		///		Read:	ObjectTree
		///		Read:	Order
		///		Read:	Scheme
		///		Read:	Security
		///		Read:	TaxLot
		/// </remarks>
		public Appraisal(DataModel.AccountRow accountRow, DataModel.SchemeRow schemeRow, bool includeChildren)
		{

			// The scheme record drives the building of the appraisal.
			this.schemeRow = schemeRow;

			// Recursively build the document outline from the active position.
			BuildAppraisalSet(accountRow, includeChildren);

		}

		/// <summary>
		/// Copies the outline of a security position from the data model into the AppraisalSet.
		/// </summary>
		/// <param name="AccountId">AccountId of the position to search through.</param>
		/// <param name="SecurityId">SecurityId of the position to search through.</param>
		/// <param name="PositionTypeCode">Position Type Code of the position to search through.</param>
		/// <remarks>
		/// Table locks needed:
		///		Read:	Security
		/// </remarks>
		protected void BuildSecurity(int AccountId, int SecurityId, int PositionTypeCode)
		{

			// See if the given security exists already in the table of security.  If this is the first time we've
			// encountered this security, we need to add it to the driver tables and then recursively search the hierarchy
			// and add every parent sector all the way up to the security classification scheme.  If we've run across it
			// already, we don't need to do anything related to the security.
			AppraisalSet.SecurityRow driverSecurity = this.Security.FindBySecurityId(SecurityId);
			if (driverSecurity == null)
			{

				// The AppraisalSet structure mirrors the structure of the MarketData.  We have a common 'object' table that
				// all object can use to navigate the tree structure.  When adding a new element to the data structure,
				// we need to add the object first to maintain integrity, then the security can be added.
				AppraisalSet.ObjectRow objectRow = this.Object.AddObjectRow(SecurityId);
				driverSecurity = this.Security.AddSecurityRow(objectRow);

				// Recursively search the hierarchy all the way up to the security classification scheme.  This will add
				// any elements of the hierarchy that are not already part of the outline.  Note that we first check to
				// see if the security belongs to the given security scheme before building the links.  This saves us from
				// having unused fragments of other security scheme in the outline.  This is useful when checking for
				// unclassified security.
				DataModel.SecurityRow securityRow = DataModel.Security.FindBySecurityId(SecurityId);
				if (IsObjectInScheme(securityRow.ObjectRow))
				{
					BuildSector(securityRow.ObjectRow);
					BuildScheme(securityRow.ObjectRow);
				}

			}

			// At this point, we know that the security and all the sector have been added to the outline.  Check to see 
			// if the position -- that is, the combination of security and whether we own it or have borrowed it -- exist
			// in the outline.  Add the combination if it doesn't.
			AppraisalSet.PositionRow positionRow = this.Position.FindBySecurityIdPositionTypeCode(SecurityId, PositionTypeCode);
			if (positionRow == null)
				positionRow = this.Position.AddPositionRow(driverSecurity, PositionTypeCode);
				
			// Finally, at the bottom of the list, is the account level information.  This operation insures that only distinct account/position combinations
			// appear in the document.  This is very useful if the same account were to appear in different groups.
			AppraisalSet.AccountRow driverAccount = this.Account.FindByAccountIdSecurityIdPositionTypeCode(AccountId, SecurityId, PositionTypeCode);
			if (driverAccount == null)
				driverAccount = this.Account.AddAccountRow(AccountId, SecurityId, PositionTypeCode);

		}
		
		/// <summary>
		/// Recurively copies the outline of the sector from the data model into the AppraisalSet.
		/// </summary>
		/// <param name="childSecurity">The end branch or leaf of the tree structure.  The sector outline is built from
		///  the security towards the classification scheme which is the root of the tree.</param>
		///  <remarks>
		///  Table locks needed:
		///		Read:	Object
		///		Read:	ObjectTree
		///		Read:	Sector
		///  </remarks>
		private void BuildSector(DataModel.ObjectRow childObject)
		{

			// The child row in the driver tables will be needed below if we have to add a relationship to the driver.
			AppraisalSet.ObjectRow childDriver = this.Object.FindByObjectId(childObject.ObjectId);

			// We are going to look at each of the parents of the given security to see if it's a member of the security
			// classification scheme.  Though there may be zero or many parents of any security or sector, there is only
			// one path to the classification scheme.  Remember, this method searches from the top of the hierarchy tree
			// down to the root
			foreach (DataModel.ObjectTreeRow objectTreeRow in
				childObject.GetObjectTreeRowsByFKObjectObjectTreeChildId())
				foreach (DataModel.SectorRow parentSector in
					objectTreeRow.ObjectRowByFKObjectObjectTreeParentId.GetSectorRows())
				{

					// Find the parent element in the driver tables.  If it doesn't exist, we're going to add it before
					// adding the parent/child relationship.
					AppraisalSet.ObjectRow parentDriver = this.Object.FindByObjectId(parentSector.SectorId);

					// Check to see if the parent already exists in the hierarchy.  If it doesn't exist and the parent is 
					// part of the classification scheme, we're going to add the parent and the relationship to the
					// outline.  If the parent does exist, then we'll check to see if the relationship should be added.
					if (parentDriver == null)
					{

						// Make sure that the parent security is part of the classification scheme of the document before
						// adding it.  This keeps fragments from other scheme from showing up in the driver tables.
						if (IsObjectInScheme(parentSector.ObjectRow))
						{

							// Add the parent sector to the outline.
							parentDriver = this.Object.AddObjectRow(parentSector.SectorId);
							this.Sector.AddSectorRow(parentDriver);

							// Add the tree branch into the outline.
							this.ObjectTree.AddObjectTreeRow(parentDriver, childDriver);

							// Recurse down the tree until every branch of this hierarchy leading up to this security is 
							// included in the document.
							BuildSector(parentSector.ObjectRow);
							BuildScheme(parentSector.ObjectRow);

						}

					}
					else
					{
				
						// If the sector already is part of the hierarchy, check to see if the relationship has is part of
						// the outline also.  If not, add it.
						if (this.ObjectTree.FindByParentIdChildId(parentSector.SectorId, childObject.ObjectId) == null)
							this.ObjectTree.AddObjectTreeRow(parentDriver, childDriver);

					}

				}

		}

		/// <summary>
		/// Copies the outline of the Scheme from the data model into the AppraisalSet.
		/// </summary>
		/// <param name="childSecurity">The end branch or leaf of the tree structure.  The scheme outline is built from
		///  the security towards the classification scheme which is the root of the tree.</param>
		///  <remarks>
		///  Table locks required:
		///		Read:	Object
		///		Read:	ObjectTree
		///		Read:	Scheme
		///  </remarks>
		private void BuildScheme(DataModel.ObjectRow childObject)
		{

			// The child row in the driver tables will be needed below if we have to add a relationship to the driver.
			AppraisalSet.ObjectRow childDriver = this.Object.FindByObjectId(childObject.ObjectId);

			// We are going to look at each of the parents of the given scheme to see if it's a member of the security
			// classification scheme.  Though there may be zero or many parents of any security or scheme, there is only
			// one path to the classification scheme.  Remember, this method searches from the children up to the parents.
			foreach (DataModel.ObjectTreeRow objectTreeRow in
				childObject.GetObjectTreeRowsByFKObjectObjectTreeChildId())
				foreach (DataModel.SchemeRow parentScheme in
					objectTreeRow.ObjectRowByFKObjectObjectTreeParentId.GetSchemeRows())
				{

					// Find the parent element in the driver tables.  If it doesn't exist, we're going to add it before
					// adding the parent/child relationship.
					AppraisalSet.ObjectRow parentDriver = this.Object.FindByObjectId(parentScheme.SchemeId);

					// Check to see if the parent already exists in the hierarchy.  If it doesn't exist and the parent is 
					// part of the classification scheme, we're going to add the parent and the relationship to the
					// outline.  If the parent does exist, then we'll check to see if the relationship should be added.
					if (parentDriver == null)
					{

						// Make sure that the parent security is part of the classification scheme of the document before
						// adding it.  This keeps fragments from other scheme from showing up in the driver tables.
						if (IsObjectInScheme(parentScheme.ObjectRow))
						{

							// Add the parent scheme to the outline.
							parentDriver = this.Object.AddObjectRow(parentScheme.SchemeId);
							this.Scheme.AddSchemeRow(parentDriver);

							// Add the tree branch into the outline.
							this.ObjectTree.AddObjectTreeRow(parentDriver, childDriver);

						}

					}
					else
					{
				
						// If the scheme already is part of the hierarchy, check to see if the relationship has is part of
						// the outline also.  If not, add it.
						if (this.ObjectTree.FindByParentIdChildId(parentScheme.SchemeId, childObject.ObjectId) == null)
							this.ObjectTree.AddObjectTreeRow(parentDriver, childDriver);

					}

				}

		}

		/// <summary>
		/// Recursively constructs a DataSet containing the outline of the document.
		/// </summary>
		/// <param name="accountRow">The current branch of the account hierarchy in the search.</param>
		/// <remarks>
		/// Locks needed:
		///		Read:	Account
		///		Read:	Allocation
		///		Read:	Object
		///		Read:	ObjectTree
		///		Read:	Order
		///		Read:	ProposedOrder
		///		Read:	Sector
		///		Read:	TaxLot
		/// </remarks>
		protected void BuildAppraisalSet(DataModel.AccountRow accountRow, bool includeChildren)
		{

			// Add all the distinct taxLot to the outline.
			foreach (DataModel.TaxLotRow taxLotRow in accountRow.GetTaxLotRows())
				BuildSecurity(taxLotRow.AccountId, taxLotRow.SecurityId, taxLotRow.PositionTypeCode);

			// Add all the distinct proposedOrder to the outline.
			foreach (DataModel.ProposedOrderRow proposedOrderRow in accountRow.GetProposedOrderRows())
				BuildSecurity(proposedOrderRow.AccountId, proposedOrderRow.SecurityId,
					proposedOrderRow.PositionTypeCode);

			// Add all the distinct order to the outline.
			foreach (DataModel.OrderRow SetPrice in accountRow.GetOrderRows())
				BuildSecurity(SetPrice.AccountId, SetPrice.SecurityId, SetPrice.PositionTypeCode);

			// Add all the distinct allocation to the outline.
			foreach (DataModel.AllocationRow allocationRow in accountRow.GetAllocationRows())
				BuildSecurity(allocationRow.AccountId, allocationRow.SecurityId, allocationRow.PositionTypeCode);
			
			// Now, if requested, recurse down into all the subaccount and add their distinct characteristics to the outline.
			if (includeChildren)
				foreach (DataModel.ObjectTreeRow objectTreeRow in
					accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
					foreach (DataModel.AccountRow childAccount in
						objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
						BuildAppraisalSet(childAccount, includeChildren);

		}

		/// <summary>
		/// Indicates if a given security is part of the document's security classification scheme.
		/// </summary>
		/// <param name="securityRow">A security to be tested.</param>
		/// <returns>True if the security belongs to the document's classification scheme.</returns>
		/// <remarks>
		/// Table locks needed:
		///		Read:	ObjectTree
		///		Read:	Object
		/// </remarks>
		private bool IsObjectInScheme(DataModel.ObjectRow objectRow)
		{

			// Some appraisals sets don't require security classification scheme as indicated by a lack of a driving 
			// scheme.  Returning false will cause the appraisal set to leave out that information.
			if (this.schemeRow == null)
				return false;

			// Recursively search the tree looking for the parent object.
			return MarkThree.Quasar.Object.IsParent(this.schemeRow.ObjectRow, objectRow);

		}

	}

}
