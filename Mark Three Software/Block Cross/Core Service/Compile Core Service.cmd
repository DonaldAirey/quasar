@echo off

echo Compiling AccountBase
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "AccountBase" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\AccountBase.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Account
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Account" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Account.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling AccountGroup
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "AccountGroup" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\AccountGroup.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Allocation
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Allocation" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Allocation.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Blotter
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Blotter" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Blotter.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Branch
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Branch" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Branch.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Broker
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Broker" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Broker.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling BrokerAccount
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "BrokerAccount" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\BrokerAccount.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling ClearingBroker
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "ClearingBroker" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\ClearingBroker.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling ComplianceOfficer
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "ComplianceOfficer" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\ComplianceOfficer.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Condition
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Condition" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Condition.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Configuration
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Configuration" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Configuration.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Country
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Country" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Country.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Currency
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Currency" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Currency.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Destination
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Destination" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Destination.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Debt
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Debt" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Debt.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling DestinationOrder
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "DestinationOrder" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\DestinationOrder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Equity
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Equity" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Equity.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Exchange
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Exchange" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Exchange.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Execution
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Execution" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Execution.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Folder
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Folder" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Folder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Holiday
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Holiday" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Holiday.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling HolidayType
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "HolidayType" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\HolidayType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Image
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Image" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Image.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Institution
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Institution" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Institution.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Issuer
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Issuer" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Issuer.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling LotHandling
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "LotHandling" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\LotHandling.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Match
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Match" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Match.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Negotiation
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Negotiation" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Negotiation.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling State
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "State" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\State.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Object
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Object" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Object.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling ObjectTree
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "ObjectTree" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\ObjectTree.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling OrderType
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "OrderType" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\OrderType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling PartyType
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "PartyType" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\PartyType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling PriceType
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "PriceType" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\PriceType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Position
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Position" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Position.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling PositionType
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "PositionType" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\PositionType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Price
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Price" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Price.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Property
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Property" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Property.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Province
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Province" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Province.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Security
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Security" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Security.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Source
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Source" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Source.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling SourceOrder
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "SourceOrder" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\SourceOrder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Status
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Status" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Status.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Stylesheet
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Stylesheet" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Stylesheet.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling StylesheetType
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "StylesheetType" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\StylesheetType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling SubmissionType
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "SubmissionType" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\SubmissionType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling SystemFolder
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "SystemFolder" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\SystemFolder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling TaxLot
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "TaxLot" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\TaxLot.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling TimeInForce
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "TimeInForce" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\TimeInForce.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Trader
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Trader" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Trader.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling TraderVolumeSetting
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "TraderVolumeSetting" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\TraderVolumeSetting.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Timer
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Timer" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Timer.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Type
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Type" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\Type.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling User
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "User" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\User.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling WorkingOrder
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "WorkingOrder" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\WorkingOrder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling VolumeCategory
"Core Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "VolumeCategory" -ns "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Core Service\VolumeCategory.cs"
if %ERRORLEVEL% NEQ 0 goto errors

goto finish

REM: error handler
:errors
echo.
echo WARNING! Error(s) were detected!
echo --------------------------------
echo Please evaluate the situation and, if needed,
echo restart this command file. You may need to
echo supply command parameters when executing
echo this command file.
echo.
pause
goto done

REM: finished Execution
:finish
echo.
echo Script Execution is complete!
:done
@echo on
