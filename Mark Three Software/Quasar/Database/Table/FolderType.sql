/*************************************************************************************************************************
*
*	File:			FolderType.sql
*	Description:	Categories for Folders
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"FolderType"
*	Description:	This table is used to qualify various the various object used in this database.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "FolderType"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('FolderType', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "FolderType"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "FolderType" (
			"FolderTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Description" "nvarchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "FolderType" WITH NOCHECK ADD 
			CONSTRAINT "PKFolderType" PRIMARY KEY  CLUSTERED 
			(
				"FolderTypeCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "FolderType" WITH NOCHECK ADD 
			CONSTRAINT "DefaultFolderTypeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultFolderTypeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Types''')
	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "FolderType"
go
