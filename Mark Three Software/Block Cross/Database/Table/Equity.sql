/*************************************************************************************************************************
*
*	File:			Equity.sql
*	Description:	This table holds security data that is particular to Equity Instruments.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Equity"
*	Description:	Holds security data that is particular to equity instruments.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Equity"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Equity', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Equity"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Equity" (
			"EquityId" "int" NOT NULL ,
			"ExchangeId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"IssuerId" "int" NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"SettlementId" "int" NOT NULL ,
			"SharesOutstanding" "decimal"(28, 7) NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Equity" WITH NOCHECK ADD 
			CONSTRAINT "PKEquity" PRIMARY KEY  CLUSTERED 
			(
				"EquityId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Equity" WITH NOCHECK ADD
			CONSTRAINT "DefaultEquityIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultEquityIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Key */

	execute ('
		ALTER TABLE "Equity" ADD 
			CONSTRAINT "FKSecurityEquityEquityId" FOREIGN KEY 
			(
				"EquityId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKSecurityEquitySettlementId" FOREIGN KEY 
			(
				"SettlementId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKExchangeEquity" FOREIGN KEY 
			(
				"ExchangeId"
			) REFERENCES "Exchange" (
				"ExchangeId"
			),
			CONSTRAINT "FKIssuerEquity" FOREIGN KEY 
			(
				"IssuerId"
			) REFERENCES "Issuer" (
				"IssuerId"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the revisions table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Equity''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Equity"
go
