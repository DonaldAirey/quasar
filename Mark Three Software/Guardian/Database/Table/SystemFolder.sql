/*************************************************************************************************************************
*
*	File:			SystemFolder.sql
*	Description:	Personal SystemFolder for organizing Quasar Object.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"SystemFolder"
*	Description:	Repository of User SystemFolder.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "SystemFolder"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('SystemFolder', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "SystemFolder"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "SystemFolder" (
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"SystemFolderId" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "SystemFolder" WITH NOCHECK ADD 
			CONSTRAINT "PKSystemFolder" PRIMARY KEY  CLUSTERED 
			(
				"SystemFolderId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "SystemFolder" WITH NOCHECK ADD 
			CONSTRAINT "DefaultSystemFolderIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultSystemFolderIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "SystemFolder" ADD 
			CONSTRAINT "FKFolderSystemFolder" FOREIGN KEY 
			(
				"SystemFolderId"
			) REFERENCES "Folder" (
				"FolderId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''SystemFolder''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "SystemFolder"
go
