/*************************************************************************************************************************
*
*	File:			Stylesheet.sql
*	Description:	Stylesheets are used to transform XML data into some presentation layer.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Stylesheet"
*	Description:	Stylesheets are used to transform XML data into some presentation layer.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Stylesheet"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Stylesheet', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Stylesheet"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Stylesheet" (
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Name" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"StylesheetId" "int" NOT NULL ,
			"StylesheetTypeCode" "int" NOT NULL ,
			"Text" "varchar"(max) NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Stylesheet" WITH NOCHECK ADD 
			CONSTRAINT "PKStylesheet" PRIMARY KEY  CLUSTERED 
			(
				"StylesheetId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Stylesheet" ADD 
			CONSTRAINT "FKStylesheetTypeStylesheet" FOREIGN KEY 
			(
				"StylesheetTypeCode"
			) REFERENCES "StylesheetType" (
				"StylesheetTypeCode"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Stylesheet" WITH NOCHECK ADD 
			CONSTRAINT "DefaultStylesheetIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultStylesheetIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Stylesheet''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Stylesheet"
go
