/*************************************************************************************************************************
*
*	File:			VersionControl.sql
*	Description:	This module contains a list of object revisions.  It'S used to determine when an upgrade to an object
*					is needed.  It'S fashioned after a conventional versionControl system and implemented in the TSQL.
*					This table drives the scripts that create and alter all the other object in the Quasar database.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"VersionControl"
*	Description:	Contains information about states and revisions within a country.
*************************************************************************************************************************/

/* Since the 'ObjectProlog' logic needs the revision tables, we need a special case for when the 'VersionControl' table
hasn't been created yet. */

if not exists (select * from sysobjects where id = object_id(N'"ObjectProlog"') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	print convert(varchar, getdate(), 120) + 'Z Table: "VersionControl", <undefined>'
else
	execute "ObjectProlog" "VersionControl"
go

/* Revision 1 */

if not exists (select * from sysobjects where id = object_id(N'"VersionControl"') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Create the table */

	execute ('
		CREATE TABLE "VersionControl" (
			"Name" "varchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"Revision" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Clustered Index. */

	execute ('
		ALTER TABLE "VersionControl" WITH NOCHECK ADD 
			CONSTRAINT "PKVersionControl" PRIMARY KEY  CLUSTERED 
			(
				"Name"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Constant Data - This data is maintained by the script: 'Generate versionControl.sql' */

	execute ('
		insert into "VersionControl" ("Name", "Revision")
		select ''Account'', 0
		union select ''Algorithm'', 0
		union select ''AlgorithmType'', 0
		union select ''Allocation'', 0
		union select ''AssetType'', 0
		union select ''BlockOrder'', 0
		union select ''BlockOrder tree'', 0
		union select ''Blotter'', 0
		union select ''Broker'', 0
		union select ''Condition'', 0
		union select ''Configuration'', 0
		union select ''Country'', 0
		union select ''Currency'', 0
		union select ''Debt'', 0
		union select ''StylesheetType'', 0
		union select ''Equity'', 0
		union select ''Exchange'', 0
		union select ''ExchangeMap'', 0
		union select ''Execution'', 0
		union select ''Folder'', 0
		union select ''Holiday'', 0
		union select ''HolidayType'', 0
		union select ''Configuration'', 0
		union select ''Issuer'', 0
		union select ''IssuerType'', 0
		union select ''User'', 0
		union select ''LotHandling'', 0
		union select ''Model'', 0
		union select ''PositionTarget'', 0
		union select ''SectorTarget'', 0
		union select ''Object'', 0
		union select ''ObjectTree'', 0
		union select ''ObjectType'', 0
		union select ''Order'', 0
		union select ''OrderTree'', 0
		union select ''OrderType'', 0
		union select ''Placement'', 0
		union select ''Position'', 0
		union select ''PositionType'', 0
		union select ''Price'', 0
		union select ''Property'', 0
		union select ''ProposedOrder'', 0
		union select ''ProposedOrderTree'', 0
		union select ''Province'', 0
		union select ''Scheme'', 0
		union select ''Sector'', 0
		union select ''Security'', 0
		union select ''SecurityRoute'', 0
		union select ''SecurityType'', 0
		union select ''Status'', 0
		union select ''Stylesheet'', 0
		union select ''TaxLot'', 0
		union select ''TimeInForce'', 0
		union select ''TransactionType'', 0
		union select ''VersionControl'', 0
		union select ''VersionHistory'', 0
		union select ''VersionTag'', 0
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''VersionControl''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

if not exists (select * from sysobjects where id = object_id(N'"ObjectEpilog"') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	print convert(varchar, getdate(), 120) + 'Z Table: "VersionControl", <undefined>'
else
	execute "ObjectEpilog" "VersionControl"
go
