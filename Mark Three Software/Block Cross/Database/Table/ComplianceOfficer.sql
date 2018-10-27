/*************************************************************************************************************************
*
*	File:			ComplianceOfficer.sql
*	Description:	Users who can't trade but can observe trading activity.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"ComplianceOfficer"
*	Description:	Primary repository of login data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "ComplianceOfficer"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('ComplianceOfficer', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "ComplianceOfficer"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "ComplianceOfficer" (
			"ComplianceOfficerId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "ComplianceOfficer" WITH NOCHECK ADD 
			CONSTRAINT "PKComplianceOfficer" PRIMARY KEY  CLUSTERED 
			(
				"ComplianceOfficerId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "ComplianceOfficer" ADD 
			CONSTRAINT "FKObjectComplianceOfficer" FOREIGN KEY 
			(
				"ComplianceOfficerId"
			) REFERENCES "User" (
				"UserId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "ComplianceOfficer" WITH NOCHECK ADD 
			CONSTRAINT "DefaultComplianceOfficerIsArchived" DEFAULT (0) FOR "IsArchived" ,
			CONSTRAINT "DefaultComplianceOfficerIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''ComplianceOfficer''')

	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "ComplianceOfficer"
go
 