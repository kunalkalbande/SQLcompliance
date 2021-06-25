REM cmd /c call C:\build\DoIderaDashboardSampleProductBuild.cmd 11 master official

set BUILDBRANCH=master
set BUILDNUMBER=11
set BUILDROOT=C:\Build\%BUILDBRANCH%SampleProduct
set BUILDTARGET=Build.Official
"C:\Nant\bin\nant.exe" -f:%BUILDROOT%\Build\IderaDashboardSampleProduct.build %BUILDTARGET% ^
 	-D:Sample.buildnumber=%BUILDNUMBER% ^
 	-D:Sample.buildroot=%BUILDROOT% ^
 	-D:Sample.buildbranch=%BUILDBRANCH% ^
	-l:%BUILDROOT%\Build\build.log