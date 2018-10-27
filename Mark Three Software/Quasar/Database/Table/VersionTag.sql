/*************************************************************************************************************************
*
*	File:			VersionTag.sql
*	Description:	This module contains a list of table revisions.  This table drives the scripts that create and
*					alter all the other tables in the Quasar database.
*					Donald Roy Airey  Copyright (C) 1999 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"VersionTag"
*	Description:	This table keeps track of what upgrades were performed and indicates which is the current version.
*************************************************************************************************************************/

/* Since the 'ObjectProlog' logic needs the revision tables, we need a special case for when the 'VersionControl' table
hasn't been created yet. */

if not exists (select * from sysobjects where id = object_id(N'"ObjectProlog"') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	print convert(varchar, getdate(), 120) + 'Z Table: "VersionTag", <undefined>'
else
	execute "ObjectProlog" "VersionTag"
go

/* Revision 1 */

if not exists (select * from sysobjects where id = object_id(N'"VersionTag"') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Create the table. */

	execute ('
		CREATE TABLE "VersionTag" (
			"Label" "sysname" NOT NULL ,
			"Name" "varchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"Revision" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "VersionTag" WITH NOCHECK ADD 
			CONSTRAINT "PKVersionTag" PRIMARY KEY  CLUSTERED 
			(
				"Label",
				"Name"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Rebuild Foreign Keys */

	execute ('
		ALTER TABLE "VersionTag" ADD 
			CONSTRAINT "FKVersionControlVersionTag" FOREIGN KEY 
			(
				"Name"
			) REFERENCES "VersionControl" (
				"Name"
			),
			CONSTRAINT "FKVersionHisotryVersionTag" FOREIGN KEY 
			(
				"Label"
			) REFERENCES "VersionHistory" (
				"Label"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* This is generated using the 'Label versions' query. */
/*
	execute ('
		insert into "VersionTag" ("Label", "Name", "Revision")
		select ''Version 1.0'', ''Get accountId'', 0
		union select ''Version 1.0'', ''Get algorithmId'', 0
		union select ''Version 1.0'', ''Get assetTypeCode'', 0
		union select ''Version 1.0'', ''Get blockingCode'', 0
		union select ''Version 1.0'', ''Get blotterId'', 0
		union select ''Version 1.0'', ''Get childId'', 0
		union select ''Version 1.0'', ''Get conditionCode'', 0
		union select ''Version 1.0'', ''Get countryId'', 0
		union select ''Version 1.0'', ''Get currencyId'', 0
		union select ''Version 1.0'', ''Get stylesheetTypeCode'', 0
		union select ''Version 1.0'', ''Get exchangeId'', 0
		union select ''Version 1.0'', ''Get folderId'', 0
		union select ''Version 1.0'', ''Get issuerId'', 0
		union select ''Version 1.0'', ''Get lotHandlingCode'', 0
		union select ''Version 1.0'', ''Get modelId'', 0
		union select ''Version 1.0'', ''Get objectId'', 0
		union select ''Version 1.0'', ''Get objectType'', 0
		union select ''Version 1.0'', ''Get orderTypeCode'', 0
		union select ''Version 1.0'', ''Get parentId'', 0
		union select ''Version 1.0'', ''Get positionTypeCode'', 0
		union select ''Version 1.0'', ''Get propertyCode'', 0
		union select ''Version 1.0'', ''Get provinceId'', 0
		union select ''Version 1.0'', ''Get schemeId'', 0
		union select ''Version 1.0'', ''Get sectorId'', 0
		union select ''Version 1.0'', ''Get securityId'', 0
		union select ''Version 1.0'', ''Get securityTypeCode'', 0
		union select ''Version 1.0'', ''Get taxLotId'', 0
		union select ''Version 1.0'', ''Get timeInForceCode'', 0
		union select ''Version 1.0'', ''Get transactionTypeCode'', 0
		union select ''Version 1.0'', ''GetstylesheetId'', 0
		union select ''Version 1.0'', ''RevisionNeeded'', 1
		union select ''Version 1.0'', ''AddBlockOrderTree'', 1
		union select ''Version 1.0'', ''AddBlotter'', 1
		union select ''Version 1.0'', ''AddFolder'', 1
		union select ''Version 1.0'', ''AddUser'', 1
		union select ''Version 1.0'', ''AddModel'', 0
		union select ''Version 1.0'', ''AddObjectTree'', 1
		union select ''Version 1.0'', ''ArchiveObject'', 1
		union select ''Version 1.0'', ''ArchiveSecurity'', 1
		union select ''Version 1.0'', ''DeleteBlockOrder'', 1
		union select ''Version 1.0'', ''DeleteHoliday'', 1
		union select ''Version 1.0'', ''DeleteSectorTarget'', 1
		union select ''Version 1.0'', ''DeletePositionTarget'', 1
		union select ''Version 1.0'', ''DeleteObjectTree'', 1
		union select ''Version 1.0'', ''DeleteOrder'', 1
		union select ''Version 1.0'', ''DeleteOrderTree'', 1
		union select ''Version 1.0'', ''DeleteProposedOrder'', 1
		union select ''Version 1.0'', ''DeleteProposedOrderTree'', 1
		union select ''Version 1.0'', ''DeleteStylesheet'', 1
		union select ''Version 1.0'', ''DeleteTaxLotExternal'', 1
		union select ''Version 1.0'', ''GetAccount'', 1
		union select ''Version 1.0'', ''GetAlgorithm'', 1
		union select ''Version 1.0'', ''GetAllocation'', 1
		union select ''Version 1.0'', ''GetBlockings'', 0
		union select ''Version 1.0'', ''GetBlockOrder'', 1
		union select ''Version 1.0'', ''GetBlockOrderTree'', 1
		union select ''Version 1.0'', ''GetBlotter'', 1
		union select ''Version 1.0'', ''GetBroker'', 1
		union select ''Version 1.0'', ''GetCountry'', 1
		union select ''Version 1.0'', ''GetCurrency'', 1
		union select ''Version 1.0'', ''GetDebt'', 1
		union select ''Version 1.0'', ''GetStylesheetType'', 1
		union select ''Version 1.0'', ''GetEquity'', 1
		union select ''Version 1.0'', ''GetExecution'', 1
		union select ''Version 1.0'', ''GetFolder'', 1
		union select ''Version 1.0'', ''GetHoliday'', 1
		union select ''Version 1.0'', ''GetHolidayType'', 1
		union select ''Version 1.0'', ''GetUser'', 1
		union select ''Version 1.0'', ''GetUserId'', 1
		union select ''Version 1.0'', ''GetModel'', 0
		union select ''Version 1.0'', ''GetPositionTarget'', 0
		union select ''Version 1.0'', ''GetSectorTarget'', 1
		union select ''Version 1.0'', ''GetObject'', 1
		union select ''Version 1.0'', ''GetObjectTree'', 1
		union select ''Version 1.0'', ''GetObjectType'', 1
		union select ''Version 1.0'', ''GetOrder'', 1
		union select ''Version 1.0'', ''GetOrderTree'', 1
		union select ''Version 1.0'', ''GetOrderType'', 1
		union select ''Version 1.0'', ''GetPlacement'', 1
		union select ''Version 1.0'', ''GetPrice'', 1
		union select ''Version 1.0'', ''GetProposedOrder'', 1
		union select ''Version 1.0'', ''GetProposedOrderTree'', 1
		union select ''Version 1.0'', ''GetProvince'', 1
		union select ''Version 1.0'', ''GetTransactionType'', 0
		union select ''Version 1.0'', ''GetScheme'', 1
		union select ''Version 1.0'', ''GetSector'', 1
		union select ''Version 1.0'', ''GetSecurity'', 1
		union select ''Version 1.0'', ''GetSecurityRoute'', 1
		union select ''Version 1.0'', ''GetSecurityType'', 1
		union select ''Version 1.0'', ''GetStatus'', 1
		union select ''Version 1.0'', ''GetStylesheet'', 1
		union select ''Version 1.0'', ''GetTaxLot'', 1
		union select ''Version 1.0'', ''GetTimeInForce'', 1
		union select ''Version 1.0'', ''InsertAccountExternal'', 1
		union select ''Version 1.0'', ''InsertBlockOrder'', 1
		union select ''Version 1.0'', ''InsertBlotterExternal'', 1
		union select ''Version 1.0'', ''InsertBroker'', 1
		union select ''Version 1.0'', ''InsertBrokerExternal'', 0
		union select ''Version 1.0'', ''InsertCurrencyExternal'', 1
		union select ''Version 1.0'', ''InsertDebtExternal'', 1
		union select ''Version 1.0'', ''InsertEquityExternal'', 1
		union select ''Version 1.0'', ''InsertExecution'', 1
		union select ''Version 1.0'', ''InsertFolderExternal'', 1
		union select ''Version 1.0'', ''InsertHoliday'', 1
		union select ''Version 1.0'', ''InsertHolidayExternal'', 1
		union select ''Version 1.0'', ''InsertUserExternal'', 1
		union select ''Version 1.0'', ''InsertModelExternal'', 1
		union select ''Version 1.0'', ''InsertSectorTarget'', 1
		union select ''Version 1.0'', ''InsertPositionTarget'', 1
		union select ''Version 1.0'', ''InsertObjectTreeExternal'', 1
		union select ''Version 1.0'', ''InsertOrder'', 1
		union select ''Version 1.0'', ''InsertOrderTree'', 1
		union select ''Version 1.0'', ''InsertProposedOrder'', 1
		union select ''Version 1.0'', ''InsertProposedOrderExternal'', 1
		union select ''Version 1.0'', ''InsertProposedOrderTree'', 1
		union select ''Version 1.0'', ''InsertSchemeExternal'', 1
		union select ''Version 1.0'', ''InsertSectorExternal'', 1
		union select ''Version 1.0'', ''InsertStylesheet'', 1
		union select ''Version 1.0'', ''InsertStylesheetExternal'', 0
		union select ''Version 1.0'', ''InsertTaxLotExternal'', 1
		union select ''Version 1.0'', ''MoveObject'', 1
		union select ''Version 1.0'', ''ObjectEpilog'', 1
		union select ''Version 1.0'', ''ObjectProlog'', 1
		union select ''Version 1.0'', ''RemoveConstraint'', 1
		union select ''Version 1.0'', ''RemoveObject'', 1
		union select ''Version 1.0'', ''RenameObject'', 1
		union select ''Version 1.0'', ''SetAccountProperty'', 1
		union select ''Version 1.0'', ''SetAccountPropertyExternal'', 1
		union select ''Version 1.0'', ''SetBlotterProperty'', 1
		union select ''Version 1.0'', ''SetBlotterPropertyExternal'', 1
		union select ''Version 1.0'', ''SetFolderProperty'', 1
		union select ''Version 1.0'', ''SetFolderPropertyExternal'', 1
		union select ''Version 1.0'', ''SetModelProperty'', 1
		union select ''Version 1.0'', ''SetModelPropertyExternal'', 1
		union select ''Version 1.0'', ''UpdateHoliday'', 1
		union select ''Version 1.0'', ''UpdateSectorTarget'', 1
		union select ''Version 1.0'', ''UpdateSectorTargetExternal'', 1
		union select ''Version 1.0'', ''UpdatePositionTarget'', 1
		union select ''Version 1.0'', ''UpdatePositionTargetExternal'', 1
		union select ''Version 1.0'', ''UpdatePriceExternal'', 1
		union select ''Version 1.0'', ''UpdateProposedOrder'', 1
		union select ''Version 1.0'', ''UpdateStylesheet'', 0
		union select ''Version 1.0'', ''Object update'', 1
		union select ''Version 1.0'', ''Security update'', 1
		union select ''Version 1.0'', ''VersionHistory delete'', 1
		union select ''Version 1.0'', ''VersionHistory insert'', 0
		union select ''Version 1.0'', ''Account'', 1
		union select ''Version 1.0'', ''Algorithm'', 1
		union select ''Version 1.0'', ''Allocation'', 1
		union select ''Version 1.0'', ''AssetType'', 1
		union select ''Version 1.0'', ''Blockings'', 0
		union select ''Version 1.0'', ''BlockOrder'', 1
		union select ''Version 1.0'', ''BlockOrderTree'', 0
		union select ''Version 1.0'', ''Blotter'', 1
		union select ''Version 1.0'', ''Broker'', 1
		union select ''Version 1.0'', ''Condition'', 1
		union select ''Version 1.0'', ''Configuration'', 1
		union select ''Version 1.0'', ''Country'', 1
		union select ''Version 1.0'', ''Currency'', 1
		union select ''Version 1.0'', ''Debt'', 1
		union select ''Version 1.0'', ''StylesheetType'', 1
		union select ''Version 1.0'', ''Equity'', 1
		union select ''Version 1.0'', ''Exchange'', 1
		union select ''Version 1.0'', ''ExchangeMap'', 1
		union select ''Version 1.0'', ''Execution'', 1
		union select ''Version 1.0'', ''Folder'', 1
		union select ''Version 1.0'', ''Holiday'', 1
		union select ''Version 1.0'', ''HolidayType'', 1
		union select ''Version 1.0'', ''Issuer'', 1
		union select ''Version 1.0'', ''IssuerType'', 1
		union select ''Version 1.0'', ''User'', 1
		union select ''Version 1.0'', ''LotHandling'', 1
		union select ''Version 1.0'', ''Model'', 1
		union select ''Version 1.0'', ''PositionTarget'', 1
		union select ''Version 1.0'', ''SectorTarget'', 1
		union select ''Version 1.0'', ''Object'', 1
		union select ''Version 1.0'', ''ObjectTree'', 1
		union select ''Version 1.0'', ''ObjectType'', 0
		union select ''Version 1.0'', ''Order'', 1
		union select ''Version 1.0'', ''OrderTree'', 1
		union select ''Version 1.0'', ''OrderType'', 1
		union select ''Version 1.0'', ''Placement'', 1
		union select ''Version 1.0'', ''Position'', 1
		union select ''Version 1.0'', ''Price'', 1
		union select ''Version 1.0'', ''Property'', 1
		union select ''Version 1.0'', ''ProposedOrder'', 1
		union select ''Version 1.0'', ''ProposedOrderTree'', 1
		union select ''Version 1.0'', ''Province'', 1
		union select ''Version 1.0'', ''Scheme'', 1
		union select ''Version 1.0'', ''Sector'', 1
		union select ''Version 1.0'', ''Security'', 1
		union select ''Version 1.0'', ''SecurityRoute'', 1
		union select ''Version 1.0'', ''SecurityType'', 1
		union select ''Version 1.0'', ''Status'', 1
		union select ''Version 1.0'', ''Stylesheet'', 1
		union select ''Version 1.0'', ''TaxLot'', 1
		union select ''Version 1.0'', ''TimeInForce'', 1
		union select ''Version 1.0'', ''TransactionType'', 1
		union select ''Version 1.0'', ''VersionControl'', 1
		union select ''Version 1.0'', ''VersionHistory'', 1
		union select ''Version 1.0'', ''VersionTag'', 1
	')
*/		
	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''VersionTag''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

if not exists (select * from sysobjects where id = object_id(N'"ObjectEpilog"') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	print convert(varchar, getdate(), 120) + 'Z Table: "VersionTag", <undefined>'
else
	execute "ObjectEpilog" "VersionTag"
go
