@echo off
REM: Command File Created by Microsoft Visual Database Tools
REM: Date Generated: 1/15/2002
REM: Authentication type: Windows NT
REM: Usage: CommandFilename [Server] [Database]

REM Version Control Table

osql -S %1 -d "Quasar" -E -b -n -i "..\Table\versionControl.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\versionHistory.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\versionTag.sql"
if %ERRORLEVEL% NEQ 0 goto errors

REM Version Control Procedures

osql -S %1 -d "Quasar" -E -b -n -i "..\Stored Procedure\ObjectProlog.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Stored Procedure\ObjectEpilog.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Function\RevisionNeeded.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Stored Procedure\RemoveConstraint.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Stored Procedure\RemoveObject.sql"
if %ERRORLEVEL% NEQ 0 goto errors

REM Table

osql -S %1 -d "Quasar" -E -b -n -i "..\Table\AccountType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\AlgorithmType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\BlotterType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\DebtType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\EquityType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\FolderType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\ModelType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Type.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\OrderType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\PositionType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\SecurityType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\StylesheetType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\TransactionType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Algorithm.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Configuration.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Status.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Country.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Province.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Property.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\TimeInForce.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\HolidayType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Holiday.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Condition.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\LotHandling.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\IssuerType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Issuer.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Object.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\ObjectTree.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Security.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Exchange.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\ExchangeMap.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Currency.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Debt.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Equity.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Scheme.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Sector.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Model.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\SectorTarget.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\PositionTarget.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Blotter.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Broker.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Account.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Folder.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\SystemFolder.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\User.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\TaxLot.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Position.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\ProposedOrder.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\ProposedOrderTree.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Price.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Restriction.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Violation.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\BlockOrder.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\BlockOrderTree.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Order.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\OrderTree.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Placement.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Execution.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Allocation.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\Stylesheet.sql"
if %ERRORLEVEL% NEQ 0 goto errors
osql -S %1 -d "Quasar" -E -b -n -i "..\Table\BlotterMap.sql"
if %ERRORLEVEL% NEQ 0 goto errors

REM Trigger

osql -S %1 -d "Quasar" -E -b -n -i "..\Trigger\VersionHistory.sql"
if %ERRORLEVEL% NEQ 0 goto errors

goto finish

REM: How to use screen
:usage
echo.
echo Usage: MyScript Server Database
echo Server: the name of the target SQL Server
echo Database: the name of the target database
echo.
echo Example: MyScript.cmd MainServer MainDatabase
echo.
echo.
goto done

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

REM: finished execution
:finish
echo.
echo Script execution is complete!
pause
:done
@echo on
