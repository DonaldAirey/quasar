"RemoveObject" "MakeTaxLot"
go

create procedure "MakeTaxLot"
	@configurationId int,
	@securityExternalId varchar(32),
	@accountExternalId varchar(32),
	@position_typeExternalId varchar(32),
	@market_value decimal(28, 7)
as

	/* Declarations */

	declare @security_type_code int
	declare @security_id int
	declare @quantity decimal(28, 7)
	declare @last_price decimal(28, 7)
	declare @average_cost decimal(28, 7)
	declare @lot_quantity decimal(28, 7)
	declare @price_change decimal(28, 7)
	declare @lot_id decimal(28, 7)

	set @security_id = "Get securityId"(@configurationId, @securityExternalId)
	set @security_type_code = (select "SecurityTypeCode" from "Security" where "SecurityId" = @security_id)
	set @last_price =
	(
		select "Price"."Last price"
		from "Equity" inner join "Price" on ("Equity"."EquityId" = "Price"."SecurityId" and "Equity"."CurrencyId" = "Price"."CurrencyId")
		where @security_id = "Equity"."EquityId"
	)

	/* Tax Lots for Cash */

	if (@security_type_code = 0)
	begin

		set @lot_id = round(rand() * 100000, 0)

		print 'Execute "InsertTaxLotExternal" ' + convert(varchar, @configurationId) + ', ''' + ltrim(str(@lot_id, 10, 0)) +
			''', ''' + @securityExternalId + ''', ''' + @accountExternalId + ''', ''' + @position_typeExternalId + ''', ' +
			ltrim(str(@market_value, 10, 0)) + ', ' + ltrim(str(1.0, 10, 2))

	end

	/* Tax Lots for Equties */

	if (@security_type_code = 1)
	begin

		set @average_cost = @last_price + round((rand() - 0.5) * (@last_price * .25), 2)
		set @quantity = round(@market_value / @average_cost, -2)
	
		while @quantity > 0
		begin
	
			set @lot_id = round(rand() * 100000, 0)
	
			/* Select a random round lot that is proportional to the market value. */

			set @lot_quantity = round(rand() * power(10, round(log10(@market_value), 0) - 1) + 100, -2)
	
			if @lot_quantity > @quantity
				set @lot_quantity = @quantity
	
			set @quantity = @quantity - @lot_quantity
	
			select @price_change = round((rand() - 0.5) * (@average_cost * .25), 2)
	
			print 'Execute "InsertTaxLotExternal" ' + convert(varchar, @configurationId) + ', ''' + ltrim(str(@lot_id, 10, 0)) +
				''', ''' + @securityExternalId + ''', ''' + @accountExternalId + ''', ''' + @position_typeExternalId +
				''', ' + ltrim(str(@lot_quantity, 10, 0)) + ', ' + ltrim(str(@average_cost + @price_change, 10, 2))
	
	end

end

go
