@echo off

echo Generating Account
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Account" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Account.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating AccountType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "AccountType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\AccountType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Algorithm
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Algorithm" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Algorithm.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating AlgorithmType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "AlgorithmType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\AlgorithmType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Allocation
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Allocation" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Allocation.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating BlockOrder
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "BlockOrder" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\BlockOrder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating BlockOrderTree
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "BlockOrderTree" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\BlockOrderTree.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Blotter
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Blotter" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Blotter.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating BlotterType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "BlotterType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\BlotterType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating BlotterMap
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "BlotterMap" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\BlotterMap.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Broker
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Broker" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Broker.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Condition
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Condition" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Condition.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Configuration
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Configuration" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Configuration.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Country
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Country" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Country.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Currency
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Currency" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Currency.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Debt
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Debt" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Debt.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating DebtType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "DebtType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\DebtType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Equity
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Equity" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Equity.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating EquityType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "EquityType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\EquityType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Exchange
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Exchange" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Exchange.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Execution
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Execution" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Execution.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Folder
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Folder" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Folder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Holiday
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Holiday" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Holiday.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating HolidayType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "HolidayType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\HolidayType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Issuer
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Issuer" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Issuer.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating IssuerType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "IssuerType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\IssuerType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating LotHandling
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "LotHandling" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\LotHandling.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Model
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Model" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Model.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating ModelType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "ModelType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\ModelType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Object
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Object" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Object.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating ObjectTree
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "ObjectTree" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\ObjectTree.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Type
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Type" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Type.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Order
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Order" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Order.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating OrderTree
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "OrderTree" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\OrderTree.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating OrderType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "OrderType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\OrderType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Placement
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Placement" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Placement.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Position
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Position" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Position.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating PositionTarget
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "PositionTarget" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\PositionTarget.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating PositionType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "PositionType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\PositionType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Price
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Price" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Price.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Property
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Property" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Property.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating ProposedOrder
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "ProposedOrder" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\ProposedOrder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating ProposedOrderTree
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "ProposedOrderTree" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\ProposedOrderTree.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Province
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Province" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Province.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Restriction
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Restriction" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Restriction.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Scheme
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Scheme" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Scheme.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Sector
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Sector" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Sector.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating SectorTarget
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "SectorTarget" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\SectorTarget.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Security
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Security" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Security.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating SecurityType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "SecurityType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\SecurityType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Status
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Status" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Status.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Stylesheet
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Stylesheet" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Stylesheet.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating StylesheetType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "StylesheetType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\StylesheetType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating SystemFolder
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "SystemFolder" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\SystemFolder.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating TaxLot
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "TaxLot" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\TaxLot.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating TimeInForce
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "TimeInForce" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\TimeInForce.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating TransactionType
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "TransactionType" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\TransactionType.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating User
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "User" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\User.cs"
if %ERRORLEVEL% NEQ 0 goto errors
echo Generating Violation
"Core Interface Compiler" -ps "Quasar" -ref "MarkThree.Quasar" -ref "MarkThree.Quasar.Server" -ds "ServerDataModel" -t "Violation" -ns "MarkThree.Quasar.Core" -i "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Data Model\DataSetMarket.xsd" -out "C:\My Documents\Visual Studio 2005\Projects\Mark Three Software\Quasar\Core Service\Violation.cs"
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
