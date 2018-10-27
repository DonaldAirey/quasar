/*************************************************************************************************************************
*
*	File:			StylesheetType.sql
*	Description:	Categories for Stylesheets.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"StylesheetType"
*	Description:	Categories for Stylesheets.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "StylesheetType"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('StylesheetType', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "StylesheetType"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "StylesheetType" (
			"StylesheetTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Name" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Description" "nvarchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "StylesheetType" WITH NOCHECK ADD 
			CONSTRAINT "PKStylesheetType" PRIMARY KEY  CLUSTERED 
			(
				"StylesheetTypeCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "StylesheetType" WITH NOCHECK ADD 
			CONSTRAINT "DefaultStylesheetTypeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultStylesheetTypeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''StylesheetType''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "StylesheetType"
go
