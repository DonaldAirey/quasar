/*************************************************************************************************************************
*
*	File:			ObjectEpilog.sql
*	Description:	This procedure remove a table, update the versionControl, and remove foreign references.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Name:			ObjectEpilog
*	Description:	Used to print out information after an upgrade has been run.  Can be compared to the "Table prolog"
*					output to see where an upgrade failed.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "ObjectEpilog"
go

/* Revision 1 */

if not exists (select * from sysobjects where id = object_id('ObjectEpilog') and objectproperty(id, 'IsProcedure') = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Create the procedure. */
	
	execute ('
		create procedure "ObjectEpilog"
			@name sysname
		with encryption
		as
		
			declare @revision int
			declare @error int
			
			/* Store the errorCode from the last SQL statement.  The very next SQL statement executed will overwrite this value. */

			select @error = @@ERROR
		
			/* Make sure the object exists in the versionControl catalogs. */
		
			if not exists (select * from "VersionControl" where "Name" = @name)
			begin
				print convert(varchar, getdate(), 120) + ''Z <Undefined>: '' + quotename(@name, ''""""'') +
				  '' doesn''''t exist in the catalogs.''
				return
			end

			/* Get the last known revision for this object. */

			select @revision = "Revision" from "VersionControl" where "Name" = @name
		
			/* Display the result of the last upgrade script. */
		
			if @error = 0
				print convert(varchar, getdate(), 120) + ''Z Table: '' + quotename(@name, ''""""'') + '', Final revision: '' + convert(varchar, @revision)
			else
				print convert(varchar, getdate(), 120) + ''Z Table: '' + quotename(@name, ''""""'') + '', Error upgrading from revision: '' + convert(varchar, @revision)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''ObjectEpilog''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final status for the log. */

execute "ObjectEpilog" "ObjectEpilog"
go
