/*************************************************************************************************************************
*
*	File:			Price.sql
*	Description:	Pricing source for security.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Price"
*	Description:	Contains the Level IPrice data for security.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Price"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Price', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Price"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Price" (
			"AskPrice" "decimal"(28, 7) NOT NULL ,
			"AskSize" "decimal"(28, 7) NOT NULL ,
			"BidPrice" "decimal"(28, 7) NOT NULL ,
			"BidSize" "decimal"(28, 7) NOT NULL ,
			"ClosePrice" "decimal"(28, 7) NOT NULL ,
			"CurrencyId" "int" NOT NULL ,
			"HighPrice" "decimal"(28, 7) NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"LastPrice" "decimal"(28, 7) NOT NULL ,
			"LastSize" "decimal"(28, 7) NOT NULL ,
			"LowPrice" "decimal"(28, 7) NOT NULL ,
			"OpenPrice" "decimal"(28, 7) NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"SecurityId" "int" NOT NULL,
			"Volume" "decimal"(28, 7) NOT NULL ,
			"VolumeWeightedAveragePrice" "decimal"(28, 7) NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Price" WITH NOCHECK ADD 
			CONSTRAINT "PKPrice" PRIMARY KEY  CLUSTERED 
			(
				"SecurityId",
				"CurrencyId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Foreign Key */

	execute ('
		ALTER TABLE "Price" ADD 
			CONSTRAINT "FKSecurityPriceSecurityId" FOREIGN KEY 
			(
				"SecurityId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKSecurityPriceCurrencyId" FOREIGN KEY 
			(
				"CurrencyId"
			) REFERENCES "Security" (
				"SecurityId"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Price" WITH NOCHECK ADD 
			CONSTRAINT "DefaultPriceIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultPriceIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Price''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Price"
go
