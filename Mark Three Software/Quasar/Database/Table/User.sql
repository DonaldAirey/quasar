/*************************************************************************************************************************
*
*	File:			User.sql
*	Description:	User Data Tables
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"User"
*	Description:	Primary repository of login data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "User"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('User', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "User"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "User" (
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Preferences" "Image" NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"SystemFolderId" "int" NULL ,
			"UserId" "int" NOT NULL ,
			"UserName" "nvarchar"(256) NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "User" WITH NOCHECK ADD 
			CONSTRAINT "PKUser" PRIMARY KEY  CLUSTERED 
			(
				"UserId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "User" ADD 
			CONSTRAINT "FKObjectUser" FOREIGN KEY 
			(
				"UserId"
			) REFERENCES "Object" (
				"ObjectId"
			),
			CONSTRAINT "FKSystemFolderUser" FOREIGN KEY 
			(
				"SystemFolderId"
			) REFERENCES "SystemFolder" (
				"SystemFolderId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "User" WITH NOCHECK ADD 
			CONSTRAINT "DefaultUserIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultUserIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''User''')

	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "User"
go
