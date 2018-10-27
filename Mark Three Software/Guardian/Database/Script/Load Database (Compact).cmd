@echo off
REM: Command File Created by Microsoft Visual Database Tools
REM: Date Generated: 1/18/2002
REM: Authentication type: Windows NT
REM: Usage: CommandFilename [Server] [Database]

REM: Reset the middle tier before loading it with data.
iisreset

REM: Load the constants
"Script Loader" -f -i "..\Data\Type.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Condition.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\StylesheetType.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\HolidayType.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\LotHandling.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\PartyType.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\SubmissionType.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\PriceType.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\PositionType.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Property.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\State.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Status.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\TimeInForce.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\OrderType.xml"
if %ERRORLEVEL% NEQ 0 goto errors

REM: Load the data.
"Script Loader" -i "..\Data\Configuration.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Volume Category.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Country.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Province.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Exchange.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Currency.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" "..\Data\Currency Price.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Russell 3000.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Russell 3000 Price.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\CA Equity.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\CA Equity Price.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\UK Equity.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\UK Equity Price.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\US Government Bill.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\US Government Bond.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\US Government Note.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\US Municipal Bond.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\US Corporate Bond.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\US Fixed Income Price.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -b 50 -i "..\Data\Corporate Logo.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -b 50 -i "..\Data\Image.xml"
if %ERRORLEVEL% NEQ 0 goto errors

REM: Stylesheets
"Stylesheet Loader" -i "..\Stylesheet\Destination Order Viewer.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Destination Order Detail Viewer.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Execution Viewer.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Execution Detail Viewer.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Match Viewer.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Match History Viewer.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Source Order Detail Viewer.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Source Order Viewer.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Working Order Viewer.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Quote Viewer.xsl"
if %ERRORLEVEL% NEQ 0 goto errors


REM: A Sample User Hierarchy
"Script Loader" -i "..\Data\Institution.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\ClearingBroker.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Broker.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Branch.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Blotter.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\User.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Trader.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Compliance Officer.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Blotter Hierarchy.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Broker Accounts.xml"
if %ERRORLEVEL% NEQ 0 goto errors

"Script Loader" -i "..\Data\System Folder.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Destination.xml"
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

REM: finished Execution
:finish
echo.
echo Script Execution is complete!
pause
:done
@echo on
