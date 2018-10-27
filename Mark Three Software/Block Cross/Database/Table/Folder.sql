/*************************************************************************************************************************
*
*	File:			Folder.sql
*	Description:	Personal Folder for organizing Quasar Object.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Folder"
*	Description:	Repository of User Folder.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Folder"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Folder', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Folder"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Folder" (
			"FolderId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Folder" WITH NOCHECK ADD 
			CONSTRAINT "PKFolder" PRIMARY KEY  CLUSTERED 
			(
				"FolderId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Folder" WITH NOCHECK ADD 
			CONSTRAINT "DefaultFolderIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultFolderIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Folder" ADD 
			CONSTRAINT "FKObjectFolder" FOREIGN KEY 
			(
				"FolderId"
			) REFERENCES "Object" (
				"ObjectId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Folder''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Folder"
go
