/*************************************************************************************************************************
*
*	File:			Debt.sql
*	Description:	"Debt" and "Debt" tables
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Debt"
*	Description:	Security Master - Contains the most relevant information about a security.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Debt"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Debt', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Debt"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Debt" (
			"DebtId" "int" NOT NULL ,
			"IssuerId" "int" NULL ,
			"SettlementId" "int" NOT NULL ,
			"DebtTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"FaceOutstanding" "decimal"(15, 4) NOT NULL ,
			"Coupon" "decimal"(15, 4) NOT NULL ,
			"MaturityDate" "Datetime" NOT NULL ,
			"DatedDate" "Datetime" NULL ,
			"FirstCoupon" "Datetime" NULL ,
			"Frequency" "int" NOT NULL ,
			"RedemptionValue" "decimal"(15, 4) NOT NULL ,
			"IncomeTaxRate" "decimal"(15, 4) NOT NULL ,
			"CapitalGainsTaxRate" "decimal"(15, 4) NOT NULL ,
			"CutoffPeriod" "int" NOT NULL ,
			"IssuePrice" "int" NOT NULL ,
			"TrueYield" bit NOT NULL ,
			"WeekendCode" "int" NOT NULL ,
			"ExdividendDays" "int" NULL ,
			"Rating0" "int" NULL ,
			"Rating1" "int" NULL ,
			"Rating2" "int" NULL ,
			"Rating3" "int" NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Debt" WITH NOCHECK ADD 
			CONSTRAINT "PKDebt" PRIMARY KEY  CLUSTERED 
			(
				"DebtId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Key */

	execute ('
		ALTER TABLE "Debt" ADD 
			CONSTRAINT "FKSecurityDebt" FOREIGN KEY 
			(
				"DebtId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKSettlementDebt" FOREIGN KEY 
			(
				"SettlementId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKIssuerDebt" FOREIGN KEY 
			(
				"IssuerId"
			) REFERENCES "Issuer" (
				"IssuerId"
			),
			CONSTRAINT "FKDebtTypeDebt" FOREIGN KEY 
			(
				"DebtTypeCode"
			) REFERENCES "DebtType" (
				"DebtTypeCode"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Debt" WITH NOCHECK ADD
			CONSTRAINT "DefaultDebtIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultDebtIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the revisions table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Debt''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Debt"
go
