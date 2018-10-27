/*************************************************************************************************************************
*
*	File:			Position.sql
*	Description:	Tax lot are aggregated into position which contain the sum total of shares and the average cost of
*					a given security for a given account.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Position"
*	Description:	Contains the aggregate information for a given security, account and position.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Position"
go

/* Version 0.1 */

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Position', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Position"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Position" (
			"AccountId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"PositionTypeCode" "int" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"SecurityId" "int" NOT NULL ,
			"UserData0" "decimal"(28, 7) NULL ,
			"UserData1" "decimal"(28, 7) NULL ,
			"UserData2" "decimal"(28, 7) NULL ,
			"UserData3" "decimal"(28, 7) NULL ,
			"UserData4" "decimal"(28, 7) NULL ,
			"UserData5" "decimal"(28, 7) NULL ,
			"UserData6" "decimal"(28, 7) NULL ,
			"UserData7" "decimal"(28, 7) NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Clustered Index. */

	execute ('
		ALTER TABLE "Position" WITH NOCHECK ADD 
			CONSTRAINT "PKPosition" PRIMARY KEY  CLUSTERED 
			(
				"SecurityId",
				"AccountId",
				"PositionTypeCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Position" ADD 
			CONSTRAINT "FKAccountPosition" FOREIGN KEY 
			(
				"AccountId"
			) REFERENCES "Account" (
				"AccountId"
			),
			CONSTRAINT "FKSecurityPosition" FOREIGN KEY 
			(
				"SecurityId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKPositionTypePosition" FOREIGN KEY 
			(
				"PositionTypeCode"
			) REFERENCES "PositionType" (
				"PositionTypeCode"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Position" WITH NOCHECK ADD 
			CONSTRAINT "DefaultPositionIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultPositionIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Position''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end

/* Print the final state for the log. */

execute "ObjectEpilog" "Position"
go
