@echo off
REM: Command File Created by Microsoft Visual Database Tools
REM: Date Generated: 1/18/2002
REM: Authentication type: Windows NT
REM: Usage: CommandFilename [Server] [Database]

REM: Drop everything in the Quasar Database.
osql -S %1 -d Guardian -E -b -n -i "..\Query\Drop All.sql"

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
