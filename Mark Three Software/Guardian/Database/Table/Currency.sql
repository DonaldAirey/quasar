/*************************************************************************************************************************
*
*	File:			Currency.sql
*	Description:	"Currency" and "Currency" tables.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Currency"
*	Description:	Security Master - Contains the most relevant information about a security.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Currency"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Currency', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Currency"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Currency" (
			"CurrencyId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Currency" WITH NOCHECK ADD 
			CONSTRAINT "PKCurrency" PRIMARY KEY  CLUSTERED 
			(
				"CurrencyId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Key */

	execute ('
		ALTER TABLE "Currency" ADD 
			CONSTRAINT "FKSecurityCurrency" FOREIGN KEY 
			(
				"CurrencyId"
			) REFERENCES "Security" (
				"SecurityId"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Currency" WITH NOCHECK ADD
			CONSTRAINT "DefaultCurrencyIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultCurrencyIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the revisions table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Currency''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Currency"
go
