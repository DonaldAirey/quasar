/*************************************************************************************************************************
*
*	File:			Needed.sql
*	Description:	This function is used to test whether a revision is needed for a particular upgrade.  It will test
*					the @revisionLevel parameter against the current revision and the target revision for the current
*					release.  If return code indicates if the block of coded identified by @revisionLevel should be run
*					to achieve the desired upgrade level.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Function:		revisionNeeded
*	Description:	This function will return a 1 if the block of code associated with the function should be executed
*					to achieve the upgrade levels indicated by the 'VersionHistory' and 'VersionTag' tables.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "RevisionNeeded"
go

/* Revision 1 */

if not exists (select * from sysobjects where id = object_id('RevisionNeeded') and objectproperty(id, 'IsTableFunction') = 0)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Create the procedure. */

	execute ('
		create function "RevisionNeeded" (@objectName sysname, @revisionLevel int)
		returns int
		with encryption
		as
		begin
		
			declare @currentRevision int
			declare @requiredRevision int
			
			/* If the versionControl table hasn''t been created, no new revisions can be created. */
		
			if not exists (select * from sysobjects where id = object_id(''VersionControl'') and objectproperty(id, ''IsUserTable'') = 1)
				return 0
			
			/* If the revision tag doesn''t exist, assume we build everything in the module.  This is so we can have a catalog of the tip
			version at the end of a build. */

			if not exists (select * from sysobjects where id = object_id(''VersionTag'') and objectproperty(id, ''IsUserTable'') = 1)
				return 1

			/* Get the current revision of this object. */
		
			select @currentRevision = "Revision" from "VersionControl" where "VersionControl"."Name" = @objectName
		
			/* Get the required revision level for this object. */
		
			select @requiredRevision = "Revision"
			from "VersionHistory", "VersionTag"
			where
				"VersionHistory"."Active" = 1 and
				"VersionHistory"."Label" = "VersionTag"."Label" and
				"VersionTag"."Name" = @objectName

			/* If the object hasn''t been tagged, then we assume that we''Re to install the lastest version available.  That is
			all revisions in a given module are necessary. */

			if @requiredRevision is null
				return 1
 
			/* If the table doesn''t exist in the system catalogs, it can''t be upgraded. */
		
			if @currentRevision is null
				return 0
		
			/* If the current revision is already higher than the level of the upgrade code, no upgrade is needed.  Also, if the
			level of the upgrade code is higher than the requested level for this version, we don''t want to run the upgrade. */
		
			if @currentRevision >= @revisionLevel or @revisionLevel > @requiredRevision
				return 0
		
			/* If we reached here, we''Ve determined that the upgrade code should be run. */
		
			return 1

		end
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''RevisionNeeded''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final status for the log. */

execute "ObjectEpilog" "RevisionNeeded"
go
