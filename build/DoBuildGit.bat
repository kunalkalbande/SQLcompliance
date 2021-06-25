@echo off
if "%COMPUTERNAME%" == "SQLCMBLD" goto DOSTARTUP
echo. NOTE:  This script can only be run from the SQLcmBuild machine
goto END

:DOSTARTUP
SET BUILDTYPE=%1
SET BUILDNUMBER=%2

if (%BUILDNUMBER%)==() (
	echo. BUILD ERROR: Missing build number parameter
	goto END
)

if (%BUILDTYPE%)==(dev) GOTO BUILD_DEV
if (%BUILDTYPE%)==(official) GOTO BUILD_OFFICIAL
if (%BUILDTYPE%)==(prep) GOTO BUILD_PREP
if (%BUILDTYPE%)==(doc) GOTO BUILD_DOCONLY
if (%BUILDTYPE%)==(docinstall) GOTO BUILD_DOCINSTALL
echo. BUILD ERROR: Missing or invalid command line parameter
echo.
echo.
echo. Syntax:
echo.    DOBUILD type [version]
echo.    type = dev, official, or prep
echo.    version = 2.1, 3.0, 3.1, 3.2, 3.3, 3.5, 3.6, 3.7, 4.0, 4.2
echo.
echo.    DOBUILD doc version
echo.    version = the full version to pull from the archive
echo.
echo.    DOBUILD docinstall version
echo.    version = the full version to pull from the archive
echo.
goto END

:BUILD_DEV
echo. Starting Development Build
set BUILDTARGET="Build.Dev"
goto DOBUILD

:BUILD_OFFICIAL
echo. Starting Official Build
set BUILDTARGET="Build.Official"
goto DOBUILD

:BUILD_PREP
echo.  Starting Prep Build
echo.  Once the script is complete, you can invoke nant directly to do the custom build.
echo.  For example:  nant -f:sqlcompliance.build Build.Dev -D:SQLcm.version=2.1.1.2
goto DOBUILD

:BUILD_DOCONLY
if (%3)==() GOTO DOCONLY_USAGE
echo.  Starting Doc Only Build
set BUILDTARGET="Build.DocOnly"
goto DOBUILD

:BUILD_DOCINSTALL
if (%3)==() GOTO DOCONLY_USAGE
echo.  Starting DocInstall Build
set BUILDTARGET="Build.DocInstallOnly"


:DOBUILD
set SQLdmBuild=C:\GitHub\SQLcm

REM **********************************
REM Nuke the entire tree except build
REM **********************************
echo. Deleting development, documentation and install directories
rd /q /s C:\GitHub\SQLcm

REM **********************************
REM Fetch the latest from GitHub
REM **********************************
echo.
echo. Fetching the lastest from GitHub
REM "C:\Program Files (x86)\Git\bin\git.exe" clone https://SQLcompliance:gud!uZc1@github.com/IderaInc/SQLcm.git C:\GitHub\SQLcm
"C:\Program Files\Git\bin\git.exe" clone https://SQLcompliance:gud!uZc1@github.com/IderaInc/SQLcm.git C:\GitHub\SQLcm

REM **********************************
REM Execute the build script
REM **********************************
if (%BUILDTYPE%)==(prep) GOTO END
if (%BUILDTYPE%)==(doc) GOTO NANT_DOCONLY
if (%BUILDTYPE%)==(docinstall) GOTO NANT_DOCONLY

nant -f:c:\GitHub\SQLcm\Build\sqlcompliancegit.build -D:SQLcm.buildnumber=%BUILDNUMBER% -D:Perforce.Branch="%BRANCH%" %BUILDTARGET% -l:C:\GitHub\Build\build.log -logger:NAnt.Core.MailLogger
GOTO END

:NANT_DOCONLY
nant -f:sqlcompliance.build -D:Perforce.Branch="%BRANCH%" -D:SQLcm.buildnumber=%BUILDNUMBER% -D:SQLcm.version="%3" -D:Perforce.TargetLabel="sqlcm_%3" %BUILDTARGET%
GOTO END

:DOCONLY_USAGE
echo. Please supply a pre-existing build version for doc only builds.

:END
echo.  Build script execution is complete