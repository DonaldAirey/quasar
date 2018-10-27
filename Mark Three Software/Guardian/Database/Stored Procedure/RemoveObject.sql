/*************************************************************************************************************************
*
*	File:			RemoveObject.sql
*	Description:	This procedure remove a table, update the revisions, and remove foreign references.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Name:			RemoveObject
*	Description:	This procedure can be used to remove any object in the database.  It will reset the revision counter
*					after the object is dropped.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "RemoveObject"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('RemoveObject', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Drop the Procedure if it already exists. */

	if exists (select * from sysobjects where id = object_id('RemoveObject') and objectproperty(id, 'IsProcedure') = 1)
		drop procedure "RemoveObject"

	if @@error <> 0 begin rollback transaction return end

	/* Create the procedure. */

	execute ('
		create procedure "RemoveObject"
			@name sysname
		with encryption
		as
		
			declare @type "varchar"(32)
			declare @type_text "varchar"(32)
		
			/* Translate the ''Sysobjects'' type into a readable string for the prolog ''Print'' statement. */
		
			select @type_text = 
				case type
					when ''U'' then ''Table''
					when ''P'' then ''Procedure''
					when ''TR'' then ''Trigger''
					when ''FN'' then ''Function''
				end,
				@type = 
				case type
					when ''U'' then ''Table''
					when ''P'' then ''Procedure''
					when ''TR'' then ''Trigger''
					when ''FN'' then ''Function''
				end
			from sysobjects
			where "name" = @name
		
			/* If a table is being removed, drop any constraints before dropping the table. */
		
			if exists (select * from sysobjects where id = object_id(@name) and objectproperty(id, ''IsUserTable'') = 1)
				execute "RemoveConstraint" @name
			
			/* Drop the parent table now that all the dependancies have been purged. */
		
			if exists (select * from sysobjects where id = object_id(@name))
			begin
					execute (''Drop '' + @type + ''"'' + @name + ''"'')
					print convert(varchar, getdate(), 120) + ''Z '' + @type_text + '': "'' + @name + ''", Dropped''
			end
		
			/* Insure that the versionControl reflects the fact that the table was removed. */
		
			update "VersionControl" set "Revision" = 0 where "Name" = @name
				
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''RemoveObject''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final status for the log. */

execute "ObjectEpilog" "RemoveObject"
go
