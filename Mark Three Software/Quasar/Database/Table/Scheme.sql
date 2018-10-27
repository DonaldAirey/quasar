/*************************************************************************************************************************
*
*	File:			Scheme.sql
*	Description:	Security schemes
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Scheme"
*	Description:	Contains information about country.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Scheme"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Scheme', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Scheme"
	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Scheme" (
			"SchemeId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Note" "nvarchar"(1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Scheme" WITH NOCHECK ADD 
			CONSTRAINT "PKScheme" PRIMARY KEY  CLUSTERED 
			(
				"SchemeId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Key */

	execute ('
		ALTER TABLE "Scheme" ADD 
			CONSTRAINT "FKObjectScheme" FOREIGN KEY 
			(
				"SchemeId"
			) REFERENCES "Object" (
				"ObjectId"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Scheme" WITH NOCHECK ADD 
			CONSTRAINT "DefaultschemeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultschemeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Scheme''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Scheme"
go
