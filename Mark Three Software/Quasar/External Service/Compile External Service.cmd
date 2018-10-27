@echo off

echo Generating Account
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Account" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Account.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating AccountType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "AccountType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\AccountType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Algorithm
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Algorithm" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Algorithm.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating AlgorithmType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "AlgorithmType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\AlgorithmType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Blotter
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Blotter" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Blotter.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating BlotterType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "BlotterType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\BlotterType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating BlotterMap
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "BlotterMap" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\BlotterMap.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Broker
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Broker" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Broker.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Condition
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Condition" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Condition.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Configuration
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Configuration" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Configuration.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Country
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Country" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Country.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Currency
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Currency" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Currency.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Debt
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Debt" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Debt.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating DebtType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "DebtType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\DebtType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Equity
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Equity" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Equity.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating EquityType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "EquityType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\EquityType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Exchange
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Exchange" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Exchange.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Folder
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Folder" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Folder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Holiday
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Holiday" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Holiday.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating HolidayType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "HolidayType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\HolidayType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Issuer
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Issuer" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Issuer.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating IssuerType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "IssuerType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\IssuerType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating LotHandling
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "LotHandling" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\LotHandling.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Model
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Model" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Model.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating ModelType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "ModelType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\ModelType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Object
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Object" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Object.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating ObjectTree
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "ObjectTree" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\ObjectTree.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Type
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Type" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Type.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Order
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Order" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Order.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating OrderType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "OrderType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\OrderType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Position
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Position" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Position.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating PositionTarget
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "PositionTarget" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\PositionTarget.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating PositionType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "PositionType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\PositionType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Price
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Price" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Price.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Property
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Property" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Property.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Province
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Province" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Province.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Scheme
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Scheme" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Scheme.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Sector
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Sector" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Sector.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating SectorTarget
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "SectorTarget" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\SectorTarget.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Security
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Security" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Security.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating SecurityType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "SecurityType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\SecurityType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Status
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Status" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Status.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Stylesheet
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Stylesheet" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\Stylesheet.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating StylesheetType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "StylesheetType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\StylesheetType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating SystemFolder
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "SystemFolder" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\SystemFolder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating TaxLot
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "TaxLot" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\TaxLot.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating TimeInForce
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "TimeInForce" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\TimeInForce.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating TransactionType
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "TransactionType" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\TransactionType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating User
"External Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "User" -ns "MarkThree.Quasar.External" -is "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\External Service\User.cs"
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
