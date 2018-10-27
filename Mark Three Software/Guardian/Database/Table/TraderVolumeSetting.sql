/*************************************************************************************************************************
*
*	File:			TraderVolumeSetting.sql
*	Description:	Major Asset category for Security
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"TraderVolumeSetting"
*	Description:	Major Asset category for Security
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "TraderVolumeSetting"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('TraderVolumeSetting', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "TraderVolumeSetting"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "TraderVolumeSetting" (
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"AutoExecuteQuantity" "decimal"(32, 7) NOT NULL,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"ThresholdQuantity" "decimal"(32, 7) NOT NULL,
			"TraderVolumeSettingId" "int" NOT NULL ,
			"TraderId" "int" NOT NULL ,
			"VolumeCategoryId" "int" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "TraderVolumeSetting" WITH NOCHECK ADD 
			CONSTRAINT "PKTraderVolumeSetting" PRIMARY KEY  CLUSTERED 
			(
				"TraderVolumeSettingId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "TraderVolumeSetting" WITH NOCHECK ADD 
			CONSTRAINT "DefaultTraderVolumeSettingIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultTraderVolumeSettingIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''TraderVolumeSetting''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "TraderVolumeSetting"
go
 
