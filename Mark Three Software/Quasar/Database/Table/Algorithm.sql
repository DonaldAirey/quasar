/*************************************************************************************************************************
*
*	File:			Algorithm.sql
*	Description:	Constant table for a generic status for table elements.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Algorithm"
*	Description:	Storage place for algorithm.  These are Microsoft .Net classes that can be called dynamically based
*					on a value stored in the database.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Algorithm"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Algorithm', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Algorithm"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Algorithm" (
			"AlgorithmId" "int" NOT NULL ,
			"AlgorithmTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Name" "nvarchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Description" "nvarchar"(1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Assembly" "varchar"(1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Type" "varchar"(1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Method" "varchar"(1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Algorithm" WITH NOCHECK ADD 
			CONSTRAINT "PKAlgorithm" PRIMARY KEY  CLUSTERED 
			(
				"AlgorithmId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Algorithm" ADD 
			CONSTRAINT "FKAlgorithmTypeAlgorithm" FOREIGN KEY 
			(
				"AlgorithmTypeCode"
			) REFERENCES "AlgorithmType" (
				"AlgorithmTypeCode"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Algorithm" WITH NOCHECK ADD 
			CONSTRAINT "DefaultAlgorithmIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultAlgorithmIsIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Algorithm''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Algorithm"
go
