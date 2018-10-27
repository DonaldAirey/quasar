@echo off

echo Compiling AccountBase
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "AccountBase" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\AccountBase.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Account
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Account" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Account.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling AccountGroup
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "AccountGroup" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\AccountGroup.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Blotter
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Blotter" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Blotter.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Broker
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Broker" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Broker.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling BrokerAccount
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "BrokerAccount" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\BrokerAccount.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Branch
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Branch" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Branch.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling ClearingBroker
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "ClearingBroker" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\ClearingBroker.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling ComplianceOfficer
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "ComplianceOfficer" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\ComplianceOfficer.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Condition
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Condition" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Condition.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Configuration
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Configuration" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Configuration.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Country
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Country" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Country.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Currency
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Currency" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Currency.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Debt
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Debt" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Debt.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Destination
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Destination" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Destination.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling DestinationOrder
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "DestinationOrder" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\DestinationOrder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Equity
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Equity" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Equity.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Exchange
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Exchange" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Exchange.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Execution
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Execution" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Execution.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Folder
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Folder" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Folder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Holiday
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Holiday" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Holiday.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling HolidayType
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "HolidayType" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\HolidayType.cs"
echo Compiling Image
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Image" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Image.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Institution
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Institution" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Institution.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Issuer
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Issuer" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Issuer.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling LotHandling
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "LotHandling" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\LotHandling.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling State
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "State" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\State.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Object
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Object" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Object.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling ObjectTree
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "ObjectTree" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\ObjectTree.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling OrderType
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "OrderType" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\OrderType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Position
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Position" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Position.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling PositionType
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "PositionType" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\PositionType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Price
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Price" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Price.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling PartyType
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "PartyType" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\PartyType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling PriceType
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "PriceType" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\PriceType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Property
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Property" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Property.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Province
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Province" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Province.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Security
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Security" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Security.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Source
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Source" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Source.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling SourceOrder
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "SourceOrder" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\SourceOrder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Status
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Status" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Status.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Stylesheet
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Stylesheet" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Stylesheet.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling StylesheetType
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "StylesheetType" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\StylesheetType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling SubmissionType
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "SubmissionType" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\SubmissionType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling SystemFolder
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "SystemFolder" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\SystemFolder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling TaxLot
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "TaxLot" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\TaxLot.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling TimeInForce
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "TimeInForce" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\TimeInForce.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Timer
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Timer" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Timer.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Trader
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Trader" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Trader.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling TraderVolumeSetting
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "TraderVolumeSetting" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\TraderVolumeSetting.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling Type
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "Type" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\Type.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling User
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "User" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\User.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling WorkingOrder
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "WorkingOrder" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\WorkingOrder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Compiling VolumeCategory
"External Interface Compiler" -ps "Guardian" -ref "MarkThree.Guardian" -ref "MarkThree.Guardian.Server" -ds "ServerMarketData" -t "VolumeCategory" -ns "MarkThree.Guardian.External" -is "MarkThree.Guardian.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\Market Data\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Guardian\External Service\VolumeCategory.cs"
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
