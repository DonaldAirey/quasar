/*************************************************************************************************************************
*
*	File:			Violation.sql
*	Description:	Tax lots are aggregated into violation which contain the sum total of shares and the average cost of
*					a given security for a given account.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Violation"
*	Description:	Contains the aggregate information for a given security, account and positionType.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Violation"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Violation', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Violation"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Violation" (
			"ViolationId" "int" NOT NULL ,
			"RestrictionId" "int" NOT NULL ,
			"AccountId" "int" NOT NULL ,
			"SecurityId" "int" NOT NULL ,
			"PositionTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL,
			"Description" "nvarchar"(2048) NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Clustered Index. */

	execute ('
		ALTER TABLE "Violation" WITH NOCHECK ADD 
			CONSTRAINT "PKViolation" PRIMARY KEY  CLUSTERED 
			(
				"ViolationId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Violation" ADD 
			CONSTRAINT "FKRestrictionViolation" FOREIGN KEY 
			(
				"RestrictionId"
			) REFERENCES "Restriction" (
				"RestrictionId"
			),
			CONSTRAINT "FKAccountViolation" FOREIGN KEY 
			(
				"AccountId"
			) REFERENCES "Account" (
				"AccountId"
			),
			CONSTRAINT "FKSecurityViolation" FOREIGN KEY 
			(
				"SecurityId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKPositionTypeViolation" FOREIGN KEY 
			(
				"PositionTypeCode"
			) REFERENCES "PositionType" (
				"PositionTypeCode"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Violation" WITH NOCHECK ADD 
			CONSTRAINT "DefaultViolationIsIsArchived" DEFAULT (0) FOR "IsArchived"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Violation''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end

/* Print the final state for the log. */

execute "ObjectEpilog" "Violation"
go
