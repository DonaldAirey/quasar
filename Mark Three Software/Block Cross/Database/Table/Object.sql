/*************************************************************************************************************************
*
*	File:			Object.sql
*	Description:	The common part of all objects in the database.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Object"
*	Description:	The common part of all objects in the database
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Object"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Object', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Object"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Object" (
			"Description" "nvarchar"(2048) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId4" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId5" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId6" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId7" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"GroupPermission" "int" NOT NULL ,
			"Hidden" "bit" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Name" "nvarchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ObjectId" "int" NOT NULL ,
			"Owner" "int" NULL ,
			"OwnerPermission" "int" NOT NULL ,
			"ReadOnly" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"TypeCode" "varchar"(128) NOT NULL ,
			"WorldPermission" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Object" WITH NOCHECK ADD 
			CONSTRAINT "PKObject" PRIMARY KEY  CLUSTERED 
			(
				"ObjectId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Object" ADD 
			CONSTRAINT "FKTypeObject" FOREIGN KEY 
			(
				"TypeCode"
			) REFERENCES "Type" (
				"TypeCode"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Object" WITH NOCHECK ADD
			CONSTRAINT "DefaultObjectIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultObjectIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Object''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Object"
go
