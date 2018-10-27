/*************************************************************************************************************************
*
*	File:			Archive Unused Security.sql
*	Description:	Sets the archive bit on security that are not active in the taxLot, order or model.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on	


declare @securityId int
declare "Security cursor" cursor local
for select "Security"."SecurityId" from "Security"
for read only

/* Open the cursor that checks for foreign keys that reference the table being dropped. */

open "Security cursor"

/* Cycle through all the contraints that reference the table being dropped and remove the contraint. */

declare @command varchar(128)

fetch next from "Security cursor" into @securityId
while (@@FETCH_STATUS = 0)
begin
	if
		not exists (select * from "PositionTarget" where "PositionTarget"."SecurityId" = @securityId) and
		not exists (select * from "TaxLot" where "TaxLot"."SecurityId" = @securityId) and
		not exists (select * from "ProposedOrder" where "ProposedOrder"."SecurityId" = @securityId) and
		not exists (select * from "Order" where "Order"."SecurityId" = @securityId) and
		not exists (select * from "Currency" where "Currency"."CurrencyId" = @securityId)
		begin
			set @command = 'Execute "ArchiveObject" ' + convert(varchar, @securityId)
			exec (@command)
		end
	fetch next from "Security cursor" into @securityId
end

print convert(varchar, getdate(), 120) + 'Z Data: Unused Security IsArchived, Added'
go
