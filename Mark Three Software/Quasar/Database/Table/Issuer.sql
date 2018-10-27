/*************************************************************************************************************************
*
*	File:			Issuer.sql
*	Description:	The entity (Country, Company, Government Agency, etc) that issues debt or equity.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Issuer"
*	Description:	Issuer are entities that issue security.
*************************************************************************************************************************/

/* Standard Prolog for Creating/Upgrading a table. */

execute "ObjectProlog" "Issuer"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Issuer', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Issuer"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Issuer" (
			"IssuerId" "int" NOT NULL ,
			"CountryId" "int" NULL ,
			"ProvinceId" "int" NULL ,
			"IssuerTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Address0" "varchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Address1" "varchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Address2" "varchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"City" "varchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"PostalCode" "varchar"(16) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Rating0" "int" NULL ,
			"Rating1" "int" NULL ,
			"Rating2" "int" NULL ,
			"Rating3" "int" NULL ,
			"UserData0" "decimal"(15, 4) NULL ,
			"UserData1" "decimal"(15, 4) NULL ,
			"UserData2" "decimal"(15, 4) NULL ,
			"UserData3" "decimal"(15, 4) NULL ,
			"UserData4" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"UserData5" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"UserData6" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"UserData7" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Issuer" WITH NOCHECK ADD 
			CONSTRAINT "PKIssuer" PRIMARY KEY  CLUSTERED 
			(
				"IssuerId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Foreign Key */

	execute ('
		ALTER TABLE "Issuer" ADD 
			CONSTRAINT "FKIssuerTypeIssuer" FOREIGN KEY 
			(
				"IssuerTypeCode"
			) REFERENCES "IssuerType" (
				"IssuerTypeCode"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Issuer" WITH NOCHECK ADD 
			CONSTRAINT "DefaultIssuerIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultIssuerIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Issuer''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final issuer for the log. */

execute "ObjectEpilog" "Issuer"
go
