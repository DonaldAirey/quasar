/*************************************************************************************************************************
*
*	File:			versionHistory.sql
*	Description:	This procedure remove a table, update the versionControl, and remove foreign references.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Trigger:		versionHistoryInsert
*	Description:	Contains information about states and province within a country.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "VersionHistoryInsert"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('VersionHistoryInsert', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "VersionHistoryInsert"

	if @@error <> 0 begin rollback transaction return end

	/* Create the procedure. */

	execute ('
		create trigger "VersionHistoryInsert"
		on "VersionHistory" with encryption
		for insert, update
		as

			if update ("Active")
			begin
			
				update "VersionHistory"
				set "Active" = 0
				from "Inserted", "VersionHistory"
				where
					"Inserted"."Active" = 1 and
					"Inserted"."Label" <> "VersionHistory"."Label" and
					"VersionHistory"."Active" = 1
					
				if (select count(*) from "VersionHistory" where "Active" = 1) <> 1
				begin
					raiserror (''One and only one version can be active at a time'', 0, 1)
					rollback transaction
				end

			end
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''VersionHistoryInsert''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "VersionHistoryInsert"
go

/*************************************************************************************************************************
*	Trigger:		versionHistoryInsert
*	Description:	Contains information about states and province within a country.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "VersionHistoryDelete"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('VersionHistoryDelete', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "VersionHistoryDelete"

	if @@error <> 0 begin rollback transaction return end

	/* Create the procedure. */

	execute ('
		create trigger "VersionHistoryDelete"
		on "VersionHistory" with encryption
		for delete
		as

			if exists (select * from "Deleted" where "Deleted"."Active" = 1)
			begin
				raiserror (''The active version can''''t be deleted'', 0, 1)
				rollback transaction
			end
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''VersionHistoryDelete''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "VersionHistoryDelete"
go
