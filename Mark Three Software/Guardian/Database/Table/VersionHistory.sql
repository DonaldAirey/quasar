/*************************************************************************************************************************
*
*	File:			VersionHistory.sql
*	Description:	This module contains a list of table versionControl.  This table drives the scripts that create and
*					alter all the other tables in the Quasar database.
*					Donald Roy Airey  Copyright (C) 1999 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"VersionHistory"
*	Description:	This table keeps track of what upgrades were performed and indicates which is the current version.
*************************************************************************************************************************/

if not exists (select * from sysobjects where id = object_id(N'"ObjectProlog"') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	print convert(varchar, getdate(), 120) + 'Z Table: "VersionHistory", <undefined>'
else
	execute "ObjectProlog" "VersionHistory"
go

if not exists (select * from sysobjects where id = object_id(N'"VersionHistory"') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Create the table. */

	execute ('
		CREATE TABLE "VersionHistory" (
			"Label" "sysname" NOT NULL ,
			"Date" "Datetime" NULL ,
			"Active" "bit" NULL 
		) ON "PRIMARY"
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "VersionHistory" WITH NOCHECK ADD 
			CONSTRAINT "PKVersionHistory" PRIMARY KEY  CLUSTERED 
			(
				"Label"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Constant Data */
	
	execute ('Insert into "VersionHistory"("Label", "Date", "Active") values (''Version 1.0'', getdate(), 1)')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''VersionHistory''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

if not exists (select * from sysobjects where id = object_id(N'"ObjectEpilog"') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	print convert(varchar, getdate(), 120) + 'Z Table: "VersionHistory", <undefined>'
else
	execute "ObjectEpilog" "VersionHistory"
go
