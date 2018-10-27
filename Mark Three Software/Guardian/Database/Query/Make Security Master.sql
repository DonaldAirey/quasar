select char(9) + char(9) + '<method assembly="External Service" type="MarkThree.Guardian.External.Equity" name="Load">' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<configurationId>US TICKER</configurationId>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<equityId>' + Object.ExternalId1 + '</equityId>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<settlementId>USD</settlementId>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<countryId>US</countryId>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<exchangeId>' + Exchange.ExternalId0 + '</exchangeId>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<equityTypeCode>COMMON STOCK</equityTypeCode>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<minimumQuantity>25000</minimumQuantity>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<name>' + Object.Name + '</name>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<symbol>' + Security.Symbol + '</symbol>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<marketCapitalization>' + Convert(varchar(30), Cast(Security.MarketCapitalization as bigint)) + '</marketCapitalization>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<averageDailyVolume>' + Convert(varchar(30), Cast(Security.AverageDailyVolume as bigint)) + '</averageDailyVolume>' + char(13) + char(10) +
	char(9) + char(9) + char(9) + '<volumeCategoryId>' + VolumeCategory.ExternalId0 + '</volumeCategoryId>' + char(13) + char(10) +
	char(9) + char(9) + '</method>' + char(13) + char(10)
from Security
inner join Object on SecurityId = ObjectId
inner join Equity on SecurityId = EquityId
inner join Exchange on Equity.ExchangeId = Exchange.ExchangeId
inner join VolumeCategory on Security.VolumeCategoryId = VolumeCategory.VolumeCategoryId
inner join Country on Country.CountryId = Security.CountryId
where Country.Abbreviation = 'US'
