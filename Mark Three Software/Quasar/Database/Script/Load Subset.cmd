@echo off
REM: Command File Created by Microsoft Visual Database Tools
REM: Date Generated: 1/18/2002
REM: Authentication type: Windows NT

"Broker Loader" -i "..\Data\broker.txt"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Appraisal.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Block Order.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Bond Block Order.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Currency Block Order.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Equity Block Order.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Equity Placement.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Bond Execution.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Equity Execution.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Ticket.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Equity Ticket.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Stylesheet Loader" -i "..\Stylesheet\Equity Order.xsl"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Currency.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" "..\Data\Currency Price.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\US Equity.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\US Equity Price.xml"
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
"Script Loader" -i "..\Data\GICS Level 1.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\GICS Level 2.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\GICS Level 3.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\GICS.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Custom Classification.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Model.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\SP100 Model.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Blotter.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Account Hierarchy Demo.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Bailey Family.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Equus Fund.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Leopard Fund.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\SP100 Wrap Account.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\Large Folder.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -i "..\Data\BlotterMap.xml"
if %ERRORLEVEL% NEQ 0 goto errors
"Script Loader" -t 480 -i "..\Data\Large Archive Security.xml"
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
