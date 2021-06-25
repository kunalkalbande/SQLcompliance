@echo off

rem --------------------------------------------------
rem -- SQL compliance Manager Stored Procedure Script Generator
rem --------------------------------------------------

echo. > UpdateSPs.sql
echo. > UpdateEventDatabasesSPs.sql
for %%b in (sp_*.sql) do if not exist %%b (call :MISSINGFILE %%b&goto :eof) else (call :APPEND %%b)
for %%b in (fn_*.sql) do if not exist %%b (call :MISSINGFILE %%b&goto :eof) else (call :APPEND %%b)
for %%b in (trg_*.sql) do if not exist %%b (call :MISSINGFILE %%b&goto :eof) else (call :APPEND %%b)
for %%b in (rename_*.sql) do if not exist %%b (call :MISSINGFILE %%b&goto :eof) else (call :APPEND %%b)
for %%b in (sp_sqlcm_UpgradeAllEventDatabase.sql) do if not exist %%b (call :MISSINGFILE %%b&goto :eof) else (call :APPENDEVENT %%b)
for %%b in (sp_sqlcm_UpgradeAllEventDatabases.sql) do if not exist %%b (call :MISSINGFILE %%b&goto :eof) else (call :APPENDEVENT %%b)
goto :eof

:APPEND
find /V /I "DEBUGONLY" %* >> UpdateSPs.sql
echo. >> UpdateSPs.sql
echo GO >> UpdateSPs.sql
goto :eof

:APPENDEVENT
find /V /I "DEBUGONLY" %* >> UpdateEventDatabasesSPs.sql
echo. >> UpdateEventDatabasesSPs.sql
echo GO >> UpdateEventDatabasesSPs.sql
goto :eof

:MISSINGFILE
echo The required file [%*] could not be found
echo. The required file [%*] could not be found > UpdateSPs.sql
