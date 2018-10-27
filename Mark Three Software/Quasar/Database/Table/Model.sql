/*************************************************************************************************************************
*
*	File:			Model.sql
*	Description:	Primary source of information about sub-model, model and model groups.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Model"
*	Description:	Primary repository of model data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Model"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Model', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Model"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Model" (
			"ModelId" "int" NOT NULL ,
			"AlgorithmId" "int" NOT NULL ,
			"SchemeId" "int" NULL ,
			"ModelTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Temporary" "bit" NOT NULL ,
			"SecuritySelf" "bit" NOT NULL ,
			"SectorSelf" "bit" NOT NULL ,
			"EquityRounding" "decimal"(15, 4) NOT NULL ,
			"DebtRounding" "decimal"(15, 4) NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Model" WITH NOCHECK ADD 
			CONSTRAINT "PKModel" PRIMARY KEY  CLUSTERED 
			(
				"ModelId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Key */

	execute ('
		ALTER TABLE "Model" ADD 
			CONSTRAINT "FKAlgorithmModel" FOREIGN KEY 
			(
				"AlgorithmId"
			) REFERENCES "Algorithm" (
				"AlgorithmId"
			),
			CONSTRAINT "FKSchemeModel" FOREIGN KEY 
			(
				"SchemeId"
			) REFERENCES "Scheme" (
				"SchemeId"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Model" WITH NOCHECK ADD 
			CONSTRAINT "DefaultModelIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultModelIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Model''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Model"
go
