/*************************************************************************************************************************
*
*	File:			Security.sql
*	Description:	Security Master Data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Security"
*	Description:	Security Master - Contains the most relevant information about a security.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Security"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Security', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Security"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Security" (
			"SecurityId" "int" NOT NULL ,
			"CountryId" "int" NOT NULL ,
			"SecurityTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"QuantityFactor" "decimal"(15, 4) NOT NULL ,
			"PriceFactor" "decimal"(15, 4) NOT NULL,
			"Symbol" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	/* Primary Key */

	execute ('
		ALTER TABLE "Security" WITH NOCHECK ADD 
			CONSTRAINT "PKSecurity" PRIMARY KEY  CLUSTERED 
			(
				"SecurityId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Key */

	execute ('
		ALTER TABLE "Security" ADD 
			CONSTRAINT "FKObjectSecurity" FOREIGN KEY 
			(
				"SecurityId"
			) REFERENCES "Object" (
				"ObjectId"
			),
			CONSTRAINT "FKSecurityTypeSecurity" FOREIGN KEY 
			(
				"SecurityTypeCode"
			) REFERENCES "SecurityType" (
				"SecurityTypeCode"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Security" WITH NOCHECK ADD 
			CONSTRAINT "DefaultsecurityIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultsecurityIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the revisions table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Security''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Security"
go
