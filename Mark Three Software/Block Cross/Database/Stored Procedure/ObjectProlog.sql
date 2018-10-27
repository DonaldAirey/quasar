/*************************************************************************************************************************
*
*	File:			ObjectProlog.sql
*	Description:	This procedure remove a table, update the ObjectProlog, and remove foreign references.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Name:			ObjectProlog
*	Description:	Used to display the revision before an upgrade script.
*************************************************************************************************************************/

/* The 'ObjectProlog'Code can't call itself if it hasn't be created yet.  This special case is used the first time
the procedure is installed.  After that, it calls itself. */

if not exists (select * from sysobjects where id = object_id('ObjectProlog') and objectproperty(id, 'IsProcedure') = 1)
	print convert(varchar, getdate(), 120) + 'Z Procedure: "ObjectProlog", Initial revision: <undefined>'
else
	execute "ObjectProlog" "ObjectProlog"
go

/* Revision 1 */

if not exists (select * from sysobjects where id = object_id('ObjectProlog') and objectproperty(id, 'IsProcedure') = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Create the procedure. */
	
	execute ('
		create procedure "ObjectProlog"
			@Name "sysname"
		with encryption
		as
		begin
		
			/* Declarations */
	
			declare @revision "varchar"(32)
			declare @error_text "varchar"(128)

			/* Make sure the object exists in the versionControl catalogs. */
		
			if not exists (select * from "VersionControl" where "Name" = @Name)
			begin
				print convert(varchar, getdate(), 120) + ''Z <Undefined>: '' + quotename(@Name, ''""""'') +
				  '' doesn''''t exist in the catalogs.''
				return
			end
			
			/* Get the current revision for the prolog output statement below. */
		
			select @revision = convert(varchar, "Revision") from "VersionControl" where "Name" = @Name
		
			/* Display the initial revision of the table for the log. */
		
			print convert(varchar, getdate(), 120) + ''Z Table: '' + quotename(@Name, ''""""'') + '', Initial revision: '' +
			  @revision

		end
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''ObjectProlog''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Simulate the actions of "ObjectEpilog" if it doesn't exist yet. */

if not exists (select * from sysobjects where id = object_id('ObjectEpilog') and objectproperty(id, 'IsProcedure') = 1)
	print convert(varchar, getdate(), 120) + 'Z Procedure: "ObjectProlog", Final revision: <undefined>'
else
	execute "ObjectEpilog" "ObjectProlog"
go
