/*************************************************************************************************************************
*
*	File:			Negotiation.sql
*	Description:	Negotiationed Working Orders
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Negotiation"
*	Description:	Contains Negotiationing Working Orders.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Negotiation"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Negotiation', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Negotiation"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Negotiation" (
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"ExecutionId" "int" NULL ,
			"MatchId" "int" NOT NULL ,
			"NegotiationId" "int" NOT NULL ,
			"Quantity" "int" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"StatusCode" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Negotiation" WITH NOCHECK ADD 
			CONSTRAINT "KeyNegotiation" PRIMARY KEY  CLUSTERED 
			(
				"NegotiationId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Foreign Key */

	execute ('
		ALTER TABLE "Negotiation" ADD 
			CONSTRAINT "MatchNegotiation" FOREIGN KEY 
			(
				"MatchId"
			) REFERENCES "Match" (
				"MatchId"
			),
			CONSTRAINT "StatusNegotiation" FOREIGN KEY 
			(
				"StatusCode"
			) REFERENCES "Status" (
				"StatusCode"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Negotiation" WITH NOCHECK ADD 
			CONSTRAINT "DefaultNegotiationIsArchived" DEFAULT (0) FOR "IsArchived" ,
			CONSTRAINT "DefaultNegotiationIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Negotiation''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Negotiation"
go
 