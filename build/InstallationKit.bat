rem _____________________________________________________________________
rem Create Self Extracting EXE for web download
rem _
rem /abs wztest.abs – Use specified ABS configuration file
rem /build wztest.exe – Build EXE by specified name
rem /zip myfile.zip – Zip file to include in self-extracting exe
rem _____________________________________________________________________
SET BUILDNUMBER=%1
"c:\program files\Absolute Packager\absolute.exe" /abs InstallationKit.abs /build Temp\SQLCMInstall-32bit-v%BUILDNUMBER%.exe /silent /zip Temp\SQLcm.zip



