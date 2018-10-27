select char(9) + char(9) + '<method assembly="External Service" type="MarkThree.Guardian.External.Price" name="Load">' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<configurationId>US TICKER</configurationId>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<securityId>' + Object.ExternalId1 + '</securityId>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<currencyId>USD</currencyId>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<lastPrice>' + isNull(Convert(varchar(30), Price.LastPrice, 128), 0.0) + '</lastPrice>' + char(13) + char(10)+
	char(9) + char(9) + '</method>' + char(13) + char(10)
from Equity
inner join Object on EquityId = ObjectId
inner join Security on EquityId = SecurityId
inner join Price on EquityId = Price.SecurityId
inner join Country on Security.CountryId = Country.CountryId
where Country.Abbreviation = 'US'