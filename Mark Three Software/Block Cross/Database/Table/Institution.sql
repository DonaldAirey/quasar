/*************************************************************************************************************************
*
*	File:			Institution.sql
*	Description:	Contains information about broker where securities are traded.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Institution"
*	Description:	Contains information about the place where securites are traded.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Institution"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Institution', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Institution"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Institution" (
			"InstitutionId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Institution" WITH NOCHECK ADD 
			CONSTRAINT "PKInstitution" PRIMARY KEY  CLUSTERED 
			(
				"InstitutionId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Institution" ADD 
			CONSTRAINT "FKObjectInstitution" FOREIGN KEY 
			(
				"InstitutionId"
			) REFERENCES "Object" (
				"ObjectId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Institution" WITH NOCHECK ADD 
			CONSTRAINT "DefaultInstitutionIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultInstitutionIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Institution''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Institution"
go
  