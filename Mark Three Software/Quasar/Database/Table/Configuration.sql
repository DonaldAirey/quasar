/*************************************************************************************************************************
*
*	File:			Configuration.sql
*	Description:	This table is used to control which external codes are used when entering data.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Configuration"
*	Description:	Used to control which external externalIds are used when adding data from the outside world.  Any number
*					ofConfiguration can be created for a given combination of externalIds.
*************************************************************************************************************************/

/* Standard Prolog for Creating/Upgrading a table. */

execute "ObjectProlog" "Configuration"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Configuration', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Configuration"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Configuration" (
			"ConfigurationId" "nvarchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"ParameterId" "nvarchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"ColumnIndex" "int" NOT NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Configuration" WITH NOCHECK ADD 
			CONSTRAINT "PKConfiguration" PRIMARY KEY  CLUSTERED 
			(
				"ConfigurationId", "ParameterId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Configuration" WITH NOCHECK ADD 
			CONSTRAINT "DefaultConfigurationIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultConfigurationIsIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Configuration''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the finalConfiguration for the log. */

execute "ObjectEpilog" "Configuration"
go
