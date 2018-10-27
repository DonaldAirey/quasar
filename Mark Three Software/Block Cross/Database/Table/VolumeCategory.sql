/*************************************************************************************************************************
*
*	File:			VolumeCategory.sql
*	Description:	Major Asset category for Security
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"VolumeCategory"
*	Description:	Major Asset category for Security
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "VolumeCategory"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('VolumeCategory', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "VolumeCategory"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "VolumeCategory" (
			"Description" "nvarchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"HighVolumeRange" "decimal"(32, 7) NULL,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"LowVolumeRange" "decimal"(32, 7) NOT NULL,
			"Mnemonic" "nvarchar"(16) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"VolumeCategoryId" "int" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "VolumeCategory" WITH NOCHECK ADD 
			CONSTRAINT "PKVolumeCategory" PRIMARY KEY  CLUSTERED 
			(
				"VolumeCategoryId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "VolumeCategory" WITH NOCHECK ADD 
			CONSTRAINT "DefaultVolumeCategoryIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultVolumeCategoryIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''VolumeCategory''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "VolumeCategory"
go
 