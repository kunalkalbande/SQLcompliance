@echo off
REM *******************************************
REM * DoIderaDashboardSampleProductBuild.cmd - script for starting IderaDashboard builds
REM * Arg 1 - Build number to be used in creating the version number
REM * Arg 2 - Branch to build. 
REM * Arg 3 - Type of build official, dev, or incremental
REM * Arg 4 - IderaDashboard build root directory
REM *******************************************
REM if "%COMPUTERNAME%" == "IDERADSHBRDBLD" goto DO_STARTUP
REM echo. NOTE:  This script can only be run from the IDERADSHBRDBLD machine
REM goto END

REM *******************************************
REM ********** DO STARTUP ********************
REM *******************************************
goto DO_STARTUP
:DO_STARTUP
set BUILD_ERROR=0
set BUILDNUMBER=%1
set BUILDBRANCH=%2
set BUILDTYPE=%3
set BUILDROOT=%4

if (%BUILDNUMBER%)==() (
	echo. BUILD ERROR: Missing build number parameter
	set BUILD_ERROR=1001
	goto END
)

if (%BUILDBRANCH%)==() (
	echo. BUILD ERROR: Missing build branch parameter
	set BUILD_ERROR=1002
	goto END
)

if (%BUILDTYPE%)==() (
	echo. BUILD ERROR: Missing build type parameter
	set BUILD_ERROR=1003
	goto END
)

if (%BUILDROOT%)==() (
	set BUILDROOT=C:\Build\%BUILDBRANCH%SampleProduct
)
REM set IDERADASHBOARDBUILDROOT=%BUILDROOT%

if (%BUILDTYPE%)==(dev) GOTO BUILD_DEV
if (%BUILDTYPE%)==(official) GOTO BUILD_OFFICIAL
if (%BUILDTYPE%)==(manualFetch) GOTO BUILD_MANUAL_FETCH

echo. BUILD ERROR: Missing or invalid build type parameter
echo. Valid entries are: obfuscated, nonobfuscated, or manualFetch
echo.
set BUILD_ERROR=1004
goto END

:BUILD_OFFICIAL
echo.
echo. Starting Official Build
set DELETE_FETCH_FILES=true
set BUILDTARGET="Build.Official"
goto DOBUILD

:BUILD_DEV
echo.
echo. Starting Dev Build
set DELETE_FETCH_FILES=true
set BUILDTARGET="Build.Dev"
goto DOBUILD

:BUILD_MANUAL_FETCH
echo.
echo. Starting Incremental Build
set DELETE_FETCH_FILES=false
set BUILDTARGET="Build.Dev"
goto DOBUILD

REM ******************************************
REM ********** DO BUILD **********************
REM ******************************************
:DOBUILD

IF EXIST "C:\New_SQLCM_Installer" (
RMDIR "C:\New_SQLCM_Installer" /s /q
)

REM md "C:\Program Files (x86)\Jenkins\workspace\SQLCM-Trunk-Build\LOCAL_BUILDS\New_SQLCM_Installer"

REM **********************************
REM * Setup user environment so the 
REM * script has the appropriate 
REM * permissions
REM **********************************
echo.
echo. Setting up build user environment
if not defined VCINSTALLDIR call "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\Tools\vsvars32.bat"

REM **********************************
REM * Skip delete/sync files if incremental
REM **********************************
if (%DELETE_FETCH_FILES%) == (false) GOTO KEEP_EXISTING_FILES

REM **********************************
REM * Delete & fetch the files
REM **********************************
echo.
REM echo. Deleting files
REM rd %BUILDROOT% /s/q
REM mkdir %BUILDROOT% 
REM echo. Fetching the files from GitHub
REM "C:\Program Files (x86)\Git\cmd\git.exe" clone -b %BUILDBRANCH% https://github.com/IderaInc/CWFSampleProduct.git %BUILDROOT%
REM if errorlevel 1 (	
	REM set BUILD_ERROR=%errorlevel%	
	REM echo GitHub clone failed!	
	REM goto END
REM )
GOTO DONANT

REM **********************************
REM * Keep existing files.
REM **********************************
:KEEP_EXISTING_FILES
echo.
echo. Using existing source files, not fetching from GitHub.
GOTO DONANT

REM **********************************
REM Execute the build script
REM **********************************

:DONANT
echo.
echo. Building the specified target.
"C:\Nant\bin\nant.exe" -f:%BUILDROOT%\Build\IderaDashboardSampleProduct.build %BUILDTARGET% ^
 	-D:Sample.buildnumber=%BUILDNUMBER% ^
 	-D:Sample.buildroot=%BUILDROOT% ^
 	-D:Sample.buildbranch=%BUILDBRANCH% ^
	-l:%BUILDROOT%\Build\build.log
REM "C:\Nant\bin\nant.exe" -f:%BUILDROOT%\Build\IderaDashboard.build %BUILDTARGET% ^
REM 	-D:IderaDashboard.buildnumber=%BUILDNUMBER% ^
REM 	-D:IderaDashboard.buildroot=%BUILDROOT% ^
REM 	-D:IderaDashboard.buildbranch=%BUILDBRANCH% ^
REM 	-l:%BUILDROOT%\Build\build.log
REM -logger:NAnt.Core.MailLogger	

if errorlevel 1 (	
	set BUILD_ERROR=%errorlevel%	
	echo Nant build failed!	
	goto END
)

\\qnap.redhouse.hq\IderaRelease\Development\SQLComplianceManager\Builds\5.5.0.%BUILDNUMBER%\Production\x64\Silent_Installer\Full\x64\SQLcompliance-x64.exe /s /x /b"\\qnap.redhouse.hq\IderaRelease\Development\SQLComplianceManager\Builds\5.5.0.%BUILDNUMBER%\Production\x64\Silent_Installer\Full\x64" /v"/qn"
\\qnap.redhouse.hq\IderaRelease\Development\SQLComplianceManager\Builds\5.5.0.%BUILDNUMBER%\Trial\x64\Silent_Installer\Full\x64\SQLcompliance-x64.exe /s /x /b"\\qnap.redhouse.hq\IderaRelease\Development\SQLComplianceManager\Builds\5.5.0.%BUILDNUMBER%\Trial\x64\Silent_Installer\Full\x64" /v"/qn"

\\qnap.redhouse.hq\IderaRelease\Development\SQLComplianceManager\Builds\5.5.0.%BUILDNUMBER%\Production\x86\Silent_Installer\Full\x86\SQLcompliance.exe /s /x /b"\\qnap.redhouse.hq\IderaRelease\Development\SQLComplianceManager\Builds\5.5.0.%BUILDNUMBER%\Production\x86\Silent_Installer\Full\x86" /v"/qn"
\\qnap.redhouse.hq\IderaRelease\Development\SQLComplianceManager\Builds\5.5.0.%BUILDNUMBER%\Trial\x86\Silent_Installer\Full\x86\SQLcompliance.exe /s /x /b"\\qnap.redhouse.hq\IderaRelease\Development\SQLComplianceManager\Builds\5.5.0.%BUILDNUMBER%\Trial\x86\Silent_Installer\Full\x86" /v"/qn"

REM XCOPY "C:\New_SQLCM_Installer" "C:\Program Files (x86)\Jenkins\workspace\SQLCM-Trunk-Build\LOCAL_BUILDS\New_SQLCM_Installer" /s /i

REM ******************************************
REM ********** END OF BUILD EXECUTION ********
REM ******************************************
:END
echo.
echo. Build script execution is complete
echo. %BUILD_ERROR%
REM exit %BUILD_ERROR%
REM pause
