cd \devel\sqlcm\build

@echo off
if "%COMPUTERNAME%" == "SQLCMBLD" goto DOSTARTUP
echo. NOTE:  This script can only be run from the SQLcmBuild machine
goto END

:DOSTARTUP
if (%1)==(dev) GOTO BUILD_DEV
if (%1)==(official) GOTO BUILD_OFFICIAL
if (%1)==(prep) GOTO BUILD_PREP
if (%1)==(doc) GOTO BUILD_DOCONLY
if (%1)==(docinstall) GOTO BUILD_DOCINSTALL
echo. BUILD ERROR: Missing or invalid command line parameter
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
echo. Available Properties (use these for custom builds - prep option):  
echo.    SQLcm.version - force a version number (major.minor.release.build)
echo.    Build.Config - Release or Debug version (default is Release)
echo.    Perforce.TargetLabel - sync to a specific label rather than the most recent version
echo.    Perforce.Sync.Force - default is true.  set to false to avoid a forced sync
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
if (%2)==() GOTO DOCONLY_USAGE
echo.  Starting Doc Only Build
set BUILDTARGET="Build.DocOnly"
goto DOBUILD

:BUILD_DOCINSTALL
if (%2)==() GOTO DOCONLY_USAGE
echo.  Starting DocInstall Build
set BUILDTARGET="Build.DocInstallOnly"


:DOBUILD
REM **********************************
REM Setup user environment so the script has the appropriate permissions
REM **********************************
p4 set p4port=perforce01.redhouse.hq:1666
p4 set p4user=build
p4 set p4tickets="c:\p4ticket\ticket.txt"

if (%2)==(2.1) GOTO BUILD_21
if (%2)==(3.0) GOTO BUILD_30
if (%2)==(3.1) GOTO BUILD_31
if (%2)==(3.2) GOTO BUILD_32
if (%2)==(3.3) GOTO BUILD_33
if (%2)==(3.5) GOTO BUILD_35
if (%2)==(3.6) GOTO BUILD_36
if (%2)==(3.7) GOTO BUILD_37
if (%2)==(4.0) GOTO BUILD_40
if (%2)==(4.2) GOTO BUILD_42

p4 set p4client=build_sqlcmbuild_main
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
set BRANCH="//sqlcm/main"
p4 sync -f //sqlcm/main/build/...
GOTO COMMON

:BUILD_21
REM **********************************
REM Setup 2.1 build - no .NET 2.0 can be on machine
REM **********************************
p4 set p4client=build_sqlcmbuild_2.1
call "C:\Program Files\Microsoft Visual Studio .NET 2003\Vc7\bin\vcvars32.bat"
set BRANCH="//sqlcm/2006/2.1"
p4 sync -f //sqlcm/2006/2.1/build/...
GOTO COMMON

:BUILD_30
REM **********************************
REM Setup 3.0 build - 
REM **********************************
p4 set p4client=build_sqlcmbuild_3.0
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
set BRANCH="//sqlcm/2007/3.0"
p4 sync -f //sqlcm/2007/3.0/build/...
GOTO COMMON

:BUILD_31
REM **********************************
REM Setup 3.1 build - 
REM **********************************
p4 set p4client=build_sqlcmbuild_3.1
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
set BRANCH="//sqlcm/2008/3.1"
p4 sync -f //sqlcm/2008/3.1/build/...
GOTO COMMON

:BUILD_32
REM **********************************
REM Setup 3.2 build - 
REM **********************************
p4 set p4client=build_sqlcmbuild_3.2
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
set BRANCH="//sqlcm/2010/3.2"
p4 sync -f //sqlcm/2010/3.2/build/...
GOTO COMMON

:BUILD_33
REM **********************************
REM Setup 3.3 build - 
REM **********************************
p4 set p4client=build_sqlcmbuild_3.3
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
set BRANCH="//sqlcm/2011/3.3"
p4 sync -f //sqlcm/2011/3.3/build/...
GOTO COMMON

:BUILD_35
REM **********************************
REM Setup 3.5 build - 
REM **********************************
p4 set p4client=build_sqlcmbuild_3.5
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
set BRANCH="//sqlcm/2011/3.5"
p4 sync -f //sqlcm/2011/3.5/build/...
GOTO COMMON

:BUILD_36
REM **********************************
REM Setup 3.6 build - 
REM **********************************
p4 set p4client=build_sqlcmbuild_3.6
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
set BRANCH="//sqlcm/2012/3.6"
p4 sync -f //sqlcm/2012/3.6/build/...
GOTO COMMON

:BUILD_37
REM **********************************
REM Setup 3.7 build - 
REM **********************************
p4 set p4client=build_sqlcmbuild_3.7
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
set BRANCH="//sqlcm/2012/3.7"
p4 sync -f //sqlcm/2012/3.7/build/...
GOTO COMMON

:BUILD_40
REM **********************************
REM Setup 4.0 build - 
REM **********************************
p4 set p4client=build_sqlcmbuild_4.0
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
set BRANCH="//sqlcm/2012/4.0"
p4 sync -f //sqlcm/2012/4.0/build/...
GOTO COMMON

:BUILD_42
REM **********************************
REM Setup 4.2 build - 
REM **********************************
p4 set p4client=build_sqlcmbuild_4.2
call "C:\Program Files\Microsoft Visual Studio 8\VC\bin\vcvars32.bat"
set BRANCH="//sqlcm/2012/4.2"
p4 sync -f //sqlcm/2012/4.2/build/...
GOTO COMMON

:COMMON
REM **********************************
REM Nuke the entire tree except build
REM **********************************
rd /q /s c:\devel\sqlcm\development
rd /q /s c:\devel\sqlcm\documentation
rd /q /s c:\devel\sqlcm\install
rd /q /s c:\devel\sqlcm\lib
rd /q /s c:\devel\sqlcm\redist
rd /q /s c:\devel\sqlcm\documents
rd /q /s c:\devel\sqlcm\bin
rd /q /s c:\devel\sqlcm\build\output
rd /q /s c:\devel\sqlcm\build\temp

REM **********************************
REM Execute the build script
REM **********************************
if (%1)==(prep) GOTO END
if (%1)==(doc) GOTO NANT_DOCONLY
if (%1)==(docinstall) GOTO NANT_DOCONLY

nant -f:sqlcompliance.build -D:Perforce.Branch="%BRANCH%" %BUILDTARGET% -l:C:\devel\sqlcm\build.log -logger:NAnt.Core.MailLogger
GOTO END

:NANT_DOCONLY
nant -f:sqlcompliance.build -D:Perforce.Branch="%BRANCH%" -D:SQLcm.version="%2" -D:Perforce.TargetLabel="sqlcm_%2" %BUILDTARGET%
GOTO END

:DOCONLY_USAGE
echo. Please supply a pre-existing build version for doc only builds.

:END
echo.  Build script execution is complete