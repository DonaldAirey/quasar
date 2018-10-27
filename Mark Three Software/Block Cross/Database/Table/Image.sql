/*************************************************************************************************************************
*
*	File:			Image.sql
*	Description:	Images, Bitmaps and Icons
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Image"
*	Description:	Image Master - Contains the most relevant information about a security.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Image"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Image', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Image"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Image" (
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Image" "varchar"(max) NOT NULL ,
			"ImageId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	/* Primary Key */

	execute ('
		ALTER TABLE "Image" WITH NOCHECK ADD 
			CONSTRAINT "PKImage" PRIMARY KEY  CLUSTERED 
			(
				"ImageId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Image" WITH NOCHECK ADD 
			CONSTRAINT "DefaultImageIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultImageIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the revisions table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Image''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Image"
go
