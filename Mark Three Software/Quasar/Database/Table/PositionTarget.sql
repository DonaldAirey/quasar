/*************************************************************************************************************************
*
*	File:			PositionTarget.sql
*	Description:	Table containing the percentages of a given security in a model.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"PositionTarget"
*	Description:	Primary repository of positionTarget data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "PositionTarget"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('PositionTarget', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "PositionTarget"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "PositionTarget"
		(
			"ModelId" "int" NOT NULL ,
			"SecurityId" "int" NOT NULL ,
			"PositionTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Percent" "decimal"(15, 4) NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "PositionTarget" WITH NOCHECK ADD 
			CONSTRAINT "PKPositionTarget" PRIMARY KEY  CLUSTERED 
			(
				"ModelId",
				"SecurityId",
				"PositionTypeCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "PositionTarget" ADD 
			CONSTRAINT "FKModelPositionTarget" FOREIGN KEY 
			(
				"ModelId"
			) REFERENCES "Model" (
				"ModelId"
			),
			CONSTRAINT "FKSecurityPositionTarget" FOREIGN KEY 
			(
				"SecurityId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKPositionTypePositionTarget" FOREIGN KEY 
			(
				"PositionTypeCode"
			) REFERENCES "PositionType" (
				"PositionTypeCode"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "PositionTarget" WITH NOCHECK ADD 
			CONSTRAINT "DefaultPositionTargetIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultPositionTargetIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''PositionTarget''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "PositionTarget"
go
