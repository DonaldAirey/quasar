/*************************************************************************************************************************
*
*	File:			AlgorithmType.sql
*	Description:	Major Asset category for Security
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"AlgorithmType"
*	Description:	Major Asset category for Security
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "AlgorithmType"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('AlgorithmType', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "AlgorithmType"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "AlgorithmType" (
			"AlgorithmTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Name" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Description" "nvarchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "AlgorithmType" WITH NOCHECK ADD 
			CONSTRAINT "PKAlgorithmType" PRIMARY KEY  CLUSTERED 
			(
				"AlgorithmTypeCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "AlgorithmType" WITH NOCHECK ADD 
			CONSTRAINT "DefaultAlgorithmTypeIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultAlgorithmTypeIsIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''AlgorithmType''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "AlgorithmType"
go
