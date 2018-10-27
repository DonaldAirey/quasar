/*************************************************************************************************************************
*
*	File:			RemoteConstraint.sql
*	Description:	This procedure remove a table, update the revisions, and remove foreign references.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Name:			RemoteConstraint
*	Description:	This procedure will remove any primary or foreign key constraints on a table.
*************************************************************************************************************************/

/* This function is installed before the revisions table.  The first time it'S installed it won't be able to use the
normal prolog and epilogs. */

/* Print the initial state for the log */

execute "ObjectProlog" "RemoveConstraint"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('RemoveConstraint', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Drop the Procedure if it already exists. */

	if exists (select * from sysobjects where id = object_id('RemoveConstraint') and objectproperty(id, 'IsProcedure') = 1)
		drop procedure "RemoveConstraint"

	if @@error <> 0 begin rollback transaction return end

	/* Create the procedure. */

	execute ('
		create procedure "RemoveConstraint"
			@name sysname
		with encryption
		as
		
			/* Variable Declaration */
			
			declare @parent_name sysname
			declare @constraint_name sysname
		
			/* TheIdea of this function is to remove the table, which means that any contraints that reference this table need to
			be dropped.  This cursor will use the ''Sysreferences'' table to locate those contraints.  Note that the foreign keys must
			be deleted before the primary keys (because the foreign keys must reference the primary key), so we have to drop them
			first.  The ''Priority'' constant allows us to order this statement do purge the foreign constraints first. */
		
			declare "PrimaryKeyCursor" cursor local
			for
				select quotename(sysobjects1.name, ''""""''), quotename(sysobjects2.name)
				from sysobjects sysobjects1, sysobjects sysobjects2, sysreferences
				where
					object_id(quotename(@name, ''""""'')) = sysreferences.rkeyid and
					sysreferences.fkeyid = sysobjects1.id and
					sysreferences.constid = sysobjects2.id
			for read only
		
			/* Open the cursor that checks for foreign keys that reference the table being dropped. */
			
			open "PrimaryKeyCursor"
		
			/* Cycle through all the contraints that reference the table being dropped and remove the contraint. */
			
			fetch next from "PrimaryKeyCursor" into @parent_name, @constraint_name
			while (@@FETCH_STATUS = 0)
			begin
				execute (''Alter table '' + @parent_name + '' drop constraint '' + @constraint_name)
				print convert(varchar, getdate(), 120) + ''Z Constraint: '' + @parent_name + ''.'' + @constraint_name + '', Dropped''
				fetch next from "PrimaryKeyCursor" into @parent_name, @constraint_name
			end
		
			/* Close down the cursor and free the resources. */
		
			close "PrimaryKeyCursor"
			deallocate "PrimaryKeyCursor"
		')
		
	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''RemoveConstraint''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final status for the log. */

execute "ObjectEpilog" "RemoveConstraint"
go
