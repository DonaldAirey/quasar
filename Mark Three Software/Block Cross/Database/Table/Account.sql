/*************************************************************************************************************************
*
*	File:			Account.sql
*	Description:	Account data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Account"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Account"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Account', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Account"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Account" (
			"AccountId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"LotHandlingCode" "int" NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Account" WITH NOCHECK ADD 
			CONSTRAINT "PKAccount" PRIMARY KEY  NONCLUSTERED 
			(
				"AccountId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Account" ADD 
			CONSTRAINT "FKOAccountBaseAccount" FOREIGN KEY 
			(
				"AccountId"
			) REFERENCES "AccountBase" (
				"AccountBaseId"
			),
			CONSTRAINT "FKLotHandlingAccount" FOREIGN KEY 
			(
				"LotHandlingCode"
			) REFERENCES "LotHandling" (
				"LotHandlingCode"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Account" WITH NOCHECK ADD 
			CONSTRAINT "DefaultAccountIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultAccountIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Account''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Account"
go
