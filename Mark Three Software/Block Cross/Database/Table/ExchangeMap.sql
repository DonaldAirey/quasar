/*************************************************************************************************************************
*
*	File:			ExchangeMap.sql
*	Description:	Maps security to the exchange they are trade on.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"ExchangeMap"
*	Description:	Maps security to exchange where they are traded.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "ExchangeMap"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('ExchangeMap', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "ExchangeMap"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "ExchangeMap" (
			"ExchangeMapId" "int" NOT NULL ,
			"ExchangeId" "int" NOT NULL ,
			"SecurityId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "ExchangeMap" WITH NOCHECK ADD 
			CONSTRAINT "PKExchangeMap" PRIMARY KEY  CLUSTERED 
			(
				"ExchangeMapId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "ExchangeMap" ADD 
			CONSTRAINT "FKExchangeExchangeMap" FOREIGN KEY 
			(
				"ExchangeId"
			) REFERENCES "Exchange" (
				"ExchangeId"
			),
			CONSTRAINT "FKSecurityExchangeMap" FOREIGN KEY 
			(
				"SecurityId"
			) REFERENCES "Security" (
				"SecurityId"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "ExchangeMap" WITH NOCHECK ADD 
			CONSTRAINT "DefaultExchangeMapIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultExchangeMapIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the revisions table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''ExchangeMap''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "ExchangeMap"
go
